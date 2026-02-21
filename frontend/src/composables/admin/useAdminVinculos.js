import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useAdminStore } from '@/stores/admin.store'

export function useAdminVinculos() {
  const adminStore = useAdminStore()
  const { vinculos, usuarios, unidadesDisponiveis, usuarioSelecionado, config } = storeToRefs(adminStore)

  const vinculosPorUsuario = computed(() => {
    const mapa = {}
    vinculos.value.forEach(v => {
      if (!mapa[v.userId]) mapa[v.userId] = []
      mapa[v.userId].push(v)
    })
    return mapa
  })

  const vinculosPorUnidade = computed(() => {
    const mapa = {}
    vinculos.value.forEach(v => {
      if (!mapa[v.unidadeId]) mapa[v.unidadeId] = []
      mapa[v.unidadeId].push(v)
    })
    return mapa
  })

  const usuarioPodeVincularMais = computed(() => {
    if (!usuarioSelecionado.value) return false
    const vinculosAtuais = vinculosPorUsuario.value[usuarioSelecionado.value.id]?.length || 0
    return vinculosAtuais < config.value.limiteVinculosPorUsuario
  })

  const unidadesVinculadasUsuarioSelecionado = computed(() => {
    if (!usuarioSelecionado.value) return []
    
    const vinculosUsuario = vinculos.value.filter(
      v => v.userId === usuarioSelecionado.value.id
    )
    
    return vinculosUsuario.map(v => 
      unidadesDisponiveis.value.find(u => u.id === v.unidadeId)
    ).filter(Boolean)
  })

  const carregarVinculos = async (userId) => {
    return await adminStore.carregarVinculosUsuario(userId)
  }

  const vincular = async (unidadeId) => {
    if (!usuarioSelecionado.value) return
    
    return await adminStore.vincularUsuarioUnidade({
      userId: usuarioSelecionado.value.id,
      unidadeId,
    })
  }

  const desvincular = async (unidadeId) => {
    if (!usuarioSelecionado.value) return
    
    return await adminStore.removerVinculo(
      usuarioSelecionado.value.id,
      unidadeId
    )
  }

  return {
    // Estado
    vinculos,
    
    // Computados
    vinculosPorUsuario,
    vinculosPorUnidade,
    usuarioPodeVincularMais,
    unidadesVinculadasUsuarioSelecionado,
    
    // Métodos
    carregarVinculos,
    vincular,
    desvincular,
  }
}