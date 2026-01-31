import api from "./api";

export const ShiftsService = {
  createTemplate(payload) {
    return api.post(`/Shifts/templates`, payload);
  },
  generate(payload) {
    return api.post(`/Shifts/generate`, payload);
  },
  listByUnidade(unidadeId) {
    return api.get(`/Shifts/unidade/${unidadeId}`);
  },
  breaks(shiftId, payload) {
    return api.post(`/Shifts/${shiftId}/breaks`, payload);
  },
};
