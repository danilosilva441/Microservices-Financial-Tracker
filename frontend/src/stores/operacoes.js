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

  async function fetchOperacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.get('/operacoes');
      // CORREÇÃO: Salva o objeto de resposta completo.
      // O componente cuidará de acessar a propriedade '$values'.
      operacoes.value = response.data;
    } catch (err) {
      console.error('Erro ao buscar operações:', err);
      error.value = 'Não foi possível carregar os dados das operações.';
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

  async function deleteFaturamento(operacaoId, faturamentoId) {
    try {
      // Chama o endpoint DELETE que já existe no backend
      await api.delete(`/operacoes/${operacaoId}/faturamentos/${faturamentoId}`);

      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        // Encontra o índice do faturamento na lista
        const index = operacaoAtual.value.faturamentos.$values.findIndex(f => f.id === faturamentoId);
        // Se encontrou, remove o item da lista para a UI atualizar
        if (index !== -1) {
          operacaoAtual.value.faturamentos.$values.splice(index, 1);
        }
      }
    } catch (err) {
      console.error('Erro ao excluir faturamento:', err);
      error.value = 'Não foi possível excluir o faturamento.';
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
    deleteFaturamento
  };
});