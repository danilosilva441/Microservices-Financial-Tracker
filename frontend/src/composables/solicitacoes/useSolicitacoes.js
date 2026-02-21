import { storeToRefs } from 'pinia';
import { useSolicitacoesStore } from '@/stores/solicitacoes.store';

export function useSolicitacoes() {
  const store = useSolicitacoesStore();
  
  const {
    solicitacoes,
    minhasSolicitacoes,
    solicitacaoAtual,
    filtros,
    estatisticas,
    dashboard,
    isLoading,
    error,
    tiposSolicitacao,
    statusSolicitacao,
    config,
  } = storeToRefs(store);

  // Getters computados
  const {
    solicitacoesFiltradas,
    solicitacoesPendentes,
    solicitacoesAprovadas,
    solicitacoesRejeitadas,
    solicitacoesHoje,
    solicitacoesSemana,
    solicitacoesPorTipo,
    solicitacoesParaRevisao,
    limiteDiarioAtingido,
    tempoMedioResolucaoCalculado,
  } = store;

  // Carregar dados
  const carregarSolicitacoes = async () => {
    return await store.carregarSolicitacoes();
  };

  const carregarMinhasSolicitacoes = async () => {
    return await store.carregarMinhasSolicitacoes();
  };

  const buscarSolicitacaoPorId = async (id) => {
    return await store.buscarSolicitacaoPorId(id);
  };

  const resetarStore = () => {
    store.resetarStore();
  };

  return {
    // Estado
    solicitacoes,
    minhasSolicitacoes,
    solicitacaoAtual,
    filtros,
    estatisticas,
    dashboard,
    isLoading,
    error,
    tiposSolicitacao,
    statusSolicitacao,
    config,
    
    // Getters
    solicitacoesFiltradas,
    solicitacoesPendentes,
    solicitacoesAprovadas,
    solicitacoesRejeitadas,
    solicitacoesHoje,
    solicitacoesSemana,
    solicitacoesPorTipo,
    solicitacoesParaRevisao,
    limiteDiarioAtingido,
    tempoMedioResolucaoCalculado,
    
    // Métodos
    carregarSolicitacoes,
    carregarMinhasSolicitacoes,
    buscarSolicitacaoPorId,
    resetarStore,
  };
}