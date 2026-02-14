// src/composables/auth/useAuth.js
import { computed, watch, ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

export function useAuth() {
  const authStore = useAuthStore()
  const isInitializing = ref(false)

  // Computeds - adaptados para o store atual
  const isAuthenticated = computed(() => authStore.isAuthenticated)
  const userData = computed(() => authStore.user) // user ao invés de userData
  const userRole = computed(() => authStore.role) // role ao invés de userRole
  const userTenantId = computed(() => authStore.tenantId) // tenantId ao invés de userTenantId
  const isLoading = computed(() => authStore.isLoading) // isLoading ao invés de isLoadingState
  const error = computed(() => authStore.error) // error ao invés de errorMessage
  const token = computed(() => authStore.token)

  // Actions (apenas passando para a store) - mantém os mesmos nomes
  const login = (credentials) => authStore.login(credentials)
  const logout = () => authStore.logout()
  const provisionTenant = (tenantData) => authStore.provisionTenant(tenantData)
  const registerTeamUser = (userData) => authStore.registerTeamUser(userData)
  const clearError = () => authStore.clearError()
  const refreshUserData = () => authStore.fetchUserData()
  const isTokenExpired = () => authStore.isTokenExpired()

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
    userData,
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