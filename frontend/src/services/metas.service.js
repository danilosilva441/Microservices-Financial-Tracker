import api from "./api";

export const MetasService = {
  list(unidadeId) {
    return api.get(`/unidades/${unidadeId}/metas`);
  },
  create(unidadeId, payload) {
    return api.post(`/unidades/${unidadeId}/metas`, payload);
  },
  periodo(unidadeId, params) {
    return api.get(`/unidades/${unidadeId}/metas/periodo`, { params });
  },
};