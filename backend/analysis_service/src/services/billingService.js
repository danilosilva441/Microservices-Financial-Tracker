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

// --- NOVAS FUNÇÕES DE CÁLCULO ---
function calcularDashboardData(operacoes) {
  if (!operacoes || operacoes.length === 0) {
    return getEmptyDashboardData();
  }

  const diaAtual = new Date().getDate();
  const totalDiasMes = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate();
  
  // Cálculos principais
  const faturamentoTotal = operacoes.reduce((total, op) => total + (op.projecaoFaturamento || 0), 0);
  const metaTotal = operacoes.reduce((total, op) => total + op.metaMensal, 0);
  const percentualMeta = metaTotal > 0 ? (faturamentoTotal / metaTotal) * 100 : 0;
  const mediaDiariaAtual = diaAtual > 0 ? faturamentoTotal / diaAtual : 0;
  const projecaoFinalMes = mediaDiariaAtual * totalDiasMes;
  const percentualProjetado = metaTotal > 0 ? (projecaoFinalMes / metaTotal) * 100 : 0;
  
  let vaiBaterMeta = 'baixa';
  if (percentualProjetado >= 95) vaiBaterMeta = 'alta';
  else if (percentualProjetado >= 70) vaiBaterMeta = 'media';

  const saldoRestante = metaTotal - faturamentoTotal;
  const diasRestantes = totalDiasMes - diaAtual;

  // Cálculos por operação
  const desempenhoOperacoes = operacoes.map(op => ({
    ...op,
    percentualAtingido: op.metaMensal > 0 ? ((op.projecaoFaturamento || 0) / op.metaMensal) * 100 : 0,
    diferenca: (op.projecaoFaturamento || 0) - op.metaMensal,
    mediaDiaria: diaAtual > 0 ? (op.projecaoFaturamento || 0) / diaAtual : 0,
    projecaoFinal: diaAtual > 0 ? ((op.projecaoFaturamento || 0) / diaAtual) * totalDiasMes : 0,
    percentualProjetado: op.metaMensal > 0 ? (((op.projecaoFaturamento || 0) / diaAtual) * totalDiasMes / op.metaMensal) * 100 : 0
  })).sort((a, b) => b.percentualAtingido - a.percentualAtingido);

  const topOperacoes = desempenhoOperacoes.slice(0, 3);
  const bottomOperacoes = [...desempenhoOperacoes].reverse().slice(0, 3);

  // Dados para gráficos
  const graficos = {
    barChartData: {
      labels: operacoes.map(op => op.nome),
      datasets: [
        {
          label: 'Meta Mensal',
          backgroundColor: '#a7f3d0',
          borderColor: '#059669',
          borderWidth: 1,
          data: operacoes.map(op => op.metaMensal)
        },
        {
          label: 'Faturamento Realizado',
          backgroundColor: '#38bdf8',
          borderColor: '#0284c7',
          borderWidth: 1,
          data: operacoes.map(op => op.projecaoFaturamento || 0)
        }
      ]
    },
    pieChartData: {
      labels: operacoes.map(op => op.nome),
      datasets: [
        {
          backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa', '#f472b6', '#60a5fa', '#34d399', '#f59e0b', '#ef4444'],
          data: operacoes.map(op => op.projecaoFaturamento || 0)
        }
      ]
    },
    projecaoChartData: {
      labels: ['Realizado até Hoje', 'Projeção do Mês'],
      datasets: [
        {
          label: 'Valor',
          backgroundColor: ['#38bdf8', vaiBaterMeta === 'alta' ? '#10b981' : vaiBaterMeta === 'media' ? '#f59e0b' : '#ef4444'],
          borderColor: ['#0284c7', vaiBaterMeta === 'alta' ? '#059669' : vaiBaterMeta === 'media' ? '#d97706' : '#dc2626'],
          borderWidth: 1,
          data: [faturamentoTotal, projecaoFinalMes]
        },
        {
          label: 'Meta',
          type: 'line',
          borderColor: '#6b7280',
          borderWidth: 2,
          borderDash: [5, 5],
          fill: false,
          data: [metaTotal, metaTotal],
          pointRadius: 0
        }
      ]
    }
  };

  return {
    kpis: {
      faturamentoTotal,
      metaTotal,
      percentualMeta,
      diaAtual,
      totalDiasMes,
      mediaDiariaAtual,
      projecaoFinalMes,
      percentualProjetado,
      vaiBaterMeta,
      saldoRestante,
      diasRestantes
    },
    desempenho: {
      todas: desempenhoOperacoes,
      top: topOperacoes,
      bottom: bottomOperacoes
    },
    graficos
  };
}

function getEmptyDashboardData() {
  const diaAtual = new Date().getDate();
  const totalDiasMes = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate();
  
  return {
    kpis: {
      faturamentoTotal: 0,
      metaTotal: 0,
      percentualMeta: 0,
      diaAtual,
      totalDiasMes,
      mediaDiariaAtual: 0,
      projecaoFinalMes: 0,
      percentualProjetado: 0,
      vaiBaterMeta: 'baixa',
      saldoRestante: 0,
      diasRestantes: totalDiasMes - diaAtual
    },
    desempenho: {
      todas: [],
      top: [],
      bottom: []
    },
    graficos: {
      barChartData: { labels: [], datasets: [] },
      pieChartData: { labels: [], datasets: [] },
      projecaoChartData: { labels: [], datasets: [] }
    }
  };
}

module.exports = {
  fetchOperacoes,
  updateProjecao,
  calcularDashboardData
};