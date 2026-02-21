import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useAdminStore } from '@/stores/admin.store'

export function useAdminFilters() {
  const adminStore = useAdminStore()
  const { filtros } = storeToRefs(adminStore)

  // Computed baseados nos filtros
  const filtrosAtivos = computed(() => {
    const ativos = {}
    if (filtros.value.buscaUsuario) ativos.buscaUsuario = filtros.value.buscaUsuario
    if (filtros.value.role) ativos.role = filtros.value.role
    if (filtros.value.unidadeId) ativos.unidadeId = filtros.value.unidadeId
    if (!filtros.value.apenasAtivos) ativos.apenasAtivos = false
    return ativos
  })

  const temFiltrosAtivos = computed(() => {
    return Object.keys(filtrosAtivos.value).length > 0
  })

  const aplicarFiltros = (novosFiltros) => {
    adminStore.aplicarFiltros(novosFiltros)
  }

  const limparFiltros = () => {
    adminStore.limparFiltros()
  }

  const setBuscaUsuario = (busca) => {
    adminStore.aplicarFiltros({ buscaUsuario: busca })
  }

  const setRoleFilter = (role) => {
    adminStore.aplicarFiltros({ role })
  }

  const setUnidadeFilter = (unidadeId) => {
    adminStore.aplicarFiltros({ unidadeId })
  }

  const setApenasAtivos = (apenasAtivos) => {
    adminStore.aplicarFiltros({ apenasAtivos })
  }

  return {
    // Estado
    filtros,
    
    // Computados
    filtrosAtivos,
    temFiltrosAtivos,
    
    // Métodos
    aplicarFiltros,
    limparFiltros,
    setBuscaUsuario,
    setRoleFilter,
    setUnidadeFilter,
    setApenasAtivos,
  }
}