// composables/metas/useMetasConfig.js
import { ref, watch } from 'vue';
import { useMetasStore } from './useMetasStore';

export function useMetasConfig() {
  const store = useMetasStore();
  
  const config = ref({ ...store.config });

  function atualizarConfig(novaConfig) {
    config.value = { ...config.value, ...novaConfig };
    store.config = { ...config.value };
  }

  function resetarConfig() {
    config.value = {
      alertaPercentual: 80,
      notificarAtingimento: true,
      corMetaAtingida: '#4caf50',
      corMetaEmAndamento: '#2196f3',
      corMetaEmRisco: '#ff9800',
      corMetaNaoAtingida: '#f44336',
    };
    store.config = { ...config.value };
  }

  function getCorPorStatus(status) {
    switch (status) {
      case 'atingida': return config.value.corMetaAtingida;
      case 'em_andamento': return config.value.corMetaEmAndamento;
      case 'em_risco': return config.value.corMetaEmRisco;
      case 'nao_atingida': return config.value.corMetaNaoAtingida;
      default: return '#9e9e9e';
    }
  }

  watch(config, (novaConfig) => {
    store.config = novaConfig;
  }, { deep: true });

  return {
    config,
    atualizarConfig,
    resetarConfig,
    getCorPorStatus,
  };
}