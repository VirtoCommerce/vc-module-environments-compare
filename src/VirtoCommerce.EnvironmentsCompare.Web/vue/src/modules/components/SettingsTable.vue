<template>
  <div class="envc">
    <div class="envc__search">
      <VcInput
        v-model="keyword"
        :placeholder="$t('ENV_COMPARE.COMPARISON.SEARCH')"
        :aria-label="$t('ENV_COMPARE.COMPARISON.SEARCH')"
        clearable
      >
        <template #prepend-inner>
          <VcIcon
            icon="lucide-search"
            size="s"
          />
        </template>
      </VcInput>
    </div>
    <div
      v-if="!visible.length && keyword"
      class="envc__note"
    >
      {{ $t("ENV_COMPARE.COMPARISON.NOT_FOUND") }}
    </div>
    <div class="envc__scroll">
      <table
        class="envc__table"
        :style="{ '--envc-cols': columns.length }"
      >
        <colgroup>
          <col class="envc__col-name" />
          <col
            v-for="c in columns"
            :key="c.name"
          />
        </colgroup>
        <thead>
          <tr>
            <th class="envc__sticky">{{ $t("ENV_COMPARE.COMPARISON.SETTING") }}</th>
            <th
              v-for="(c, i) in columns"
              :key="c.name"
              :class="{ 'envc__env--error': c.error, 'envc__env--clickable': c.clickable }"
              :title="c.error || (c.clickable ? $t('ENV_COMPARE.COMPARISON.SET_BASE') : c.isBase ? $t('ENV_COMPARE.COMPARISON.BASE') : undefined)"
              :tabindex="c.clickable ? 0 : undefined"
              :role="c.clickable ? 'button' : undefined"
              @click="c.clickable && $emit('column-click', i)"
              @keydown.enter="c.clickable && $emit('column-click', i)"
            >
              <span class="envc__env">
                <VcIcon
                  v-if="c.isBase"
                  icon="lucide-star"
                  size="s"
                />
                <VcIcon
                  v-else-if="c.clickable"
                  icon="lucide-microscope"
                  size="s"
                />
                <span>{{ c.name }}</span>
                <VcIcon
                  v-if="c.error"
                  icon="lucide-triangle-alert"
                  size="s"
                />
              </span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-if="columns.some((c) => c.error)"
            class="envc__group"
          >
            <td class="envc__sticky">{{ $t("ENV_COMPARE.COMPARISON.SUMMARY") }}</td>
            <td
              v-for="c in columns"
              :key="c.name"
              :class="{ 'envc__cell--error': c.error }"
            >
              {{ c.error }}
            </td>
          </tr>
          <template
            v-for="(scope, si) in visible"
            :key="`s${si}`"
          >
            <tr class="envc__scope">
              <td
                class="envc__sticky"
                :class="marker(scope.index)"
              >
                {{ scope.scopeName }}
              </td>
              <td :colspan="columns.length" />
            </tr>
            <template
              v-for="(group, gi) in scope.groups"
              :key="`s${si}g${gi}`"
            >
              <tr class="envc__group">
                <td
                  class="envc__sticky envc__group-name"
                  :class="marker(scope.index)"
                >
                  {{ group.groupName }}
                </td>
                <td :colspan="columns.length" />
              </tr>
              <tr
                v-for="setting in group.settings"
                :key="setting.name"
                :class="{ 'envc__row--diff': setting.diff }"
              >
                <td
                  class="envc__sticky envc__name"
                  :class="marker(scope.index)"
                >
                  <span class="envc__name-inner">
                    <span>{{ setting.name }}</span>
                    <button
                      v-if="setting.description"
                      type="button"
                      class="envc__help"
                      :aria-label="$t('ENV_COMPARE.COMPARISON.DESCRIPTION')"
                      :aria-expanded="open.has(setting.name)"
                      @click="toggle(setting.name)"
                    >
                      <VcIcon
                        icon="lucide-circle-help"
                        size="s"
                      />
                    </button>
                  </span>
                  <span
                    v-if="open.has(setting.name)"
                    class="envc__description"
                  >{{ setting.description }}</span>
                </td>
                <td
                  v-for="(cell, ci) in setting.cells"
                  :key="ci"
                  :class="{ 'envc__cell--diff': cell.diff, 'envc__cell--error': cell.error }"
                >
                  {{ cell.text }}
                </td>
              </tr>
            </template>
          </template>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
// Settings grouped by scope and group, one value column per environment (legacy environments-comparison /
// environment-item tables): keyword filter on the setting name, sections without matches hidden, description toggle.
import { VcIcon, VcInput } from "@vc-shell/framework/ui";
import { computed, reactive, ref } from "vue";

export interface Column {
  name: string;
  isBase?: boolean;
  clickable?: boolean;
  error?: string;
}
export interface Row {
  name: string;
  description?: string;
  diff?: boolean;
  cells: { text: string; diff?: boolean; error?: boolean }[];
}
export interface Scope {
  scopeName?: string;
  groups: { groupName?: string; settings: Row[] }[];
}

const props = defineProps<{ scopes: Scope[]; columns: Column[] }>();
defineEmits<{ "column-click": [index: number] }>();

const keyword = ref("");
const open = reactive(new Set<string>());
const toggle = (name: string) => (open.has(name) ? open.delete(name) : open.add(name));

const visible = computed(() => {
  const term = keyword.value.trim().toLowerCase();
  return props.scopes
    .map((scope, index) => ({
      index,
      scopeName: scope.scopeName,
      groups: scope.groups
        .map((g) => ({ ...g, settings: g.settings.filter((s) => !term || s.name.toLowerCase().includes(term)) }))
        .filter((g) => g.settings.length),
    }))
    .filter((s) => s.groups.length);
});

// scope colour bar (legacy scope-marker-N), from the host's tokens
const marker = (index: number) => `envc__marker--${index % 8}`;
</script>

<style lang="scss">
.envc {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;

  &__search {
    padding: 12px 16px;
    border-bottom: 1px solid var(--color-neutral-200);

    .vc-input {
      max-width: 360px;
    }
  }

  &__note {
    padding: 16px;
    color: var(--color-neutral-500);
  }

  &__scroll {
    flex: 1;
    min-height: 0;
    overflow: auto;
  }

  // name column + value columns; narrower on phones (the table scrolls sideways inside the blade)
  &__table {
    --envc-name: 320px;
    --envc-value: 200px;
    width: 100%;
    min-width: calc(var(--envc-name) + var(--envc-cols) * var(--envc-value));
    border-collapse: separate;
    border-spacing: 0;
    table-layout: fixed;
    font-size: 13px;
    line-height: 1.4;
    color: var(--color-neutral-800);

    th,
    td {
      padding: 8px 12px;
      text-align: left;
      vertical-align: top;
      overflow-wrap: anywhere;
      border-bottom: 1px solid var(--color-neutral-200);
      background: var(--color-neutral-0);
    }

    thead th {
      position: sticky;
      top: 0;
      z-index: 2;
      font-weight: 600;
      background: var(--color-neutral-50);
    }

    thead th.envc__sticky {
      z-index: 3;
    }
  }

  &__col-name {
    width: var(--envc-name);
  }

  @media (max-width: 767px) {
    &__table {
      --envc-name: 150px;
      --envc-value: 150px;
    }
  }

  &__sticky {
    position: sticky;
    left: 0;
    z-index: 1;
    border-right: 1px solid var(--color-neutral-200);
  }

  &__env {
    display: inline-flex;
    align-items: flex-start;
    gap: 6px;
  }

  &__env--clickable {
    cursor: pointer;

    &:hover,
    &:focus-visible {
      color: var(--color-primary-600);
    }
  }

  &__scope td {
    font-weight: 600;
    background: var(--color-neutral-100);
  }

  &__group td {
    font-weight: 600;
    background: var(--color-neutral-50);
  }

  &__group-name {
    padding-left: 24px !important;
  }

  &__name {
    padding-left: 24px !important;
    font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
    font-size: 12px;
    color: var(--color-neutral-700);
  }

  &__name-inner {
    display: flex;
    align-items: center;
    gap: 6px;
  }

  &__help {
    display: inline-flex;
    padding: 0;
    border: 0;
    background: none;
    color: var(--color-info-500);
    cursor: pointer;
  }

  &__description {
    display: block;
    margin-top: 4px;
    font-family: inherit;
    font-style: italic;
    color: var(--color-neutral-500);
  }

  &__row--diff td {
    background: var(--color-info-50);
  }

  &__cell--diff {
    background: var(--color-info-100) !important;
    box-shadow: inset 3px 0 0 var(--color-info-500);
    font-weight: 500;
  }

  &__cell--error,
  &__env--error {
    color: var(--color-danger-600);
    font-style: italic;
  }

  $markers: primary-500, warning-500, success-500, danger-400, secondary-500, info-500, accent-500, neutral-400;
  @each $m in $markers {
    $i: index($markers, $m) - 1;
    &__marker--#{$i} {
      box-shadow: inset 4px 0 0 var(--color-#{$m});
    }
  }
}
</style>
