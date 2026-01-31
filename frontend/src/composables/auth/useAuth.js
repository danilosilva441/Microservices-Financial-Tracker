// src/composables/auth/useAuth.js
import { computed, watch, ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

export function useAuth() {
  const authStore = useAuthStore()
  const isInitializing = ref(false)

  // Computeds
  const isAuthenticated = computed(() => authStore.isAuthenticated)
  const currentUser = computed(() => authStore.currentUser)
  const userRole = computed(() => authStore.userRole)
  const userTenantId = computed(() => authStore.userTenantId)
  const isLoading = computed(() => authStore.isLoadingState)
  const error = computed(() => authStore.errorMessage)
  const token = computed(() => authStore.token)

  // Actions (apenas passando para a store)
  const login = authStore.login
  const logout = authStore.logout
  const provisionTenant = authStore.provisionTenant
  const registerTeamUser = authStore.registerTeamUser
  const clearError = authStore.clearError
  const refreshUserData = authStore.fetchUserData
  const isTokenExpired = authStore.isTokenExpired

  // Inicializar auth
  const initializeAuth = async () => {
    isInitializing.value = true
    try {
      return await authStore.initializeAuth()
    } finally {
      isInitializing.value = false
    }
  }

  // Watchers
  watch(isAuthenticated, (newValue) => {
    console.log('Estado de autenticação alterado:', newValue)
  })

  watch(error, (newError) => {
    if (newError) {
      console.error('Erro de autenticação:', newError)
    }
  })

  // Inicialização automática
  onMounted(() => {
    if (!isAuthenticated.value) {
      initializeAuth()
    }
  })

  return {
    // State
    isAuthenticated,
    currentUser,
    userRole,
    userTenantId,
    isLoading,
    error,
    token,
    isInitializing: computed(() => isInitializing.value),

    // Actions
    login,
    logout,
    provisionTenant,
    registerTeamUser,
    clearError,
    initializeAuth,
    refreshUserData,
    isTokenExpired,
  }
}