<!--
 * src/components/analysis/tables/PerformanceTable.vue
 * PerformanceTable.vue
 *
 * A Vue component that displays a performance table for financial analysis.
 * It includes features like unit icons, percentage bars, and status indicators.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="overflow-hidden bg-white shadow-sm dark:bg-gray-800 rounded-xl">
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700/50">
          <tr>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Unidade
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Receita
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Despesa
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Lucro
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Meta
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              % Meta
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              Status
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr
            v-for="item in items"
            :key="item.id"
            class="transition-colors hover:bg-gray-50 dark:hover:bg-gray-700/50"
          >
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="flex items-center justify-center flex-shrink-0 w-8 h-8 rounded-lg bg-primary-100 dark:bg-primary-900/30">
                  <IconBuildingOffice class="w-4 h-4 text-primary-600 dark:text-primary-400" />
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ item.nome }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    ID: {{ item.id.slice(0, 8) }}
                  </div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-right text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatCurrency(item.receita) }}
            </td>
            <td class="px-6 py-4 text-sm text-right text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatCurrency(item.despesa) }}
            </td>
            <td class="px-6 py-4 text-sm font-medium text-right whitespace-nowrap" :class="getLucroClass(item.lucro)">
              {{ formatCurrency(item.lucro) }}
            </td>
            <td class="px-6 py-4 text-sm text-right text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatCurrency(item.metaMensal) }}
            </td>
            <td class="px-6 py-4 text-center whitespace-nowrap">
              <div class="flex items-center justify-center gap-2">
                <div class="w-16 h-2 bg-gray-200 rounded-full dark:bg-gray-700">
                  <div
                    class="h-full rounded-full"
                    :class="getProgressBarClass(item.percentualMeta)"
                    :style="{ width: `${Math.min(item.percentualMeta, 100)}%` }"
                  ></div>
                </div>
                <span class="text-sm font-medium" :class="getPercentClass(item.percentualMeta)">
                  {{ item.percentualMeta.toFixed(1) }}%
                </span>
              </div>
            </td>
            <td class="px-6 py-4 text-center whitespace-nowrap">
              <span
                class="px-2 py-1 text-xs font-medium rounded-full"
                :class="getStatusClass(item.status)"
              >
                {{ getStatusText(item.status) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="items.length === 0" class="p-8">
      <EmptyState
        title="Nenhuma unidade encontrada"
        message="Não há dados de desempenho para exibir no momento."
        type="table"
      />
    </div>
  </div>
</template>

<script setup>
import EmptyState from '../shared/EmptyState.vue'
import IconBuildingOffice from '@/components/icons/BuildingOfficeIcon.vue'

defineProps({
  items: {
    type: Array,
    required: true
  }
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 0
  }).format(value)
}

const getLucroClass = (lucro) => {
  if (lucro > 0) return 'text-green-600 dark:text-green-400'
  if (lucro < 0) return 'text-red-600 dark:text-red-400'
  return 'text-gray-900 dark:text-white'
}

const getProgressBarClass = (percentual) => {
  if (percentual >= 100) return 'bg-green-500'
  if (percentual >= 70) return 'bg-primary-500'
  if (percentual >= 30) return 'bg-yellow-500'
  return 'bg-red-500'
}

const getPercentClass = (percentual) => {
  if (percentual >= 100) return 'text-green-600 dark:text-green-400'
  if (percentual >= 70) return 'text-primary-600 dark:text-primary-400'
  if (percentual >= 30) return 'text-yellow-600 dark:text-yellow-400'
  return 'text-red-600 dark:text-red-400'
}

const getStatusClass = (status) => {
  const classes = {
    success: 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300',
    warning: 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300',
    danger: 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300',
    neutro: 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300'
  }
  return classes[status] || classes.neutro
}

const getStatusText = (status) => {
  const texts = {
    success: 'Acima da meta',
    warning: 'Atenção',
    danger: 'Crítico',
    neutro: 'Neutro'
  }
  return texts[status] || status
}
</script>