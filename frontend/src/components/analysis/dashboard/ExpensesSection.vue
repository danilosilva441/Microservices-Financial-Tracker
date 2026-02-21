<!-- /* src/components/analysis/dashboard/ExpensesSection.vue
 * ExpensesSection.vue
 *
 * A Vue component that represents the "Despesas" section of the financial dashboard.
 * It includes KPIs, charts, and a detailed table to analyze expenses by category and unit.
 * The component is designed to be responsive and theme-aware, with export functionality for charts.
 */ -->
<template>
  <div class="space-y-6">
    <SectionHeader
      title="Despesas"
      subtitle="Análise de gastos e categorias"
      :icon="CreditCardIcon"
    >
      <template #actions>
        <ChartExportButton
          chart-id="expenses-charts"
          filename="despesas"
          @export="handleExport"
        />
      </template>
    </SectionHeader>

    <!-- KPIs de Despesas -->
    <KpiGrid :kpis="expensesKpis" />

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Gráfico de Rosca - Categorias -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Despesas por Categoria
        </h3>
        <DoughnutChart
          :data="expensesByCategory"
          height="300px"
          :loading="loading"
          filename="despesas-categorias"
          @export="handleExport"
        />
        <div class="mt-4 space-y-2">
          <div
            v-for="category in topExpenseCategories"
            :key="category.name"
            class="flex items-center justify-between p-2 rounded bg-gray-50 dark:bg-gray-700/50"
          >
            <div class="flex items-center gap-2">
              <div class="w-3 h-3 rounded-full" :style="{ backgroundColor: category.color }"></div>
              <span class="text-sm text-gray-700 dark:text-gray-300">{{ category.name }}</span>
            </div>
            <div class="text-right">
              <span class="text-sm font-medium text-gray-900 dark:text-white">
                {{ formatCurrency(category.value) }}
              </span>
              <span class="ml-2 text-xs text-gray-500 dark:text-gray-400">
                ({{ category.percentage }}%)
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Maiores Gastos por Unidade -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Maiores Gastos por Unidade
        </h3>
        <div class="space-y-4">
          <div
            v-for="expense in topUnitExpenses"
            :key="expense.id"
            class="p-4 rounded-lg bg-gray-50 dark:bg-gray-700/50"
          >
            <div class="flex items-center justify-between mb-2">
              <div class="flex items-center gap-2">
                <IconBuildingOffice class="w-5 h-5 text-gray-400" />
                <span class="font-medium text-gray-900 dark:text-white">{{ expense.unit }}</span>
              </div>
              <span class="text-sm font-bold text-red-600 dark:text-red-400">
                {{ formatCurrency(expense.total) }}
              </span>
            </div>
            <div class="space-y-2">
              <div
                v-for="item in expense.items.slice(0, 3)"
                :key="item.name"
                class="flex items-center justify-between text-sm"
              >
                <span class="text-gray-600 dark:text-gray-400">{{ item.name }}</span>
                <span class="text-gray-900 dark:text-white">{{ formatCurrency(item.value) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabela de Despesas -->
    <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
      <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        Detalhamento de Despesas
      </h3>
      <ExpensesTable :items="expensesList" />
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import SectionHeader from '../shared/SectionHeader.vue'
import KpiGrid from '../kpis/KpiGrid.vue'
import DoughnutChart from '../charts/DoughnutChart.vue'
import ChartExportButton from '../charts/ChartExportButton.vue'
import ExpensesTable from '../tables/ExpensesTable.vue'
import IconBuildingOffice from '@/components/icons/BuildingOfficeIcon.vue'
import IconCreditCard from '@/components/icons/CreditCardIcon.vue'

const props = defineProps({
  loading: {
    type: Boolean,
    default: false
  },
  data: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['export'])

// KPIs de despesas
const expensesKpis = computed(() => [
  {
    id: 'total-expenses',
    title: 'Despesas Totais',
    value: props.data?.kpis?.despesaTotal || 0,
    subtitle: 'Período atual',
    icon: IconCreditCard,
    trend: 'up',
    trendValue: 8.3,
    progress: 65,
    iconColor: 'red'
  },
  {
    id: 'avg-expense',
    title: 'Média por Unidade',
    value: (props.data?.kpis?.despesaTotal || 0) / (props.data?.kpis?.unidadesAtivas || 1),
    subtitle: 'Por unidade',
    icon: IconCreditCard,
    format: 'currency',
    iconColor: 'yellow'
  },
  {
    id: 'top-category',
    title: 'Maior Categoria',
    value: 'Salários',
    subtitle: 'R$ 245.678',
    icon: IconCreditCard,
    format: 'text',
    iconColor: 'blue'
  },
  {
    id: 'expense-ratio',
    title: '% da Receita',
    value: 42,
    subtitle: 'Percentual comprometido',
    icon: IconCreditCard,
    format: 'percentage',
    iconColor: 'purple'
  }
])

const expensesByCategory = ref({
  labels: ['Salários', 'Aluguel', 'Energia', 'Internet', 'Logística', 'Suprimentos'],
  datasets: [
    {
      data: [245678, 120000, 45000, 12000, 68000, 89000],
      backgroundColor: ['#ef4444', '#f97316', '#eab308', '#3b82f6', '#8b5cf6', '#ec4899']
    }
  ]
})

const topExpenseCategories = ref([
  { name: 'Salários', value: 245678, percentage: 42, color: '#ef4444' },
  { name: 'Aluguel', value: 120000, percentage: 20.5, color: '#f97316' },
  { name: 'Logística', value: 68000, percentage: 11.6, color: '#8b5cf6' },
  { name: 'Suprimentos', value: 89000, percentage: 15.2, color: '#ec4899' },
  { name: 'Energia', value: 45000, percentage: 7.7, color: '#eab308' }
])

const topUnitExpenses = ref([
  {
    id: 1,
    unit: 'Unidade Centro',
    total: 245678,
    items: [
      { name: 'Salários', value: 120000 },
      { name: 'Aluguel', value: 45000 },
      { name: 'Energia', value: 18000 }
    ]
  },
  {
    id: 2,
    unit: 'Unidade Norte',
    total: 198456,
    items: [
      { name: 'Salários', value: 98000 },
      { name: 'Logística', value: 35000 },
      { name: 'Aluguel', value: 28000 }
    ]
  },
  {
    id: 3,
    unit: 'Unidade Sul',
    total: 167890,
    items: [
      { name: 'Salários', value: 82000 },
      { name: 'Suprimentos', value: 42000 },
      { name: 'Aluguel', value: 25000 }
    ]
  }
])

const expensesList = ref([
  {
    id: 1,
    category: 'Salários',
    unidade: 'Unidade Centro',
    valor: 120000,
    percentual: 42.5,
    variacao: 5.2,
    color: '#ef4444'
  },
  {
    id: 2,
    category: 'Aluguel',
    unidade: 'Unidade Centro',
    valor: 45000,
    percentual: 15.9,
    variacao: 0,
    color: '#f97316'
  },
  {
    id: 3,
    category: 'Energia',
    unidade: 'Unidade Norte',
    valor: 18000,
    percentual: 9.1,
    variacao: -2.5,
    color: '#eab308'
  },
  {
    id: 4,
    category: 'Logística',
    unidade: 'Unidade Norte',
    valor: 35000,
    percentual: 17.6,
    variacao: 12.3,
    color: '#8b5cf6'
  },
  {
    id: 5,
    category: 'Suprimentos',
    unidade: 'Unidade Sul',
    valor: 42000,
    percentual: 25.0,
    variacao: 8.7,
    color: '#ec4899'
  }
])

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const handleExport = ({ format, data }) => {
  emit('export', { section: 'expenses', format, data })
}
</script>