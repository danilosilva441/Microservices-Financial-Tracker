import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useOperacoesStore = defineStore('operacoes', () => {
  // State
  const operacoes = ref([]);
  const isLoading = ref(false);
  const error = ref(null);
  const operacaoAtual = ref(null);

  // Actions
  async function fetchOperacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.get('/operacoes');
      operacoes.value = response.data.$values || [];
    } catch (err) {
      console.error('Erro ao buscar operações:', err);
      error.value = 'Não foi possível carregar os dados das operações.';
    } finally {
      isLoading.value = false;
    }
  }

  async function createOperacao(operacaoData) {
    // ... (seu código de createOperacao)
  }

  async function fetchOperacaoById(id) {
    isLoading.value = true;
    error.value = null;
    operacaoAtual.value = null;
    try {
      const response = await api.get(`/operacoes/${id}`);
      operacaoAtual.value = response.data;
    } catch (err) {
      console.error(`Erro ao buscar operação ${id}:`, err);
      error.value = 'Não foi possível carregar os dados da operação.';
    } finally {
      isLoading.value = false;
    }
  }

  async function addFaturamento(operacaoId, faturamentoData) {
    try {
      const response = await api.post(`/operacoes/${operacaoId}/faturamentos`, faturamentoData);
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        operacaoAtual.value.faturamentos.$values.push(response.data);
      }
    } catch (err) {
      console.error('Erro ao adicionar faturamento:', err);
      error.value = 'Não foi possível adicionar o faturamento.';
    }
  }

  async function deactivateFaturamento(operacaoId, faturamentoId) {
    try {
      await api.patch(`/operacoes/${operacaoId}/faturamentos/${faturamentoId}/desativar`);
      const faturamento = operacaoAtual.value.faturamentos.$values.find(f => f.id === faturamentoId);
      if (faturamento) {
        faturamento.isAtivo = false;
      }
    } catch (err) {
      console.error('Erro ao desativar faturamento:', err);
      error.value = 'Não foi possível desativar o faturamento.';
    }
  }

  return {
    operacoes,
    isLoading,
    error,
    operacaoAtual,
    fetchOperacoes,
    createOperacao,
    fetchOperacaoById,
    addFaturamento, 
    deactivateFaturamento
  };
});