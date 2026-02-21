import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useAdminStore } from '@/stores/admin.store'

export function useAdminUI() {
  const adminStore = useAdminStore()
  const { usuarioSelecionado, isLoading, error } = storeToRefs(adminStore)

  const hasUsuarioSelecionado = computed(() => !!usuarioSelecionado.value)
  
  const usuarioSelecionadoNome = computed(() => 
    usuarioSelecionado.value?.fullName || ''
  )
  
  const usuarioSelecionadoRole = computed(() => 
    usuarioSelecionado.value?.role || ''
  )

  const selecionarUsuario = (usuario) => {
    adminStore.selecionarUsuario(usuario)
  }

  const limparSelecao = () => {
    adminStore.limparSelecao()
  }

  const formatarData = (data) => {
    return adminStore.formatarData(data)
  }

  return {
    // Estado
    usuarioSelecionado,
    isLoading,
    error,
    
    // Computados
    hasUsuarioSelecionado,
    usuarioSelecionadoNome,
    usuarioSelecionadoRole,
    
    // Métodos
    selecionarUsuario,
    limparSelecao,
    formatarData,
  }
}