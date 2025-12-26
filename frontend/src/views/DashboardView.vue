<script setup>
import { ref, onMounted, computed } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore';

// Componentes
import DashboardHeader from '@/components/dashboard/DashboardHeader.vue';
import DashboardTabs from '@/components/dashboard/DashboardTabs.vue';
import BenchmarkCards from '@/components/dashboard/BenchmarkCards.vue'; // ✅ Novo
import ExportModal from '@/components/common/ExportModal.vue';       // ✅ Novo
import LoadingState from '@/components/common/LoadingState.vue';
import ErrorState from '@/components/common/ErrorState.vue';

// Tabs
import TabGeral from '@/components/dashboard/tabs/TabGeral.vue';
import TabFinanceiro from '@/components/dashboard/tabs/TabFinanceiro.vue';

const dashboardStore = useDashboardStore();
const activeTab = ref('geral');
const showExportModal = ref(false); // ✅ Controle do modal

const filters = ref({ periodo: 'month' });

const refreshData = () => dashboardStore.fetchDashboardData(filters.value);

onMounted(() => refreshData());

const handleTabChange = (tab) => activeTab.value = tab;
const handleFilterChange = (newFilters) => {
  filters.value = { ...filters.value, ...newFilters };
  refreshData();
};

// Prepara dados para exportação
const exportData = computed(() => ({
  kpis: dashboardStore.kpis,
  // Achata os dados para tabela
  tableData: dashboardStore.desempenho.todas.map(u => ({
    Unidade: u.nome,
    Receita: u.receita,
    Despesa: u.despesa,
    Lucro: u.lucro,
    Status: u.status
  }))
}));
</script>

<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900 p-4 lg:p-8 transition-colors duration-300">
    
    <ExportModal 
      :is-open="showExportModal" 
      :data-to-export="exportData"
      @close="showExportModal = false"
    />

    <DashboardHeader 
      :title="`Dashboard - ${dashboardStore.period.mesAtual || 'Geral'}`"
      :is-loading="dashboardStore.isLoading"
      @filter-change="handleFilterChange"
      @refresh="refreshData"
      @open-export="showExportModal = true"
    />

    <div v-if="dashboardStore.isLoading" class="mt-8">
      <LoadingState text="Calculando indicadores..." />
    </div>

    <div v-else-if="dashboardStore.error" class="mt-8">
      <ErrorState :message="dashboardStore.error" @retry="refreshData" />
    </div>

    <div v-else class="mt-6">
      
      <BenchmarkCards 
        :kpis="dashboardStore.kpis" 
        :benchmark="dashboardStore.kpis.benchmark" 
      />

      <DashboardTabs :active-tab="activeTab" @change="handleTabChange" />

      <div class="bg-white dark:bg-slate-800 rounded-b-xl rounded-tr-xl shadow-sm border border-gray-200 dark:border-slate-700 p-6 min-h-[500px] transition-colors">
        
        <TabGeral v-if="activeTab === 'geral'" />

        <TabFinanceiro 
          v-if="activeTab === 'faturamento'"
          type="receita"
          title="Detalhamento de Faturamento"
          color="green"
        />
        
        <TabFinanceiro 
          v-if="activeTab === 'despesas'"
          type="despesa"
          title="Detalhamento de Despesas"
          color="red"
        />

        <TabFinanceiro 
          v-if="activeTab === 'lucros'"
          type="lucro"
          title="Análise de Lucratividade"
          color="blue"
        />
      </div>
    </div>
  </div>
</template>