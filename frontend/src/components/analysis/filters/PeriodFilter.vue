<!--
 * src/components/analysis/filters/PeriodFilter.vue
 * PeriodFilter.vue
 *
 * Componente de filtro de período (botões + período personalizado).
 -->
<template>
  <div class="flex flex-wrap items-center gap-2">
    <button
      v-for="option in periodOptions"
      :key="option.value"
      type="button"
      @click="selectPeriod(option.value)"
      class="px-4 py-2 text-sm font-medium transition-all duration-200 rounded-lg"
      :class="[
        currentValue === option.value
          ? 'bg-primary-500 text-white shadow-md'
          : 'bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700'
      ]"
    >
      {{ option.label }}
    </button>

    <div class="relative" v-if="showCustom">
      <button
        type="button"
        @click="toggleDatePicker"
        class="flex items-center gap-2 px-4 py-2 text-sm font-medium transition-all duration-200 rounded-lg"
        :class="[
          currentValue === 'custom'
            ? 'bg-primary-500 text-white shadow-md'
            : 'text-gray-700 bg-gray-100 dark:bg-gray-800 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700'
        ]"
      >
        <IconCalendar class="w-4 h-4" />
        <span>Personalizado</span>
      </button>

      <div
        v-if="showDatePicker"
        v-click-outside="closeDatePicker"
        class="absolute right-0 z-50 p-4 mt-2 bg-white border border-gray-200 rounded-lg shadow-xl dark:bg-gray-800 dark:border-gray-700"
      >
        <div class="flex flex-col gap-4">
          <div>
            <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
              Data Inicial
            </label>
            <input
              type="date"
              v-model="customStartDate"
              class="w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-lg dark:border-gray-600 dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div>
            <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
              Data Final
            </label>
            <input
              type="date"
              v-model="customEndDate"
              class="w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-lg dark:border-gray-600 dark:bg-gray-700 dark:text-white"
            />
          </div>

          <div class="flex gap-2">
            <button type="button" @click="applyCustomDate" class="flex-1 btn-primary">
              Aplicar
            </button>
            <button type="button" @click="closeDatePicker" class="flex-1 btn-outline">
              Cancelar
            </button>
          </div>

          <p v-if="errorMessage" class="text-sm text-red-600 dark:text-red-400">
            {{ errorMessage }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import IconCalendar from '@/components/icons/CalendarIcon.vue'
import { vClickOutside } from '@/directives/clickOutside'

const props = defineProps({
  modelValue: {
    type: String,
    required: false,
    default: 'month'
  },
  showCustom: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue', 'custom-date'])

const showDatePicker = ref(false)
const customStartDate = ref('')
const customEndDate = ref('')
const errorMessage = ref('')

const periodOptions = [
  { value: 'today', label: 'Hoje' },
  { value: 'yesterday', label: 'Ontem' },
  { value: 'week', label: 'Esta Semana' },
  { value: 'lastWeek', label: 'Semana Passada' },
  { value: 'month', label: 'Este Mês' },
  { value: 'lastMonth', label: 'Mês Passado' },
  { value: 'quarter', label: 'Este Trimestre' },
  { value: 'year', label: 'Este Ano' },
  { value: 'lastYear', label: 'Ano Passado' }
]

// Garante que nunca trabalhamos com undefined/null
const currentValue = computed(() => {
  const v = props.modelValue
  return typeof v === 'string' && v.trim().length > 0 ? v : 'month'
})

const closeDatePicker = () => {
  showDatePicker.value = false
  errorMessage.value = ''
}

const toggleDatePicker = () => {
  showDatePicker.value = !showDatePicker.value
  errorMessage.value = ''
}

/**
 * Anti-loop: só emite se realmente mudar.
 * Também fecha o datepicker ao trocar período “normal”.
 */
const selectPeriod = (value) => {
  const next = typeof value === 'string' && value.trim().length > 0 ? value : 'month'
  if (next === currentValue.value) return

  emit('update:modelValue', next)
  closeDatePicker()
}

const applyCustomDate = () => {
  errorMessage.value = ''

  if (!customStartDate.value || !customEndDate.value) {
    errorMessage.value = 'Selecione a data inicial e a data final.'
    return
  }

  // Strings YYYY-MM-DD permitem comparação lexicográfica
  let start = customStartDate.value
  let end = customEndDate.value

  if (start > end) {
    // swap automático pra evitar erro bobo
    ;[start, end] = [end, start]
  }

  // Emite datas
  emit('custom-date', { startDate: start, endDate: end })

  // Também emite o período como 'custom' (deixa consistente com v-model)
  if (currentValue.value !== 'custom') {
    emit('update:modelValue', 'custom')
  }

  closeDatePicker()
}

/**
 * Se o pai trocar o período (ex.: reset pra 'month'),
 * garantimos que o datepicker feche.
 */
watch(
  () => props.modelValue,
  () => {
    if (showDatePicker.value) closeDatePicker()
  }
)
</script>