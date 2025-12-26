import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '@/services/api';

export const useDashboardStore = defineStore('dashboard', () => {
  // State
  const data = ref(null);
  const isLoading = ref(false);
  const error = ref(null);

  // Actions
  async function fetchDashboardData(filters = {}) {
    isLoading.value = true;
    error.value = null;
    try {
      // Passa filtros como query params (inicio, fim, unidade_id etc)
      const params = new URLSearchParams(filters).toString();
      const response = await api.get(`/api/analysis/dashboard-data?${params}`);

      data.value = response.data; // Espera o objeto retornado pelo DashboardCalculator
    } catch (err) {
      console.error('Erro ao carregar dashboard:', err);
      error.value = 'Não foi possível carregar os dados atualizados.';
    } finally {
      isLoading.value = false;
    }
  }

  // Getters - Mapeando a estrutura do DashboardCalculator
  const kpis = computed(() => {
    // Se data.value ou data.value.kpis for null, retorna estrutura segura
    const safeKpis = data.value?.kpis || {};

    return {
      receitaTotal: safeKpis.receitaTotal || 0,
      despesaTotal: safeKpis.despesaTotal || 0,
      lucroTotal: safeKpis.lucroTotal || 0,
      metaTotal: safeKpis.metaTotal || 0,
      percentualMetaTotal: safeKpis.percentualMetaTotal || 0,
      unidadesAtivas: safeKpis.unidadesAtivas || 0,

      // Garante que benchmark existe
      benchmark: safeKpis.benchmark || {
        variacaoReceita: 0,
        variacaoLucro: 0,
        variacaoTicket: 0
      },

      // Garante que analise existe
      analise: safeKpis.analise || {
        ticketMedio: 0,
        crescimentoPeriodo: 0,
        estabilidade: 'N/A'
      },

      // Garante que projecao existe
      projecao: safeKpis.projecao || {
        status: 'neutro',
        probabilidade: 'Calculando...',
        percentualEstimado: 0,
        valorEstimado: 0
      },

      melhorUnidade: safeKpis.melhorUnidade,
      piorUnidade: safeKpis.piorUnidade
    };
  });

  const desempenho = computed(() => data.value?.desempenho || { todas: [], resumo: {} });

  // O backend já manda as configs de 'data', mas recriaremos as 'options' no front 
  // para garantir que as funções de callback (tooltip de moeda) funcionem no navegador.
  const graficos = computed(() => data.value?.graficos || {});

  const period = computed(() => data.value?.period || {});

  return {
    data,
    isLoading,
    error,
    fetchDashboardData,
    kpis,
    desempenho,
    graficos,
    period
  };
});