<template>
  <VcBlade
    :title="param"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="70%"
  >
    <SettingsTable
      :scopes="scopes"
      :columns="columns"
    />
  </VcBlade>
</template>

<script setup lang="ts">
// environment-item: one environment's settings (scope → group → setting = value)
import { useApiClient, useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  EnvironmentsCompareClient,
  type ComparableEnvironmentSettings,
} from "../../api_client/virtocommerce.environmentscompare";
import SettingsTable, { type Column, type Scope } from "../components/SettingsTable.vue";
import { describe, valueText } from "../text";

defineBlade({
  url: "/environment",
  name: "EnvCompareEnvironment",
  permissions: ["environments-compare:read"],
});

const { t, te } = useI18n({ useScope: "global" });
const { param } = useBlade();
const { getApiClient } = useApiClient(EnvironmentsCompareClient);

const data = ref<ComparableEnvironmentSettings>();
const { loading, action: load } = useAsync(async () => {
  data.value = await (await getApiClient()).getEnvironmentSettings(param.value!);
});
load();

const columns = computed<Column[]>(() => [{ name: param.value ?? "", error: data.value?.errorMessage }]);
const scopes = computed<Scope[]>(() =>
  (data.value?.settingScopes ?? []).map((scope) => ({
    scopeName: scope.scopeName,
    groups: (scope.settingGroups ?? []).map((g) => ({
      groupName: g.groupName,
      settings: (g.settings ?? []).map((s) => ({
        name: s.name ?? "",
        description: describe(s.name, s.description, te, t),
        cells: [{ text: valueText(s.value) }],
      })),
    })),
  })),
);

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("ENV_COMPARE.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
]);
</script>
