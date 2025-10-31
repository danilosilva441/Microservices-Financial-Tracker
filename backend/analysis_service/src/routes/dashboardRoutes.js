const express = require('express');
const router = express.Router();
const { getAuthToken } = require('../auth');
// Precisamos de importar as funções de cálculo do billingService
const { fetchOperacoes, calcularDashboardData } = require('../services/billingService');

// Rota: GET /dashboard-data
router.get('/dashboard-data', async (req, res) => {
    try {
        console.log('AnalysisService: Rota GET /dashboard-data atingida');

        // 1. Obter o token de sistema
        const token = await getAuthToken();
        if (!token) {
            return res.status(500).json({ error: 'Falha ao autenticar serviço' });
        }

        // 2. Buscar os dados brutos do BillingService
        const operacoes = await fetchOperacoes(token);

        // 3. Calcular os KPIs (usando a função que já temos)
        const dashboardData = calcularDashboardData(operacoes);

        // 4. Retornar os dados processados
        res.status(200).json({
            message: "Dados do dashboard processados com sucesso",
            data: dashboardData
        });

    } catch (error) {
        console.error('Erro no endpoint /dashboard-data:', error.message);
        res.status(500).json({ error: 'Erro interno ao processar dados do dashboard' });
    }
});

module.exports = router;