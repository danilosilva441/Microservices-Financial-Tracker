import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useUnidadesStore = defineStore('unidades', () => {
  // State
  const unidades = ref(null);
  const isLoading = ref(false);
  const error = ref(null);
  const unidadeAtual = ref(null);

  // Actions
  async function fetchUnidades() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await api.get('/api/unidades');
      unidades.value = response.data;
    } catch (err) {
      console.error('❌ Erro ao buscar unidades:', err);
      error.value = 'Não foi possível carregar os dados das unidades.';
    } finally {
      isLoading.value = false;
    }
  }

  async function createUnidade(unidadeData) {
    isLoading.value = true;
    error.value = null;
    try {
      const dadosValidados = {
        nome: unidadeData.nome?.trim(),
        descricao: unidadeData.descricao?.trim() || '',
        metaMensal: parseFloat(unidadeData.metaMensal) || 0,
        dataInicio: new Date().toISOString(),
        dataFim: null
      };

      const response = await api.post('/api/unidades', dadosValidados);
      await fetchUnidades();
      return { success: true, data: response.data };
      
    } catch (err) {
      const msg = err.response?.data?.title || 'Erro desconhecido ao criar unidade.';
      return { success: false, error: msg };
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchUnidadeById(id) {
    isLoading.value = true;
    error.value = null;
    // Não limpamos unidadeAtual.value = null aqui para evitar "piscar" a tela
    try {
      const response = await api.get(`/api/unidades/${id}`);
      unidadeAtual.value = response.data;
    } catch (err) {
      console.error(`❌ Erro ao buscar unidade ${id}:`, err);
      error.value = 'Não foi possível carregar os dados da unidade.';
    } finally {
      isLoading.value = false;
    }
  }

  async function addFaturamento(unidadeId, faturamentoData) {
    try {
      console.log('💰 Adicionando faturamento:', { unidadeId, faturamentoData });
      
      const response = await api.post(`/api/unidades/${unidadeId}/faturamentos-parciais`, faturamentoData);
      
      // ✅ CORREÇÃO: Recarrega a unidade inteira para atualizar a lista na tela
      // Isso garante que temos os dados atualizados do servidor (IDs, datas, etc)
      await fetchUnidadeById(unidadeId);

      return response.data;
    } catch (err) {
      console.error('❌ Erro ao adicionar faturamento:', err);
      throw err;
    }
  }

  async function deleteFaturamento(unidadeId, faturamentoId) {
    try {
      console.log('🗑️ Excluindo faturamento:', { unidadeId, faturamentoId });
      
      await api.delete(`/api/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`);
      
      // ✅ CORREÇÃO: Recarrega a unidade para remover o item da lista visualmente
      await fetchUnidadeById(unidadeId);
      
    } catch (err) {
      console.error('❌ Erro ao excluir faturamento:', err);
      throw err;
    }
  }

  async function deleteUnidade(unidadeId) {
    isLoading.value = true;
    try {
      await api.delete(`/api/unidades/${unidadeId}`);
      if (unidades.value && unidades.value.$values) {
        const index = unidades.value.$values.findIndex(op => op.id === unidadeId);
        if (index !== -1) unidades.value.$values.splice(index, 1);
      }
      return true;
    } catch (err) {
      console.error('❌ Erro ao excluir unidade:', err);
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  return {
    unidades,
    isLoading,
    error,
    unidadeAtual,
    fetchUnidades,
    createUnidade,
    fetchUnidadeById,
    addFaturamento,
    deleteFaturamento,
    deleteUnidade
  };
});