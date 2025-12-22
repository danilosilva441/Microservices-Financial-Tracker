// src/Config/auth.js
const axios = require('axios');

const authServiceUrl = process.env.AUTH_SERVICE_URL || 'http://auth_service:8080';

// Variáveis para Cache (Singleton)
let cachedToken = null;
let tokenExpiration = null; // Para controlarmos renovação futura se precisar

const authApi = axios.create({
    baseURL: authServiceUrl,
});

async function getSystemToken() {
    // 1. Se já temos um token válido em memória, retorna ele imediatamente.
    if (cachedToken) {
        return cachedToken;
    }

    // 2. Se não temos, faz o login
    try {
        console.log(`🔐 AnalysisService: Autenticando System User em: ${authServiceUrl}`);
        const response = await authApi.post('/api/token', {
            email: process.env.SYSTEM_EMAIL, 
            password: process.env.SYSTEM_PASSWORD,
        });

        if (response.data && response.data.token) {
            cachedToken = response.data.token;
            console.log('✅ Token de Sistema obtido e cacheado com sucesso.');
            
            // Opcional: Resetar token após 50 minutos (se o token dura 1h)
            setTimeout(() => { cachedToken = null; }, 50 * 60 * 1000);
            
            return cachedToken;
        }
    } catch (error) {
        console.error('❌ AnalysisService: Falha Crítica ao autenticar.', error.message);
        throw error; // Lança erro para parar o fluxo se não logar
    }
}

module.exports = { getSystemToken };