<script setup>
import { onMounted, computed, ref } from 'vue';
import { useOperacoesStore } from '@/stores/operacoes';
import { formatCurrency } from '@/utils/formatters';

// Importações do Chart.js (mantidas)
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

// Registra os componentes do Chart.js
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement, LineElement, PointElement);

const operacoesStore = useOperacoesStore();
const timeframe = ref('month');

// Estados para os dados processados
const dashboardData = ref(null);
const isLoading = ref(false);
const error = ref(null);

// Busca os dados quando o componente é montado
onMounted(async () => {
  await loadDashboardData();
});

// Função para carregar dados processados
async function loadDashboardData() {
  isLoading.value = true;
  error.value = null;
  try {
    console.log('📊 Carregando dados processados do dashboard...');
    dashboardData.value = await operacoesStore.fetchDashboardData();
    console.log('✅ Dados do dashboard carregados:', dashboardData.value);
  } catch (err) {
    console.error('❌ Erro ao carregar dados do dashboard:', err);
    error.value = 'Não foi possível carregar os dados do dashboard.';
  } finally {
    isLoading.value = false;
  }
}

// Computed properties simplificadas
const operacoes = computed(() => dashboardData.value?.operacoes || []);
const kpis = computed(() => dashboardData.value?.kpis || {});
const desempenho = computed(() => dashboardData.value?.desempenho || {});
const graficos = computed(() => dashboardData.value?.graficos || {});

// Configurações dos gráficos (mantidas iguais)
const chartOptions = {
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
        },
        maxRotation: window.innerWidth < 640 ? 45 : window.innerWidth < 1024 ? 30 : 0
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

const pieChartOptions = {
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
};

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
</script>

<template>
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8">
    <!-- Header (mantido igual) -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
      <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0">Dashboard Geral</h1>
      <div class="flex items-center space-x-2">
        <select v-model="timeframe" class="text-xs sm:text-sm border border-gray-300 rounded-lg px-2 sm:px-3 py-1.5 sm:py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 w-full sm:w-auto">
          <option value="month">Este Mês</option>
          <option value="quarter">Este Trimestre</option>
          <option value="year">Este Ano</option>
        </select>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-8 sm:py-12">
      <div class="inline-block animate-spin rounded-full h-6 sm:h-8 w-6 sm:w-8 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
      <p class="text-gray-600 text-sm sm:text-base">Carregando dados...</p>
    </div>

    <div v-else-if="error" class="text-center py-8 sm:py-12">
      <svg class="w-12 h-12 sm:w-16 sm:h-16 mx-auto text-red-400 mb-3 sm:mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
      </svg>
      <h3 class="text-base sm:text-lg lg:text-xl font-medium text-gray-900 mb-2">Erro ao carregar dashboard</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto">
        {{ error }}
      </p>
      <button @click="loadDashboardData" class="mt-4 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors">
        Tentar Novamente
      </button>
    </div>

    <div v-else-if="operacoes.length > 0">
      <!-- KPIs Principais - Agora usando dados processados -->
      <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Faturamento Total -->
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4 border-blue-500">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Faturamento Total</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark number-format">
                {{ formatCurrency(kpis.faturamentoTotal) }}
              </p>
              <p class="text-xs text-gray-500 mt-2">
                Dia {{ kpis.diaAtual }}/{{ kpis.totalDiasMes }}
              </p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Meta Atingida -->
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4 border-yellow-500">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Meta Atingida</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format" 
                 :class="kpis.percentualMeta >= 100 ? 'text-green-500' : kpis.percentualMeta >= 70 ? 'text-yellow-500' : 'text-red-500'">
                {{ kpis.percentualMeta.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 number-format">
                {{ formatCurrency(kpis.faturamentoTotal) }} / {{ formatCurrency(kpis.metaTotal) }}
              </p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-yellow-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Projeção do Mês -->
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4" 
             :class="kpis.vaiBaterMeta === 'alta' ? 'border-green-500' : kpis.vaiBaterMeta === 'media' ? 'border-orange-500' : 'border-red-500'">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Projeção do Mês</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format" 
                 :class="kpis.vaiBaterMeta === 'alta' ? 'text-green-500' : kpis.vaiBaterMeta === 'media' ? 'text-orange-500' : 'text-red-500'">
                {{ kpis.percentualProjetado.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-2 number-format">
                {{ formatCurrency(kpis.projecaoFinalMes) }} projetado
              </p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3"
                 :class="kpis.vaiBaterMeta === 'alta' ? 'bg-green-100' : kpis.vaiBaterMeta === 'media' ? 'bg-orange-100' : 'bg-red-100'">
              <svg class="w-5 h-5 sm:w-6 sm:h-6" 
                   :class="kpis.vaiBaterMeta === 'alta' ? 'text-green-600' : kpis.vaiBaterMeta === 'media' ? 'text-orange-600' : 'text-red-600'" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Resto do template mantido IGUAL, apenas usando os dados processados -->
      <!-- Segunda Linha de KPIs -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Média Diária Atual -->
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4 border-blue-400">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Média Diária Atual</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-blue-600 number-format">
                {{ formatCurrency(kpis.mediaDiariaAtual) }}
              </p>
              <p class="text-xs text-gray-500 mt-2">por dia</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-blue-50 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Saldo Restante -->
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4" 
             :class="kpis.saldoRestante > 0 ? 'border-red-400' : 'border-green-400'">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Saldo Restante</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format" 
                 :class="kpis.saldoRestante > 0 ? 'text-red-500' : 'text-green-500'">
                {{ formatCurrency(kpis.saldoRestante) }}
              </p>
              <p class="text-xs text-gray-500 mt-2">para bater a meta</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3"
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
        <div class="bg-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-card border-l-4" 
             :class="kpis.vaiBaterMeta === 'alta' ? 'border-green-400' : kpis.vaiBaterMeta === 'media' ? 'border-orange-400' : 'border-red-400'">
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2">Previsão & Dias</h3>
              <div class="flex items-center space-x-3">
                <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format" 
                   :class="kpis.vaiBaterMeta === 'alta' ? 'text-green-500' : kpis.vaiBaterMeta === 'media' ? 'text-orange-500' : 'text-red-500'">
                  {{ kpis.vaiBaterMeta === 'alta' ? 'Alta' : kpis.vaiBaterMeta === 'media' ? 'Média' : 'Baixa' }}
                </p>
                <span class="text-lg sm:text-xl" 
                      :class="kpis.vaiBaterMeta === 'alta' ? 'text-green-500' : kpis.vaiBaterMeta === 'media' ? 'text-orange-500' : 'text-red-500'">
                  {{ kpis.vaiBaterMeta === 'alta' ? '🎯' : kpis.vaiBaterMeta === 'media' ? '📊' : '⚠️' }}
                </span>
              </div>
              <p class="text-xs text-gray-500 mt-2">
                {{ kpis.diasRestantes }} dias restantes
              </p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center flex-shrink-0 ml-3"
                 :class="kpis.vaiBaterMeta === 'alta' ? 'bg-green-50' : kpis.vaiBaterMeta === 'media' ? 'bg-orange-50' : 'bg-red-50'">
              <svg class="w-5 h-5 sm:w-6 sm:h-6" 
                   :class="kpis.vaiBaterMeta === 'alta' ? 'text-green-500' : kpis.vaiBaterMeta === 'media' ? 'text-orange-500' : 'text-red-500'" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos Principais -->
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <div class="xl:col-span-2 bg-white p-3 sm:p-4 lg:p-6 rounded-lg shadow-card h-48 sm:h-64 lg:h-80 xl:h-96">
          <h3 class="font-bold text-sm sm:text-base lg:text-lg mb-2 sm:mb-3 lg:mb-4">Meta vs. Faturamento</h3>
          <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="graficos.barChartData" :options="chartOptions" />
          </div>
        </div>
        
        <div class="bg-white p-3 sm:p-4 lg:p-6 rounded-lg shadow-card h-48 sm:h-64 lg:h-80 xl:h-96">
          <h3 class="font-bold text-sm sm:text-base lg:text-lg mb-2 sm:mb-3 lg:mb-4">Projeção do Mês</h3>
          <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
            <Bar :data="graficos.projecaoChartData" :options="projecaoChartOptions" />
          </div>
        </div>
      </div>

      <!-- Performance por Operação -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-3 sm:gap-4 lg:gap-6">
        <!-- Top Performers -->
        <div class="bg-white p-3 sm:p-4 lg:p-6 rounded-lg shadow-card">
          <h3 class="font-bold text-sm sm:text-base lg:text-lg mb-3 sm:mb-4 flex items-center">
            <svg class="w-4 h-4 sm:w-5 sm:h-5 text-green-500 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"/>
            </svg>
            <span class="truncate">Melhores Desempenhos</span>
          </h3>
          <div class="space-y-2 sm:space-y-3">
            <div v-for="(op, index) in desempenho.top" :key="op.id" class="flex items-center justify-between p-2 sm:p-3 bg-gray-50 rounded-lg">
              <div class="flex items-center min-w-0 flex-1">
                <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 rounded-full flex items-center justify-center text-white font-bold text-xs sm:text-sm mr-2 sm:mr-3 flex-shrink-0"
                     :class="index === 0 ? 'bg-yellow-500' : index === 1 ? 'bg-gray-400' : 'bg-orange-500'">
                  {{ index + 1 }}
                </div>
                <div class="min-w-0 flex-1">
                  <p class="font-medium text-xs sm:text-sm truncate">{{ op.nome }}</p>
                  <p class="text-xs text-gray-500 truncate">
                    Proj: {{ op.percentualProjetado.toFixed(1) }}%
                  </p>
                </div>
              </div>
              <span class="text-xs sm:text-sm font-bold ml-2 flex-shrink-0" :class="op.percentualAtingido >= 100 ? 'text-green-500' : 'text-blue-500'">
                {{ op.percentualAtingido.toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>

        <!-- Áreas de Atenção -->
        <div class="bg-white p-3 sm:p-4 lg:p-6 rounded-lg shadow-card">
          <h3 class="font-bold text-sm sm:text-base lg:text-lg mb-3 sm:mb-4 flex items-center">
            <svg class="w-4 h-4 sm:w-5 sm:h-5 text-red-500 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
            </svg>
            <span class="truncate">Necessitam de Atenção</span>
          </h3>
          <div class="space-y-2 sm:space-y-3">
            <div v-for="op in desempenho.bottom" :key="op.id" 
                 class="flex items-center justify-between p-2 sm:p-3 bg-red-50 rounded-lg">
              <div class="flex items-center min-w-0 flex-1">
                <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 rounded-full bg-red-100 flex items-center justify-center text-red-600 mr-2 sm:mr-3 flex-shrink-0">
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
              <span class="text-xs sm:text-sm font-bold text-red-500 ml-2 flex-shrink-0">
                {{ op.percentualAtingido.toFixed(1) }}%
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-8 sm:py-12">
      <svg class="w-12 h-12 sm:w-16 sm:h-16 mx-auto text-gray-400 mb-3 sm:mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
      </svg>
      <h3 class="text-base sm:text-lg lg:text-xl font-medium text-gray-900 mb-2">Nenhuma operação cadastrada</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto">
        Comece cadastrando suas operações para visualizar métricas e desempenho.
      </p>
    </div>
  </div>
</template>

<style scoped>
/* Estilos mantidos exatamente iguais */
.shadow-card {
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
}

@media (max-width: 640px) {
  .shadow-card {
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  }
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

@media (max-width: 380px) {
  .text-lg {
    font-size: 1.125rem;
  }
  
  .text-xl {
    font-size: 1.25rem;
  }
}

.leading-tight {
  line-height: 1.25;
}

.min-w-0 {
  min-width: 0;
}

@media (max-width: 340px) {
  .text-xl {
    font-size: 1.125rem;
  }
  
  .text-2xl {
    font-size: 1.375rem;
  }
  
  .text-3xl {
    font-size: 1.5rem;
  }
}

@media (max-width: 480px) {
  .number-format {
    font-size: 1.1rem !important;
    line-height: 1.3;
  }
}

.font-bold {
  word-break: break-word;
}
</style>