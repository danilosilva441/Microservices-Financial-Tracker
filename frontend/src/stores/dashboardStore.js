import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api';

export const useDashboardStore = defineStore('dashboard', () => {
  // state
  const data = ref(null);
  const isLoading = ref(false);
  const error = ref(null);

  // actions
  async function fetchDashboardData() {
    isLoading.value = true;
    error.value = null;
    try {
      console.log('📊 DashboardStore: Carregando dados processados...');
      
      // 1. Chama o endpoint do AnalysisService que já faz todo o trabalho
      const response = await api.get('/api/analysis/dashboard-data');
      
      data.value = response.data.data; // Armazena o objeto { kpis, desempenho, graficos, operacoes }
      console.log('✅ DashboardStore: Dados carregados:', data.value);
      
      return data.value;
    } catch (err) {
      console.error('❌ Erro ao carregar dados do dashboard:', err);
      error.value = 'Não foi possível carregar os dados do dashboard.';
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  // getters (acesso fácil aos dados)
  const kpis = computed(() => data.value?.kpis || {});
  const desempenho = computed(() => data.value?.desempenho || {});
  const graficos = computed(() => data.value?.graficos || {});
  const operacoes = computed(() => data.value?.operacoes || []); // Lista de operações (se o backend enviar)

  return { 
    data, 
    isLoading, 
    error, 
    fetchDashboardData,
    // Exporta os dados já processados
    kpis,
    desempenho,
    graficos,
    operacoes
  };
});