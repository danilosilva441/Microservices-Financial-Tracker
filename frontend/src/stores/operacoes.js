import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useOperacoesStore = defineStore('operacoes', () => {
  // State
  const operacoes = ref(null); // Inicia como nulo para diferenciar do estado "vazio"
  const isLoading = ref(false);
  const error = ref(null);
  const operacaoAtual = ref(null);

  // Actions
  async function fetchOperacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.get('/operacoes');
      // Salva o objeto de resposta completo, pois o componente acessa '$values'
      operacoes.value = response.data;
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
      // Recarrega a lista para garantir consistência
      await fetchOperacoes();
      return true;
    } catch (err) {
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
    try {
      const response = await api.post(`/operacoes/${operacaoId}/faturamentos`, faturamentoData);
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        operacaoAtual.value.faturamentos.$values.push(response.data);
      }
    } catch (err) {
      console.error('Erro ao adicionar faturamento:', err);
      error.value = 'Não foi possível adicionar o faturamento.';
      // Lança o erro para o componente poder tratá-lo
      throw err;
    }
  }

  async function deleteFaturamento(operacaoId, faturamentoId) {
    try {
      await api.delete(`/operacoes/${operacaoId}/faturamentos/${faturamentoId}`);
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        const index = operacaoAtual.value.faturamentos.$values.findIndex(f => f.id === faturamentoId);
        if (index !== -1) {
          operacaoAtual.value.faturamentos.$values.splice(index, 1);
        }
      }
    } catch (err) {
      console.error('Erro ao excluir faturamento:', err);
      error.value = 'Não foi possível excluir o faturamento.';
    }
  }
  async function deleteOperacao(operacaoId) {
    isLoading.value = true;
    error.value = null;
    try {
      await api.delete(`/operacoes/${operacaoId}`);

      // Remove a operação da lista local para a UI atualizar instantaneamente
      const index = operacoes.value.$values.findIndex(op => op.id === operacaoId);
      if (index !== -1) {
        operacoes.value.$values.splice(index, 1);
      }
      return true; // Retorna sucesso
    } catch (err) {
      error.value = err.response?.data || 'Erro ao excluir operação.';
      console.error('Erro ao excluir operação:', error.value);
      return false; // Retorna falha
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
    deleteFaturamento,
    deleteOperacao
  };
});