import { useAdminStore } from '@/stores/admin.store'

export function useAdminActions() {
  const adminStore = useAdminStore()

  const carregarUsuarios = async () => {
    return await adminStore.carregarUsuarios()
  }

  const carregarUnidades = async () => {
    return await adminStore.carregarUnidades()
  }

  const carregarVinculosUsuario = async (userId) => {
    return await adminStore.carregarVinculosUsuario(userId)
  }

  const vincularUsuarioUnidade = async (vinculoData) => {
    return await adminStore.vincularUsuarioUnidade(vinculoData)
  }

  const removerVinculo = async (userId, unidadeId) => {
    return await adminStore.removerVinculo(userId, unidadeId)
  }

  const atualizarRoleUsuario = async (userId, novaRole) => {
    return await adminStore.atualizarRoleUsuario(userId, novaRole)
  }

  const gerarRelatorioAdmin = () => {
    adminStore.gerarRelatorioAdmin()
  }

  const resetarStore = () => {
    adminStore.resetarStore()
  }

  const clearError = () => {
    adminStore.clearError()
  }

  return {
    carregarUsuarios,
    carregarUnidades,
    carregarVinculosUsuario,
    vincularUsuarioUnidade,
    removerVinculo,
    atualizarRoleUsuario,
    gerarRelatorioAdmin,
    resetarStore,
    clearError,
  }
}