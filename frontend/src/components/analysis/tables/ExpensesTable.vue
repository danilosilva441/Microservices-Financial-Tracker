<!--
 * src/components/analysis/tables/ExpensesTable.vue
 * ExpensesTable.vue
 *
 * A Vue component that displays a table of expenses in the financial analysis dashboard.
 * It includes features like category icons, percentage bars, and variation indicators.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="overflow-hidden bg-white shadow-sm dark:bg-gray-800 rounded-xl">
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700/50">
          <tr>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Categoria
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Unidade
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Valor
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              % do Total
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              Variação
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
                <div
                  class="flex items-center justify-center w-8 h-8 rounded-lg"
                  :style="{ backgroundColor: item.color + '20' }"
                >
                  <Component
                    :is="getCategoryIcon(item.category)"
                    class="w-4 h-4"
                    :style="{ color: item.color }"
                  />
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900 dark:text-white">
                  {{ item.category }}
                </span>
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
              {{ item.unidade }}
            </td>
            <td class="px-6 py-4 text-sm text-right text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatCurrency(item.valor) }}
            </td>
            <td class="px-6 py-4 text-right whitespace-nowrap">
              <div class="flex items-center justify-end gap-2">
                <span class="text-sm text-gray-900 dark:text-white">{{ item.percentual.toFixed(1) }}%</span>
                <div class="w-16 h-2 bg-gray-200 rounded-full dark:bg-gray-700">
                  <div
                    class="h-full rounded-full bg-primary-500"
                    :style="{ width: `${item.percentual}%` }"
                  ></div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 text-center whitespace-nowrap">
              <span
                class="inline-flex items-center gap-1 text-sm"
                :class="getVariationClass(item.variacao)"
              >
                <IconArrowTrendingUp v-if="item.variacao > 0" class="w-4 h-4" />
                <IconArrowTrendingDown v-if="item.variacao < 0" class="w-4 h-4" />
                <IconMinus v-if="item.variacao === 0" class="w-4 h-4" />
                {{ Math.abs(item.variacao) }}%
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="items.length === 0" class="p-8">
      <EmptyState
        title="Nenhuma despesa encontrada"
        message="Não há dados de despesas para exibir no momento."
        type="table"
      />
    </div>
  </div>
</template>

<script setup>
import EmptyState from '../shared/EmptyState.vue'
import IconBuildingOffice from '@/components/icons/BuildingOfficeIcon.vue'
import IconArrowTrendingUp from '@/components/icons/ArrowTrendingUpIcon.vue'
import IconArrowTrendingDown from '@/components/icons/ArrowTrendingDownIcon.vue'
import IconMinus from '@/components/icons/MinusIcon.vue'

// Ícones para categorias (podem ser criados ou usar SVGs inline)
const HomeIcon = 'svg' // Placeholder - substitua pelo ícone real se existir
const UserGroupIcon = 'svg'
const BoltIcon = 'svg'
const WifiIcon = 'svg'
const TruckIcon = 'svg'
const ShoppingBagIcon = 'svg'

defineProps({
  items: {
    type: Array,
    required: true
  }
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const getCategoryIcon = (category) => {
  const icons = {
    'Aluguel': HomeIcon,
    'Salários': UserGroupIcon,
    'Energia': BoltIcon,
    'Internet': WifiIcon,
    'Logística': TruckIcon,
    'Suprimentos': ShoppingBagIcon
  }
  return icons[category] || IconBuildingOffice
}

const getVariationClass = (variacao) => {
  if (variacao > 0) return 'text-red-600 dark:text-red-400'
  if (variacao < 0) return 'text-green-600 dark:text-green-400'
  return 'text-gray-600 dark:text-gray-400'
}
</script>