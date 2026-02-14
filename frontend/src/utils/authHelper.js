// src/utils/authHelper.js (mantém igual, com pequeno ajuste no nome da store)
export function getSafeUsuarioLogado() {
  try {
    // Tentar importar dinamicamente para evitar erros de importação
    const { useAuthStore } = require('@/stores/auth.store');
    const authStore = useAuthStore();
    
    if (authStore && authStore.user) {
      // Mesma lógica de extração de nome do código acima
      let nomeUsuario = 'Operador';
      
      if (authStore.user.name) {
        nomeUsuario = authStore.user.name;
      } else if (authStore.user.nome) {
        nomeUsuario = authStore.user.nome;
      } else if (authStore.user.username) {
        nomeUsuario = authStore.user.username;
      } else if (authStore.user.email) {
        nomeUsuario = authStore.user.email.split('@')[0];
      } else if (authStore.user.sub) {
        nomeUsuario = authStore.user.sub;
      }
      
      return {
        nome: nomeUsuario,
        id: authStore.user.nameid || authStore.user.id || null,
        email: authStore.user.email || null
      };
    }
  } catch (error) {
    console.warn('Não foi possível acessar authStore:', error);
  }
  
  // Fallback: tentar pegar do localStorage
  try {
    const token = localStorage.getItem('token'); // Alterado de authToken para token
    if (token) {
      const { jwtDecode } = require('jwt-decode');
      const decoded = jwtDecode(token);
      
      let nomeUsuario = 'Operador';
      if (decoded.name) {
        nomeUsuario = decoded.name;
      } else if (decoded.email) {
        nomeUsuario = decoded.email.split('@')[0];
      } else if (decoded.sub) {
        nomeUsuario = decoded.sub;
      }
      
      return {
        nome: nomeUsuario,
        id: decoded.nameid || decoded.sub || null,
        email: decoded.email || null
      };
    }
  } catch (error) {
    console.warn('Não foi possível decodificar token do localStorage:', error);
  }
  
  // Fallback final
  return {
    nome: 'Sistema',
    id: null,
    email: null
  };
}