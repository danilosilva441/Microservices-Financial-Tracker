// unidade.service.js
import api from './api';

export default {
  async getAll() {
    const response = await api.get('/api/unidades');
    return response.data;
  },

  async create(payload) {
    const response = await api.post('/api/unidades', payload);
    return response.data;
  },

  async getById(id) {
    const response = await api.get(`/api/unidades/${id}`);
    return response.data;
  },

  async updateProjecao(id, valor) {
    const response = await api.patch(`/api/unidades/${id}/projecao`, {
      projecaoFaturamento: valor
    });
    return response.data;
  },

  async desativarUnidade(id) {
    const response = await api.patch(`/api/unidades/${id}/desativar`);
    return response.data;
  },

  async atualizarUnidade(id, payload) {
    const response = await api.put(`/api/unidades/${id}`, payload);
    return response.data;
  },

  async deletarUnidade(id) {
    const response = await api.delete(`/api/unidades/${id}`);
    return response.data;
  },

  async ativarUnidade(id) {
    // Se a API tiver endpoint para ativar
    const response = await api.patch(`/api/unidades/${id}/ativar`);
    return response.data;
  }
};