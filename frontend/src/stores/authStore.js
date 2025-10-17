import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '@/services/api';
import { useRouter } from 'vue-router';

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(localStorage.getItem('authToken'));
  const user = ref(null); // Guardará os dados decodificados do token
  const router = useRouter();

  // Getter para verificar se o usuário é Admin
  const isAdmin = computed(() => {
    return user.value?.role?.includes('Admin');
  });

  // Action para decodificar e salvar os dados do usuário
  function setUserFromToken(jwt) {
    if (!jwt) {
        user.value = null;
        return;
    }
    try {
      const payload = JSON.parse(atob(jwt.split('.')[1]));
      user.value = {
        id: payload.sub,
        email: payload.email,
        // A claim de role pode ter nomes diferentes, então verificamos os dois
        role: payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      };
    } catch (e) {
      console.error("Erro ao decodificar token:", e);
      user.value = null;
    }
  }

  // Action de Login
  async function login(credentials) {
    const response = await api.post('/token', credentials);
    const newToken = response.data.token;
    localStorage.setItem('authToken', newToken);
    token.value = newToken;
    setUserFromToken(newToken);
  }

  // Action de Logout
  function logout() {
    localStorage.removeItem('authToken');
    token.value = null;
    user.value = null;
    router.push('/');
  }

  // Carrega o usuário do token ao iniciar a store
  setUserFromToken(token.value);

  return { token, user, isAdmin, login, logout };
});