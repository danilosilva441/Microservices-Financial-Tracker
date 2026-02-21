import { storeToRefs } from 'pinia';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useFechamentos() {
  const fechamentosStore = useFechamentosStore();
  
  const {
    fechamentos,
    fechamentoAtual,
    fechamentosPendentes,
    fechamentosPorData,
    isLoading,
    error,
    estatisticas,
    dashboard,
    fechamentosFiltrados,
    fechamentosAbertosHoje,
    fechamentosFechadosPendentes,
    fechamentosEsteMes,
    faturamentoTotalMes,
    fechamentosComDiferenca,
    distribuicaoPorStatus,
    fechamentoHoje,
  } = storeToRefs(fechamentosStore);

  const carregarFechamentos = async (unidadeId) => {
    return await fechamentosStore.carregarFechamentos(unidadeId);
  };

  const buscarFechamentoPorId = async (unidadeId, id) => {
    return await fechamentosStore.buscarFechamentoPorId(unidadeId, id);
  };

  const verificarFechamentoHoje = async (unidadeId) => {
    return await fechamentosStore.verificarFechamentoHoje(unidadeId);
  };

  return {
    // State
    fechamentos,
    fechamentoAtual,
    fechamentosPendentes,
    fechamentosPorData,
    isLoading,
    error,
    estatisticas,
    dashboard,
    
    // Getters
    fechamentosFiltrados,
    fechamentosAbertosHoje,
    fechamentosFechadosPendentes,
    fechamentosEsteMes,
    faturamentoTotalMes,
    fechamentosComDiferenca,
    distribuicaoPorStatus,
    fechamentoHoje,
    
    // Actions
    carregarFechamentos,
    buscarFechamentoPorId,
    verificarFechamentoHoje,
    clearError: fechamentosStore.clearError,
    resetarStore: fechamentosStore.resetarStore,
  };
}