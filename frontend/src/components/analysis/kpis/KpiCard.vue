<!--
 * src/components/analysis/kpis/KpiCard.vue
 * KpiCard.vue
 *
 * A reusable Vue component for displaying a KPI (Key Performance Indicator) card in the financial analysis dashboard.
 * It includes features like trend indicators, comparison values, and an optional progress bar.
 * The component is designed to be flexible and theme-aware, with customizable icons and colors.
 -->
<template>
  <div
    class="p-6 transition-all duration-200 bg-white shadow-sm dark:bg-gray-800 rounded-xl hover:shadow-md"
    :class="{
      'border-l-4 border-green-500': trend === 'up',
      'border-l-4 border-red-500': trend === 'down',
      'border-l-4 border-yellow-500': trend === 'stable'
    }"
  >
    <div class="flex items-start justify-between mb-4">
      <div class="flex items-center gap-3">
        <div
          class="p-3 rounded-lg"
          :class="iconBackgroundClass"
        >
          <Component :is="icon" class="w-6 h-6" :class="iconColorClass" />
        </div>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">{{ title }}</p>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ formatValue(value) }}
          </p>
        </div>
      </div>

      <div
        v-if="trend"
        class="flex items-center gap-1 px-2 py-1 text-xs font-medium rounded-full"
        :class="trendClass"
      >
        <IconArrowTrendingUp v-if="trend === 'up'" class="w-3 h-3" />
        <IconArrowTrendingDown v-if="trend === 'down'" class="w-3 h-3" />
        <IconMinus v-if="trend === 'stable'" class="w-3 h-3" />
        <span>{{ trendValue }}%</span>
      </div>
    </div>

    <div class="flex items-center justify-between text-sm">
      <span class="text-gray-600 dark:text-gray-400">{{ subtitle }}</span>
      <span class="font-medium" :class="comparisonClass">
        {{ comparisonValue }}
      </span>
    </div>

    <div v-if="progress !== undefined" class="mt-4">
      <div class="flex items-center justify-between mb-1 text-sm">
        <span class="text-gray-600 dark:text-gray-400">Progresso</span>
        <span class="font-medium text-gray-900 dark:text-white">{{ progress }}%</span>
      </div>
      <div class="w-full h-2 overflow-hidden bg-gray-200 rounded-full dark:bg-gray-700">
        <div
          class="h-full transition-all duration-500 rounded-full"
          :class="progressBarClass"
          :style="{ width: `${progress}%` }"
        ></div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import IconArrowTrendingUp from '@/components/icons/ArrowTrendingUpIcon.vue'
import IconArrowTrendingDown from '@/components/icons/ArrowTrendingDownIcon.vue'
import IconMinus from '@/components/icons/MinusIcon.vue'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  value: {
    type: [Number, String],
    required: true
  },
  subtitle: {
    type: String,
    default: 'Comparado ao período anterior'
  },
  comparisonValue: {
    type: String,
    default: '0%'
  },
  icon: {
    // 👇 REMOVA O TYPE ESPECÍFICO. Deixe apenas required.
    // Isso impede o Vue de tentar validar a estrutura interna do componente.
    required: true
  },
  trend: {
    type: String,
    // Permite undefined para casos sem tendência
    validator: (value) => !value || ['up', 'down', 'stable'].includes(value)
  },
  trendValue: {
    type: Number,
    default: 0
  },
  progress: {
    type: Number,
    validator: (value) => value === undefined || (value >= 0 && value <= 100)
  },
  format: {
    type: String,
    default: 'currency',
    // Permite undefined ou texto livre
    validator: (value) => !value || ['currency', 'number', 'percentage', 'text'].includes(value)
  },
  iconColor: {
    type: String,
    default: 'primary'
  }
})

const formatValue = (value) => {
  if (typeof value === 'string') return value
  
  switch (props.format) {
    case 'currency':
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value)
    case 'percentage':
      return `${value}%`
    default:
      return new Intl.NumberFormat('pt-BR').format(value)
  }
}

const iconBackgroundClass = computed(() => {
  const colors = {
    primary: 'bg-primary-100 dark:bg-primary-900/30',
    green: 'bg-green-100 dark:bg-green-900/30',
    red: 'bg-red-100 dark:bg-red-900/30',
    yellow: 'bg-yellow-100 dark:bg-yellow-900/30',
    blue: 'bg-blue-100 dark:bg-blue-900/30'
  }
  return colors[props.iconColor] || colors.primary
})

const iconColorClass = computed(() => {
  const colors = {
    primary: 'text-primary-600 dark:text-primary-400',
    green: 'text-green-600 dark:text-green-400',
    red: 'text-red-600 dark:text-red-400',
    yellow: 'text-yellow-600 dark:text-yellow-400',
    blue: 'text-blue-600 dark:text-blue-400'
  }
  return colors[props.iconColor] || colors.primary
})

const trendClass = computed(() => {
  if (!props.trend) return ''
  
  const classes = {
    up: 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300',
    down: 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300',
    stable: 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300'
  }
  return classes[props.trend]
})

const comparisonClass = computed(() => {
  if (!props.trend) return 'text-gray-900 dark:text-white'
  
  const classes = {
    up: 'text-green-600 dark:text-green-400',
    down: 'text-red-600 dark:text-red-400',
    stable: 'text-yellow-600 dark:text-yellow-400'
  }
  return classes[props.trend] || 'text-gray-900 dark:text-white'
})

const progressBarClass = computed(() => {
  if (props.progress >= 100) return 'bg-green-500'
  if (props.progress >= 70) return 'bg-primary-500'
  if (props.progress >= 30) return 'bg-yellow-500'
  return 'bg-red-500'
})
</script>