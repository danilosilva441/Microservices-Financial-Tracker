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

  // CORREÇÃO: isAdmin como computed property
  const isAdmin = computed(() => {
    const role = user.value?.role;
    return role === 'Admin' || role === 'admin';
  });

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
        
        const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        
        user.value = {
          email: decodedToken.email,
          nameid: decodedToken.nameid,
          role: Array.isArray(role) ? role[0] : role 
        };
        
        localStorage.setItem('authToken', newToken);
        api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        error.value = null;
        console.log('🔑 Token definido com sucesso para utilizador:', user.value);
        console.log('👑 isAdmin:', isAdmin.value); // Debug

      } catch (decodeError) {
        console.error('❌ Erro ao descodificar token:', decodeError);
        clearAuth();
      }
    } else {
      clearAuth();
    }
  }

  async function login(credentials) {
    loading.value = true;
    error.value = null;
    
    try {
      console.log('🔐 Iniciando processo de login...', {
        url: api.defaults.baseURL + '/api/token',
        email: credentials.email
      });

      const response = await api.post('/api/token', credentials);

      console.log('📨 Resposta recebida:', {
        status: response.status,
        data: response.data
      });

      if (response.data && response.data.token) {
        setUserAndToken(response.data.token);
        
        console.log('✅ Login bem-sucedido, redirecionando...');
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

  // Verifica autenticação ao inicializar a store
  checkAuth();

  return { 
    token, 
    user, 
    loading, 
    error,
    isAdmin, // ✅ CORRIGIDO: computed property
    login, 
    logout, 
    checkAuth,
    clearAuth 
  };
});