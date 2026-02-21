<!--/* src/components/analysis/dashboard/RequestsSection.vue
 * RequestsSection.vue
 *
 * A Vue component that represents the "Solicitações" section of the financial dashboard.
 * It includes KPIs, charts, and a detailed table to analyze requests by status and unit.
 * The component is designed to be responsive and theme-aware, with export functionality for charts.
 */ -->
<template>
  <div class="space-y-6">
    <SectionHeader
      title="Lista de Solicitações"
      subtitle="Controle de fechamentos e solicitações"
      :icon="IconDocumentText"
    >
      <template #actions>
        <ChartExportButton
          chart-id="requests-charts"
          filename="solicitacoes"
          @export="handleExport"
        />
      </template>
    </SectionHeader>

    <!-- KPIs de Solicitações -->
    <KpiGrid :kpis="requestsKpis" />

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Gráfico de Rosca - Status -->
      <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Distribuição por Status
        </h3>
        <DoughnutChart
          :data="requestsByStatus"
          height="250px"
          :loading="loading"
          filename="solicitacoes-status"
          @export="handleExport"
        />
        <div class="mt-4 space-y-2">
          <div
            v-for="status in statusDistribution"
            :key="status.name"
            class="flex items-center justify-between"
          >
            <div class="flex items-center gap-2">
              <div class="w-3 h-3 rounded-full" :style="{ backgroundColor: status.color }"></div>
              <span class="text-sm text-gray-700 dark:text-gray-300">{{ status.name }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{ status.count }}</span>
              <span class="text-xs text-gray-500 dark:text-gray-400">({{ status.percentage }}%)</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Tendência de Solicitações -->
      <div class="p-6 bg-white shadow-sm lg:col-span-2 dark:bg-gray-800 rounded-xl">
        <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          Tendência de Solicitações
        </h3>
        <LineChart
          :data="requestsTrend"
          height="250px"
          :loading="loading"
          filename="tendencia-solicitacoes"
          @export="handleExport"
        />
      </div>
    </div>

    <!-- Tabela de Solicitações -->
    <div class="p-6 bg-white shadow-sm dark:bg-gray-800 rounded-xl">
      <RequestsTable
        :items="requestsList"
        :status-filter="statusFilter"
        @update:status-filter="statusFilter = $event"
        @view="viewRequest"
      />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SectionHeader from '../shared/SectionHeader.vue'
import KpiGrid from '../kpis/KpiGrid.vue'
import DoughnutChart from '../charts/DoughnutChart.vue'
import LineChart from '../charts/LineChart.vue'
import ChartExportButton from '../charts/ChartExportButton.vue'
import RequestsTable from '../tables/RequestsTable.vue'
import IconDocumentText from '@/components/icons/DocumentTextIcon.vue'

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

const emit = defineEmits(['export', 'view-request'])

const statusFilter = ref('all')

// KPIs de solicitações
const requestsKpis = [
  {
    id: 'total-requests',
    title: 'Total Solicitações',
    value: 156,
    subtitle: 'Período atual',
    icon: IconDocumentText,
    trend: 'up',
    trendValue: 12.5,
    format: 'number',
    iconColor: 'primary'
  },
  {
    id: 'pending-requests',
    title: 'Pendentes',
    value: 23,
    subtitle: 'Aguardando análise',
    icon: IconDocumentText,
    format: 'number',
    iconColor: 'yellow'
  },
  {
    id: 'approved-requests',
    title: 'Aprovados',
    value: 89,
    subtitle: 'Taxa 57%',
    icon: IconDocumentText,
    format: 'number',
    iconColor: 'green'
  },
  {
    id: 'rejected-requests',
    title: 'Rejeitados',
    value: 44,
    subtitle: 'Taxa 28%',
    icon: IconDocumentText,
    format: 'number',
    iconColor: 'red'
  }
]

const requestsByStatus = ref({
  labels: ['Aprovados', 'Rejeitados', 'Pendentes', 'Cancelados'],
  datasets: [
    {
      data: [89, 44, 23, 12],
      backgroundColor: ['#10b981', '#ef4444', '#f59e0b', '#6b7280']
    }
  ]
})

const statusDistribution = ref([
  { name: 'Aprovados', count: 89, percentage: 53, color: '#10b981' },
  { name: 'Rejeitados', count: 44, percentage: 26.2, color: '#ef4444' },
  { name: 'Pendentes', count: 23, percentage: 13.7, color: '#f59e0b' },
  { name: 'Cancelados', count: 12, percentage: 7.1, color: '#6b7280' }
])

const requestsTrend = ref({
  labels: ['Semana 1', 'Semana 2', 'Semana 3', 'Semana 4', 'Semana 5'],
  datasets: [
    {
      label: 'Solicitações',
      data: [32, 35, 38, 42, 45],
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      fill: true
    },
    {
      label: 'Média Móvel',
      data: [30, 33, 36, 40, 43],
      borderColor: '#10b981',
      borderDash: [5, 5],
      fill: false
    }
  ]
})

const requestsList = ref([
  {
    id: 'req_001',
    tipo: 'Remoção',
    unidade: 'Unidade Centro',
    solicitante: 'João Silva',
    cargo: 'Gerente',
    valor: 25000,
    data: '2025-01-15',
    status: 'Pendente'
  },
  {
    id: 'req_002',
    tipo: 'Alteração',
    unidade: 'Unidade Norte',
    solicitante: 'Maria Santos',
    cargo: 'Supervisora',
    valor: 15000,
    data: '2025-01-14',
    status: 'Aprovado'
  },
  {
    id: 'req_003',
    tipo: 'Adição',
    unidade: 'Unidade Sul',
    solicitante: 'Pedro Oliveira',
    cargo: 'Coordenador',
    valor: 45000,
    data: '2025-01-14',
    status: 'Rejeitado'
  },
  {
    id: 'req_004',
    tipo: 'Remoção',
    unidade: 'Unidade Leste',
    solicitante: 'Ana Costa',
    cargo: 'Gerente',
    valor: 18000,
    data: '2025-01-13',
    status: 'Cancelado'
  },
  {
    id: 'req_005',
    tipo: 'Alteração',
    unidade: 'Unidade Oeste',
    solicitante: 'Carlos Souza',
    cargo: 'Analista',
    valor: 8500,
    data: '2025-01-13',
    status: 'Pendente'
  },
  {
    id: 'req_006',
    tipo: 'Adição',
    unidade: 'Unidade Centro',
    solicitante: 'João Silva',
    cargo: 'Gerente',
    valor: 32000,
    data: '2025-01-12',
    status: 'Aprovado'
  }
])

const handleExport = ({ format, data }) => {
  emit('export', { section: 'requests', format, data })
}

const viewRequest = (request) => {
  emit('view-request', request)
}
</script>