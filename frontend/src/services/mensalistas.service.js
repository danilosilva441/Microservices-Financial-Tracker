import api from "./api";

export const MensalistasService = {
  list(unidadeId) {
    return api.get(`/unidades/${unidadeId}/mensalistas`);
  },
  create(unidadeId, payload) {
    return api.post(`/unidades/${unidadeId}/mensalistas`, payload);
  },
  update(unidadeId, mensalistaId, payload) {
    return api.put(`/unidades/${unidadeId}/mensalistas/${mensalistaId}`, payload);
  },
  getById(unidadeId, mensalistaId) {
    return api.get(`/unidades/${unidadeId}/mensalistas/${mensalistaId}`);
  },
  desativar(unidadeId, mensalistaId) {
    return api.patch(`/unidades/${unidadeId}/mensalistas/${mensalistaId}/desativar`);
  },
};
