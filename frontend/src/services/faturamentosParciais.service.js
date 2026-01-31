import api from "./api";

export const FaturamentosParciaisService = {
  list(unidadeId) {
    return api.get(`/unidades/${unidadeId}/faturamentos-parciais`);
  },
  create(unidadeId, payload) {
    return api.post(`/unidades/${unidadeId}/faturamentos-parciais`, payload);
  },
  update(unidadeId, faturamentoId, payload) {
    return api.put(`/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`, payload);
  },
  remove(unidadeId, faturamentoId) {
    return api.delete(`/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`);
  },
  getById(unidadeId, faturamentoId) {
    return api.get(`/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`);
  },
  desativar(unidadeId, faturamentoId) {
    return api.patch(`/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}/desativar`);
  },
};
