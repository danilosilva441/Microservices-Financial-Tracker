// src/Controllers/DashboardController.js
const dashboardService = require('../Services/DashboardService');

class DashboardController {
    
    // Construtor não é estritamente necessário se usar arrow functions ou bind manual,
    // mas vamos manter simples.
    
    async getDashboardData(req, res) {
        const startTime = Date.now();
        // ID único para rastrear logs desta requisição específica
        const requestId = `req_${Date.now()}_${Math.random().toString(36).substr(2, 5)}`;
        
        console.log(`[${requestId}] 📥 DashboardController: GET /dashboard-data recebido`);
        
        try {
            // Converte string 'true' para booleano
            const forceRefresh = req.query.refresh === 'true';
            
            if (forceRefresh) {
                console.log(`[${requestId}] 🔄 Solicitado refresh forçado do cache.`);
            }
            
            const data = await dashboardService.getDashboardData(forceRefresh);
            
            const responseTime = Date.now() - startTime;
            console.log(`[${requestId}] ✅ DashboardController: Sucesso em ${responseTime}ms`);
            
            return res.status(200).json({
                success: true,
                message: "Dados do dashboard obtidos com sucesso",
                data: data,
                metadata: {
                    requestId,
                    responseTime: `${responseTime}ms`,
                    timestamp: new Date().toISOString()
                }
            });

        } catch (error) {
            const errorTime = Date.now() - startTime;
            console.error(`[${requestId}] ❌ Erro no DashboardController:`, error);
            
            let statusCode = 500;
            let errorMessage = 'Erro interno no servidor';

            // Tratamento de erros conhecidos
            if (error.response) {
                // Erro vindo de chamadas axios (ex: 401 do Billing)
                statusCode = error.response.status;
                errorMessage = `Erro no serviço externo: ${error.response.statusText}`;
            } else if (error.code === 'ECONNREFUSED') {
                statusCode = 503;
                errorMessage = 'Serviço de Billing indisponível';
            }

            return res.status(statusCode).json({ 
                success: false,
                error: errorMessage,
                requestId,
                // Em produção, evite enviar error.stack
                details: process.env.NODE_ENV === 'development' ? error.message : undefined 
            });
        }
    }
    
    async clearCache(req, res) {
        try {
            dashboardService.clearCache();
            return res.status(200).json({
                success: true,
                message: "Cache limpo com sucesso"
            });
        } catch (error) {
            return res.status(500).json({
                success: false,
                error: "Erro ao limpar cache"
            });
        }
    }
}

module.exports = new DashboardController();