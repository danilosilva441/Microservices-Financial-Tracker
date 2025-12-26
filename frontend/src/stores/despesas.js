import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/api';

export const useDespesasStore = defineStore('despesas', () => {
  const despesas = ref([]);
  const categorias = ref([]);
  const isLoading = ref(false);
  const error = ref(null);

  // --- CATEGORIAS ---
  async function fetchCategorias() {
    try {
      const response = await api.get('/api/Expenses/categories');
      categorias.value = response.data;
    } catch (err) {
      console.error('Erro ao buscar categorias:', err);
    }
  }

  async function createCategoria(nome, descricao) {
    try {
      await api.post('/api/Expenses/categories', { name: nome, description: descricao });
      await fetchCategorias();
      return { success: true };
    } catch (err) {
      return { success: false, error: err.response?.data?.detail || 'Erro ao criar categoria' };
    }
  }

  // --- DESPESAS ---
  async function fetchDespesasPorUnidade(unidadeId) {
    isLoading.value = true;
    try {
      const response = await api.get(`/api/Expenses/unidade/${unidadeId}`);
      despesas.value = response.data;
    } catch (err) {
      console.error('Erro ao buscar despesas:', err);
      error.value = 'Erro ao carregar despesas.';
    } finally {
      isLoading.value = false;
    }
  }

  async function createDespesa(dados) {
    isLoading.value = true;
    try {
      await api.post('/api/Expenses', dados);
      return { success: true };
    } catch (err) {
      const msg = err.response?.data?.title || 'Erro ao salvar despesa';
      return { success: false, error: msg };
    } finally {
      isLoading.value = false;
    }
  }

  async function deleteDespesa(id) {
    try {
      await api.delete(`/api/Expenses/${id}`);
      return true;
    } catch (err) {
      console.error('Erro ao excluir despesa:', err);
      return false;
    }
  }

  // Upload de Comprovante (Multipart Form Data)
  async function uploadComprovante(file) {
    const formData = new FormData();
    formData.append('file', file);
    try {
      const response = await api.post('/api/Expenses/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      return { success: true, path: response.data }; // Ajustar conforme retorno da API
    } catch (err) {
      return { success: false, error: 'Falha no upload' };
    }
  }

  return {
    despesas,
    categorias,
    isLoading,
    error,
    fetchCategorias,
    createCategoria,
    fetchDespesasPorUnidade,
    createDespesa,
    deleteDespesa,
    uploadComprovante
  };
});