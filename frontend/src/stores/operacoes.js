import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useOperacoesStore = defineStore('operacoes', () => {
  // State (os dados que a store guarda)
  const operacoes = ref([]);
  const isLoading = ref(false);
  const error = ref(null);
  const operacaoAtual = ref(null);

  // Actions (as funções que buscam/modificam os dados)
  async function fetchOperacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.get('/operacoes');
      // A API .NET retorna os dados dentro de '$values' por causa do ReferenceHandler
      operacoes.value = response.data.$values || [];
    } catch (err) {
      console.error('Erro ao buscar operações:', err);
      error.value = 'Não foi possível carregar os dados das operações.';
    } finally {
      isLoading.value = false;
    }
    // Dentro de /stores/operacoes.js -> fetchOperacoes
    try {
      const response = await api.get('/operacoes');


      //console.log('Resposta da API recebida:', response.data); 
      operacoes.value = response.data.$values || [];

    } catch (err) {
      // ...
    }
  }

  async function createOperacao(operacaoData) {
    isLoading.value = true;
    error.value = null;
    try {
      // Faz a chamada POST para criar a operação
      const response = await api.post('/operacoes', operacaoData);
      // Adiciona a nova operação à lista local para a tela atualizar na hora
      operacoes.value.push(response.data);
    } catch (err) {
      console.error('Erro ao criar operação:', err);
      error.value = 'Não foi possível criar a operação.';
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchOperacaoById(id) {
    isLoading.value = true;
    error.value = null;
    operacaoAtual.value = null; // Limpa o estado anterior
    try {
      // Precisamos criar este endpoint no backend!
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
      // Usa o endpoint aninhado que criamos no backend
      const response = await api.post(`/operacoes/${operacaoId}/faturamentos`, faturamentoData);

      // Atualiza a lista de faturamentos na operação atual para a tela refletir a mudança
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        operacaoAtual.value.faturamentos.$values.push(response.data);
      }
    } catch (err) {
      console.error('Erro ao adicionar faturamento:', err);
      // Opcional: definir uma mensagem de erro no estado
    }
  }

  return {
    operacoes, isLoading, error, operacaoAtual,
    fetchOperacoes, createOperacao, fetchOperacaoById, addFaturamento
  };
});