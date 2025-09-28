import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useOperacoesStore = defineStore('operacoes', () => {
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
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.post('/operacoes', operacaoData);
      operacoes.value.push(response.data);
      return true;
    } catch (err) {
      // CORREÇÃO: Extrai a mensagem de texto do erro
      error.value = err.response?.data?.title || err.response?.data || 'Erro desconhecido ao criar operação.';
      console.error('Erro ao criar operação:', error.value);
      return false;
    } finally {
      isLoading.value = false;
    }
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
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.post(`/operacoes/${operacaoId}/faturamentos`, faturamentoData);
      if (operacaoAtual.value?.id === operacaoId) {
        operacaoAtual.value.faturamentos.$values.push(response.data);
      }
      return true;
    } catch (err) {
      // CORREÇÃO: Extrai a mensagem de texto do erro
      error.value = err.response?.data || 'Erro desconhecido ao adicionar faturamento.';
      console.error('Erro ao adicionar faturamento:', error.value);
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  async function deactivateFaturamento(operacaoId, faturamentoId) {
    isLoading.value = true;
    error.value = null;
    try {
      await api.patch(`/operacoes/${operacaoId}/faturamentos/${faturamentoId}/desativar`);
      const faturamento = operacaoAtual.value.faturamentos.$values.find(f => f.id === faturamentoId);
      if (faturamento) {
        faturamento.isAtivo = false;
      }
      return true;
    } catch (err) {
      error.value = err.response?.data || 'Erro desconhecido ao desativar faturamento.';
      console.error('Erro ao desativar faturamento:', error.value);
      return false;
    } finally {
      isLoading.value = false;
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