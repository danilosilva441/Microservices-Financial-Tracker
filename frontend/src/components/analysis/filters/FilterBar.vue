<!--
 * src/components/analysis/filters/FilterBar.vue
 * FilterBar.vue
 *
 * Barra de filtros do dashboard de análise.
 * - Período (PeriodFilter)
 * - Unidades (UnitMultiSelect)
 * - Aplicar / Limpar
 -->
<template>
  <div class="p-4 mb-6 bg-white rounded-lg shadow-sm dark:bg-gray-800">
    <div class="flex flex-col gap-4 lg:flex-row">
      <!-- Filtro de Período -->
      <div class="flex-1">
        <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
          Período
        </label>

        <PeriodFilter
          :model-value="periodModelValue"
          @update:model-value="updatePeriod"
          @custom-date="handleCustomDate"
        />
      </div>

      <!-- Filtro de Unidades -->
      <div class="flex-1">
        <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
          Unidades
        </label>

        <UnitMultiSelect v-model="localFilters.units" :units="units" />
      </div>

      <!-- Botões de Ação -->
      <div class="flex items-end gap-2">
        <button @click="applyFilters" class="btn-primary">
          <IconFunnel class="w-5 h-5 mr-2" />
          Aplicar Filtros
        </button>

        <button @click="resetFilters" class="btn-outline">
          <IconArrowPath class="w-5 h-5 mr-2" />
          Limpar
        </button>
      </div>
    </div>

    <!-- Filtros Ativos -->
    <div v-if="activeFiltersCount > 0" class="flex flex-wrap gap-2 mt-4">
      <span class="text-sm text-gray-600 dark:text-gray-400">
        Filtros ativos:
      </span>

      <span
        v-for="filter in activeFilters"
        :key="filter.key"
        class="inline-flex items-center px-3 py-1 text-sm rounded-full bg-primary-100 dark:bg-primary-900/30 text-primary-700 dark:text-primary-300"
      >
        {{ filter.label }}
        <button
          @click="removeFilter(filter.key)"
          class="ml-2 hover:text-primary-900 dark:hover:text-primary-100"
        >
          <IconXMark class="w-4 h-4" />
        </button>
      </span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import PeriodFilter from './PeriodFilter.vue'
import UnitMultiSelect from './UnitMultiSelect.vue'
import IconFunnel from '@/components/icons/FunnelIcon.vue'
import IconArrowPath from '@/components/icons/ArrowPathIcon.vue'
import IconXMark from '@/components/icons/XMarkIcon.vue'

const props = defineProps({
  modelValue: {
    type: Object,
    default: () => ({ period: 'month', units: [], startDate: null, endDate: null })
  },
  units: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:modelValue', 'apply', 'reset'])

/**
 * Normaliza + clona (quebra reatividade sem JSON stringify)
 * - period sempre string válida
 * - units sempre array novo
 * - start/end sempre null ou valor
 */
const normalizeFilters = (v) => {
  const period =
    typeof v?.period === 'string' && v.period.trim().length > 0 ? v.period : 'month'

  return {
    period,
    units: Array.isArray(v?.units) ? [...v.units] : [],
    startDate: v?.startDate ?? null,
    endDate: v?.endDate ?? null
  }
}

const getInitialState = () => normalizeFilters(props.modelValue)

const localFilters = ref(getInitialState())

const periodOptions = [
  { value: 'today', label: 'Hoje' },
  { value: 'yesterday', label: 'Ontem' },
  { value: 'week', label: 'Esta Semana' },
  { value: 'lastWeek', label: 'Semana Passada' },
  { value: 'month', label: 'Este Mês' },
  { value: 'lastMonth', label: 'Mês Passado' },
  { value: 'quarter', label: 'Este Trimestre' },
  { value: 'year', label: 'Este Ano' },
  { value: 'lastYear', label: 'Ano Passado' },
  { value: 'custom', label: 'Personalizado' }
]

/**
 * GARANTIA: o PeriodFilter NUNCA recebe undefined
 * (isso elimina o warning de prop e reduz chance de loop)
 */
const periodModelValue = computed(() => {
  const p = localFilters.value?.period
  return typeof p === 'string' && p.trim().length > 0 ? p : 'month'
})

const activeFiltersCount = computed(() => {
  let count = 0
  if (localFilters.value.period && localFilters.value.period !== 'month') count++
  if (localFilters.value.units && localFilters.value.units.length > 0) count++
  return count
})

const activeFilters = computed(() => {
  const filters = []

  if (localFilters.value.period && localFilters.value.period !== 'month') {
    const periodLabel = periodOptions.find(p => p.value === localFilters.value.period)?.label
    if (periodLabel) {
      if (localFilters.value.period === 'custom' && localFilters.value.startDate) {
        filters.push({
          key: 'period',
          label: `De ${localFilters.value.startDate} até ${localFilters.value.endDate}`
        })
      } else {
        filters.push({ key: 'period', label: `Período: ${periodLabel}` })
      }
    }
  }

  if (localFilters.value.units && localFilters.value.units.length > 0) {
    const unitCount = localFilters.value.units.length
    const unitText = unitCount === 1 ? 'unidade' : 'unidades'
    filters.push({ key: 'units', label: `${unitCount} ${unitText} selecionada(s)` })
  }

  return filters
})

/**
 * Anti-loop: só atualiza se mudar de verdade.
 * E também limpa start/end quando sair do custom.
 */
const updatePeriod = (val) => {
  const next = typeof val === 'string' && val.trim().length > 0 ? val : 'month'
  if (next === localFilters.value.period) return

  localFilters.value.period = next

  if (next !== 'custom') {
    localFilters.value.startDate = null
    localFilters.value.endDate = null
  }
}

const handleCustomDate = (payload = {}) => {
  const startDate = payload.startDate ?? null
  const endDate = payload.endDate ?? null

  const changed =
    localFilters.value.period !== 'custom' ||
    localFilters.value.startDate !== startDate ||
    localFilters.value.endDate !== endDate

  if (!changed) return

  localFilters.value.startDate = startDate
  localFilters.value.endDate = endDate
  localFilters.value.period = 'custom'
}

const applyFilters = () => {
  const payload = normalizeFilters(localFilters.value)
  emit('update:modelValue', payload)
  emit('apply', payload)
}

const resetFilters = () => {
  localFilters.value = {
    period: 'month',
    units: [],
    startDate: null,
    endDate: null
  }

  const payload = normalizeFilters(localFilters.value)
  emit('update:modelValue', payload)
  emit('reset')
}

const removeFilter = (key) => {
  if (key === 'period') {
    localFilters.value.period = 'month'
    localFilters.value.startDate = null
    localFilters.value.endDate = null
  }

  if (key === 'units') {
    localFilters.value.units = []
  }

  applyFilters()
}

/**
 * Watcher defensivo:
 * sincroniza quando o PAI muda o modelValue (inclusive custom dates).
 */
watch(
  () => props.modelValue,
  (newVal) => {
    if (!newVal) return

    const current = normalizeFilters(localFilters.value)
    const incoming = normalizeFilters(newVal)

    const samePeriod = current.period === incoming.period
    const sameUnits = JSON.stringify(current.units) === JSON.stringify(incoming.units)
    const sameStart = current.startDate === incoming.startDate
    const sameEnd = current.endDate === incoming.endDate

    if (!samePeriod || !sameUnits || !sameStart || !sameEnd) {
      localFilters.value = incoming
    }
  },
  { deep: true }
)
</script>