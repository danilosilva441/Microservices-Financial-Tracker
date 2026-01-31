import { ref, computed, onMounted, watch } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore';

export function useDashboard() {
  const dashboardStore = useDashboardStore();
  const timeframe = ref('month');
  const activeTab = ref('geral');
  const showExportModal = ref(false);
  const exportLoading = ref(false);

  const filters = ref({ periodo: timeframe.value });

  // Busca os dados ao montar
  onMounted(() => {
    dashboardStore.fetchDashboardData();
  });

  // Watch para timeframe
  watch(timeframe, (newTimeframe) => {
    filters.value.periodo = newTimeframe;
    dashboardStore.fetchDashboardData(newTimeframe);
  });

  const refreshData = () => {
    dashboardStore.fetchDashboardData(timeframe.value);
  };

  const handleTabChange = (tab) => {
    activeTab.value = tab;
  };

  const handleFilterChange = (newTimeframe) => {
    timeframe.value = newTimeframe;
  };

  const handleExport = async () => {
    exportLoading.value = true;
    try {
      const dados = {
        kpis: dashboardStore.kpis,
        advancedMetrics: dashboardStore.advancedMetrics,
        operacoes: dashboardStore.operacoes,
        timestamp: new Date().toISOString()
      };
      
      const blob = new Blob([JSON.stringify(dados, null, 2)], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `bi-dashboard-${new Date().toISOString().split('T')[0]}.json`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Erro ao exportar dados:', error);
      alert('Erro ao exportar dados');
    } finally {
      exportLoading.value = false;
    }
  };

  const exportData = computed(() => ({
    kpis: dashboardStore.kpis,
    advancedMetrics: dashboardStore.advancedMetrics,
    operacoes: dashboardStore.operacoes,
    timestamp: new Date().toISOString()
  }));

  return {
    dashboardStore,
    filters,
    timeframe,
    activeTab,
    showExportModal,
    exportLoading,
    exportData,
    refreshData,
    handleTabChange,
    handleFilterChange,
    handleExport
  };
}