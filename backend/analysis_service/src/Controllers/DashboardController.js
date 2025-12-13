// src/Controllers/DashboardController.js
const dashboardService = require('../Services/DashboardService');

class DashboardController {

    async getDashboardData(req, res) {
        try {
            console.log('AnalysisService: Rota GET /dashboard-data (v2.0) atingida');
            
            const data = await dashboardService.getDashboardData();
            
            return res.status(200).json({
                message: "Dados do dashboard de lucratividade processados com sucesso",
                data: data
            });

        } catch (error) {
            console.error('Erro no DashboardController:', error.message);
            return res.status(500).json({ error: 'Erro interno ao processar dados do dashboard' });
        }
    }
}

module.exports = new DashboardController();