// src/Config/auth.js
const axios = require('axios');

const authServiceUrl = process.env.AUTH_SERVICE_URL || 'http://auth_service:8080';

const authApi = axios.create({
    baseURL: authServiceUrl,
});

async function getAuthToken() {
    try {
        console.log(`AnalysisService: A autenticar no endereço: ${authServiceUrl}`);
        const response = await authApi.post('/api/token', {
            email: process.env.SYSTEM_EMAIL,
            password: process.env.SYSTEM_PASSWORD,
        });
        return response.data.token;
    } catch (error) {
        console.error('AnalysisService: Falha ao autenticar.', error.message);
        return null;
    }
}

module.exports = { getAuthToken };