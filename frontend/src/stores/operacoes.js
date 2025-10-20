import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useOperacoesStore = defineStore('operacoes', () => {
  // State
  const operacoes = ref(null);
  const isLoading = ref(false);
  const error = ref(null);
  const operacaoAtual = ref(null);

  // Actions
  async function fetchOperacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      console.log('📦 Buscando operações...');
      // --- CORREÇÃO AQUI ---
      const response = await api.get('/api/operacoes');
      console.log('✅ Operações recebidas:', response.data);
      operacoes.value = response.data;
    } catch (err) {
      console.error('❌ Erro ao buscar operações:', err);
      error.value = 'Não foi possível carregar os dados das operações.';
      throw err; // Propaga o erro
    } finally {
      isLoading.value = false;
    }
  }

 // NOVA FUNÇÃO: Busca dados processados do dashboard
  async function fetchDashboardData() {
    isLoading.value = true;
    error.value = null;
    try {
      console.log('📊 Buscando dados processados do dashboard...');
      // Chama o AnalysisService que já tem os cálculos
      const response = await api.get('/api/analysis/dashboard');
      console.log('✅ Dados do dashboard recebidos:', response.data);
      return response.data;
    } catch (err) {
      console.error('❌ Erro ao buscar dados do dashboard:', err);
      error.value = 'Não foi possível carregar os dados do dashboard.';
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  async function createOperacao(operacaoData) {
    isLoading.value = true;
    error.value = null;
    try {
      console.log('📦 Criando operação:', operacaoData);
      
      const dadosValidados = {
        nome: operacaoData.nome?.trim(),
        descricao: operacaoData.descricao?.trim() || '',
        metaMensal: parseFloat(operacaoData.metaMensal) || 0,
        isAtivo: operacaoData.isAtivo !== undefined ? operacaoData.isAtivo : true
      };

      if (!dadosValidados.nome) {
        throw new Error('Nome da operação é obrigatório');
      }

      console.log('📤 Enviando dados validados:', dadosValidados);
      
      // --- CORREÇÃO AQUI ---
      const response = await api.post('/api/operacoes', dadosValidados);
      console.log('✅ Operação criada com sucesso:', response.data);
      
      // Recarrega a lista para garantir consistência
      await fetchOperacoes();
      return { success: true, data: response.data };
      
    } catch (err) {
      console.error('❌ Erro detalhado ao criar operação:', err);
      
      if (err.response?.status === 405) {
        error.value = 'Método não permitido. Verifique se o endpoint está correto (ex: falta /api/).';
      } else if (err.response?.status === 401) {
        error.value = 'Não autorizado. Faça login novamente.';
      } else if (err.response?.status === 403) {
        error.value = 'Acesso negado. Você não tem permissão para criar operações.';
      } else if (err.response?.data) {
        error.value = err.response.data.title || err.response.data;
      } else if (err.message) {
        error.value = err.message;
      } else {
        error.value = 'Erro desconhecido ao criar operação.';
      }
      
      return { success: false, error: error.value };
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchOperacaoById(id) {
    isLoading.value = true;
    error.value = null;
    operacaoAtual.value = null;
    try {
      // --- CORREÇÃO AQUI ---
      const response = await api.get(`/api/operacoes/${id}`);
      operacaoAtual.value = response.data;
    } catch (err) {
      console.error(`❌ Erro ao buscar operação ${id}:`, err);
      error.value = 'Não foi possível carregar os dados da operação.';
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  async function addFaturamento(operacaoId, faturamentoData) {
    try {
      console.log('💰 Adicionando faturamento:', { operacaoId, faturamentoData });
      // --- CORREÇÃO AQUI ---
      const response = await api.post(`/api/operacoes/${operacaoId}/faturamentos`, faturamentoData);
      
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        if (!operacaoAtual.value.faturamentos) {
          operacaoAtual.value.faturamentos = { $values: [] };
        }
        operacaoAtual.value.faturamentos.$values.push(response.data);
      }
      return response.data;
    } catch (err) {
      console.error('❌ Erro ao adicionar faturamento:', err);
      error.value = 'Não foi possível adicionar o faturamento.';
      throw err;
    }
  }

  async function deleteFaturamento(operacaoId, faturamentoId) {
    try {
      console.log('🗑️ Excluindo faturamento:', { operacaoId, faturamentoId });
      // --- CORREÇÃO AQUI ---
      await api.delete(`/api/operacoes/${operacaoId}/faturamentos/${faturamentoId}`);
      
      if (operacaoAtual.value && operacaoAtual.value.id === operacaoId) {
        const index = operacaoAtual.value.faturamentos.$values.findIndex(f => f.id === faturamentoId);
        if (index !== -1) {
          operacaoAtual.value.faturamentos.$values.splice(index, 1);
        }
      }
    } catch (err) {
      console.error('❌ Erro ao excluir faturamento:', err);
      error.value = 'Não foi possível excluir o faturamento.';
      throw err;
    }
  }

  async function deleteOperacao(operacaoId) {
    isLoading.value = true;
    error.value = null;
    try {
      console.log('🗑️ Excluindo operação:', operacaoId);
      // --- CORREÇÃO AQUI ---
      await api.delete(`/api/operacoes/${operacaoId}`);

      // Remove a operação da lista local
      if (operacoes.value && operacoes.value.$values) {
        const index = operacoes.value.$values.findIndex(op => op.id === operacaoId);
        if (index !== -1) {
          operacoes.value.$values.splice(index, 1);
        }
      }
      return true;
    } catch (err) {
      console.error('❌ Erro ao excluir operação:', err);
      error.value = err.response?.data || 'Erro ao excluir operação.';
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
    fetchDashboardData,
    fetchOperacaoById,
    addFaturamento,
    deleteFaturamento,
    deleteOperacao
  };
});