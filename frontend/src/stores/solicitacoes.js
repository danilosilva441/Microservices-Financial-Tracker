import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useSolicitacoesStore = defineStore('solicitacoes', () => {
  const solicitacoes = ref([]);
  const isLoading = ref(false);

  // Buscar todas (Filtros: status, tipo)
  async function fetchSolicitacoes(filtros = {}) {
    isLoading.value = true;
    try {
      const params = new URLSearchParams(filtros).toString();
      const response = await api.get(`/api/Solicitacoes?${params}`);
      solicitacoes.value = response.data;
    } catch (err) {
      console.error('Erro ao buscar solicitações', err);
    } finally {
      isLoading.value = false;
    }
  }

  // Buscar minhas solicitações
  async function fetchMinhasSolicitacoes() {
    isLoading.value = true;
    try {
      const response = await api.get('/api/Solicitacoes/minhas');
      solicitacoes.value = response.data;
    } finally {
      isLoading.value = false;
    }
  }

  // Criar nova solicitação (Operador pede ajuste)
  async function criarSolicitacao(dados) {
    try {
      await api.post('/api/Solicitacoes', dados);
      return { success: true };
    } catch (err) {
      return { success: false, error: err.response?.data?.detail || 'Erro ao criar solicitação' };
    }
  }

  // Revisar (Admin aprova/rejeita)
  async function revisarSolicitacao(id, acao, justificativa) {
    try {
      // acao: 'APROVAR' | 'REJEITAR'
      await api.patch(`/api/Solicitacoes/${id}/revisar`, { acao, justificativa });
      // Remove da lista local para atualizar UI
      solicitacoes.value = solicitacoes.value.filter(s => s.id !== id);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao processar solicitação' };
    }
  }

  return {
    solicitacoes,
    isLoading,
    fetchSolicitacoes,
    fetchMinhasSolicitacoes,
    criarSolicitacao,
    revisarSolicitacao
  };
});