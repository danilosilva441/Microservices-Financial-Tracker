import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useFaturamentoStore = defineStore('faturamento', () => {
  const fechamentos = ref([]);
  const isLoading = ref(false);

  // Buscar histórico de fechamentos
  async function fetchFechamentos(unidadeId) {
    isLoading.value = true;
    try {
      const response = await api.get(`/api/unidades/${unidadeId}/fechamentos`);
      fechamentos.value = response.data;
    } catch (err) {
      console.error(err);
    } finally {
      isLoading.value = false;
    }
  }

  // Realizar Fechamento do Dia (Caixa)
  async function criarFechamento(unidadeId, dados) {
    try {
      await api.post(`/api/unidades/${unidadeId}/fechamentos`, dados);
      await fetchFechamentos(unidadeId);
      return { success: true };
    } catch (err) {
      return { success: false, error: err.response?.data?.detail || 'Erro no fechamento' };
    }
  }

  // Atualizar fechamento (Supervisor)
  async function updateFechamento(unidadeId, fechamentoId, dados) {
    try {
      await api.put(`/api/unidades/${unidadeId}/fechamentos/${fechamentoId}`, dados);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao atualizar fechamento' };
    }
  }

  // Buscar fechamentos pendentes (Admin/Dash)
  async function fetchPendentes() {
    try {
      const response = await api.get('/api/fechamentos/pendentes');
      return response.data;
    } catch (err) {
      return [];
    }
  }

  return {
    fechamentos,
    isLoading,
    fetchFechamentos,
    criarFechamento,
    updateFechamento,
    fetchPendentes
  };
});