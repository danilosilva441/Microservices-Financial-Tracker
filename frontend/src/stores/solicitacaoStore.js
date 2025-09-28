import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useSolicitacaoStore = defineStore('solicitacao', () => {
  const solicitacoes = ref([]);
  const isLoading = ref(false);
  const error = ref(null);

  async function fetchSolicitacoes() {
    isLoading.value = true;
    error.value = null;
    try {
      // Endpoint para Admin buscar todas as solicitações
      const response = await api.get('/solicitacoes');
      solicitacoes.value = response.data.$values || [];
    } catch (err) {
      error.value = 'Falha ao buscar solicitações.';
      console.error(err);
    } finally {
      isLoading.value = false;
    }
  }

  async function criarSolicitacao(solicitacaoData) {
    try {
      await api.post('/solicitacoes', solicitacaoData);
    } catch (err) {
      console.error('Erro ao criar solicitação:', err);
      throw new Error('Falha ao enviar solicitação.');
    }
  }

  async function revisarSolicitacao(id, acao, motivoRejeicao = null) {
    try {
      await api.patch(`/solicitacoes/${id}/revisar`, { acao, motivoRejeicao });
      // Atualiza o status localmente para a UI reagir
      const solicitacao = solicitacoes.value.find(s => s.id === id);
      if (solicitacao) {
        solicitacao.status = acao === 'aprovar' ? 'APROVADA' : 'REJEITADA';
      }
    } catch (err) {
      console.error('Erro ao revisar solicitação:', err);
      throw new Error('Falha ao processar a revisão.');
    }
  }

  return { solicitacoes, isLoading, error, fetchSolicitacoes, criarSolicitacao, revisarSolicitacao };
});