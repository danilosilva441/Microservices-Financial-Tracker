// Caminho: backend/analysis_service/src/services/billingService.js
const axios = require('axios');

const billingApi = axios.create({
    baseURL: process.env.BILLING_SERVICE_URL || 'http://billing_service:8080',
});

/**
 * Busca todas as Unidades (v2.0)
 */
async function fetchUnidades(token) {
    if (!token) return [];
    try {
        console.log('AnalysisService: Buscando Unidades no Billing Service (v2.0)...');
        const response = await billingApi.get('/api/unidades', {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        return response.data.$values || response.data || [];
    } catch (error) {
        console.error('AnalysisService: Erro ao buscar Unidades.', error.response?.data || error.message);
        return [];
    }
}

/**
 * Busca todos os Fechamentos Diários (v2.0) de uma Unidade específica.
 */
async function fetchFechamentos(token, unidadeId) {
    if (!token || !unidadeId) return [];
    try {
        const response = await billingApi.get(`/api/unidades/${unidadeId}/fechamentos`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        return response.data.$values || response.data || [];
    } catch (error) {
        console.error(`AnalysisService: Erro ao buscar fechamentos para ${unidadeId}.`, error.response?.data || error.message);
        return [];
    }
}

/**
 * Busca todas as Despesas (v2.0) de uma Unidade específica.
 */
async function fetchDespesas(token, unidadeId) {
    if (!token || !unidadeId) return [];
    try {
        const response = await billingApi.get(`/api/expenses/unidade/${unidadeId}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        return response.data.$values || response.data || [];
    } catch (error) {
        console.error(`AnalysisService: Erro ao buscar despesas para ${unidadeId}.`, error.response?.data || error.message);
        return [];
    }
}

// As funções de CÁLCULO foram movidas para 'dashboardCalculator.js'

module.exports = {
    fetchUnidades,
    fetchFechamentos,
    fetchDespesas
};