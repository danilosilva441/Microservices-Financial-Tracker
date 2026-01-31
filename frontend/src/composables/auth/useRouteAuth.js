// src/composables/auth/useRouteAuth.js
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from './useAuth'

export function useRouteAuth() {
  const router = useRouter()
  const route = useRoute()
  const { isAuthenticated, userRole, userTenantId, initializeAuth } = useAuth()

  // Verifica se tem acesso à rota
  const hasRouteAccess = computed(() => {
    const routeMeta = route.meta || {}
    
    // Se a rota não requer autenticação
    if (!routeMeta.requiresAuth) {
      return true
    }
    
    // Verifica autenticação
    if (!isAuthenticated.value) {
      return false
    }
    
    // Verifica roles específicas
    if (routeMeta.roles && Array.isArray(routeMeta.roles)) {
      const currentRole = userRole.value || ''
      return routeMeta.roles.some(role => 
        role.toLowerCase() === currentRole.toLowerCase()
      )
    }
    
    // Verifica tenant específico
    if (routeMeta.tenantId && routeMeta.tenantId !== userTenantId.value) {
      return false
    }
    
    return true
  })

  // Redireciona para login
  const redirectToLogin = (redirectPath) => {
    const redirect = redirectPath || route.fullPath
    router.push({
      name: 'login',
      query: { redirect }
    })
  }

  // Redireciona para dashboard
  const redirectToDashboard = () => {
    router.push({ name: 'dashboard' })
  }

  // Garante autenticação
  const ensureAuthentication = async () => {
    if (isAuthenticated.value) {
      return true
    }
    
    // Tenta restaurar a sessão
    const restored = await initializeAuth()
    if (!restored) {
      redirectToLogin()
      return false
    }
    
    return true
  }

  // Middleware para proteger componentes
  const withAuth = async (callback) => {
    const hasAccess = await ensureAuthentication()
    if (hasAccess) {
      callback()
    }
  }

  return {
    // Computeds
    hasRouteAccess,
    currentRoute: computed(() => route),
    
    // Actions
    redirectToLogin,
    redirectToDashboard,
    ensureAuthentication,
    withAuth,
    
    // Informações do usuário
    userRole,
    userTenantId,
    isAuthenticated,
  }
}