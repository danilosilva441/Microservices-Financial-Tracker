import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api';
import { jwtDecode } from 'jwt-decode';
import router from '@/router';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(null);
  const loading = ref(false);
  const error = ref(null);

  function clearAuth() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('authToken');
    delete api.defaults.headers.common['Authorization'];
    error.value = null;
    console.log('🔒 Auth limpo');
  }

  function setUserAndToken(newToken) {
    token.value = newToken;
    if (newToken) {
      try {
        const decodedToken = jwtDecode(newToken);
        console.log('🔍 Token decodificado:', decodedToken);
        
        // Busca flexível pela role
        let userRoles = [];
        
        const possibleRoleClaims = [
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
          "role",
          "roles",
          "Role",
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"
        ];
        
        for (const claim of possibleRoleClaims) {
          if (decodedToken[claim]) {
            userRoles = Array.isArray(decodedToken[claim]) 
              ? decodedToken[claim] 
              : [decodedToken[claim]];
            console.log(`✅ Roles encontradas na claim: ${claim}`, userRoles);
            break;
          }
        }

        user.value = {
          email: decodedToken.email || decodedToken.sub,
          nameid: decodedToken.nameid,
          roles: userRoles // Agora armazenamos como array
        };
        
        console.log('👤 Utilizador definido:', user.value);
        
        localStorage.setItem('authToken', newToken);
        api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        error.value = null;

      } catch (decodeError) {
        console.error('❌ Erro ao descodificar token:', decodeError);
        clearAuth();
      }
    } else {
      clearAuth();
    }
  }

  // CORREÇÃO PRINCIPAL: Verificação correta para arrays
  const isAdmin = computed(() => {
    if (!user.value || !user.value.roles || user.value.roles.length === 0) {
      console.log('❌ Sem roles definidas');
      return false;
    }
    
    console.log('🔍 Verificando roles:', user.value.roles);
    
    // Verifica se qualquer uma das roles é 'Admin' (case insensitive)
    const isAdmin = user.value.roles.some(role => 
      role.toString().toLowerCase() === 'admin'
    );
    
    console.log('👑 É admin?', isAdmin);
    return isAdmin;
  });

  // Para compatibilidade, mantemos também a propriedade role
  const userRole = computed(() => {
    if (!user.value || !user.value.roles || user.value.roles.length === 0) {
      return null;
    }
    return user.value.roles[0]; // Retorna a primeira role
  });

  async function login(credentials) {
    loading.value = true;
    error.value = null;
    
    try {
      const response = await api.post('/api/token', credentials);
      if (response.data && response.data.token) {
        setUserAndToken(response.data.token);
        await router.push('/dashboard');
        return { success: true };
      } else {
        error.value = 'Token não recebido na resposta';
        return { success: false, error: error.value };
      }
    } catch (err) {
      console.error('❌ Erro completo no login:', err);
      let errorMessage = 'Login ou senha inválidos.';
      if (err.code === 'ERR_NETWORK') {
        errorMessage = 'Erro de conexão. O servidor parece estar offline.';
      }
      error.value = errorMessage;
      clearAuth();
      return { success: false, error: errorMessage };
    } finally {
      loading.value = false;
    }
  }

  function logout() {
    console.log('🚪 Fazendo logout...');
    clearAuth();
    router.push('/');
  }

  function checkAuth() {
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      try {
        const decoded = jwtDecode(storedToken);
        const currentTime = Date.now() / 1000;
        
        if (decoded.exp < currentTime) {
          console.warn('⚠️ Token expirado');
          clearAuth();
          return false;
        }
        
        setUserAndToken(storedToken);
        return true;
      } catch (error) {
        console.error('❌ Erro ao verificar token:', error);
        clearAuth();
        return false;
      }
    }
    return false;
  }

  console.log('🔄 Inicializando auth store...');
  checkAuth();

  return { 
    token, user, loading, error, isAdmin, userRole,
    login, logout, checkAuth, clearAuth 
  };
});