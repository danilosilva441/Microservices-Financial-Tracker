<!-- /* src/components/analysis/dashboard/ProfitSection.vue
 * ProfitSection.vue
 *
 * A Vue component that represents the "Lucros" section of the financial dashboard.
 * It includes KPIs, charts, and detailed analysis of profitability and margins.
 * The component is designed to be responsive and theme-aware, with export functionality for charts.
 */ --> 
<template>
  <div class="space-y-6">
    <SectionHeader
      title="Lucros"
      subtitle="Análise de rentabilidade e margens"
      :icon="IconChartBar"
    >
      <template #actions>
        <ChartExportButton
          chart-id="profit-charts"
          filename="lucros"
          @export="handleExport"
        />
      </template>
    </SectionHeader>

    <!-- KPIs de Lucro -->
    <KpiGrid :kpis="profitKpis" />

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Gráfico de Barras - Lucro por Unidade -->
      <div class="p-6 bg-white shadow-sm lg:col-span-2 dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Lucro por Unidade
        </h3>
        <BarChart
          :data="profitByUnit"
          height="300px"
          :loading="loading"
          filename="lucro-unidades"
          @export="handleExport"
        />
      </div>

      <!-- Margem de Lucro -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Margem de Lucro
        </h3>
        <div class="mb-6 text-center">
          <span class="text-4xl font-bold" :class="marginClass">
            {{ profitMargin }}%
          </span>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            Margem líquida média
          </p>
        </div>
        <div class="space-y-4">
          <div
            v-for="unit in profitMarginsByUnit"
            :key="unit.id"
            class="flex items-center justify-between"
          >
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ unit.name }}</span>
            <div class="flex items-center gap-3">
              <div class="w-24 h-2 bg-gray-200 rounded-full dark:bg-gray-700">
                <div
                  class="h-full rounded-full"
                  :class="getMarginBarClass(unit.margin)"
                  :style="{ width: `${unit.margin}%` }"
                ></div>
              </div>
              <span class="text-sm font-medium" :class="getMarginTextClass(unit.margin)">
                {{ unit.margin }}%
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Fatores que Impactam o Lucro -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Fatores que Impactam o Lucro
        </h3>
        <div class="space-y-4">
          <div
            v-for="factor in impactFactors"
            :key="factor.name"
            class="p-4 rounded-lg"
            :class="factor.impact === 'positive' ? 'bg-green-50 dark:bg-green-900/20' : 'bg-red-50 dark:bg-red-900/20'"
          >
            <div class="flex items-center justify-between mb-2">
              <div class="flex items-center gap-2">
                <Component
                  :is="factor.impact === 'positive' ? IconArrowTrendingUp : IconArrowTrendingDown"
                  class="w-5 h-5"
                  :class="factor.impact === 'positive' ? 'text-green-600' : 'text-red-600'"
                />
                <span class="font-medium text-gray-900 dark:text-white">{{ factor.name }}</span>
              </div>
              <span
                class="text-sm font-bold"
                :class="factor.impact === 'positive' ? 'text-green-600' : 'text-red-600'"
              >
                {{ factor.impact === 'positive' ? '+' : '-' }}{{ factor.value }}%
              </span>
            </div>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ factor.description }}</p>
          </div>
        </div>
      </div>

      <!-- Comparativo Receita vs Despesa -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Receita vs Despesa
        </h3>
        <LineChart
          :data="revenueVsExpenses"
          height="250px"
          :loading="loading"
          filename="receita-vs-despesa"
          @export="handleExport"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import SectionHeader from '../shared/SectionHeader.vue'
import KpiGrid from '../kpis/KpiGrid.vue'
import BarChart from '../charts/BarChart.vue'
import LineChart from '../charts/LineChart.vue'
import ChartExportButton from '../charts/ChartExportButton.vue'
import IconChartBar from '@/components/icons/ChartBarIcon.vue'
import IconArrowTrendingUp from '@/components/icons/ArrowTrendingUpIcon.vue'
import IconArrowTrendingDown from '@/components/icons/ArrowTrendingDownIcon.vue'

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

// KPIs de lucro
const profitKpis = computed(() => [
  {
    id: 'total-profit',
    title: 'Lucro Total',
    value: props.data?.kpis?.lucroTotal || 0,
    subtitle: 'Período atual',
    icon: IconChartBar,
    trend: 'up',
    trendValue: 15.2,
    progress: 82,
    iconColor: 'green'
  },
  {
    id: 'profit-margin',
    title: 'Margem de Lucro',
    value: 28.5,
    subtitle: 'Média do período',
    icon: IconChartBar,
    format: 'percentage',
    iconColor: 'blue'
  },
  {
    id: 'best-profit',
    title: 'Melhor Margem',
    value: 'Unidade Centro',
    subtitle: '32% de margem',
    icon: IconChartBar,
    format: 'text',
    iconColor: 'purple'
  },
  {
    id: 'profit-growth',
    title: 'Crescimento',
    value: 18.3,
    subtitle: 'vs período anterior',
    icon: IconChartBar,
    trend: 'up',
    trendValue: 18.3,
    format: 'percentage',
    iconColor: 'green'
  }
])

const profitByUnit = ref({
  labels: ['Centro', 'Norte', 'Sul', 'Leste', 'Oeste'],
  datasets: [
    {
      label: 'Lucro',
      data: [125000, 88000, 67000, 56000, 52000],
      backgroundColor: '#10b981'
    },
    {
      label: 'Receita',
      data: [245000, 198000, 167000, 145000, 138000],
      backgroundColor: '#94a3b8'
    }
  ]
})

const profitMargin = ref(28.5)

const profitMarginsByUnit = ref([
  { id: 1, name: 'Unidade Centro', margin: 32 },
  { id: 2, name: 'Unidade Norte', margin: 28 },
  { id: 3, name: 'Unidade Sul', margin: 26 },
  { id: 4, name: 'Unidade Leste', margin: 24 },
  { id: 5, name: 'Unidade Oeste', margin: 22 }
])

const marginClass = computed(() => {
  if (profitMargin.value >= 30) return 'text-green-600 dark:text-green-400'
  if (profitMargin.value >= 20) return 'text-yellow-600 dark:text-yellow-400'
  return 'text-red-600 dark:text-red-400'
})

const getMarginBarClass = (margin) => {
  if (margin >= 30) return 'bg-green-500'
  if (margin >= 20) return 'bg-yellow-500'
  return 'bg-red-500'
}

const getMarginTextClass = (margin) => {
  if (margin >= 30) return 'text-green-600 dark:text-green-400'
  if (margin >= 20) return 'text-yellow-600 dark:text-yellow-400'
  return 'text-red-600 dark:text-red-400'
}

const impactFactors = ref([
  {
    name: 'Aumento de Vendas',
    impact: 'positive',
    value: 15.2,
    description: 'Crescimento nas vendas da Unidade Centro e Norte'
  },
  {
    name: 'Redução de Despesas',
    impact: 'positive',
    value: 8.5,
    description: 'Otimização de custos operacionais'
  },
  {
    name: 'Aumento de Aluguel',
    impact: 'negative',
    value: 5.3,
    description: 'Reajuste contratual em 3 unidades'
  },
  {
    name: 'Queda em Suprimentos',
    impact: 'negative',
    value: 3.8,
    description: 'Aumento no custo de matérias-primas'
  }
])

const revenueVsExpenses = ref({
  labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
  datasets: [
    {
      label: 'Receita',
      data: [245000, 278000, 298000, 315000, 342000, 368000],
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      fill: true
    },
    {
      label: 'Despesa',
      data: [178000, 192000, 205000, 218000, 235000, 251000],
      borderColor: '#ef4444',
      backgroundColor: 'rgba(239, 68, 68, 0.1)',
      fill: true
    }
  ]
})

const handleExport = ({ format, data }) => {
  emit('export', { section: 'profit', format, data })
}
</script>