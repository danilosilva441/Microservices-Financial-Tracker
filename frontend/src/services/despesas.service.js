import api from "./api";

export const DespesasService = {
  listByUnidade(unidadeId) {
    return api.get(`/Expenses/unidade/${unidadeId}`);
  },
  create(payload) {
    return api.post(`/Expenses`, payload);
  },
  upload(formData) {
    // normalmente upload é multipart/form-data
    return api.post(`/Expenses/upload`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },
  remove(id) {
    return api.delete(`/Expenses/${id}`);
  },
  categories() {
    return api.get(`/Expenses/categories`);
  },
  createCategory(payload) {
    return api.post(`/Expenses/categories`, payload);
  },
};
