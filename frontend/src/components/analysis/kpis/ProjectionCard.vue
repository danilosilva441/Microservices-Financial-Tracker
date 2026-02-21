<!--
 * src/components/analysis/kpis/ProjectionCard.vue
 * ProjectionCard.vue
 *
 * A Vue component that displays a projection card for financial analysis.
 * It compares the target (meta) with the actual (realizado) and provides a projection based on current performance.
 * The card includes visual indicators for progress, probability, and recommendations based on the projection.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
    <div class="flex items-center justify-between mb-4">
      <div class="flex items-center gap-3">
        <div class="p-3 bg-purple-100 rounded-lg dark:bg-purple-900/30">
          <IconChartBar class="w-6 h-6 text-purple-600 dark:text-purple-400" />
        </div>
        <div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Projeção</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Meta vs Realizado</p>
        </div>
      </div>

      <div
        class="px-3 py-1 text-sm font-medium rounded-full"
        :class="statusClass"
      >
        {{ statusText }}
      </div>
    </div>

    <div class="space-y-4">
      <!-- Meta -->
      <div>
        <div class="flex justify-between mb-1 text-sm">
          <span class="text-gray-600 dark:text-gray-400">Meta</span>
          <span class="font-medium text-gray-900 dark:text-white">
            {{ formatCurrency(meta) }}
          </span>
        </div>
        <div class="w-full h-2 bg-gray-200 rounded-full dark:bg-gray-700">
          <div
            class="h-full bg-gray-400 rounded-full dark:bg-gray-500"
            :style="{ width: '100%' }"
          ></div>
        </div>
      </div>

      <!-- Realizado -->
      <div>
        <div class="flex justify-between mb-1 text-sm">
          <span class="text-gray-600 dark:text-gray-400">Realizado</span>
          <span class="font-medium text-gray-900 dark:text-white">
            {{ formatCurrency(realizado) }}
          </span>
        </div>
        <div class="w-full h-2 bg-gray-200 rounded-full dark:bg-gray-700">
          <div
            class="h-full transition-all duration-500 rounded-full"
            :class="progressBarClass"
            :style="{ width: `${progressoAtual}%` }"
          ></div>
        </div>
      </div>

      <!-- Projeção -->
      <div class="pt-4 mt-6 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between mb-2">
          <span class="text-sm text-gray-600 dark:text-gray-400">
            Projeção final
          </span>
          <span class="text-xl font-bold text-gray-900 dark:text-white">
            {{ formatCurrency(projecao) }}
          </span>
        </div>

        <div class="flex items-center justify-between text-sm">
          <span class="text-gray-600 dark:text-gray-400">
            Probabilidade
          </span>
          <span class="font-medium" :class="probabilityClass">
            {{ probability }}
          </span>
        </div>

        <div class="flex items-center justify-between mt-1 text-sm">
          <span class="text-gray-600 dark:text-gray-400">
            Dias restantes
          </span>
          <span class="font-medium text-gray-900 dark:text-white">
            {{ diasRestantes }} dias
          </span>
        </div>

        <div class="flex items-center justify-between mt-1 text-sm">
          <span class="text-gray-600 dark:text-gray-400">
            Média diária necessária
          </span>
          <span class="font-medium text-gray-900 dark:text-white">
            {{ formatCurrency(mediaNecessaria) }}/dia
          </span>
        </div>
      </div>

      <!-- Recomendação -->
      <div
        class="p-3 mt-4 rounded-lg"
        :class="recommendationClass"
      >
        <p class="text-sm" :class="recommendationTextClass">
          {{ recommendation }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import IconChartBar from '@/components/icons/ChartBarIcon.vue'

const props = defineProps({
  meta: {
    type: Number,
    required: true
  },
  realizado: {
    type: Number,
    required: true
  },
  projecao: {
    type: Number,
    required: true
  },
  probabilidade: {
    type: String,
    required: true
  },
  diasRestantes: {
    type: Number,
    required: true
  },
  mediaNecessaria: {
    type: Number,
    required: true
  }
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const progressoAtual = computed(() => {
  return (props.realizado / props.meta) * 100
})

const progressoProjetado = computed(() => {
  return (props.projecao / props.meta) * 100
})

const statusClass = computed(() => {
  if (progressoProjetado.value >= 100) {
    return 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300'
  }
  if (progressoProjetado.value >= 70) {
    return 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300'
  }
  if (progressoProjetado.value >= 50) {
    return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300'
  }
  return 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300'
})

const statusText = computed(() => {
  if (progressoProjetado.value >= 100) return 'Meta será atingida'
  if (progressoProjetado.value >= 70) return 'Provável'
  if (progressoProjetado.value >= 50) return 'Possível'
  return 'Crítico'
})

const progressBarClass = computed(() => {
  if (progressoAtual.value >= 100) return 'bg-green-500'
  if (progressoAtual.value >= 70) return 'bg-primary-500'
  if (progressoAtual.value >= 30) return 'bg-yellow-500'
  return 'bg-red-500'
})

const probabilityClass = computed(() => {
  if (progressoProjetado.value >= 100) return 'text-green-600 dark:text-green-400'
  if (progressoProjetado.value >= 70) return 'text-blue-600 dark:text-blue-400'
  if (progressoProjetado.value >= 50) return 'text-yellow-600 dark:text-yellow-400'
  return 'text-red-600 dark:text-red-400'
})

const recommendationClass = computed(() => {
  if (progressoProjetado.value >= 100) {
    return 'bg-green-50 dark:bg-green-900/20'
  }
  if (progressoProjetado.value >= 70) {
    return 'bg-blue-50 dark:bg-blue-900/20'
  }
  if (progressoProjetado.value >= 50) {
    return 'bg-yellow-50 dark:bg-yellow-900/20'
  }
  return 'bg-red-50 dark:bg-red-900/20'
})

const recommendationTextClass = computed(() => {
  if (progressoProjetado.value >= 100) {
    return 'text-green-800 dark:text-green-200'
  }
  if (progressoProjetado.value >= 70) {
    return 'text-blue-800 dark:text-blue-200'
  }
  if (progressoProjetado.value >= 50) {
    return 'text-yellow-800 dark:text-yellow-200'
  }
  return 'text-red-800 dark:text-red-200'
})

const recommendation = computed(() => {
  if (progressoProjetado.value >= 100) {
    return '🎉 Excelente! A meta será superada com folga.'
  }
  if (progressoProjetado.value >= 70) {
    return '📈 Bom progresso! Continue no ritmo para atingir a meta.'
  }
  if (progressoProjetado.value >= 50) {
    return '⚠️ Atenção! É necessário aumentar o ritmo para atingir a meta.'
  }
  return '🔴 Crítico! Ações urgentes são necessárias para reverter a situação.'
})
</script>