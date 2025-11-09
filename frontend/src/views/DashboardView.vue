<script setup>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
import { onMounted, computed, ref } from 'vue';
<<<<<<< Updated upstream
// 1. Importa a NOVA dashboardStore
import { useDashboardStore } from '@/stores/dashboardStore'; 
=======
import { useOperacoesStore } from '@/stores/operacoes';
=======
import { onMounted, computed, ref, watch } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore'; 
>>>>>>> Stashed changes
>>>>>>> Stashed changes
import { formatCurrency } from '@/utils/formatters';

// Importações do Chart.js
import { Bar, Pie, Line } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ArcElement,
  LineElement,
  PointElement
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement, LineElement, PointElement);

<<<<<<< Updated upstream
// 2. Usa a dashboardStore
const dashboardStore = useDashboardStore();
const timeframe = ref('month');

// 3. Busca os dados ao montar
=======
<<<<<<< Updated upstream
const operacoesStore = useOperacoesStore();
const timeframe = ref('month'); // month, quarter, year

// Busca os dados quando o componente é montado
=======
const dashboardStore = useDashboardStore();
const timeframe = ref('month');
const showAdvancedMetrics = ref(false);
const exportLoading = ref(false);

// Busca os dados ao montar
>>>>>>> Stashed changes
>>>>>>> Stashed changes
onMounted(() => {
  dashboardStore.fetchDashboardData();
});

<<<<<<< Updated upstream
// 4. Acede aos dados PRONTOS vindos da store
=======
<<<<<<< Updated upstream
// --- CÁLCULOS PARA OS KPIs E GRÁFICOS ---
=======
// Watch para timeframe
watch(timeframe, (newTimeframe) => {
  dashboardStore.fetchDashboardData(newTimeframe);
});

// Acede aos dados PRONTOS vindos da store
>>>>>>> Stashed changes
const isLoading = computed(() => dashboardStore.isLoading);
const error = computed(() => dashboardStore.error);
const kpis = computed(() => dashboardStore.kpis || {});
const desempenho = computed(() => dashboardStore.desempenho || { top: [], bottom: [] });
const graficos = computed(() => dashboardStore.graficos || {});
const operacoes = computed(() => dashboardStore.operacoes || []);
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes

// Helper para verificar se há dados
const hasData = computed(() => {
  return operacoes.value.length > 0 || 
         Object.keys(kpis.value).length > 0 ||
         Object.keys(graficos.value).length > 0;
});

<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
// Calcula o desempenho por operação
const desempenhoOperacoes = computed(() =>
  operacoes.value.map(op => ({
    ...op,
    percentualAtingido: op.metaMensal > 0 ? ((op.projecaoFaturamento || 0) / op.metaMensal) * 100 : 0,
    diferenca: (op.projecaoFaturamento || 0) - op.metaMensal,
    // Projeção individual
    mediaDiaria: diaAtual.value > 0 ? (op.projecaoFaturamento || 0) / diaAtual.value : 0,
    projecaoFinal: diaAtual.value > 0 ? ((op.projecaoFaturamento || 0) / diaAtual.value) * totalDiasMes.value : 0,
    percentualProjetado: op.metaMensal > 0 ? (((op.projecaoFaturamento || 0) / diaAtual.value) * totalDiasMes.value / op.metaMensal) * 100 : 0
  })).sort((a, b) => b.percentualAtingido - a.percentualAtingido)
);
=======
// Métricas Avançadas de BI
const advancedMetrics = computed(() => {
  const baseKpis = kpis.value;
  
  // Cálculo de tendência
  const tendencia = baseKpis.mediaDiariaAtual > 0 ? 
    (baseKpis.mediaDiariaNecessaria / baseKpis.mediaDiariaAtual) * 100 : 0;
  
  // Eficiência operacional (quanto cada operação contribui para a meta)
  const eficienciaOperacional = operacoes.value.map(op => ({
    nome: op.nome,
    contribuicao: op.metaMensal > 0 ? (op.projecaoFaturamento / op.metaMensal) * 100 : 0,
    peso: op.metaMensal / (baseKpis.metaTotal || 1) * 100
  })).sort((a, b) => b.contribuicao - a.contribuicao);
  
  // Análise de sazonalidade semanal
  const diasDaSemana = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
  const faturamentoSemanal = diasDaSemana.map((dia, index) => ({
    dia,
    valor: Math.random() * 50000 + 10000, // Simulado - substituir por dados reais
    percentual: Math.random() * 20 + 10
  }));
  
  // Previsão para próximos meses
  const previsaoMensal = [
    { mes: 'Jan', previsao: baseKpis.projecaoFinalMes || 0, meta: baseKpis.metaTotal || 0 },
    { mes: 'Fev', previsao: (baseKpis.projecaoFinalMes || 0) * 1.1, meta: (baseKpis.metaTotal || 0) * 1.05 },
    { mes: 'Mar', previsao: (baseKpis.projecaoFinalMes || 0) * 1.15, meta: (baseKpis.metaTotal || 0) * 1.1 }
  ];
  
  return {
    tendencia,
    eficienciaOperacional,
    faturamentoSemanal,
    previsaoMensal,
    // KPIs avançados
    roi: baseKpis.faturamentoTotal > 0 ? (baseKpis.faturamentoTotal / (baseKpis.metaTotal || 1)) * 100 : 0,
    crescimentoMensal: 15.2, // Simulado - baseado em dados históricos
    variacaoSemanal: 8.7, // Simulado
    pontosCriticos: eficienciaOperacional.filter(op => op.contribuicao < 70).length
  };
});

// Exportar dados
const exportarDadosBI = async () => {
  exportLoading.value = true;
  try {
    const dados = {
      kpis: kpis.value,
      advancedMetrics: advancedMetrics.value,
      operacoes: operacoes.value,
      timestamp: new Date().toISOString()
    };
    
    const blob = new Blob([JSON.stringify(dados, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `bi-dashboard-${new Date().toISOString().split('T')[0]}.json`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Erro ao exportar dados:', error);
    alert('Erro ao exportar dados');
  } finally {
    exportLoading.value = false;
  }
};

>>>>>>> Stashed changes
// --- OPÇÕES DOS GRÁFICOS ---
const getResponsiveFontSize = () => {
  const width = typeof window !== 'undefined' ? window.innerWidth : 1024;
  if (width < 640) return 9;
  if (width < 1024) return 10;
  return 12;
};
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes

const getResponsivePadding = () => {
  const width = typeof window !== 'undefined' ? window.innerWidth : 1024;
  return width < 640 ? 8 : 12;
};

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: { duration: 800, easing: 'easeInOutQuart' },
  plugins: {
    legend: {
      position: 'bottom',
      labels: { 
        boxWidth: 10, 
        font: { size: getResponsiveFontSize() }, 
        padding: getResponsivePadding() 
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: { size: getResponsiveFontSize() },
        maxRotation: (typeof window !== 'undefined' && window.innerWidth < 640) ? 45 : 
                    (typeof window !== 'undefined' && window.innerWidth < 1024) ? 30 : 0
      }
    },
    y: {
      ticks: {
        font: { size: getResponsiveFontSize() },
        callback: function(value) {
          if (value >= 1000000) return 'R$ ' + (value / 1000000).toFixed(1) + 'M';
          if (value >= 1000) return 'R$ ' + (value / 1000).toFixed(0) + 'K';
          return 'R$ ' + value;
        }
      }
    }
  }
}));

const projecaoChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: { duration: 800, easing: 'easeInOutQuart' },
  plugins: {
    legend: {
      position: 'bottom',
      labels: { 
        boxWidth: 10, 
        font: { size: getResponsiveFontSize() }, 
        padding: getResponsivePadding() 
      }
    }
  },
  scales: {
    x: { 
      ticks: { 
        font: { size: getResponsiveFontSize() } 
      } 
    },
    y: {
      ticks: {
        font: { size: getResponsiveFontSize() },
        callback: function(value) {
          if (value >= 1000000) return 'R$ ' + (value / 1000000).toFixed(1) + 'M';
          if (value >= 1000) return 'R$ ' + (value / 1000).toFixed(0) + 'K';
          return 'R$ ' + value;
        }
      }
    }
  }
}));

// --- DADOS PARA OS GRÁFICOS ---
const barChartData = computed(() => {
  const data = graficos.value.barChartData; 
  if (!data || !data.labels || !data.datasets) {
    return { 
      labels: [], 
      datasets: [
        {
          label: 'Meta Mensal',
          backgroundColor: '#a7f3d0',
          borderColor: '#059669',
          borderWidth: 1,
          data: []
        },
        {
          label: 'Faturamento Realizado',
          backgroundColor: '#38bdf8',
          borderColor: '#0284c7',
          borderWidth: 1,
          data: []
        }
      ]
    };
  }
  
  return {
    labels: data.labels,
    datasets: [
      {
        label: 'Meta Mensal',
        backgroundColor: '#a7f3d0',
        borderColor: '#059669',
        borderWidth: 1,
        data: data.datasets[0]?.data || []
      },
      {
        label: 'Faturamento Realizado',
        backgroundColor: '#38bdf8',
        borderColor: '#0284c7',
        borderWidth: 1,
        data: data.datasets[1]?.data || []
      }
    ]
  };
});

const projecaoChartData = computed(() => {
  const data = graficos.value.projecaoChartData; 
  const metaStatus = kpis.value.vaiBaterMeta || 'media';
  
  if (!data || !data.labels || !data.datasets) {
    return { 
      labels: [], 
      datasets: [
        {
          label: 'Valor',
          backgroundColor: ['#38bdf8', getStatusColor(metaStatus)],
          borderColor: ['#0284c7', getStatusBorderColor(metaStatus)],
          borderWidth: 1,
          data: []
        },
        {
          label: 'Meta',
          type: 'line',
          borderColor: '#6b7280',
          borderWidth: 2,
          borderDash: [5, 5],
          fill: false,
          data: [],
          pointRadius: 0
        }
      ]
    };
  }
  
  return {
    labels: data.labels,
    datasets: [
      {
        label: 'Valor',
        backgroundColor: ['#38bdf8', getStatusColor(metaStatus)],
        borderColor: ['#0284c7', getStatusBorderColor(metaStatus)],
        borderWidth: 1,
        data: data.datasets[0]?.data || []
      },
      {
        label: 'Meta',
        type: 'line',
        borderColor: '#6b7280',
        borderWidth: 2,
        borderDash: [5, 5],
        fill: false,
        data: data.datasets[1]?.data || [],
        pointRadius: 0
      }
    ]
  };
});

// Funções auxiliares para cores baseadas no status
const getStatusColor = (status) => {
  switch (status) {
    case 'alta': return '#10b981';
    case 'media': return '#f59e0b';
    case 'baixa': return '#ef4444';
    default: return '#f59e0b';
  }
};

const getStatusBorderColor = (status) => {
  switch (status) {
    case 'alta': return '#059669';
    case 'media': return '#d97706';
    case 'baixa': return '#dc2626';
    default: return '#d97706';
  }
};
<<<<<<< Updated upstream
</script>

<template>
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8 modern-dashboard">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
=======

// Gráfico de projeção futura
const projecaoChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 10,
        font: {
          size: window.innerWidth < 640 ? 9 : window.innerWidth < 1024 ? 10 : 12
        },
        padding: window.innerWidth < 640 ? 8 : 12
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: {
          size: window.innerWidth < 640 ? 9 : window.innerWidth < 1024 ? 10 : 12
        }
      }
    },
    y: {
      ticks: {
        font: {
          size: window.innerWidth < 640 ? 9 : window.innerWidth < 1024 ? 10 : 12
        },
        callback: function(value) {
          if (value >= 1000000) {
            return 'R$ ' + (value / 1000000).toFixed(1) + 'M';
          }
          if (value >= 1000) {
            return 'R$ ' + (value / 1000).toFixed(0) + 'K';
          }
          return 'R$ ' + value;
        }
      }
    }
  }
};

// Dados para o Gráfico de Barras (Meta vs. Realizado)
const barChartData = computed(() => ({
  labels: operacoes.value.map(op => op.nome),
  datasets: [
    {
      label: 'Meta Mensal',
      backgroundColor: '#a7f3d0',
      borderColor: '#059669',
      borderWidth: 1,
      data: operacoes.value.map(op => op.metaMensal)
    },
    {
      label: 'Faturamento Realizado',
      backgroundColor: '#38bdf8',
      borderColor: '#0284c7',
      borderWidth: 1,
      data: operacoes.value.map(op => op.projecaoFaturamento || 0)
    }
  ]
}));

<<<<<<< Updated upstream
<<<<<<< Updated upstream
// Dados para o Gráfico de Pizza (Distribuição do Faturamento)
const pieChartData = computed(() => ({
  labels: operacoes.value.map(op => op.nome),
  datasets: [
    {
      backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa', '#f472b6', '#60a5fa', '#34d399', '#f59e0b', '#ef4444'],
      data: operacoes.value.map(op => op.projecaoFaturamento || 0)
    }
  ]
}));
=======
// Dados para gráficos de BI
const tendenciaChartData = computed(() => ({
  labels: ['Realizado', 'Necessário'],
  datasets: [
    {
      label: 'Média Diária',
      data: [kpis.value.mediaDiariaAtual || 0, kpis.value.mediaDiariaNecessaria || 0],
      backgroundColor: ['#10b981', kpis.value.mediaDiariaAtual >= kpis.value.mediaDiariaNecessaria ? '#10b981' : '#ef4444'],
      borderColor: ['#059669', kpis.value.mediaDiariaAtual >= kpis.value.mediaDiariaNecessaria ? '#059669' : '#dc2626'],
      borderWidth: 1
    }
  ]
}));

const eficienciaChartData = computed(() => ({
  labels: advancedMetrics.value.eficienciaOperacional.map(op => op.nome),
  datasets: [
    {
      label: 'Eficiência (%)',
      data: advancedMetrics.value.eficienciaOperacional.map(op => op.contribuicao),
      backgroundColor: advancedMetrics.value.eficienciaOperacional.map(op => 
        op.contribuicao >= 100 ? '#10b981' : 
        op.contribuicao >= 70 ? '#f59e0b' : '#ef4444'
      ),
      borderColor: advancedMetrics.value.eficienciaOperacional.map(op => 
        op.contribuicao >= 100 ? '#059669' : 
        op.contribuicao >= 70 ? '#d97706' : '#dc2626'
      ),
      borderWidth: 1
    }
  ]
}));

// --- DADOS PARA OS GRÁFICOS ---
const barChartData = computed(() => {
  const data = graficos.value.barChartData; 
  if (!data || !data.labels || !data.datasets) {
    return { 
      labels: [], 
      datasets: [
        {
          label: 'Meta Mensal',
          backgroundColor: '#a7f3d0',
          borderColor: '#059669',
          borderWidth: 1,
          data: []
        },
        {
          label: 'Faturamento Realizado',
          backgroundColor: '#38bdf8',
          borderColor: '#0284c7',
          borderWidth: 1,
          data: []
        }
      ]
    };
  }
  
  return {
    labels: data.labels,
    datasets: [
      {
        label: 'Meta Mensal',
        backgroundColor: '#a7f3d0',
        borderColor: '#059669',
        borderWidth: 1,
        data: data.datasets[0]?.data || []
      },
      {
        label: 'Faturamento Realizado',
        backgroundColor: '#38bdf8',
        borderColor: '#0284c7',
        borderWidth: 1,
        data: data.datasets[1]?.data || []
      }
    ]
  };
});
>>>>>>> Stashed changes
=======
// Dados para gráficos de BI
const tendenciaChartData = computed(() => ({
  labels: ['Realizado', 'Necessário'],
  datasets: [
    {
      label: 'Média Diária',
      data: [kpis.value.mediaDiariaAtual || 0, kpis.value.mediaDiariaNecessaria || 0],
      backgroundColor: ['#10b981', kpis.value.mediaDiariaAtual >= kpis.value.mediaDiariaNecessaria ? '#10b981' : '#ef4444'],
      borderColor: ['#059669', kpis.value.mediaDiariaAtual >= kpis.value.mediaDiariaNecessaria ? '#059669' : '#dc2626'],
      borderWidth: 1
    }
  ]
}));

const eficienciaChartData = computed(() => ({
  labels: advancedMetrics.value.eficienciaOperacional.map(op => op.nome),
  datasets: [
    {
      label: 'Eficiência (%)',
      data: advancedMetrics.value.eficienciaOperacional.map(op => op.contribuicao),
      backgroundColor: advancedMetrics.value.eficienciaOperacional.map(op => 
        op.contribuicao >= 100 ? '#10b981' : 
        op.contribuicao >= 70 ? '#f59e0b' : '#ef4444'
      ),
      borderColor: advancedMetrics.value.eficienciaOperacional.map(op => 
        op.contribuicao >= 100 ? '#059669' : 
        op.contribuicao >= 70 ? '#d97706' : '#dc2626'
      ),
      borderWidth: 1
    }
  ]
}));

// --- DADOS PARA OS GRÁFICOS ---
const barChartData = computed(() => {
  const data = graficos.value.barChartData; 
  if (!data || !data.labels || !data.datasets) {
    return { 
      labels: [], 
      datasets: [
        {
          label: 'Meta Mensal',
          backgroundColor: '#a7f3d0',
          borderColor: '#059669',
          borderWidth: 1,
          data: []
        },
        {
          label: 'Faturamento Realizado',
          backgroundColor: '#38bdf8',
          borderColor: '#0284c7',
          borderWidth: 1,
          data: []
        }
      ]
    };
  }
  
  return {
    labels: data.labels,
    datasets: [
      {
        label: 'Meta Mensal',
        backgroundColor: '#a7f3d0',
        borderColor: '#059669',
        borderWidth: 1,
        data: data.datasets[0]?.data || []
      },
      {
        label: 'Faturamento Realizado',
        backgroundColor: '#38bdf8',
        borderColor: '#0284c7',
        borderWidth: 1,
        data: data.datasets[1]?.data || []
      }
    ]
  };
});
>>>>>>> Stashed changes

// Dados para o Gráfico de Projeção
const projecaoChartData = computed(() => ({
  labels: ['Realizado até Hoje', 'Projeção do Mês'],
  datasets: [
    {
      label: 'Valor',
      backgroundColor: ['#38bdf8', vaiBaterMeta.value === 'alta' ? '#10b981' : vaiBaterMeta.value === 'media' ? '#f59e0b' : '#ef4444'],
      borderColor: ['#0284c7', vaiBaterMeta.value === 'alta' ? '#059669' : vaiBaterMeta.value === 'media' ? '#d97706' : '#dc2626'],
      borderWidth: 1,
      data: [faturamentoTotal.value, projecaoFinalMes.value]
    },
    {
      label: 'Meta',
      type: 'line',
      borderColor: '#6b7280',
      borderWidth: 2,
      borderDash: [5, 5],
      fill: false,
      data: [metaTotal.value, metaTotal.value],
      pointRadius: 0
    }
  ]
}));
</script>

<template>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
      <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0">Dashboard Geral</h1>
      <div class="flex items-center space-x-2">
        <select v-model="timeframe" class="text-xs sm:text-sm border border-gray-300 rounded-lg px-2 sm:px-3 py-1.5 sm:py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 w-full sm:w-auto">
=======
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8 modern-dashboard">
    <!-- Header Modernizado -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
>>>>>>> Stashed changes
      <div class="header-content">
        <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0 modern-title">
          Dashboard Geral
        </h1>
        <p class="text-xs sm:text-sm text-gray-500 mt-1 hidden sm:block">
<<<<<<< Updated upstream
          Visão geral do desempenho e métricas
=======
          Visão geral do desempenho e métricas avançadas de BI
>>>>>>> Stashed changes
        </p>
      </div>
      <div class="controls-container flex items-center space-x-2 mt-2 sm:mt-0">
        <select 
          v-model="timeframe" 
          class="modern-select text-xs sm:text-sm border border-gray-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white shadow-sm hover:shadow-md w-full sm:w-auto"
        >
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes
          <option value="month">Este Mês</option>
          <option value="quarter">Este Trimestre</option>
          <option value="year">Este Ano</option>
        </select>
        <button 
          @click="exportarDadosBI"
          :disabled="exportLoading"
          class="modern-button px-3 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-opacity-50 text-xs sm:text-sm"
        >
          {{ exportLoading ? 'Exportando...' : 'Exportar BI' }}
        </button>
        <button 
          @click="exportarDadosBI"
          :disabled="exportLoading"
          class="modern-button px-3 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-opacity-50 text-xs sm:text-sm"
        >
          {{ exportLoading ? 'Exportando...' : 'Exportar BI' }}
        </button>
      </div>
    </div>

<<<<<<< Updated upstream
    <div v-if="isLoading" class="text-center py-8 sm:py-12 modern-loading">
      <div class="inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
=======
<<<<<<< Updated upstream
    <div v-if="operacoesStore.isLoading" class="text-center py-8 sm:py-12">
      <div class="inline-block animate-spin rounded-full h-6 sm:h-8 w-6 sm:w-8 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
=======
    <!-- Botão para métricas avançadas -->
    <div class="flex justify-center mb-6">
      <button 
        @click="showAdvancedMetrics = !showAdvancedMetrics"
        class="flex items-center px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-all duration-200 text-sm font-medium"
      >
        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
        </svg>
        {{ showAdvancedMetrics ? 'Ocultar Métricas Avançadas' : 'Mostrar Métricas Avançadas' }}
      </button>
    </div>

    <div v-if="isLoading" class="text-center py-8 sm:py-12 modern-loading">
      <div class="inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
>>>>>>> Stashed changes
>>>>>>> Stashed changes
      <p class="text-gray-600 text-sm sm:text-base">Carregando dados...</p>
    </div>

    <div v-else-if="error" class="text-center py-8 sm:py-12 modern-error">
      <div class="w-12 h-12 sm:w-16 sm:h-16 mx-auto text-red-400 mb-3 sm:mb-4">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
        </svg>
      </div>
      <h3 class="text-base sm:text-lg lg:text-xl font-semibold text-gray-900 mb-2">Erro ao carregar dashboard</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto mb-4">
        {{ error }}
      </p>
      <button 
        @click="dashboardStore.fetchDashboardData" 
        class="modern-button px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
      >
        Tentar Novamente
      </button>
    </div>

    <div v-else-if="hasData" class="dashboard-content">
      
      <!-- KPIs Principais -->
      <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Faturamento Total -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-500 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Faturamento Total</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark number-format kpi-value">
                {{ formatCurrency(kpis.faturamentoTotal || 0) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">
                Dia {{ kpis.diaAtual || 0 }}/{{ kpis.totalDiasMes || 30 }}
              </p>
            </div>
          </div>
        </div>

        <!-- Meta Atingida -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-yellow-500 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Meta Atingida</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="{
                   'text-green-500': (kpis.percentualMeta || 0) >= 100,
                   'text-yellow-500': (kpis.percentualMeta || 0) >= 70 && (kpis.percentualMeta || 0) < 100,
                   'text-red-500': (kpis.percentualMeta || 0) < 70
                 }">
                {{ ((kpis.percentualMeta || 0)).toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
                {{ formatCurrency(kpis.faturamentoTotal || 0) }} / {{ formatCurrency(kpis.metaTotal || 0) }}
              </p>
            </div>
          </div>
        </div>

        <!-- Projeção do Mês -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
             :class="{
               'border-green-500': kpis.vaiBaterMeta === 'alta',
               'border-orange-500': kpis.vaiBaterMeta === 'media',
               'border-red-500': kpis.vaiBaterMeta === 'baixa'
             }">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Projeção do Mês</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="{
                   'text-green-500': kpis.vaiBaterMeta === 'alta',
                   'text-orange-500': kpis.vaiBaterMeta === 'media',
                   'text-red-500': kpis.vaiBaterMeta === 'baixa'
                 }">
                {{ ((kpis.percentualProjetado || 0)).toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
                {{ formatCurrency(kpis.projecaoFinalMes || 0) }} projetado
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- KPIs Secundários -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Média Diária -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-400 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Média Diária Atual</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-blue-500 number-format kpi-value">
                {{ formatCurrency(kpis.mediaDiariaAtual || 0) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">Por dia útil</p>
            </div>
          </div>
        </div>

        <!-- Saldo Restante -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
             :class="(kpis.saldoRestante || 0) > 0 ? 'border-red-400' : 'border-green-400'">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Saldo para Meta</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="(kpis.saldoRestante || 0) > 0 ? 'text-red-500' : 'text-green-500'">
                {{ formatCurrency(kpis.saldoRestante || 0) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">Para atingir a meta</p>
            </div>
          </div>
        </div>

        <!-- Probabilidade -->
        <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
             :class="{
               'border-green-400': kpis.vaiBaterMeta === 'alta',
               'border-orange-400': kpis.vaiBaterMeta === 'media',
               'border-red-400': kpis.vaiBaterMeta === 'baixa'
             }">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Probabilidade</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="{
                   'text-green-500': kpis.vaiBaterMeta === 'alta',
                   'text-orange-500': kpis.vaiBaterMeta === 'media',
                   'text-red-500': kpis.vaiBaterMeta === 'baixa'
                 }">
                {{ kpis.vaiBaterMeta === 'alta' ? 'Alta' : kpis.vaiBaterMeta === 'media' ? 'Média' : 'Baixa' }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">
                {{ kpis.diasRestantes || 0 }} dias restantes
              </p>
            </div>
          </div>
        </div>
      </div>

<<<<<<< Updated upstream
      <!-- Gráficos -->
=======
<<<<<<< Updated upstream
      <!-- Gráficos Principais - Com projeção -->
=======
      <!-- Métricas Avançadas de BI -->
      <div v-if="showAdvancedMetrics" class="modern-card bg-white rounded-xl shadow-card p-6 mb-6 lg:mb-8">
        <h3 class="text-lg font-bold text-gray-800 mb-6 border-b pb-3">Métricas Avançadas de Business Intelligence</h3>
        
        <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-4 gap-6 mb-6">
          <!-- ROI Operacional -->
          <div class="text-center p-4 bg-gradient-to-br from-green-50 to-blue-50 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">ROI Operacional</h4>
            <p class="text-2xl font-bold text-green-600">{{ advancedMetrics.roi.toFixed(1) }}%</p>
            <p class="text-xs text-gray-500 mt-1">Retorno sobre investimento</p>
          </div>

          <!-- Crescimento Mensal -->
          <div class="text-center p-4 bg-gradient-to-br from-blue-50 to-purple-50 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">Crescimento</h4>
            <p class="text-2xl font-bold text-blue-600">{{ advancedMetrics.crescimentoMensal.toFixed(1) }}%</p>
            <p class="text-xs text-gray-500 mt-1">Vs mês anterior</p>
          </div>

          <!-- Variação Semanal -->
          <div class="text-center p-4 bg-gradient-to-br from-orange-50 to-red-50 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">Variação Semanal</h4>
            <p class="text-2xl font-bold text-orange-600">{{ advancedMetrics.variacaoSemanal.toFixed(1) }}%</p>
            <p class="text-xs text-gray-500 mt-1">Performance semanal</p>
          </div>

          <!-- Pontos de Atenção -->
          <div class="text-center p-4 bg-gradient-to-br from-red-50 to-pink-50 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">Pontos de Atenção</h4>
            <p class="text-2xl font-bold text-red-600">{{ advancedMetrics.pontosCriticos }}</p>
            <p class="text-xs text-gray-500 mt-1">Operações críticas</p>
          </div>
        </div>

        <!-- Gráficos de BI -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Tendência de Performance -->
          <div class="modern-card bg-gray-50 p-4 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-800 mb-3">Tendência de Performance</h4>
            <div class="h-48">
              <Bar :data="tendenciaChartData" :options="chartOptions" />
            </div>
            <p class="text-xs text-gray-600 mt-2 text-center">
              {{ advancedMetrics.tendencia <= 100 ? 'No caminho certo' : 'Necessita de ajustes' }}
            </p>
          </div>

          <!-- Eficiência por Operação -->
          <div class="modern-card bg-gray-50 p-4 rounded-lg">
            <h4 class="text-sm font-semibold text-gray-800 mb-3">Eficiência por Operação</h4>
            <div class="h-48">
              <Bar :data="eficienciaChartData" :options="chartOptions" />
            </div>
          </div>
        </div>

        <!-- Previsão para Próximos Meses -->
        <div class="mt-6">
          <h4 class="text-sm font-semibold text-gray-800 mb-3">Previsão para Próximos Meses</h4>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div v-for="(previsao, index) in advancedMetrics.previsaoMensal" :key="index" 
                 class="bg-white p-4 rounded-lg border border-gray-200">
              <h5 class="font-semibold text-gray-700 mb-2">{{ previsao.mes }}</h5>
              <p class="text-lg font-bold text-blue-600">{{ formatCurrency(previsao.previsao) }}</p>
              <p class="text-xs text-gray-500">Previsão de faturamento</p>
              <div class="mt-2 text-xs">
                <span :class="previsao.previsao >= previsao.meta ? 'text-green-600' : 'text-orange-600'">
                  {{ ((previsao.previsao / previsao.meta) * 100).toFixed(1) }}% da meta
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos Principais -->
>>>>>>> Stashed changes
>>>>>>> Stashed changes
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <div class="modern-card xl:col-span-2 bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">Meta vs. Faturamento</h3>
          <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="barChartData" :options="chartOptions" />
          </div>
        </div>
        
        <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">Projeção do Mês</h3>
          <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="projecaoChartData" :options="projecaoChartOptions" />
          </div>
        </div>
      </div>

      <!-- Desempenho -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-3 sm:gap-4 lg:gap-6">
        <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">
            <span class="truncate">Melhores Desempenhos</span>
          </h3>
          <div class="space-y-2 sm:space-y-3">
            <div v-for="(op, index) in desempenho.top" :key="op.id" 
                 class="modern-list-item flex items-center justify-between p-2 sm:p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-all duration-200">
              <div class="flex items-center space-x-2 sm:space-x-3 flex-1 min-w-0">
                <span class="rank-indicator flex-shrink-0 w-6 h-6 bg-green-500 text-white rounded-full flex items-center justify-center text-xs font-bold">
                  {{ index + 1 }}
                </span>
                <div class="flex-1 min-w-0">
                  <p class="text-xs sm:text-sm font-medium text-gray-800 truncate">{{ op.nome || 'N/A' }}</p>
                  <p class="text-xs text-gray-500 truncate">Proj: {{ (op.percentualProjetado || 0).toFixed(1) }}%</p>
                </div>
              </div>
              <span class="flex-shrink-0 text-sm sm:text-base font-bold text-green-500 ml-2">
                {{ (op.percentualAtingido || 0).toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>

        <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">
            <span class="truncate">Necessitam de Atenção</span>
          </h3>
          <div class="space-y-2 sm:space-y-3">
            <div v-for="op in desempenho.bottom" :key="op.id" 
                 class="modern-list-item flex items-center justify-between p-2 sm:p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-all duration-200">
              <div class="flex items-center space-x-2 sm:space-x-3 flex-1 min-w-0">
                <span class="warning-indicator flex-shrink-0 text-red-500 text-lg">
                  ⚠️
                </span>
                <div class="flex-1 min-w-0">
                  <p class="text-xs sm:text-sm font-medium text-gray-800 truncate">{{ op.nome || 'N/A' }}</p>
                  <p class="text-xs text-gray-500 truncate">Proj: {{ (op.percentualProjetado || 0).toFixed(1) }}%</p>
                </div>
              </div>
              <span class="flex-shrink-0 text-sm sm:text-base font-bold text-red-500 ml-2">
                {{ (op.percentualAtingido || 0).toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-8 sm:py-12 modern-empty">
      <div class="w-16 h-16 sm:w-20 sm:h-20 mx-auto text-gray-300 mb-4">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
        </svg>
      </div>
      <h3 class="text-base sm:text-lg lg:text-xl font-semibold text-gray-900 mb-2">Nenhum dado disponível</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto">
        Não há dados para exibir no dashboard no momento.
      </p>
    </div>
  </div>
</template>

<style scoped>
/* (O CSS permanece igual - já estava correto) */
.modern-dashboard {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  min-height: 100vh;
}

.shadow-card {
  box-shadow: 
    0 1px 3px 0 rgba(0, 0, 0, 0.1),
    0 1px 2px 0 rgba(0, 0, 0, 0.06),
    0 0 0 1px rgba(0, 0, 0, 0.02);
}

.hover\:shadow-lg:hover {
  box-shadow: 
    0 10px 15px -3px rgba(0, 0, 0, 0.1),
    0 4px 6px -2px rgba(0, 0, 0, 0.05),
    0 0 0 1px rgba(0, 0, 0, 0.02);
}

.modern-title {
  background: linear-gradient(135deg, #1e293b 0%, #475569 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.modern-card,
.chart-container {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.modern-loading {
  animation: fadeInUp 0.6s ease-out;
}

.modern-empty {
  animation: fadeInUp 0.6s ease-out;
}

.modern-select {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.kpi-value {
  font-feature-settings: 'tnum';
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
}

.kpi-subtext {
  opacity: 0.7;
}

.modern-list-item {
  transition: all 0.2s ease;
}

.modern-list-item:hover {
  transform: translateX(2px);
}

.rank-indicator,
.warning-indicator {
  transition: all 0.3s ease;
}

.rank-indicator:hover {
  transform: scale(1.1);
}

.truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.number-format {
  word-break: break-word;
  overflow-wrap: break-word;
  white-space: normal;
  line-height: 1.2;
}

.min-w-0 {
  min-width: 0;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 640px) {
  .shadow-card {
    box-shadow: 
      0 1px 2px 0 rgba(0, 0, 0, 0.05),
      0 0 0 1px rgba(0, 0, 0, 0.02);
  }
  
  .kpi-value {
    font-size: 1.25rem !important;
    line-height: 1.3;
  }
}

@media (max-width: 480px) {
  .number-format {
    font-size: 1.1rem !important;
  }
  
  .modern-card {
    padding: 1rem !important;
  }
}

@media (max-width: 380px) {
  .text-lg { font-size: 1.125rem; }
  .text-xl { font-size: 1.25rem; }
  .text-2xl { font-size: 1.375rem; }
  .text-3xl { font-size: 1.5rem; }
}

@media (prefers-reduced-motion: reduce) {
  .modern-card,
  .chart-container,
  .modern-list-item,
  .rank-indicator,
  .modern-select {
    transition: none;
    animation: none;
  }
}

.modern-select:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}
</style>