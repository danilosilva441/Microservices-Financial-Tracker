// composables/mensalistas/useMensalistasUI.js
import { computed } from 'vue';

export function useMensalistasUI(store) {
  const isLoading = computed(() => store.isLoading);
  const error = computed(() => store.error);
  const config = computed(() => store.config);
  
  const formatarValor = (valor) => {
    return store.formatarValor(valor);
  };

  const formatarData = (data) => {
    return store.formatarData(data);
  };

  const formatarDataComHora = (data) => {
    return store.formatarDataComHora(data);
  };

  const formatarCPF = (cpf) => {
    return store.formatarCPF(cpf);
  };

  const atualizarConfig = (novaConfig) => {
    store.config = { ...store.config, ...novaConfig };
  };

  const notificacoesAtivas = computed(() => {
    return {
      notificarVencimento: store.config.notificarVencimento,
      diasAntesVencimento: store.config.diasAntesVencimento,
      gerarCobrancaAutomatica: store.config.gerarCobrancaAutomatica,
    };
  });

  const statusMensalista = (isAtivo) => {
    return isAtivo
      ? { text: 'Ativo', color: 'success', icon: 'check-circle' }
      : { text: 'Inativo', color: 'error', icon: 'cancel' };
  };

  return {
    isLoading,
    error,
    config,
    notificacoesAtivas,
    formatarValor,
    formatarData,
    formatarDataComHora,
    formatarCPF,
    atualizarConfig,
    statusMensalista,
  };
}