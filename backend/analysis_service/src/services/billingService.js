const axios = require('axios');

const billingApi = axios.create({
  baseURL: process.env.BILLING_SERVICE_URL || 'http://billing_service:8080',
});

async function fetchOperacoes(token) {
  if (!token) return [];
  try {
    console.log('AnalysisService: Buscando operações no Billing Service...');
    const response = await billingApi.get('/api/operacoes', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    // Lida com o formato de resposta do .NET que pode vir com '$values'
    return response.data.$values || response.data || [];
  } catch (error) {
    console.error('AnalysisService: Erro ao buscar operações.', error.response?.data || error.message);
    return [];
  }
}

// ... (função fetchOperacoes existente)

async function updateProjecao(operacaoId, projecao, token) {
  try {
    await billingApi.patch(`/api/operacoes/${operacaoId}/projecao`,
      { projecaoFaturamento: projecao },
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    console.log(`AnalysisService: Projeção para operação ${operacaoId} atualizada com sucesso.`);
  } catch (error) {
    console.error(`AnalysisService: Erro ao atualizar projeção para ${operacaoId}.`, error.response?.data);
  }
}

module.exports = { fetchOperacoes, updateProjecao };