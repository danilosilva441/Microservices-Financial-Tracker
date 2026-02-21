// src/composables/analysis/useAnalysis.js

import { storeToRefs } from 'pinia';
import { useAnalysisStore } from '@/stores/analysis.store';

export function useAnalysis() {
  const store = useAnalysisStore();
  
  const {
    // Estado
    dashboardData,
    loading,
    error,
    filters,
    lastUpdated,
    isAutoRefreshEnabled,
    refreshInterval,
    
    // Getters básicos
    kpis,
    graficos,
    desempenho,
    period,
    metadata,
    
    // Getters de métricas
    receitaTotal,
    lucroTotal,
    despesaTotal,
    metaTotal,
    percentualMeta,
    unidadesAtivas,
    melhorUnidade,
    piorUnidade,
    projecao,
    benchmark,
    analise,
    
    // Getters de análise
    sortedDesempenho,
    unidadesComLucro,
    unidadesComPrejuizo,
    unidadesAcimaDaMeta,
    unidadesAbaixoDaMeta,
    ticketMedioGeral,
    crescimentoPeriodo,
    metaStatus,
    metaStatusText,
    projecaoStatus,
    probabilidadeMeta,
  } = storeToRefs(store);

  // Ações
  const {
    fetchDashboardData,
    updateFilters,
    resetFilters,
    refreshData,
    clearCache,
    clearSpecificCache,
    formatCurrency,
    getStatusColor,
  } = store;

  return {
    // Estado
    dashboardData,
    loading,
    error,
    filters,
    lastUpdated,
    isAutoRefreshEnabled,
    refreshInterval,
    
    // Getters
    kpis,
    graficos,
    desempenho,
    period,
    metadata,
    receitaTotal,
    lucroTotal,
    despesaTotal,
    metaTotal,
    percentualMeta,
    unidadesAtivas,
    melhorUnidade,
    piorUnidade,
    projecao,
    benchmark,
    analise,
    sortedDesempenho,
    unidadesComLucro,
    unidadesComPrejuizo,
    unidadesAcimaDaMeta,
    unidadesAbaixoDaMeta,
    ticketMedioGeral,
    crescimentoPeriodo,
    metaStatus,
    metaStatusText,
    projecaoStatus,
    probabilidadeMeta,
    
    // Ações
    fetchDashboardData,
    updateFilters,
    resetFilters,
    refreshData,
    clearCache,
    clearSpecificCache,
    formatCurrency,
    getStatusColor,
  };
}