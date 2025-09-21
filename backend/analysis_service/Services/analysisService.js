const axios = require('axios');
const BILLING_SERVICE_URL = 'http://billing_service:8080/api';

function calcularProjecao(metaMensal, faturamentos) {
    if (!faturamentos || faturamentos.length === 0) {
        return { valorAcumuladoAtual: 0, projecaoFinal: 0, percentualProjetado: 0 };
    }
    const hoje = new Date();
    const diaAtualDoMes = hoje.getDate();
    const totalDeDiasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();
    const valorAcumuladoAtual = faturamentos.reduce((total, fat) => total + fat.valor, 0);
    const mediaDiaria = valorAcumuladoAtual / diaAtualDoMes;
    const projecaoFinal = mediaDiaria * totalDeDiasNoMes;
    const percentualProjetado = (projecaoFinal / metaMensal) * 100;
    return { valorAcumuladoAtual, projecaoFinal, percentualProjetado };
}

async function rodarAnaliseAutomatica() {
    console.log('Iniciando análise automática diária...');
    try {
        const response = await axios.get(`${BILLING_SERVICE_URL}/operacoes`);
        const operacoes = response.data.$values;
        if (!operacoes || operacoes.length === 0) {
            console.log('Nenhuma operação encontrada para análise.');
            return;
        }
        for (const operacao of operacoes) {
            const { projecaoFinal } = calcularProjecao(operacao.metaMensal, operacao.faturamentos.$values);
            await axios.patch(`${BILLING_SERVICE_URL}/operacoes/${operacao.id}/projecao`, projecaoFinal, {
                headers: { 'Content-Type': 'application/json' }
            });
            console.log(`Projeção de ${projecaoFinal.toFixed(2)} salva para a operação: ${operacao.descricao}`);
        }
        console.log('Análise automática concluída.');
    } catch (error) {
        console.error('Erro durante a análise automática:', error.message);
    }
}

module.exports = { calcularProjecao, rodarAnaliseAutomatica };