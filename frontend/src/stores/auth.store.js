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
    
    // Estados adicionais para melhor gerenciamento
    users: [], // Lista de usuários do tenant
    currentTenant: null, // Dados do tenant atual
    tenants: [], // Lista de tenants (para admin)
    lastFetched: null // Timestamp do último fetch
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    currentUser: (state) => state.user,
    userRole: (state) => state.role,
    userTenantId: (state) => state.tenantId,
    isLoadingState: (state) => state.isLoading,
    errorMessage: (state) => state.error,
    
    // Getters adicionais
    isAdmin: (state) => state.role === 'Admin' || state.role === 'MasterAdmin',
    isManager: (state) => state.role === 'Gerente' || state.role === 'Manager',
    canManageUsers: (state) => ['Admin', 'Gerente', 'Manager', 'Leader'].includes(state.role),
    getUserById: (state) => (userId) => state.users.find(u => u.id === userId)
  },

  actions: {
    // ==================== ENDPOINTS DE AUTENTICAÇÃO ====================
    
    /**
     * ENDPOINT: POST /api/token
     * DESCRIÇÃO: Autentica o usuário e retorna o token JWT
     * USO: Login de usuários existentes
     * PAYLOAD: { email, password }
     * RETORNO: Token JWT (string ou objeto com token)
     */
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
        
        // Extrai informações do token
        this.token = token;
        this.tenantId = this.extractTenantId(decodedToken);
        this.role = this.extractUserRole(decodedToken);
        
        // Persiste no localStorage
        this.persistAuthData();
        
        // Busca informações detalhadas do usuário
        await this.fetchUserData();
        
        router.push('/dashboard');
        
        return { success: true, data: response };
      } catch (error) {
        console.error('Erro no login:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao fazer login';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: POST /api/logout
     * DESCRIÇÃO: Faz logout no backend (invalida token)
     * NOTA: Implementar no backend se necessário
     */
    async logout() {
      this.isLoading = true;
      
      try {
        // Se existir endpoint de logout no backend
        // await AuthService.logout();
        
        // Limpa estado local
        this.clearAuthState();
        
        // Redireciona para login
        if (router.currentRoute.value.path !== '/login') {
          router.push('/login');
        }
        
        return { success: true };
      } catch (error) {
        console.error('Erro no logout:', error);
        // Mesmo com erro, faz logout local
        this.clearAuthState();
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: POST /api/refresh-token
     * DESCRIÇÃO: Renova o token JWT expirado
     * NOTA: Implementar se houver refresh token
     */
    async refreshToken() {
      try {
        // const response = await AuthService.refreshToken();
        // this.token = response.token;
        // localStorage.setItem('token', this.token);
        // return response;
      } catch (error) {
        console.error('Erro ao renovar token:', error);
        this.logout();
        throw error;
      }
    },

    // ==================== ENDPOINTS DE USUÁRIOS ====================
    
    /**
     * ENDPOINT: GET /api/users/me
     * DESCRIÇÃO: Busca dados do usuário logado (perfil)
     * USO: Carregar perfil do usuário atual
     * RETORNO: { id, fullName, email, role, tenantId, ... }
     */
    async fetchUserData() {
      if (!this.token) return;
      
      try {
        const userData = await AuthService.getMe();
        this.user = userData;
        
        // Atualiza informações se disponíveis
        this.updateUserInfo(userData);
        
        // Se o usuário tiver permissão, carrega lista de usuários
        if (this.canManageUsers) {
          await this.fetchTenantUsers();
        }
        
        this.lastFetched = new Date().toISOString();
        
        return userData;
      } catch (error) {
        console.error('Erro ao buscar dados do usuário:', error);
        if (error.response?.status === 401) {
          this.logout();
        }
        throw error;
      }
    },

    /**
     * ENDPOINT: POST /api/users/tenant-user
     * DESCRIÇÃO: Cria um usuário dentro do time (Operador, Líder, etc)
     * USO: Gerentes/Admins criarem novos usuários no tenant
     * PAYLOAD: { fullName, email, password, role, ... }
     * RETORNO: Usuário criado
     */
    async registerTeamUser(userData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await AuthService.registerTeamUser(userData);
        
        // Atualiza lista local de usuários se existir
        if (this.users) {
          this.users.push(response);
        }
        
        return { success: true, data: response };
      } catch (error) {
        console.error('Erro ao criar usuário:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar usuário';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: GET /api/users/tenant
     * DESCRIÇÃO: Lista todos os usuários do tenant atual
     * USO: Gerenciamento de usuários do time
     * RETORNO: Array de usuários
     */
    async fetchTenantUsers() {
      if (!this.tenantId) return;
      
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.getTenantUsers();
        // this.users = response;
        // return response;
      } catch (error) {
        console.error('Erro ao buscar usuários:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: GET /api/users/{id}
     * DESCRIÇÃO: Busca um usuário específico por ID
     * USO: Visualizar detalhes de um usuário
     */
    async fetchUserById(userId) {
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.getUserById(userId);
        // return response;
      } catch (error) {
        console.error('Erro ao buscar usuário:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: PUT /api/users/{id}
     * DESCRIÇÃO: Atualiza dados de um usuário
     * USO: Editar perfil ou gerenciar usuários
     */
    async updateUser(userId, userData) {
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.updateUser(userId, userData);
        
        // Atualiza lista local se necessário
        if (this.users) {
          const index = this.users.findIndex(u => u.id === userId);
          if (index !== -1) {
            this.users[index] = { ...this.users[index], ...userData };
          }
        }
        
        return { success: true, data: response };
      } catch (error) {
        console.error('Erro ao atualizar usuário:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: DELETE /api/users/{id}
     * DESCRIÇÃO: Remove um usuário
     * USO: Desativar/remover usuários do sistema
     */
    async deleteUser(userId) {
      this.isLoading = true;
      
      try {
        // Implementar no service
        // await AuthService.deleteUser(userId);
        
        // Remove da lista local
        if (this.users) {
          this.users = this.users.filter(u => u.id !== userId);
        }
        
        return { success: true };
      } catch (error) {
        console.error('Erro ao deletar usuário:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    // ==================== ENDPOINTS DE TENANT ====================
    
    /**
     * ENDPOINT: POST /api/tenant/provision
     * DESCRIÇÃO: Cria um novo Tenant (Empresa) + Gerente
     * USO: Cadastro de novas empresas no sistema
     * PAYLOAD: { nomeDaEmpresa, emailDoGerente, nomeCompletoGerente, senhaDoGerente }
     * RETORNO: Dados do tenant criado + credenciais
     */
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
        
        return { success: true, data: response };
      } catch (error) {
        console.error('Erro ao criar tenant:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar empresa';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: GET /api/tenant/current
     * DESCRIÇÃO: Busca dados do tenant atual
     * USO: Configurações e informações da empresa
     */
    async fetchCurrentTenant() {
      if (!this.tenantId) return;
      
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.getCurrentTenant();
        // this.currentTenant = response;
        // return response;
      } catch (error) {
        console.error('Erro ao buscar tenant:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: PUT /api/tenant/{id}
     * DESCRIÇÃO: Atualiza dados do tenant
     * USO: Editar informações da empresa
     */
    async updateTenant(tenantData) {
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.updateTenant(tenantData);
        // this.currentTenant = response;
        // return response;
      } catch (error) {
        console.error('Erro ao atualizar tenant:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    // ==================== ENDPOINTS DE ADMIN ====================
    
    /**
     * ENDPOINT: GET /api/admin/tenants
     * DESCRIÇÃO: Lista todos os tenants (apenas admin master)
     * USO: Administração global do sistema
     */
    async fetchAllTenants() {
      if (!this.isAdmin) return;
      
      this.isLoading = true;
      
      try {
        // Implementar no service
        // const response = await AuthService.getAllTenants();
        // this.tenants = response;
        // return response;
      } catch (error) {
        console.error('Erro ao buscar tenants:', error);
        this.error = error.message;
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * ENDPOINT: GET /api/admin/stats
     * DESCRIÇÃO: Estatísticas globais do sistema
     * USO: Dashboard administrativo
     */
    async fetchSystemStats() {
      if (!this.isAdmin) return;
      
      try {
        // Implementar no service
        // const response = await AuthService.getSystemStats();
        // return response;
      } catch (error) {
        console.error('Erro ao buscar estatísticas:', error);
        return { success: false, error: error.message };
      }
    },

    // ==================== MÉTODOS AUXILIARES ====================

    /**
     * Inicializa a autenticação ao carregar a aplicação
     * Verifica token existente e restaura sessão
     */
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

    /**
     * Decodifica token JWT
     */
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

    /**
     * Extrai tenantId do token decodificado
     */
    extractTenantId(decodedToken) {
      if (!decodedToken) return null;
      
      return decodedToken?.tenantId || 
             decodedToken?.TenantId || 
             decodedToken?.tenantid ||
             decodedToken?.tenant_id ||
             decodedToken?.sub_tenantId ||
             decodedToken?.['tenant-id'] ||
             decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/tenantid'];
    },

    /**
     * Extrai role do token decodificado
     */
    extractUserRole(decodedToken) {
      if (!decodedToken) return null;
      
      return decodedToken?.role || 
             decodedToken?.Role || 
             decodedToken?.userRole ||
             decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
             decodedToken?.roles?.[0] ||
             decodedToken?.user_type;
    },

    /**
     * Persiste dados de autenticação
     */
    persistAuthData() {
      localStorage.setItem('token', this.token);
      if (this.tenantId) localStorage.setItem('tenantId', this.tenantId);
      if (this.role) localStorage.setItem('role', this.role);
    },

    /**
     * Atualiza informações do usuário
     */
    updateUserInfo(userData) {
      if (userData.role && !this.role) {
        this.role = userData.role;
        localStorage.setItem('role', userData.role);
      }
      
      if (userData.tenantId && !this.tenantId) {
        this.tenantId = userData.tenantId;
        localStorage.setItem('tenantId', userData.tenantId);
      }
    },

    /**
     * Limpa estado de autenticação
     */
    clearAuthState() {
      this.token = null;
      this.user = null;
      this.tenantId = null;
      this.role = null;
      this.users = [];
      this.currentTenant = null;
      this.error = null;
      this.isLoading = false;
      
      localStorage.removeItem('token');
      localStorage.removeItem('tenantId');
      localStorage.removeItem('role');
    },

    /**
     * Verifica se token está expirado
     */
    isTokenExpired() {
      if (!this.token) return true;
      
      const decoded = this.decodeJWT(this.token);
      if (!decoded || !decoded.exp) return true;
      
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp < currentTime;
    },

    /**
     * Limpa mensagem de erro
     */
    clearError() {
      this.error = null;
    },

    /**
     * Reseta estado completo
     */
    resetState() {
      this.$reset();
      localStorage.removeItem('token');
      localStorage.removeItem('tenantId');
      localStorage.removeItem('role');
    }
  },
});