// Caminho: backend/analysis_service/src/routes/dashboardRoutes.js
const express = require('express');
const router = express.Router();
const { getAuthToken } = require('../auth');
const { 
    fetchUnidades, 
    fetchFechamentos, 
    fetchDespesas, 
    calcularDashboardLucro, 
    getEmptyDashboardData 
} = require('../services/billingService');

// Rota: GET /api/analysis/dashboard-data (v2.0)
router.get('/dashboard-data', async (req, res) => {
    try {
        console.log('AnalysisService: Rota GET /dashboard-data (v2.0) atingida');

        // 1. Obter o token de sistema (Admin)
        const systemToken = await getAuthToken();
        if (!systemToken) {
            return res.status(500).json({ error: 'Falha ao autenticar serviço' });
        }

        // 2. Buscar as Unidades (baseado no Tenant do token de sistema)
        const unidades = await fetchUnidades(systemToken);
        if (!unidades || unidades.length === 0) {
            console.log("AnalysisService: Nenhuma unidade encontrada.");
            return res.status(200).json({
                message: "Nenhum dado encontrado",
                data: getEmptyDashboardData()
            });
        }

        // 3. Buscar (em paralelo) os fechamentos e despesas de CADA unidade
        const dataPromises = unidades.map(async (unidade) => {
            const [fechamentos, despesas] = await Promise.all([
                fetchFechamentos(systemToken, unidade.id),
                fetchDespesas(systemToken, unidade.id)
            ]);
            return {
                unidadeId: unidade.id,
                fechamentos,
                despesas
            };
        });

        const allDataResults = await Promise.all(dataPromises);

        // 4. Mapear os resultados para fácil acesso
        const dadosPorUnidade = allDataResults.reduce((acc, data) => {
            acc[data.unidadeId] = {
                fechamentos: data.fechamentos,
                despesas: data.despesas
            };
            return acc;
        }, {});

        // 5. Calcular os KPIs (v2.0)
        const dashboardData = calcularDashboardLucro(unidades, dadosPorUnidade);

        // 6. Retornar os dados processados
        res.status(200).json({
            message: "Dados do dashboard de lucratividade processados com sucesso",
            data: dashboardData
        });

    } catch (error) {
        console.error('Erro no endpoint /dashboard-data (v2.0):', error.message);
        res.status(500).json({ error: 'Erro interno ao processar dados do dashboard' });
    }
});

module.exports = router;