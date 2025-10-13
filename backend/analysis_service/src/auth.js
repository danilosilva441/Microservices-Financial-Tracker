const axios = require('axios');

// Aponta DIRETAMENTE para o nome do serviço no Docker Compose
const authApi = axios.create({
    baseURL: '$AUTH_SERVICE_HOST:$AUTH_SERVICE_PORT',
});

// Função que faz o login como "usuário de sistema" e retorna o token
async function getAuthToken() {
    try {
        console.log('AnalysisService: Autenticando...');
        const response = await authApi.post('/api/token', {
            email: process.env.SYSTEM_EMAIL,
            password: process.env.SYSTEM_PASSWORD,
        });
        console.log('AnalysisService: Autenticado com sucesso.');
        return response.data.token;
    } catch (error) {
        console.error('AnalysisService: Falha ao autenticar.', error.response?.data || error.message);
        return null;
    }
}

module.exports = { getAuthToken };