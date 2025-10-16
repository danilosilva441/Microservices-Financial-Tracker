const axios = require('axios');

// Define a URL base para o AuthService.
// 1. Tenta usar a variável de ambiente AUTH_SERVICE_URL (para produção/Railway).
// 2. Se não existir, usa o valor padrão 'http://auth_service:8080' (para o Docker Compose local).
const authServiceUrl = process.env.AUTH_SERVICE_URL || 'http://auth_service:8080';

// Cria uma instância do axios que aponta para o AuthService.
const authApi = axios.create({
    baseURL: authServiceUrl,
});

/**
 * Autentica o serviço como um "utilizador de sistema" e retorna um token JWT.
 * @returns {Promise<string|null>} O token JWT ou nulo em caso de falha.
 */
async function getAuthToken() {
    try {
        console.log(`AnalysisService: A autenticar no endereço: ${authServiceUrl}`);
        const response = await authApi.post('/api/token', {
            email: process.env.SYSTEM_EMAIL,
            password: process.env.SYSTEM_PASSWORD,
        });
        console.log('AnalysisService: Autenticado com sucesso.');
        return response.data.token;
    } catch (error) {
        // Regista o erro de forma mais detalhada para facilitar a depuração
        const errorMessage = error.response?.data?.title || error.response?.data || error.message;
        console.error('AnalysisService: Falha ao autenticar.', errorMessage);
        return null;
    }
}

module.exports = { getAuthToken };