// src/composables/analysis/useAnalysisAutoRefresh.js

import { storeToRefs } from 'pinia';
import { ref, onUnmounted, watch } from 'vue';
import { useAnalysisStore } from '@/stores/analysis.store';

// eslint-disable-next-line no-unused-vars
export function useAnalysisAutoRefresh(options = {}) {
  const store = useAnalysisStore();
  const { isAutoRefreshEnabled, refreshInterval, loading } = storeToRefs(store);

  const autoRefreshActive = ref(isAutoRefreshEnabled.value);
  const countdown = ref(0);
  const refreshTimer = ref(null);

  // Opções de intervalo predefinidas
  const intervalOptions = [
    { value: 60000, label: '1 minuto' },
    { value: 300000, label: '5 minutos' },
    { value: 600000, label: '10 minutos' },
    { value: 1800000, label: '30 minutos' },
    { value: 3600000, label: '1 hora' },
  ];

  // Iniciar auto-refresh
  const startAutoRefresh = (interval = null) => {
    const refreshTime = interval || refreshInterval.value;
    store.setAutoRefresh(true, refreshTime);
    autoRefreshActive.value = true;

    // Iniciar contador
    startCountdown(refreshTime);
  };

  // Parar auto-refresh
  const stopAutoRefresh = () => {
    store.setAutoRefresh(false);
    autoRefreshActive.value = false;

    if (refreshTimer.value) {
      clearInterval(refreshTimer.value);
      refreshTimer.value = null;
    }
    countdown.value = 0;
  };

  // Iniciar contador regressivo
  const startCountdown = (interval) => {
    if (refreshTimer.value) {
      clearInterval(refreshTimer.value);
    }

    countdown.value = Math.floor(interval / 1000);

    refreshTimer.value = setInterval(() => {
      if (countdown.value > 0) {
        countdown.value--;
      } else {
        // Reinicia contador após atualização
        countdown.value = Math.floor(refreshInterval.value / 1000);
      }
    }, 1000);
  };

  // Atualizar intervalo
  const updateRefreshInterval = (newInterval) => {
    if (autoRefreshActive.value) {
      stopAutoRefresh();
      startAutoRefresh(newInterval);
    }
  };

  // Forçar atualização manual
  const forceRefresh = async () => {
    if (loading.value) return;

    await store.refreshData();

    // Reinicia contador
    if (autoRefreshActive.value) {
      countdown.value = Math.floor(refreshInterval.value / 1000);
    }
  };

  // Monitorar mudanças no estado do auto-refresh
  watch(isAutoRefreshEnabled, (newVal) => {
    autoRefreshActive.value = newVal;

    if (newVal) {
      startCountdown(refreshInterval.value);
    } else if (refreshTimer.value) {
      clearInterval(refreshTimer.value);
      refreshTimer.value = null;
      countdown.value = 0;
    }
  });

  // Limpeza ao desmontar
  onUnmounted(() => {
    if (refreshTimer.value) {
      clearInterval(refreshTimer.value);
    }
  });

  return {
    // Estado
    autoRefreshActive,
    countdown,
    loading,

    // Opções
    intervalOptions,
    currentInterval: refreshInterval,

    // Ações
    startAutoRefresh,
    stopAutoRefresh,
    updateRefreshInterval,
    forceRefresh,
    refreshData: store.refreshData,
  };
}