import { ref } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api';
import { jwtDecode } from 'jwt-decode'; // Agora isto vai funcionar
import router from '@/router'; // Importa o "GPS"

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(token.value ? jwtDecode(token.value) : null);

  function setUserAndToken(newToken) {
    token.value = newToken;
    if (newToken) {
      user.value = jwtDecode(newToken);
      localStorage.setItem('authToken', newToken);
      api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
    } else {
      user.value = null;
      localStorage.removeItem('authToken');
      delete api.defaults.headers.common['Authorization'];
    }
  }

  async function login(credentials) {
    try {
      const response = await api.post('/api/token', credentials);
      if (response.data && response.data.token) {
        // 1. Guarda o token
        setUserAndToken(response.data.token); 
        
        // 2. Comanda a navegação para o dashboard
        await router.push('/dashboard'); // <-- A LINHA MÁGICA
        
        return true;
      }
      return false;
    } catch (error) {
      console.error("Erro no login:", error);
      setUserAndToken(null);
      return false;
    }
  }

  function logout() {
    setUserAndToken(null);
    router.push('/');
  }

  // Carrega o token ao iniciar
  const initialToken = localStorage.getItem('authToken');
  if (initialToken) {
    setUserAndToken(initialToken);
  }

  return { token, user, login, logout };
});