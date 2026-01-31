// auth.store.js
import { defineStore } from 'pinia';
import AuthService from '@/services/auth.service.js';
import router from '@/router';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token') || null,
    isLoading: false,
    error: null,
    tenantId: localStorage.getItem('tenantId') || null,
    role: localStorage.getItem('role') || null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    currentUser: (state) => state.user,
    userRole: (state) => state.role,
    userTenantId: (state) => state.tenantId,
    isLoadingState: (state) => state.isLoading,
    errorMessage: (state) => state.error,
  },

  actions: {
    async login({ email, password }) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await AuthService.login(email, password);
        
        // Verifica o formato da resposta
        const token = response.token || response.access_token || response;
        
        if (!token) {
          throw new Error('Token não recebido da API');
        }

        // Decodifica o token JWT
        const decodedToken = this.decodeJWT(token);
        
        // Extrai informações do token de forma mais segura
        this.token = token;
        
        // Procura tenantId em diferentes possíveis claims
        this.tenantId = decodedToken?.tenantId || 
                       decodedToken?.TenantId || 
                       decodedToken?.tenantid ||
                       decodedToken?.['tenant_id'] ||
                       decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/tenantid'];
        
        // Procura role em diferentes possíveis claims
        this.role = decodedToken?.role || 
                   decodedToken?.Role || 
                   decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
                   decodedToken?.roles?.[0] || 
                   decodedToken?.userRole;
        
        localStorage.setItem('token', token);
        if (this.tenantId) localStorage.setItem('tenantId', this.tenantId);
        if (this.role) localStorage.setItem('role', this.role);
        
        // Busca informações do usuário
        await this.fetchUserData();
        
        router.push('/dashboard');
        
        return { success: true };
      } catch (error) {
        console.error('Erro no login:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao fazer login';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    async provisionTenant(tenantData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await AuthService.provisionTenant(tenantData);
        
        // Se a criação for bem-sucedida e tiver credenciais, faz login
        if (response && tenantData.emailDoGerente && tenantData.senhaDoGerente) {
          await this.login({
            email: tenantData.emailDoGerente,
            password: tenantData.senhaDoGerente
          });
        }
        
        return response;
      } catch (error) {
        console.error('Erro ao criar tenant:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar empresa';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    async fetchUserData() {
      if (!this.token) return;
      
      try {
        const userData = await AuthService.getMe();
        this.user = userData;
        
        // Atualiza informações se disponíveis
        if (userData.role && !this.role) {
          this.role = userData.role;
          localStorage.setItem('role', userData.role);
        }
        
        if (userData.tenantId && !this.tenantId) {
          this.tenantId = userData.tenantId;
          localStorage.setItem('tenantId', userData.tenantId);
        }
        
        return userData;
      } catch (error) {
        console.error('Erro ao buscar dados do usuário:', error);
        if (error.response?.status === 401) {
          this.logout();
        }
        throw error;
      }
    },

    async registerTeamUser(userData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await AuthService.registerTeamUser(userData);
        return response;
      } catch (error) {
        console.error('Erro ao criar usuário:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar usuário';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    logout() {
      // Limpa estado
      this.token = null;
      this.user = null;
      this.tenantId = null;
      this.role = null;
      this.error = null;
      this.isLoading = false;
      
      // Limpa localStorage
      localStorage.removeItem('token');
      localStorage.removeItem('tenantId');
      localStorage.removeItem('role');
      localStorage.removeItem('userData');
      
      // Redireciona para login
      if (router.currentRoute.path !== '/login') {
        router.push('/login');
      }
    },

    async initializeAuth() {
      const token = localStorage.getItem('token');
      
      if (token) {
        this.token = token;
        this.tenantId = localStorage.getItem('tenantId');
        this.role = localStorage.getItem('role');
        
        try {
          // Verifica se o token está expirado
          if (this.isTokenExpired()) {
            this.logout();
            return false;
          }
          
          // Busca dados do usuário
          await this.fetchUserData();
          return true;
        } catch (error) {
          console.error('Erro ao restaurar sessão:', error);
          this.logout();
          return false;
        }
      }
      return false;
    },

    decodeJWT(token) {
      try {
        const parts = token.split('.');
        if (parts.length !== 3) {
          throw new Error('Token JWT inválido');
        }
        
        const base64Url = parts[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
          atob(base64)
            .split('')
            .map(c => {
              return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            })
            .join('')
        );
        
        return JSON.parse(jsonPayload);
      } catch (error) {
        console.error('Erro ao decodificar token:', error);
        return null;
      }
    },

    isTokenExpired() {
      if (!this.token) return true;
      
      const decoded = this.decodeJWT(this.token);
      if (!decoded || !decoded.exp) return true;
      
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp < currentTime;
    },

    clearError() {
      this.error = null;
    },
  },
});