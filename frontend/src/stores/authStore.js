import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api'; // Certifique-se que este arquivo existe
import { jwtDecode } from 'jwt-decode';
import router from '@/router';

export const useAuthStore = defineStore('auth', () => {
  // Estado
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(null);
  const loading = ref(false);
  const error = ref(null);

  // --- Actions Auxiliares ---

  function clearAuth() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('authToken');
    // Remove o header padrão do axios
    delete api.defaults.headers.common['Authorization'];
    error.value = null;
  }

  function setUserAndToken(newToken) {
    token.value = newToken;
    if (newToken) {
      try {
        const decodedToken = jwtDecode(newToken);
        
        // Lógica para encontrar as Roles no token (compatível com Identity .NET e outros)
        let userRoles = [];
        const possibleRoleClaims = [
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
          "role",
          "roles",
          "Role"
        ];
        
        for (const claim of possibleRoleClaims) {
          if (decodedToken[claim]) {
            userRoles = Array.isArray(decodedToken[claim]) 
              ? decodedToken[claim] 
              : [decodedToken[claim]];
            break;
          }
        }

        user.value = {
          email: decodedToken.email || decodedToken.sub,
          nameid: decodedToken.nameid,
          roles: userRoles
        };
        
        // Salva e configura o Axios
        localStorage.setItem('authToken', newToken);
        api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        error.value = null;

      } catch (decodeError) {
        console.error('Erro ao decodificar token:', decodeError);
        clearAuth();
      }
    } else {
      clearAuth();
    }
  }

  // --- Actions Principais ---

  async function login(credentials) {
    loading.value = true;
    error.value = null;
    
    try {
      // ✅ Endpoint correto para pegar o token
      const response = await api.post('/api/token', credentials);
      
      if (response.data && response.data.token) {
        setUserAndToken(response.data.token);
        // Redirecionamento acontece aqui ou no componente, 
        // mas retornar true ajuda o componente a saber que deu certo
        return { success: true };
      } else {
        throw new Error('Token não recebido na resposta do servidor');
      }
    } catch (err) {
      console.error('Erro no login:', err);
      let errorMessage = 'Email ou senha inválidos.';
      
      if (err.code === 'ERR_NETWORK') {
        errorMessage = 'Erro de conexão. Verifique se o servidor está rodando.';
      } else if (err.response && err.response.data) {
        // Tenta pegar mensagem específica do backend
        errorMessage = err.response.data.message || errorMessage;
      }
      
      error.value = errorMessage;
      clearAuth();
      return { success: false, error: errorMessage };
    } finally {
      loading.value = false;
    }
  }

  function logout() {
    clearAuth();
    router.push('/'); // Redireciona para o login
  }

  function checkAuth() {
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      try {
        const decoded = jwtDecode(storedToken);
        const currentTime = Date.now() / 1000;
        
        if (decoded.exp < currentTime) {
          console.warn('Token expirado');
          logout();
          return false;
        }
        
        setUserAndToken(storedToken);
        return true;
      } catch (error) {
        logout();
        return false;
      }
    }
    return false;
  }

  // --- Getters ---
  const isAdmin = computed(() => {
    if (!user.value?.roles) return false;
    return user.value.roles.some(role => role.toLowerCase() === 'admin');
  });

  const isAuthenticated = computed(() => !!token.value);

  // Inicialização
  checkAuth();

  return { 
    token, 
    user, 
    loading, 
    error, 
    isAdmin, 
    isAuthenticated,
    login, 
    logout, 
    checkAuth 
  };
});