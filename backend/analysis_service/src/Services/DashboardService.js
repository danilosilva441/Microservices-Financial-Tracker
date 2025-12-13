// src/Services/DashboardService.js
const { getAuthToken } = require('../Config/auth');
const billingIntegration = require('./BillingIntegrationService');
const dashboardCalculator = require('./DashboardCalculator');

class DashboardService {

    async getDashboardData() {
        // 1. Obter Token de Sistema
        const systemToken = await getAuthToken();
        if (!systemToken) {
            throw new Error('Falha ao autenticar AnalysisService no AuthService');
        }

        // 2. Buscar Unidades
        const unidades = await billingIntegration.fetchUnidades(systemToken);
        if (!unidades || unidades.length === 0) {
            return dashboardCalculator.getEmptyDashboardData();
        }

        // 3. Buscar Dados em Paralelo
        const dataPromises = unidades.map(async (unidade) => {
            const [fechamentos, despesas] = await Promise.all([
                billingIntegration.fetchFechamentos(systemToken, unidade.id),
                billingIntegration.fetchDespesas(systemToken, unidade.id)
            ]);
            return {
                unidadeId: unidade.id,
                fechamentos,
                despesas
            };
        });

        const allDataResults = await Promise.all(dataPromises);

        // 4. Mapear Resultados
        const dadosPorUnidade = allDataResults.reduce((acc, data) => {
            acc[data.unidadeId] = {
                fechamentos: data.fechamentos,
                despesas: data.despesas
            };
            return acc;
        }, {});

        // 5. Calcular e Retornar
        return dashboardCalculator.calcularDashboardLucro(unidades, dadosPorUnidade);
    }
}

module.exports = new DashboardService();