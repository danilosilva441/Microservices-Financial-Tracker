// src/composables/auth/useAppInitializer.js
import { ref, onMounted } from 'vue'
import { useAuth } from './useAuth'
import { useRBAC } from './useRBAC'

export function useAppInitializer() {
  const { initializeAuth, isAuthenticated, isLoading } = useAuth()
  const { hasPermission } = useRBAC()
  
  const isAppReady = ref(false)
  const initializationError = ref(null)

  // Inicializa a aplicação
  const initializeApp = async () => {
    try {
      console.log('🔧 Inicializando aplicação...')
      
      // 1. Inicializa autenticação
      await initializeAuth()
      
      // 2. Verifica permissões se autenticado
      if (isAuthenticated.value) {
        if (!hasPermission('view_dashboard')) {
          initializationError.value = 'Usuário não tem permissão para acessar o dashboard'
          console.warn('Permissões insuficientes:', initializationError.value)
        }
      }
      
      // 3. Outras inicializações podem ser adicionadas aqui
      
      isAppReady.value = true
      console.log('✅ Aplicação inicializada com sucesso')
      
    } catch (error) {
      console.error('❌ Erro ao inicializar aplicação:', error)
      initializationError.value = error.message || 'Erro desconhecido'
      isAppReady.value = false
    }
  }

  // Reinicializa a aplicação
  const reinitializeApp = async () => {
    isAppReady.value = false
    initializationError.value = null
    await initializeApp()
  }

  // Inicializa quando o composable é criado
  onMounted(() => {
    initializeApp()
  })

  return {
    // State
    isAppReady,
    initializationError,
    isAuthenticated,
    isLoading,
    
    // Actions
    initializeApp,
    reinitializeApp,
  }
}