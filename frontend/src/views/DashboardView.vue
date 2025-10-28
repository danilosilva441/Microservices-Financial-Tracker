<script setup>
import { onMounted, computed, ref, watch } from 'vue';
import { useOperacoesStore } from '@/stores/operacoes';
import { formatCurrency } from '@/utils/formatters';

// Importações otimizadas do Chart.js
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

// Registra apenas os componentes necessários
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement, LineElement, PointElement);

const operacoesStore = useOperacoesStore();
const timeframe = ref('month');

// Estados para os dados processados
const dashboardData = ref(null);
const isLoading = ref(false);
const error = ref(null);

// Cache simples para evitar requisições repetidas
const dataCache = ref(null);
const lastFetchTime = ref(0);
const CACHE_DURATION = 30000; // 30 segundos

// Busca os dados quando o componente é montado
onMounted(async () => {
  await loadDashboardData();
});

// Watch para timeframe com debounce
let timeoutId;
watch(timeframe, () => {
  clearTimeout(timeoutId);
  timeoutId = setTimeout(() => {
    loadDashboardData();
  }, 300);
});

// Função otimizada para carregar dados com cache
async function loadDashboardData() {
  const now = Date.now();
  
  // Verifica cache
  if (dataCache.value && (now - lastFetchTime.value) < CACHE_DURATION) {
    dashboardData.value = dataCache.value;
    return;
  }

  isLoading.value = true;
  error.value = null;
  
  try {
    console.log('📊 Carregando dados processados do dashboard...');
    const data = await operacoesStore.fetchDashboardData();
    
    // Armazena em cache
    dataCache.value = data;
    lastFetchTime.value = now;
    dashboardData.value = data;
    
    console.log('✅ Dados do dashboard carregados:', data);
  } catch (err) {
    console.error('❌ Erro ao carregar dados do dashboard:', err);
    error.value = 'Não foi possível carregar os dados do dashboard.';
  } finally {
    isLoading.value = false;
  }
}

// Computed properties otimizadas
const operacoes = computed(() => dashboardData.value?.operacoes || []);
const kpis = computed(() => dashboardData.value?.kpis || {});
const desempenho = computed(() => dashboardData.value?.desempenho || {});
const graficos = computed(() => dashboardData.value?.graficos || {});

// Configurações responsivas dos gráficos
const getResponsiveFontSize = () => {
  const width = window.innerWidth;
  if (width < 640) return 9;
  if (width < 1024) return 10;
  return 12;
};

const getResponsivePadding = () => {
  const width = window.innerWidth;
  return width < 640 ? 8 : 12;
};

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: {
    duration: 800,
    easing: 'easeInOutQuart'
  },
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 10,
        font: {
          size: getResponsiveFontSize()
        },
        padding: getResponsivePadding()
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: {
          size: getResponsiveFontSize()
        },
        maxRotation: window.innerWidth < 640 ? 45 : window.innerWidth < 1024 ? 30 : 0
      }
    },
    y: {
      ticks: {
        font: {
          size: getResponsiveFontSize()
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
}));

const pieChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: {
    duration: 800,
    easing: 'easeInOutQuart'
  },
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 10,
        font: {
          size: getResponsiveFontSize()
        },
        padding: getResponsivePadding()
      }
    },
    tooltip: {
      callbacks: {
        label: function(context) {
          const label = context.label || '';
          const value = context.raw || 0;
          const total = context.dataset.data.reduce((a, b) => a + b, 0);
          const percentage = Math.round((value / total) * 100);
          return `${label}: ${formatCurrency(value)} (${percentage}%)`;
        }
      }
    }
  }
}));

const projecaoChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: {
    duration: 800,
    easing: 'easeInOutQuart'
  },
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 10,
        font: {
          size: getResponsiveFontSize()
        },
        padding: getResponsivePadding()
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: {
          size: getResponsiveFontSize()
        }
      }
    },
    y: {
      ticks: {
        font: {
          size: getResponsiveFontSize()
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
}));
</script>

<template>
  <div class="dashboard-container p-3 sm:p-4 lg:p-6 xl:p-8">
    <!-- Header Modernizado -->
    <div class="dashboard-header flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6 lg:mb-8">
      <div class="header-content">
        <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0 gradient-text">
          Dashboard Geral
        </h1>
        <p class="text-xs sm:text-sm text-gray-500 mt-1 hidden sm:block">
          Visão geral do desempenho e métricas
        </p>
      </div>
      <div class="controls-container flex items-center space-x-2 mt-2 sm:mt-0">
        <select 
          v-model="timeframe" 
          class="timeframe-select text-xs sm:text-sm border border-gray-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white shadow-sm hover:shadow-md w-full sm:w-auto"
        >
          <option value="month">Este Mês</option>
          <option value="quarter">Este Trimestre</option>
          <option value="year">Este Ano</option>
        </select>
      </div>
    </div>

    <!-- Estados de Loading e Error -->
    <div v-if="isLoading" class="loading-state text-center py-8 sm:py-12 lg:py-16">
      <div class="loading-spinner inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
      <p class="text-gray-600 text-sm sm:text-base loading-text">Carregando dados...</p>
    </div>

    <div v-else-if="error" class="error-state text-center py-8 sm:py-12 lg:py-16">
      <div class="error-icon w-12 h-12 sm:w-16 sm:h-16 mx-auto text-red-400 mb-3 sm:mb-4">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
        </svg>
      </div>
      <h3 class="text-base sm:text-lg lg:text-xl font-semibold text-gray-900 mb-2">Erro ao carregar dashboard</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto mb-4">
        {{ error }}
      </p>
      <button 
        @click="loadDashboardData" 
        class="retry-btn px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
      >
        Tentar Novamente
      </button>
    </div>

    <div v-else-if="operacoes.length > 0" class="dashboard-content">
      <!-- KPIs Principais - Design Moderno -->
      <div class="kpi-grid grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Faturamento Total -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-500 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Faturamento Total</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark number-format kpi-value">
                {{ formatCurrency(kpis.faturamentoTotal) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">
                Dia {{ kpis.diaAtual }}/{{ kpis.totalDiasMes }}
              </p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Meta Atingida -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-yellow-500 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Meta Atingida</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="{
                   'text-green-500': kpis.percentualMeta >= 100,
                   'text-yellow-500': kpis.percentualMeta >= 70 && kpis.percentualMeta < 100,
                   'text-red-500': kpis.percentualMeta < 70
                 }">
                {{ kpis.percentualMeta.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
                {{ formatCurrency(kpis.faturamentoTotal) }} / {{ formatCurrency(kpis.metaTotal) }}
              </p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 bg-yellow-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Projeção do Mês -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
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
                {{ kpis.percentualProjetado.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
                {{ formatCurrency(kpis.projecaoFinalMes) }} projetado
              </p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200"
                 :class="{
                   'bg-green-100': kpis.vaiBaterMeta === 'alta',
                   'bg-orange-100': kpis.vaiBaterMeta === 'media',
                   'bg-red-100': kpis.vaiBaterMeta === 'baixa'
                 }">
              <svg class="w-5 h-5 sm:w-6 sm:h-6" 
                   :class="{
                     'text-green-600': kpis.vaiBaterMeta === 'alta',
                     'text-orange-600': kpis.vaiBaterMeta === 'media',
                     'text-red-600': kpis.vaiBaterMeta === 'baixa'
                   }" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Segunda Linha de KPIs -->
      <div class="kpi-grid-secondary grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Média Diária Atual -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-400 transform hover:-translate-y-1">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Média Diária Atual</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-blue-600 number-format kpi-value">
                {{ formatCurrency(kpis.mediaDiariaAtual) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">por dia</p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 bg-blue-50 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Saldo Restante -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
             :class="kpis.saldoRestante > 0 ? 'border-red-400' : 'border-green-400'">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Saldo Restante</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                 :class="kpis.saldoRestante > 0 ? 'text-red-500' : 'text-green-500'">
                {{ formatCurrency(kpis.saldoRestante) }}
              </p>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">para bater a meta</p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200"
                 :class="kpis.saldoRestante > 0 ? 'bg-red-50' : 'bg-green-50'">
              <svg class="w-5 h-5 sm:w-6 sm:h-6" 
                   :class="kpis.saldoRestante > 0 ? 'text-red-500' : 'text-green-500'" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Previsão de Meta + Dias Restantes -->
        <div class="kpi-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
             :class="{
               'border-green-400': kpis.vaiBaterMeta === 'alta',
               'border-orange-400': kpis.vaiBaterMeta === 'media',
               'border-red-400': kpis.vaiBaterMeta === 'baixa'
             }">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Previsão & Dias</h3>
              <div class="flex items-center space-x-3">
                <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
                   :class="{
                     'text-green-500': kpis.vaiBaterMeta === 'alta',
                     'text-orange-500': kpis.vaiBaterMeta === 'media',
                     'text-red-500': kpis.vaiBaterMeta === 'baixa'
                   }">
                  {{ kpis.vaiBaterMeta === 'alta' ? 'Alta' : kpis.vaiBaterMeta === 'media' ? 'Média' : 'Baixa' }}
                </p>
                <span class="text-lg sm:text-xl emoji-indicator" 
                      :class="{
                        'text-green-500': kpis.vaiBaterMeta === 'alta',
                        'text-orange-500': kpis.vaiBaterMeta === 'media',
                        'text-red-500': kpis.vaiBaterMeta === 'baixa'
                      }">
                  {{ kpis.vaiBaterMeta === 'alta' ? '🎯' : kpis.vaiBaterMeta === 'media' ? '📊' : '⚠️' }}
                </span>
              </div>
              <p class="text-xs text-gray-500 mt-2 kpi-subtext">
                {{ kpis.diasRestantes }} dias restantes
              </p>
            </div>
            <div class="kpi-icon w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3 transition-colors duration-200"
                 :class="{
                   'bg-green-50': kpis.vaiBaterMeta === 'alta',
                   'bg-orange-50': kpis.vaiBaterMeta === 'media',
                   'bg-red-50': kpis.vaiBaterMeta === 'baixa'
                 }">
              <svg class="w-5 h-5 sm:w-6 sm:h-6" 
                   :class="{
                     'text-green-500': kpis.vaiBaterMeta === 'alta',
                     'text-orange-500': kpis.vaiBaterMeta === 'media',
                     'text-red-500': kpis.vaiBaterMeta === 'baixa'
                   }" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos Principais com Layout Melhorado -->
      <div class="charts-grid grid grid-cols-1 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <div class="chart-container xl:col-span-2 bg-white p-3 sm:p-4 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 h-48 sm:h-64 lg:h-80 xl:h-96">
          <h3 class="chart-title font-bold text-sm sm:text-base lg:text-lg mb-2 sm:mb-3 lg:mb-4 text-gray-800">Meta vs. Faturamento</h3>
          <div class="chart-wrapper h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="graficos.barChartData" :options="chartOptions" />
          </div>
        </div>
        
        <div class="chart-container bg-white p-3 sm:p-4 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 h-48 sm:h-64 lg:h-80 xl:h-96">
          <h3 class="chart-title font-bold text-sm sm:text-base lg:text-lg mb-2 sm:mb-3 lg:mb-4 text-gray-800">Projeção do Mês</h3>
          <div class="chart-wrapper h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="graficos.projecaoChartData" :options="projecaoChartOptions" />
          </div>
        </div>
      </div>

      <!-- Performance por Operação -->
      <div class="performance-grid grid grid-cols-1 lg:grid-cols-2 gap-3 sm:gap-4 lg:gap-6">
        <!-- Top Performers -->
        <div class="performance-card bg-white p-3 sm:p-4 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="performance-title font-bold text-sm sm:text-base lg:text-lg mb-3 sm:mb-4 flex items-center text-gray-800">
            <svg class="w-4 h-4 sm:w-5 sm:h-5 text-green-500 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"/>
            </svg>
            <span class="truncate">Melhores Desempenhos</span>
          </h3>
          <div class="performance-list space-y-2 sm:space-y-3">
            <div 
              v-for="(op, index) in desempenho.top" 
              :key="op.id" 
              class="performance-item flex items-center justify-between p-2 sm:p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors duration-200"
            >
              <div class="flex items-center min-w-0 flex-1">
                <div class="rank-indicator w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 rounded-full flex items-center justify-center text-white font-bold text-xs sm:text-sm mr-2 sm:mr-3 flex-shrink-0 transition-all duration-200"
                     :class="{
                       'bg-yellow-500': index === 0,
                       'bg-gray-400': index === 1,
                       'bg-orange-500': index >= 2
                     }">
                  {{ index + 1 }}
                </div>
                <div class="min-w-0 flex-1">
                  <p class="font-medium text-xs sm:text-sm truncate">{{ op.nome }}</p>
                  <p class="text-xs text-gray-500 truncate">
                    Proj: {{ op.percentualProjetado.toFixed(1) }}%
                  </p>
                </div>
              </div>
              <span class="performance-value text-xs sm:text-sm font-bold ml-2 flex-shrink-0 transition-colors duration-200" 
                    :class="op.percentualAtingido >= 100 ? 'text-green-500' : 'text-blue-500'">
                {{ op.percentualAtingido.toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>

        <!-- Áreas de Atenção -->
        <div class="performance-card bg-white p-3 sm:p-4 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
          <h3 class="performance-title font-bold text-sm sm:text-base lg:text-lg mb-3 sm:mb-4 flex items-center text-gray-800">
            <svg class="w-4 h-4 sm:w-5 sm:h-5 text-red-500 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
            </svg>
            <span class="truncate">Necessitam de Atenção</span>
          </h3>
          <div class="performance-list space-y-2 sm:space-y-3">
            <div 
              v-for="op in desempenho.bottom" 
              :key="op.id" 
              class="performance-item flex items-center justify-between p-2 sm:p-3 bg-red-50 rounded-lg hover:bg-red-100 transition-colors duration-200"
            >
              <div class="flex items-center min-w-0 flex-1">
                <div class="warning-indicator w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 rounded-full bg-red-100 flex items-center justify-center text-red-600 mr-2 sm:mr-3 flex-shrink-0 transition-colors duration-200">
                  <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
                  </svg>
                </div>
                <div class="min-w-0 flex-1">
                  <p class="font-medium text-xs sm:text-sm truncate">{{ op.nome }}</p>
                  <p class="text-xs text-gray-500 truncate">
                    Proj: {{ op.percentualProjetado.toFixed(1) }}%
                  </p>
                </div>
              </div>
              <span class="performance-value text-xs sm:text-sm font-bold text-red-500 ml-2 flex-shrink-0 transition-colors duration-200">
                {{ op.percentualAtingido.toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Estado Vazio -->
    <div v-else class="empty-state text-center py-8 sm:py-12 lg:py-16">
      <div class="empty-icon w-12 h-12 sm:w-16 sm:h-16 mx-auto text-gray-400 mb-3 sm:mb-4">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
        </svg>
      </div>
      <h3 class="text-base sm:text-lg lg:text-xl font-semibold text-gray-900 mb-2">Nenhuma operação cadastrada</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto">
        Comece cadastrando suas operações para visualizar métricas e desempenho.
      </p>
    </div>
  </div>
</template>

<style scoped>
/* Design System Moderno */
.dashboard-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

/* Cards e Shadows */
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

/* Texto Gradient */
.gradient-text {
  background: linear-gradient(135deg, #1e293b 0%, #475569 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Animações Suaves */
.kpi-card,
.chart-container,
.performance-card {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Estados de Loading */
.loading-spinner {
  border-top-color: transparent;
  animation: spin 1s linear infinite;
}

.loading-text {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

/* Estados de Error */
.error-state,
.empty-state {
  animation: fadeInUp 0.6s ease-out;
}

/* Botões Interativos */
.retry-btn,
.timeframe-select {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.retry-btn:active {
  transform: scale(0.95);
}

/* Tipografia Responsiva */
.kpi-value {
  font-feature-settings: 'tnum';
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
}

.kpi-subtext {
  opacity: 0.7;
}

/* Performance Lists */
.performance-item {
  transition: all 0.2s ease;
}

.performance-item:hover {
  transform: translateX(2px);
}

/* Rank Indicators */
.rank-indicator,
.warning-indicator {
  transition: all 0.3s ease;
}

.rank-indicator:hover {
  transform: scale(1.1);
}

/* Emoji Indicators */
.emoji-indicator {
  filter: grayscale(0.3);
  transition: filter 0.3s ease;
}

.emoji-indicator:hover {
  filter: grayscale(0);
}

/* Utilitários Responsivos */
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

/* Animações Keyframes */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
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

/* Media Queries para Mobile First */
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
  
  .kpi-card {
    padding: 1rem !important;
  }
}

@media (max-width: 380px) {
  .text-lg { font-size: 1.125rem; }
  .text-xl { font-size: 1.25rem; }
  .text-2xl { font-size: 1.375rem; }
  .text-3xl { font-size: 1.5rem; }
}

/* Melhorias de Acessibilidade */
@media (prefers-reduced-motion: reduce) {
  .kpi-card,
  .chart-container,
  .performance-card,
  .performance-item,
  .rank-indicator,
  .retry-btn,
  .timeframe-select {
    transition: none;
    animation: none;
  }
}

/* Focus States para Acessibilidade */
.retry-btn:focus,
.timeframe-select:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* High Contrast Mode Support */
@media (prefers-contrast: high) {
  .shadow-card {
    box-shadow: 0 0 0 2px currentColor;
  }
  
  .kpi-card {
    border-width: 2px;
  }
}

/* Dark Mode Ready */
@media (prefers-color-scheme: dark) {
  .dashboard-container {
    background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  }
  
  .kpi-card,
  .chart-container,
  .performance-card {
    background-color: #1e293b;
    color: #f8fafc;
  }
  
  .performance-item {
    background-color: #334155;
  }
}
</style>