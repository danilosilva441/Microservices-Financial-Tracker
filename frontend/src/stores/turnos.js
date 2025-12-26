import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useTurnosStore = defineStore('turnos', () => {
  const turnos = ref([]);
  const isLoading = ref(false);

  async function fetchTurnos(unidadeId, start, end) {
    isLoading.value = true;
    try {
      const response = await api.get(`/api/Shifts/unidade/${unidadeId}`, {
        params: { start, end }
      });
      turnos.value = response.data;
    } catch (err) {
      console.error(err);
    } finally {
      isLoading.value = false;
    }
  }

  async function gerarEscala(dados) {
    try {
      await api.post('/api/Shifts/generate', dados);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao gerar escala' };
    }
  }

  async function criarTemplate(dados) {
    try {
      await api.post('/api/Shifts/templates', dados);
      return { success: true };
    } catch (err) {
      return { success: false, error: 'Erro ao criar template' };
    }
  }

  return {
    turnos,
    isLoading,
    fetchTurnos,
    gerarEscala,
    criarTemplate
  };
});