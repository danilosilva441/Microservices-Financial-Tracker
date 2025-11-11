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

/**
 * Calcula os dados de Lucratividade (v2.0)
 */
function calcularDashboardLucro(unidades, dadosPorUnidade) {
    if (!unidades || unidades.length === 0) {
        return getEmptyDashboardData();
    }

    let receitaTotal = 0;
    let despesaTotal = 0;
    let metaTotal = 0;

    const desempenhoUnidades = unidades.map(unidade => {
        const dados = dadosPorUnidade[unidade.id];
        if (!dados) return null; // Unidade sem dados

        // 1. Calcula a Receita (Apenas de fechamentos APROVADOS)
        const receitaUnidade = dados.fechamentos
            .filter(f => f.status === "Aprovado")
            .reduce((total, f) => total + f.valorTotalParciais, 0); // Usamos o total calculado pelo C#

        // 2. Calcula a Despesa
        const despesaUnidade = dados.despesas
            .reduce((total, d) => total + d.amount, 0);

        // 3. Calcula o Lucro
        const lucroUnidade = receitaUnidade - despesaUnidade;

        // Adiciona aos totais
        receitaTotal += receitaUnidade;
        despesaTotal += despesaUnidade;
        metaTotal += unidade.metaMensal;

        return {
            id: unidade.id,
            nome: unidade.nome,
            metaMensal: unidade.metaMensal,
            receita: receitaUnidade,
            despesa: despesaUnidade,
            lucro: lucroUnidade,
            percentualMeta: unidade.metaMensal > 0 ? (receitaUnidade / unidade.metaMensal) * 100 : 0
        };
    }).filter(u => u != null); // Remove unidades nulas

    const lucroTotal = receitaTotal - despesaTotal;
    const percentualMetaTotal = metaTotal > 0 ? (receitaTotal / metaTotal) * 100 : 0;

    // Gráficos v2.0
    const barChartData = {
        labels: desempenhoUnidades.map(u => u.nome),
        datasets: [
            {
                label: 'Receita (Aprovada)',
                backgroundColor: '#34d399', // Verde
                data: desempenhoUnidades.map(u => u.receita)
            },
            {
                label: 'Despesa',
                backgroundColor: '#f87171', // Vermelho
                data: desempenhoUnidades.map(u => u.despesa)
            },
            {
                label: 'Lucro',
                backgroundColor: '#38bdf8', // Azul
                data: desempenhoUnidades.map(u => u.lucro)
            }
        ]
    };

    const pieChartData = {
        labels: desempenhoUnidades.map(u => u.nome),
        datasets: [{
            backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa'],
            data: desempenhoUnidades.map(u => u.lucro > 0 ? u.lucro : 0) // Mostra apenas lucro positivo
        }]
    };

    return {
        kpis: {
            receitaTotal,
            despesaTotal,
            lucroTotal,
            metaTotal,
            percentualMetaTotal
        },
        desempenho: {
            todas: desempenhoUnidades.sort((a, b) => b.lucro - a.lucro)
        },
        graficos: {
            barChartData,
            pieChartData
        }
    };
}

function getEmptyDashboardData() {
    return {
        kpis: {
            receitaTotal: 0,
            despesaTotal: 0,
            lucroTotal: 0,
            metaTotal: 0,
            percentualMetaTotal: 0
        },
        desempenho: { todas: [] },
        graficos: {
            barChartData: { labels: [], datasets: [] },
            pieChartData: { labels: [], datasets: [] }
        }
    };
}

module.exports = {
    fetchUnidades,
    fetchFechamentos,
    fetchDespesas,
    calcularDashboardLucro,
    getEmptyDashboardData
    // As funções v1.0 (fetchOperacoes, updateProjecao) foram removidas
};