<!-- /* src/components/analysis/dashboard/RevenueSection.vue
 * RevenueSection.vue
 *
 * A Vue component that represents the "Faturamentos" section of the financial dashboard.
 * It includes KPIs, charts, and detailed analysis of revenues and billing.
 * The component is designed to be responsive and theme-aware, with export functionality for charts.
 */ -->
<template>
  <div class="space-y-6">
    <SectionHeader
      title="Faturamentos"
      subtitle="Análise detalhada de receitas e faturamento"
      :icon="IconBanknotes"
    >
      <template #actions>
        <ChartExportButton
          chart-id="revenue-charts"
          filename="faturamentos"
          @export="handleExport"
        />
      </template>
    </SectionHeader>

    <!-- KPIs de Faturamento -->
    <KpiGrid :kpis="revenueKpis" />

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Gráfico de Linhas - Evolução Anual -->
      <div class="p-6 bg-white shadow-sm lg:col-span-2 dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Evolução do Faturamento
        </h3>
        <LineChart
          :data="revenueTrend"
          height="300px"
          :loading="loading"
          filename="evolucao-faturamento"
          @export="handleExport"
        />
      </div>

      <!-- Melhor Desempenho -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Melhor Desempenho
        </h3>
        <div class="space-y-4">
          <div
            v-for="(unit, index) in topPerformers"
            :key="unit.id"
            class="flex items-center justify-between p-3 rounded-lg bg-gray-50 dark:bg-gray-700/50"
          >
            <div class="flex items-center gap-3">
              <div class="flex items-center justify-center w-6 h-6 text-sm font-medium rounded-full bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400">
                {{ index + 1 }}
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ unit.nome }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ unit.percentualMeta }}% da meta</p>
              </div>
            </div>
            <p class="text-sm font-bold text-gray-900 dark:text-white">
              {{ formatCurrency(unit.receita) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Gráfico de Rosca - Formas de Pagamento -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Formas de Pagamento
        </h3>
        <DoughnutChart
          :data="paymentMethods"
          height="250px"
          :loading="loading"
          filename="formas-pagamento"
          @export="handleExport"
        />
        <div class="grid grid-cols-2 gap-2 mt-4">
          <div
            v-for="method in paymentMethodsList"
            :key="method.name"
            class="flex items-center justify-between p-2 rounded bg-gray-50 dark:bg-gray-700/50"
          >
            <span class="text-xs text-gray-600 dark:text-gray-400">{{ method.name }}</span>
            <span class="text-xs font-medium text-gray-900 dark:text-white">{{ method.percentage }}%</span>
          </div>
        </div>
      </div>

      <!-- Gráfico de Colunas - Mensal -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            Faturamento Mensal
          </h3>
          <select
            v-model="selectedYear"
            class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1.5 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
          >
            <option v-for="year in availableYears" :key="year" :value="year">
              {{ year }}
            </option>
          </select>
        </div>
        <BarChart
          :data="monthlyRevenue"
          height="250px"
          :loading="loading"
          filename="faturamento-mensal"
          @export="handleExport"
        />
      </div>
    </div>

    <!-- Comparação entre Unidades -->
    <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
      <div class="flex items-center justify-between mb-4">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
          Comparativo entre Unidades
        </h3>
        <UnitMultiSelect
          v-model="selectedUnits"
          :units="availableUnits"
        />
      </div>
      <BarChart
        :data="unitComparison"
        height="300px"
        :loading="loading"
        filename="comparativo-unidades"
        @export="handleExport"
      />
    </div>

    <!-- Projeção -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <div class="lg:col-span-2">
        <ProjectionCard
          :meta="projection.meta"
          :realizado="projection.realizado"
          :projecao="projection.projecao"
          :probabilidade="projection.probabilidade"
          :dias-restantes="projection.diasRestantes"
          :media-necessaria="projection.mediaNecessaria"
        />
      </div>
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Insights Rápidos
        </h3>
        <div class="space-y-3">
          <div class="p-3 rounded-lg bg-blue-50 dark:bg-blue-900/20">
            <p class="text-sm text-blue-800 dark:text-blue-200">
              📈 Melhor dia: {{ bestDay }}
            </p>
          </div>
          <div class="p-3 rounded-lg bg-yellow-50 dark:bg-yellow-900/20">
            <p class="text-sm text-yellow-800 dark:text-yellow-200">
              ⚡ Ticket médio: {{ formatCurrency(averageTicket) }}
            </p>
          </div>
          <div class="p-3 rounded-lg bg-purple-50 dark:bg-purple-900/20">
            <p class="text-sm text-purple-800 dark:text-purple-200">
              🎯 Crescimento: {{ growth }}%
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, markRaw } from 'vue'
import SectionHeader from '../shared/SectionHeader.vue'
import KpiGrid from '../kpis/KpiGrid.vue'
import LineChart from '../charts/LineChart.vue'
import BarChart from '../charts/BarChart.vue'
import DoughnutChart from '../charts/DoughnutChart.vue'
import ChartExportButton from '../charts/ChartExportButton.vue'
import ProjectionCard from '../kpis/ProjectionCard.vue'
import UnitMultiSelect from '../filters/UnitMultiSelect.vue'
import IconBanknotes from '@/components/icons/BanknotesIcon.vue'

const props = defineProps({
  loading: {
    type: Boolean,
    default: false
  },
  data: {
    type: Object,
    default: () => ({ kpis: {} }) // 👈 GARANTE QUE NUNCA SEJA NULL
  }
})

const emit = defineEmits(['export'])

// KPIs de faturamento
const revenueKpis = computed(() => {
  // Cria uma variável local segura para não ler null toda hora
  const kpis = props.data?.kpis || {}

  return [
    {
      id: 'total-revenue',
      title: 'Faturamento Total',
      value: kpis.receitaTotal || 0,
      subtitle: 'Período atual',
      icon: markRaw(IconBanknotes),
      trend: 'up',
      trendValue: 12.5,
      progress: 78,
      iconColor: 'primary',
      format: 'currency' // 👈 Adicione formato para garantir
    },
    {
      id: 'avg-ticket',
      title: 'Ticket Médio',
      value: kpis.ticketMedioGeral || 0,
      subtitle: 'Por venda',
      icon: markRaw(IconBanknotes),
      trend: 'up',
      trendValue: 5.2,
      format: 'currency',
      iconColor: 'green'
    },
    {
      id: 'best-unit',
      title: 'Melhor Unidade',
      value: kpis.melhorUnidade?.nome || 'Nenhuma',
      subtitle: `R$ ${kpis.melhorUnidade?.receita || 0}`,
      icon: markRaw(IconBanknotes),
      format: 'text',
      iconColor: 'blue',
      // 👇 Remova progress e trend desse se não for usar
      trend: undefined, 
      progress: undefined
    },
    {
      id: 'growth',
      title: 'Crescimento',
      value: kpis.crescimentoPeriodo || 0,
      subtitle: 'vs período anterior',
      icon: markRaw(IconBanknotes),
      trend: (kpis.crescimentoPeriodo || 0) >= 0 ? 'up' : 'down',
      trendValue: Math.abs(kpis.crescimentoPeriodo || 0),
      format: 'percentage',
      iconColor: (kpis.crescimentoPeriodo || 0) >= 0 ? 'green' : 'red',
      progress: undefined
    }
  ]
})

// Dados simulados - substituir pelos dados reais da store
const revenueTrend = ref({
  labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
  datasets: [
    {
      label: 'Faturamento 2025',
      data: [65000, 72000, 68000, 81000, 79000, 85000, 92000, 88000, 95000, 101000, 108000, 115000],
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      fill: true
    },
    {
      label: 'Faturamento 2024',
      data: [58000, 61000, 59000, 72000, 70000, 75000, 81000, 78000, 84000, 89000, 95000, 102000],
      borderColor: '#94a3b8',
      borderDash: [5, 5],
      fill: false
    }
  ]
})

const topPerformers = ref([
  { id: 1, nome: 'Unidade Centro', receita: 125000, percentualMeta: 98 },
  { id: 2, nome: 'Unidade Norte', receita: 98000, percentualMeta: 92 },
  { id: 3, nome: 'Unidade Sul', receita: 87000, percentualMeta: 87 },
  { id: 4, nome: 'Unidade Leste', receita: 76000, percentualMeta: 81 },
  { id: 5, nome: 'Unidade Oeste', receita: 72000, percentualMeta: 78 }
])

const paymentMethods = ref({
  labels: ['Pix', 'Cartão Crédito', 'Cartão Débito', 'Dinheiro', 'Boleto'],
  datasets: [
    {
      data: [45, 30, 15, 7, 3],
      backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899']
    }
  ]
})

const paymentMethodsList = ref([
  { name: 'Pix', percentage: 45 },
  { name: 'Crédito', percentage: 30 },
  { name: 'Débito', percentage: 15 },
  { name: 'Dinheiro', percentage: 7 },
  { name: 'Boleto', percentage: 3 }
])

const selectedYear = ref(2025)
const availableYears = [2023, 2024, 2025]

const monthlyRevenue = computed(() => ({
  labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
  datasets: [
    {
      label: `Faturamento ${selectedYear.value}`,
      data: [65000, 72000, 68000, 81000, 79000, 85000, 92000, 88000, 95000, 101000, 108000, 115000],
      backgroundColor: '#3b82f6'
    }
  ]
}))

const selectedUnits = ref([])
const availableUnits = ref([
  { id: 1, name: 'Unidade Centro' },
  { id: 2, name: 'Unidade Norte' },
  { id: 3, name: 'Unidade Sul' },
  { id: 4, name: 'Unidade Leste' },
  { id: 5, name: 'Unidade Oeste' }
])

const unitComparison = computed(() => ({
  labels: selectedUnits.value.length > 0 
    ? availableUnits.value.filter(u => selectedUnits.value.includes(u.id)).map(u => u.name)
    : availableUnits.value.map(u => u.name),
  datasets: [
    {
      label: 'Faturamento',
      data: selectedUnits.value.length > 0
        ? [125000, 98000, 87000, 76000, 72000].filter((_, i) => 
            selectedUnits.value.includes(availableUnits.value[i].id))
        : [125000, 98000, 87000, 76000, 72000],
      backgroundColor: '#3b82f6'
    },
    {
      label: 'Meta',
      data: selectedUnits.value.length > 0
        ? [127000, 106000, 100000, 94000, 92000].filter((_, i) => 
            selectedUnits.value.includes(availableUnits.value[i].id))
        : [127000, 106000, 100000, 94000, 92000],
      backgroundColor: '#94a3b8'
    }
  ]
}))

const projection = ref({
  meta: 1000000,
  realizado: 780000,
  projecao: 950000,
  probabilidade: 'Provável',
  diasRestantes: 7,
  mediaNecessaria: 31428.57
})

const bestDay = ref('15/01/2025 - R$ 45.678')
const averageTicket = ref(1250)
const growth = ref(12.5)

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 0
  }).format(value)
}

const handleExport = ({ format, data }) => {
  emit('export', { section: 'revenue', format, data })
}
</script>