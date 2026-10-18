<template>
  <VcBlade
    :title="$t('ENV_COMPARE.MENU.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="40%"
  >
    <VcDataTable
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      data-key="name"
      :active-item-id="activeName"
      :show-all-columns="!activeName"
      :empty-state="{ icon: 'lucide-server', title: $t('ENV_COMPARE.LIST.EMPTY') }"
      @row-click="(e: { data: EnvironmentResponseItem }) => open(e.data)"
    >
      <VcColumn
        id="name"
        :title="$t('ENV_COMPARE.LIST.NAME')"
        mobile-role="title"
        always-visible
      />
      <VcColumn
        id="url"
        :title="$t('ENV_COMPARE.LIST.URL')"
        mobile-role="field"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
// environments-list: the current platform + the configured comparable environments; select 2+ to compare,
// 1 to export its settings; row click shows one environment's settings
import { useApiClient, useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { EnvironmentsCompareClient, type EnvironmentResponseItem } from "../../api_client/virtocommerce.environmentscompare";

defineBlade({
  url: "/environments-compare",
  name: "EnvCompareList",
  isWorkspace: true,
  permissions: ["environments-compare:access"],
  menuItem: { title: "ENV_COMPARE.MENU.TITLE", icon: "lucide-git-compare-arrows", priority: 106 },
});

const { t } = useI18n({ useScope: "global" });
const { openBlade } = useBlade();
const { getApiClient } = useApiClient(EnvironmentsCompareClient);

const items = ref<EnvironmentResponseItem[]>([]);
const selection = ref<EnvironmentResponseItem[]>([]);
const activeName = ref<string>();

const { loading, action: load } = useAsync(async () => {
  items.value = (await (await getApiClient()).getEnvironments()) ?? [];
  selection.value = [];
});
load();

function open(env: EnvironmentResponseItem) {
  openBlade({
    name: "EnvCompareEnvironment",
    param: env.name,
    onOpen: () => (activeName.value = env.name),
    onClose: () => (activeName.value = undefined),
  });
}

function compare() {
  activeName.value = undefined;
  openBlade({ name: "EnvCompareComparison", options: { environmentNames: selection.value.map((x) => x.name!) } });
}

// same-origin download of the environment's settings as JSON (cookie auth)
function exportSettings() {
  const link = document.createElement("a");
  link.href = `/api/environments-compare/export-settings/${encodeURIComponent(selection.value[0].name!)}`;
  link.download = "";
  document.body.appendChild(link);
  link.click();
  link.remove();
}

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("ENV_COMPARE.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "compare",
    title: t("ENV_COMPARE.TOOLBAR.COMPARE"),
    icon: "lucide-microscope",
    disabled: computed(() => selection.value.length < 2),
    clickHandler: compare,
  },
  {
    id: "export",
    title: t("ENV_COMPARE.TOOLBAR.EXPORT"),
    icon: "lucide-upload",
    disabled: computed(() => selection.value.length !== 1),
    clickHandler: exportSettings,
  },
]);
</script>
