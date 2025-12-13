// src/Services/BillingIntegrationService.js
const axios = require('axios');

const billingApi = axios.create({
    baseURL: process.env.BILLING_SERVICE_URL || 'http://billing_service:8080',
});

class BillingIntegrationService {
    
    async fetchUnidades(token) {
        if (!token) return [];
        try {
            const response = await billingApi.get('/api/unidades', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            return response.data.$values || response.data || [];
        } catch (error) {
            console.error('AnalysisService: Erro ao buscar Unidades.', error.message);
            return [];
        }
    }

    async fetchFechamentos(token, unidadeId) {
        if (!token || !unidadeId) return [];
        try {
            const response = await billingApi.get(`/api/unidades/${unidadeId}/fechamentos`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            return response.data.$values || response.data || [];
        } catch (error) {
            console.error(`AnalysisService: Erro ao buscar fechamentos para ${unidadeId}.`, error.message);
            return [];
        }
    }

    async fetchDespesas(token, unidadeId) {
        if (!token || !unidadeId) return [];
        try {
            const response = await billingApi.get(`/api/expenses/unidade/${unidadeId}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            return response.data.$values || response.data || [];
        } catch (error) {
            console.error(`AnalysisService: Erro ao buscar despesas para ${unidadeId}.`, error.message);
            return [];
        }
    }
}

module.exports = new BillingIntegrationService();