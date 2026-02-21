import { useSolicitacoesStore } from '@/stores/solicitacoes.store';

export function useSolicitacoesActions() {
  const store = useSolicitacoesStore();

  const criarSolicitacao = async (solicitacaoData) => {
    return await store.criarSolicitacao(solicitacaoData);
  };

  const revisarSolicitacao = async (id, acao, justificativa) => {
    return await store.revisarSolicitacao(id, acao, justificativa);
  };

  const cancelarSolicitacao = async (id, motivo) => {
    return await store.cancelarSolicitacao(id, motivo);
  };

  const criarSolicitacaoAjusteFaturamento = (faturamentoParcialId, tipo, dadosAntigos, dadosNovos, motivo) => {
    return store.criarSolicitacaoAjusteFaturamento(faturamentoParcialId, tipo, dadosAntigos, dadosNovos, motivo);
  };

  const enviarNotificacao = (titulo, mensagem, tipo = 'info') => {
    store.enviarNotificacao(titulo, mensagem, tipo);
  };

  const iniciarMonitoramento = () => {
    store.iniciarMonitoramento();
  };

  const podeSolicitarAjuste = (faturamentoParcial) => {
    return store.podeSolicitarAjuste(faturamentoParcial);
  };

  const clearError = () => {
    store.clearError();
  };

  return {
    criarSolicitacao,
    revisarSolicitacao,
    cancelarSolicitacao,
    criarSolicitacaoAjusteFaturamento,
    enviarNotificacao,
    iniciarMonitoramento,
    podeSolicitarAjuste,
    clearError,
  };
}