<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 analytics-view-container">
    <div class="px-4 py-8 mx-auto max-w-7xl sm:px-6 lg:px-8">
      
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
          Análise e BI
        </h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">
          Dashboard completo com KPIs, gráficos e análise de desempenho
        </p>
      </div>

      <FilterBar
        v-model="filters"
        :units="availableUnits"
        @apply="handleFilterApply"
        @reset="handleFilterReset"
      />

      <div class="flex items-center justify-end mb-4 text-sm text-gray-600 dark:text-gray-400">
        <span>Última atualização: {{ lastUpdatedFormatted }}</span>
        <button
          @click="handleRefresh"
          class="p-1 ml-2 transition-colors rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700"
          :class="{ 'animate-spin': loading }"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>
      </div>

      <div v-if="loading" class="flex justify-center py-12">
        <LoadingSpinner />
      </div>

      <div v-else-if="error" class="py-12">
        <ErrorState
          :message="error"
          @retry="handleRefresh"
        />
      </div>

      <div v-else-if="dashboardData" class="space-y-12">
        
        <RevenueSection
          :loading="loading"
          :data="dashboardData"
          @export="handleExport"
        />

        <ExpensesSection
          :loading="loading"
          :data="dashboardData"
          @export="handleExport"
        />

        <ProfitSection
          :loading="loading"
          :data="dashboardData"
          @export="handleExport"
        />

        <RequestsSection
          :loading="loading"
          :data="dashboardData"
          @export="handleExport"
          @view-request="handleViewRequest"
        />
      </div>
      
      <div v-else class="py-12">
        <EmptyState 
           title="Nenhum dado encontrado" 
           message="Tente ajustar os filtros ou recarregar a página." 
        />
      </div>
      
    </div>
  </div>
</template>

<script setup>
import { onMounted, computed } from 'vue'
import { useAnalysis } from '@/composables/analysis'

// Importações baseadas na sua estrutura de pastas real
import FilterBar from '@/components/analysis/filters/FilterBar.vue'
import LoadingSpinner from '@/components/analysis/shared/LoadingSpinner.vue'
import ErrorState from '@/components/analysis/shared/ErrorState.vue'
import EmptyState from '@/components/analysis/shared/EmptyState.vue'

import RevenueSection from '@/components/analysis/dashboard/RevenueSection.vue'
import ExpensesSection from '@/components/analysis/dashboard/ExpensesSection.vue'
import ProfitSection from '@/components/analysis/dashboard/ProfitSection.vue'
import RequestsSection from '@/components/analysis/dashboard/RequestsSection.vue'

const {
  dashboardData,
  loading,
  error,
  filters,
  lastUpdated,
  fetchDashboardData,
  refreshData,
  updateFilters,
  resetFilters
} = useAnalysis()

// Unidades disponíveis para filtro
const availableUnits = computed(() => {
  // O uso do optional chaining (?.) garante que o Vue não quebre se a API não retornar esse objeto
  return dashboardData.value?.data?.desempenho?.todas?.map(unit => ({
    id: unit.id,
    name: unit.nome
  })) || []
})

// Formatar última atualização
const lastUpdatedFormatted = computed(() => {
  if (!lastUpdated.value) return 'Nunca'
  return new Date(lastUpdated.value).toLocaleString('pt-BR')
})

// Handlers
const handleFilterApply = (newFilters) => {
  updateFilters(newFilters)
}

const handleFilterReset = () => {
  resetFilters()
}

const handleRefresh = () => {
  refreshData()
}

const handleExport = ({ section, format, data }) => {
  console.log('Exportando:', { section, format, data })
}

const handleViewRequest = (request) => {
  console.log('Visualizar solicitação:', request)
}

onMounted(() => {
  fetchDashboardData()
})
</script>