import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useAdminStore } from '@/stores/admin.store'

export function useAdminRelatorios() {
  const adminStore = useAdminStore()
  const { estatisticas, dashboard, logsAtividades } = storeToRefs(adminStore)

  const totalUsuariosAtivos = computed(() => estatisticas.value.totalUsuarios)
  
  const distribuicaoRolesFormatada = computed(() => {
    const roles = adminStore.distribuicaoRoles
    return Object.entries(roles).map(([role, quantidade]) => ({
      role,
      quantidade,
      nome: adminStore.formatarRole(role),
      cor: adminStore.getCorRole(role),
    }))
  })

  const ultimosLogs = computed(() => {
    return logsAtividades.value.slice(0, 10)
  })

  const gerarRelatorio = () => {
    adminStore.gerarRelatorioAdmin()
  }

  const getEstatisticasUnidade = (unidadeId) => {
    return adminStore.usuariosPorUnidade[unidadeId]
  }

  return {
    // Estado
    estatisticas,
    dashboard,
    logsAtividades,
    
    // Computados
    totalUsuariosAtivos,
    distribuicaoRolesFormatada,
    ultimosLogs,
    
    // Métodos
    gerarRelatorio,
    getEstatisticasUnidade,
  }
}