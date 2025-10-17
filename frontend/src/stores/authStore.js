import { ref } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api';
import { jwtDecode } from 'jwt-decode';
import router from '@/router';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(token.value ? jwtDecode(token.value) : null);
  const loading = ref(false);
  const error = ref(null);

  function setUserAndToken(newToken) {
    token.value = newToken;
    if (newToken) {
      try {
        user.value = jwtDecode(newToken);
        localStorage.setItem('authToken', newToken);
        api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        error.value = null;
        console.log('🔑 Token definido com sucesso para usuário:', user.value);
      } catch (decodeError) {
        console.error('❌ Erro ao decodificar token:', decodeError);
        clearAuth();
      }
    } else {
      clearAuth();
    }
  }

  function clearAuth() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('authToken');
    delete api.defaults.headers.common['Authorization'];
    error.value = null;
    console.log('🔒 Auth limpo');
  }

  async function login(credentials) {
    loading.value = true;
    error.value = null;
    
    try {
      console.log('🔐 Iniciando processo de login...', {
        url: api.defaults.baseURL + '/api/token',
        email: credentials.email
      });

      const response = await api.post('/api/token', credentials, {
        headers: {
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache'
        }
      });

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
    } catch (error) {
      console.error('❌ Erro completo no login:', error);
      
      let errorMessage = 'Erro ao fazer login';
      
      if (error.code === 'NETWORK_ERROR' || error.message?.includes('Network Error')) {
        errorMessage = 'Erro de conexão. Verifique sua internet e tente novamente.';
      } else if (error.response) {
        // Servidor respondeu com erro
        errorMessage = error.response.data?.message || `Erro ${error.response.status}: ${error.response.statusText}`;
      } else if (error.request) {
        // Requisição foi feita mas não houve resposta
        errorMessage = 'Servidor não respondeu. Verifique se o backend está rodando.';
      }
      
      error.value = errorMessage;
      clearAuth();
      
      return { 
        success: false, 
        error: errorMessage,
        details: error.response?.data 
      };
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
        // Verifica se o token não está expirado
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

  // Verifica autenticação ao inicializar
  const initialToken = localStorage.getItem('authToken');
  if (initialToken) {
    checkAuth();
  }

  return { 
    token, 
    user, 
    loading, 
    error, 
    login, 
    logout, 
    checkAuth,
    clearAuth 
  };
});