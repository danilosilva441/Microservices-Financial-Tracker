import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useMensalistasStore = defineStore('mensalistas', () => {
  const mensalistas = ref([]);
  const isLoading = ref(false);
  const error = ref(null);

  async function fetchMensalistas(unidadeId) {
    isLoading.value = true;
    try {
      const response = await api.get(`/api/unidades/${unidadeId}/mensalistas`);
      mensalistas.value = response.data;
    } catch (err) {
      error.value = 'Erro ao carregar mensalistas.';
      console.error(err);
    } finally {
      isLoading.value = false;
    }
  }

  async function createMensalista(unidadeId, dados) {
    try {
      await api.post(`/api/unidades/${unidadeId}/mensalistas`, dados);
      await fetchMensalistas(unidadeId);
      return { success: true };
    } catch (err) {
      return { success: false, error: err.response?.data?.detail || 'Erro ao criar mensalista' };
    }
  }

  async function updateMensalista(unidadeId, mensalistaId, dados) {
    try {
      await api.put(`/api/unidades/${unidadeId}/mensalistas/${mensalistaId}`, dados);
      await fetchMensalistas(unidadeId);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao atualizar mensalista' };
    }
  }

  async function desativarMensalista(unidadeId, mensalistaId) {
    try {
      await api.patch(`/api/unidades/${unidadeId}/mensalistas/${mensalistaId}/desativar`);
      await fetchMensalistas(unidadeId);
      return true;
    } catch (err) {
      return false;
    }
  }

  return {
    mensalistas,
    isLoading,
    error,
    fetchMensalistas,
    createMensalista,
    updateMensalista,
    desativarMensalista
  };
});