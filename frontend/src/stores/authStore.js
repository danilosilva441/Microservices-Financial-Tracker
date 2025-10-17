import { ref } from 'vue';
import { defineStore } from 'pinia';
import api from '@/services/api';
import { jwtDecode } from 'jwt-decode';
import router from '@/router';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(token.value ? jwtDecode(token.value) : null);

  function setUserAndToken(newToken) {
    token.value = newToken;
    if (newToken) {
      user.value = jwtDecode(newToken);
      localStorage.setItem('authToken', newToken);
      // Define o token no cabeçalho do Axios para todas as requisições futuras
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
        // --- LÓGICA CRUCIAL ---
        // 1. Guarda o token e descodifica o utilizador
        setUserAndToken(response.data.token); 
        // 2. Navega para o dashboard
        router.push('/dashboard'); 
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
    router.push('/login');
  }

  // Tenta carregar o token do localStorage ao iniciar
  const initialToken = localStorage.getItem('authToken');
  if (initialToken) {
    setUserAndToken(initialToken);
  }

  return { token, user, login, logout };
});