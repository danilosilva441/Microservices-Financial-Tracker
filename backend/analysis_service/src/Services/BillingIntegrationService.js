// src/Services/BillingIntegrationService.js
const axios = require('axios');
const { getSystemToken } = require('../Config/auth'); // Importa nosso gerenciador

const billingApi = axios.create({
    baseURL: process.env.BILLING_SERVICE_URL || 'http://billing_service:8080',
});

class BillingIntegrationService {

    // Helper privado para configurar o Header
    async _getHeaders() {
        const token = await getSystemToken();
        return {
            headers: { 'Authorization': `Bearer ${token}` }
        };
    }

    async fetchUnidades() { // Removemos o parametro 'token'
        try {
            const config = await this._getHeaders();
            const response = await billingApi.get('/api/unidades', config);
            return response.data.$values || response.data || [];
        } catch (error) {
            this._handleError('Unidades', error);
            return [];
        }
    }

    async fetchFechamentos(unidadeId) { // Removemos o parametro 'token'
        if (!unidadeId) return [];
        try {
            const config = await this._getHeaders();
            const response = await billingApi.get(`/api/unidades/${unidadeId}/fechamentos`, config);
            return response.data.$values || response.data || [];
        } catch (error) {
            this._handleError(`Fechamentos (${unidadeId})`, error);
            return [];
        }
    }

    async fetchDespesas(unidadeId) { // Removemos o parametro 'token'
        if (!unidadeId) return [];
        try {
            const config = await this._getHeaders();
            // AQUI ESTAVA O ERRO PROVAVELMENTE: O endpoint deve aceitar System User
            const response = await billingApi.get(`/api/expenses/unidade/${unidadeId}`, config);
            return response.data.$values || response.data || [];
        } catch (error) {
            this._handleError(`Despesas (${unidadeId})`, error);
            return [];
        }
    }

    _handleError(context, error) {
        if (error.response && error.response.status === 401) {
            console.error(`⛔ AnalysisService: 401 Unauthorized em ${context}. O Token de Sistema pode não ter permissão de Admin no BillingService.`);
        } else {
            console.error(`⚠️ AnalysisService: Erro em ${context}:`, error.message);
        }
    }
}

module.exports = new BillingIntegrationService();