import api from './api';

export default {
    // Faz o login e espera receber o Token JWT
    async login(email, password) {
        // POST /api/Token
        const response = await api.post('/api/token', { email, password });
        return response.data; // Retorna o token (string) ou objeto de resposta
    },

    // Cria um novo Tenant (Empresa) + Gerente
    async provisionTenant(payload) {
        // POST /api/Tenant/provision
        // payload deve conter: { nomeDaEmpresa, emailDoGerente, nomeCompletoGerente, senhaDoGerente }
        const response = await api.post('/api/tenant/provision', payload);
        return response.data;
    },

    // Busca os dados do usuário logado (Perfil)
    async getMe() {
        // GET /api/Users/me
        const response = await api.get('/api/users/me');
        return response.data; // Retorna { id, fullName, email, role, ... }
    },

    // Cria um usuário dentro do time (Operador, Líder, etc)
    async registerTeamUser(userData) {
        // POST /api/Users/tenant-user
        const response = await api.post('/api/users/tenant-user', userData);
        return response.data;
    }
};