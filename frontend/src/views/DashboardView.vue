<script setup>
import { onMounted, computed, ref } from 'vue';
// 1. Importa a NOVA dashboardStore
import { useDashboardStore } from '@/stores/dashboardStore'; 
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

// 2. Usa a dashboardStore
const dashboardStore = useDashboardStore();
const timeframe = ref('month');

// 3. Busca os dados ao montar
onMounted(() => {
  dashboardStore.fetchDashboardData();
});

// 4. Acede aos dados PRONTOS vindos da store
const isLoading = computed(() => dashboardStore.isLoading);
const error = computed(() => dashboardStore.error);
const kpis = computed(() => dashboardStore.kpis || {});
const desempenho = computed(() => dashboardStore.desempenho || { top: [], bottom: [] });
const graficos = computed(() => dashboardStore.graficos || {});
const operacoes = computed(() => dashboardStore.operacoes || []);

// Helper para verificar se há dados
const hasData = computed(() => {
  return operacoes.value.length > 0 || 
         Object.keys(kpis.value).length > 0 ||
         Object.keys(graficos.value).length > 0;
});

// --- OPÇÕES DOS GRÁFICOS ---
const getResponsiveFontSize = () => {
  const width = typeof window !== 'undefined' ? window.innerWidth : 1024;
  if (width < 640) return 9;
  if (width < 1024) return 10;
  return 12;
};

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
</script>

<template>
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8 modern-dashboard">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
      <div class="header-content">
        <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0 modern-title">
          Dashboard Geral
        </h1>
        <p class="text-xs sm:text-sm text-gray-500 mt-1 hidden sm:block">
          Visão geral do desempenho e métricas
        </p>
      </div>
      <div class="controls-container flex items-center space-x-2 mt-2 sm:mt-0">
        <select 
          v-model="timeframe" 
          class="modern-select text-xs sm:text-sm border border-gray-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white shadow-sm hover:shadow-md w-full sm:w-auto"
        >
          <option value="month">Este Mês</option>
          <option value="quarter">Este Trimestre</option>
          <option value="year">Este Ano</option>
        </select>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-8 sm:py-12 modern-loading">
      <div class="inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
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

      <!-- Gráficos -->
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