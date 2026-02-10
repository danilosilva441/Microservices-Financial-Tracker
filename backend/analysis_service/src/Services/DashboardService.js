// src/Services/DashboardService.js
const BillingIntegration = require('./BillingIntegrationService');
const DashboardCalculator = require('./DashboardCalcular');


class DashboardService {
    
    constructor() {
        this.cache = new Map();
        // Tempo de vida do cache: 5 minutos (300000 ms)
        this.CACHE_TTL = 5 * 60 * 1000; 
    }

    async getDashboardData(forceRefresh = false) {
        console.log('DashboardService: Iniciando busca de dados...');

        try {
            const cacheKey = 'dashboard_data';
            const now = Date.now();
            
            // Lógica de Cache
            if (!forceRefresh && this.cache.has(cacheKey)) {
                const cached = this.cache.get(cacheKey);
                if (now - cached.timestamp < this.CACHE_TTL) {
                    console.log('DashboardService: Retornando dados do cache (Hit)');
                    return cached.data;
                }
            }

            // 1. Buscar Unidades (O Service cuida do Token internamente agora)
            const unidades = await this._fetchUnidadesSafe();
            
            if (!unidades || unidades.length === 0) {
                console.warn('DashboardService: Nenhuma unidade encontrada.');
                return dashboardCalculator.getEmptyDashboardData(); // Certifique-se que esta função existe no Calculator
            }

            console.log(`DashboardService: Encontradas ${unidades.length} unidades. Buscando detalhes...`);
            
            // 2. Buscar dados em paralelo
            const dadosPorUnidade = await this._fetchUnidadesDataParallel(unidades);
            
            // 3. Calcular dashboard
            console.log('DashboardService: Dados coletados. Calculando totais...');
            
            // Verifica se o calculator existe antes de chamar
            if (!dashboardCalculator || !dashboardCalculator.calcularDashboardLucro) {
                throw new Error('DashboardCalculator não está configurado corretamente.');
            }

            const result = dashboardCalculator.calcularDashboardLucro(unidades, dadosPorUnidade);
            
            // Atualizar cache
            this.cache.set(cacheKey, {
                data: result,
                timestamp: now
            });
            
            return result;
            
        } catch (error) {
            console.error('DashboardService: Erro crítico ao processar dashboard:', error);
            throw error; // Repassa o erro para o Controller tratar
        }
    }

    async _fetchUnidadesSafe() {
        try {
            // Chama sem passar token, pois o BillingIntegrationService já gerencia isso
            const unidades = await billingIntegration.fetchUnidades();
            
            if (!unidades || !Array.isArray(unidades)) {
                return [];
            }
            
            return unidades.filter(u => u && u.id);
            
        } catch (error) {
            console.error('DashboardService: Erro ao buscar unidades:', error.message);
            return [];
        }
    }

    async _fetchUnidadesDataParallel(unidades, maxConcurrent = 5) {
        const dadosPorUnidade = {};
        
        // Processar em chunks para não sobrecarregar
        for (let i = 0; i < unidades.length; i += maxConcurrent) {
            const batch = unidades.slice(i, i + maxConcurrent);
            
            const batchPromises = batch.map(async (unidade) => {
                try {
                    // Chama sem token
                    const [fechamentos, despesas] = await Promise.all([
                        billingIntegration.fetchFechamentos(unidade.id).catch(e => {
                            console.warn(`Erro ao buscar fechamentos da unidade ${unidade.id}:`, e.message);
                            return [];
                        }),
                        billingIntegration.fetchDespesas(unidade.id).catch(e => {
                            console.warn(`Erro ao buscar despesas da unidade ${unidade.id}:`, e.message);
                            return [];
                        })
                    ]);
                    
                    return {
                        unidadeId: unidade.id,
                        fechamentos: fechamentos || [],
                        despesas: despesas || []
                    };
                    
                } catch (error) {
                    console.error(`Erro fatal processando unidade ${unidade.id}`, error);
                    return { unidadeId: unidade.id, fechamentos: [], despesas: [] };
                }
            });
            
            const batchResults = await Promise.all(batchPromises);
            
            batchResults.forEach(data => {
                dadosPorUnidade[data.unidadeId] = {
                    fechamentos: data.fechamentos,
                    despesas: data.despesas
                };
            });
        }
        
        return dadosPorUnidade;
    }
    
    clearCache() {
        this.cache.clear();
        console.log('DashboardService: Cache limpo manualmente.');
    }
}

module.exports = new DashboardService();