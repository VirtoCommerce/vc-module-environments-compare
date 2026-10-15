<template>
  <VcBlade
    :title="$t('ENV_COMPARE.COMPARISON.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="100%"
  >
    <div
      v-if="identical"
      class="envc-empty"
    >
      <VcIcon
        icon="lucide-circle-check"
        size="xxl"
        class="envc-empty__icon"
      />
      <div class="envc-empty__title">{{ $t("ENV_COMPARE.COMPARISON.NO_DIFFERENCES_TITLE") }}</div>
      <div class="envc-empty__text">{{ $t("ENV_COMPARE.COMPARISON.NO_DIFFERENCES_TEXT") }}</div>
      <VcButton
        v-if="!showAll"
        variant="secondary"
        @click="setShowAll(true)"
      >
        {{ $t("ENV_COMPARE.TOOLBAR.SHOW_ALL") }}
      </VcButton>
    </div>
    <SettingsTable
      v-else-if="data"
      :scopes="scopes"
      :columns="columns"
      @column-click="(i: number) => setBase(columns[i].name)"
    />
  </VcBlade>
</template>

<script setup lang="ts">
// environments-comparison: settings side by side; the base environment (star) is what the others are compared
// with (click another header to change it); differences only or all settings
import { useApiClient, useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcButton, VcIcon } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  EnvironmentsCompareClient,
  type ComparedEnvironmentSettingValue,
  type SettingsComparisonResult,
} from "../../api_client/virtocommerce.environmentscompare";
import SettingsTable, { type Column, type Scope } from "../components/SettingsTable.vue";
import { describe, valueText } from "../text";

defineBlade({
  url: "/environments-comparison",
  name: "EnvCompareComparison",
  permissions: ["environments-compare:read"],
});

const { t, te } = useI18n({ useScope: "global" });
const { options } = useBlade<{ environmentNames: string[] }>();
const { getApiClient } = useApiClient(EnvironmentsCompareClient);

const names = computed(() => options.value?.environmentNames ?? []);
const base = ref(names.value[0]);
const showAll = ref(false);
const data = ref<SettingsComparisonResult>();

const { loading, action: load } = useAsync(async () => {
  data.value = await (
    await getApiClient()
  ).compareEnvironments({ environmentNames: names.value, baseEnvironmentName: base.value, showAll: showAll.value });
});
load();

function setBase(name: string) {
  base.value = name;
  load();
}
function setShowAll(value: boolean) {
  showAll.value = value;
  load();
}

const environments = computed(() => data.value?.comparedEnvironments ?? []);
const identical = computed(
  () => !!data.value && !data.value.settingScopes?.length && environments.value.every((e) => !e.errorMessage),
);
const columns = computed<Column[]>(() =>
  environments.value.map((e) => ({
    name: e.environmentName ?? "",
    isBase: e.isComparisonBase,
    clickable: !e.isComparisonBase && !e.errorMessage,
    error: e.errorMessage,
  })),
);

const hasDiff = (v?: ComparedEnvironmentSettingValue) => v?.equalsBaseValue === false && !v.errorMessage;
const scopes = computed<Scope[]>(() =>
  (data.value?.settingScopes ?? []).map((scope) => ({
    scopeName: scope.scopeName,
    groups: (scope.settingGroups ?? []).map((g) => ({
      groupName: g.groupName,
      settings: (g.settings ?? []).map((s) => {
        const values = environments.value.map((e) =>
          s.comparedValues?.find((v) => v.environmentName === e.environmentName),
        );
        return {
          name: s.name ?? "",
          description: describe(s.name, undefined, te, t),
          diff: values.some(hasDiff),
          cells: values.map((v, i) => ({
            text: v?.errorMessage || valueText(v?.value),
            diff: !environments.value[i].errorMessage && hasDiff(v),
            error: !!environments.value[i].errorMessage || !!v?.errorMessage,
          })),
        };
      }),
    })),
  })),
);

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("ENV_COMPARE.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "show-all",
    title: t("ENV_COMPARE.TOOLBAR.SHOW_ALL"),
    icon: "lucide-equal",
    isVisible: computed(() => !showAll.value),
    clickHandler: () => setShowAll(true),
  },
  {
    id: "show-diff",
    title: t("ENV_COMPARE.TOOLBAR.SHOW_DIFF"),
    icon: "lucide-equal-not",
    isVisible: computed(() => showAll.value),
    clickHandler: () => setShowAll(false),
  },
]);
</script>

<style lang="scss">
.envc-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 300px;
  padding: 40px 20px;
  text-align: center;

  &__icon {
    color: var(--color-success-500);
    margin-bottom: 8px;
  }

  &__title {
    font-size: 18px;
    font-weight: 600;
    color: var(--color-neutral-900);
  }

  &__text {
    font-size: 13px;
    color: var(--color-neutral-500);
    margin-bottom: 8px;
  }
}
</style>
