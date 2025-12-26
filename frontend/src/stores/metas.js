import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useMetasStore = defineStore('metas', () => {
  const metas = ref([]);
  const isLoading = ref(false);

  async function fetchMetas(unidadeId) {
    isLoading.value = true;
    try {
      const response = await api.get(`/api/unidades/${unidadeId}/metas`);
      metas.value = response.data;
    } catch (err) {
      console.error(err);
    } finally {
      isLoading.value = false;
    }
  }

  async function setMeta(unidadeId, dados) {
    try {
      await api.post(`/api/unidades/${unidadeId}/metas`, dados);
      await fetchMetas(unidadeId);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao definir meta' };
    }
  }

  return {
    metas,
    isLoading,
    fetchMetas,
    setMeta
  };
});