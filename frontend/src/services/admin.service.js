import api from "./api";

export const AdminService = {
  vincularUsuarioUnidade(payload) {
    return api.post(`/billingadmin/vincular-usuario-unidade`, payload);
  }
};

import api from '@/services/api';
import { unwrapDotNetList } from '@/utils/dotnet';

export async function getVinculosDoUsuario(userId) {
  const { data } = await api.get(`/api/billingadmin/usuario/${userId}/vinculos`);
  return unwrapDotNetList(data);
}