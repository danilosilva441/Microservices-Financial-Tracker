import api from "./api";

export const SolicitacoesService = {
  create(payload) {
    return api.post(`/Solicitacoes`, payload);
  },
  list(params) {
    return api.get(`/Solicitacoes`, { params });
  },
  getById(id) {
    return api.get(`/Solicitacoes/${id}`);
  },
  minhas() {
    return api.get(`/Solicitacoes/minhas`);
  },
  revisar(id, payload) {
    return api.patch(`/Solicitacoes/${id}/revisar`, payload);
  },
};
