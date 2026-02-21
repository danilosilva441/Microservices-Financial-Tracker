import { storeToRefs } from 'pinia'
import { useAdminStore } from '@/stores/admin.store'

export function useAdmin() {
  const adminStore = useAdminStore()
  
  const {
    // Estado
    usuarios,
    vinculos,
    usuarioSelecionado,
    unidadesDisponiveis,
    estatisticas,
    dashboard,
    logsAtividades,
    config,
    isLoading,
    error,
  } = storeToRefs(adminStore)

  // Getters
  const {
    usuariosFiltrados,
    usuariosComVinculos,
    usuariosPorUnidade,
    unidadesParaVinculo,
    distribuicaoRoles,
  } = adminStore

  return {
    // Estado
    usuarios,
    vinculos,
    usuarioSelecionado,
    unidadesDisponiveis,
    estatisticas,
    dashboard,
    logsAtividades,
    config,
    isLoading,
    error,
    
    // Getters
    usuariosFiltrados,
    usuariosComVinculos,
    usuariosPorUnidade,
    unidadesParaVinculo,
    distribuicaoRoles,
    
    // Store original para ações
    adminStore,
  }
}