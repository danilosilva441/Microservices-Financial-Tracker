import api from "./api";

export const FechamentosService = {
  abrirDia(unidadeId, payload) {
    return api.post(`/unidades/${unidadeId}/fechamentos`, payload);
  },
  listar(unidadeId) {
    return api.get(`/unidades/${unidadeId}/fechamentos`);
  },
  getById(unidadeId, id) {
    return api.get(`/unidades/${unidadeId}/fechamentos/${id}`);
  },
  atualizar(unidadeId, id, payload) {
    return api.put(`/unidades/${unidadeId}/fechamentos/${id}`, payload);
  },
  fechar(unidadeId, id, payload) {
    return api.post(`/unidades/${unidadeId}/fechamentos/${id}/fechar`, payload);
  },
  conferir(unidadeId, id, payload) {
    return api.post(`/unidades/${unidadeId}/fechamentos/${id}/conferir`, payload);
  },
  reabrir(unidadeId, id, payload) {
    return api.post(`/unidades/${unidadeId}/fechamentos/${id}/reabrir`, payload);
  },
  pendentes() {
    return api.get(`/fechamentos/pendentes`);
  },
  porData(unidadeId, params) {
    // sua rota é /fechamentos/por-data (provavelmente com querystring)
    return api.get(`/unidades/${unidadeId}/fechamentos/por-data`, { params });
  },
};
