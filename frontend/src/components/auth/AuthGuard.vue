<!-- src/components/auth/AuthGuard.vue -->
<template>
  <slot v-if="isAuthenticated && hasAccess"></slot>
  
  <div v-else-if="!isAuthenticated" class="auth-guard">
    <div class="unauthorized">
      <i class="fas fa-lock"></i>
      <h2>Acesso Restrito</h2>
      <p>Você precisa estar autenticado para acessar esta página.</p>
      <router-link to="/login" class="btn-login">
        <i class="fas fa-sign-in-alt"></i>
        Fazer Login
      </router-link>
    </div>
  </div>
  
  <div v-else class="auth-guard">
    <div class="forbidden">
      <i class="fas fa-ban"></i>
      <h2>Acesso Negado</h2>
      <p>Você não tem permissão para acessar esta página.</p>
      <router-link to="/dashboard" class="btn-back">
        <i class="fas fa-arrow-left"></i>
        Voltar ao Dashboard
      </router-link>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue'
import { useAuth } from '@/composables/auth/useAuth'
import { useRBAC } from '@/composables/auth/useRBAC'

export default {
  name: 'AuthGuard',
  
  props: {
    requireAuth: {
      type: Boolean,
      default: true
    },
    requiredRoles: {
      type: Array,
      default: () => []
    },
    requiredPermissions: {
      type: Array,
      default: () => []
    }
  },
  
  setup(props) {
    const { isAuthenticated } = useAuth()
    const { hasRole, hasAllPermissions } = useRBAC()
    
    const hasAccess = computed(() => {
      // Se não requer autenticação, permite acesso
      if (!props.requireAuth) return true
      
      // Verifica se está autenticado
      if (!isAuthenticated.value) return false
      
      // Verifica roles
      if (props.requiredRoles.length > 0) {
        if (!hasRole(props.requiredRoles)) return false
      }
      
      // Verifica permissões
      if (props.requiredPermissions.length > 0) {
        if (!hasAllPermissions(props.requiredPermissions)) return false
      }
      
      return true
    })
    
    return {
      isAuthenticated,
      hasAccess
    }
  }
}
</script>

<style scoped>
.auth-guard {
  min-height: 60vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.unauthorized, .forbidden {
  text-align: center;
  max-width: 400px;
  padding: 40px;
  border-radius: 16px;
  background: white;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
}

.unauthorized {
  border-top: 5px solid #ff944d;
}

.forbidden {
  border-top: 5px solid #ff4d4d;
}

.unauthorized i, .forbidden i {
  font-size: 48px;
  margin-bottom: 20px;
}

.unauthorized i {
  color: #ff944d;
}

.forbidden i {
  color: #ff4d4d;
}

.unauthorized h2, .forbidden h2 {
  color: #333;
  margin-bottom: 10px;
}

.unauthorized p, .forbidden p {
  color: #666;
  margin-bottom: 30px;
}

.btn-login, .btn-back {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 12px 24px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.3s;
}

.btn-login {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-login:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
}

.btn-back {
  background: #f5f5f5;
  color: #333;
  border: 1px solid #ddd;
}

.btn-back:hover {
  background: #eee;
  transform: translateY(-2px);
}
</style>