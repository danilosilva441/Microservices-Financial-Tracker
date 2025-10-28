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

// --- TODA A LÓGICA DE CÁLCULO FOI REMOVIDA DAQUI ---

// 4. Acede aos dados PRONTOS vindos da store
const isLoading = computed(() => dashboardStore.isLoading);
const error = computed(() => dashboardStore.error);
const kpis = computed(() => dashboardStore.kpis);
const desempenho = computed(() => dashboardStore.desempenho);
const graficos = computed(() => dashboardStore.graficos);
const operacoes = computed(() => dashboardStore.operacoes);


// --- OPÇÕES DOS GRÁFICOS (Continuam as mesmas) ---
// (Estas funções de formatação são de UI, por isso continuam aqui)

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'bottom', /* ...etc... */ }
  },
  scales: {
    x: { /* ...etc... */ },
    y: {
      ticks: {
        callback: function(value) {
          if (value >= 1000000) return 'R$ ' + (value / 1000000).toFixed(1) + 'M';
          if (value >= 1000) return 'R$ ' + (value / 1000).toFixed(0) + 'K';
          return 'R$ ' + value;
        }
      }
    }
  }
}));

const pieChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'bottom', /* ...etc... */ },
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
  plugins: {
    legend: { position: 'bottom', /* ...etc... */ }
  },
  scales: {
    x: { /* ...etc... */ },
    y: {
      ticks: {
        callback: function(value) {
          if (value >= 1000000) return 'R$ ' + (value / 1000000).toFixed(1) + 'M';
          if (value >= 1000) return 'R$ ' + (value / 1000).toFixed(0) + 'K';
          return 'R$ ' + value;
        }
      }
    }
  }
}));

// --- DADOS PARA OS GRÁFICOS (Agora muito mais simples) ---
// (Apenas leem os dados da store e aplicam cores)

const barChartData = computed(() => {
    const data = graficos.value.barChart; // Lê os dados pré-calculados
    if (!data) return { labels: [], datasets: [] };
    return {
        labels: data.labels,
        datasets: [
            {
                label: 'Meta Mensal',
                backgroundColor: '#a7f3d0',
                borderColor: '#059669',
                borderWidth: 1,
                data: data.datasets[0].data
            },
            {
                label: 'Faturamento Realizado',
                backgroundColor: '#38bdf8',
                borderColor: '#0284c7',
                borderWidth: 1,
                data: data.datasets[1].data
            }
        ]
    };
});

const pieChartData = computed(() => {
    const data = graficos.value.pieChart; // Lê os dados pré-calculados
    if (!data) return { labels: [], datasets: [] };
    return {
        labels: data.labels,
        datasets: [
            {
                backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa', '#f472b6'],
                data: data.data
            }
        ]
    };
});

const projecaoChartData = computed(() => {
    const data = graficos.value.projecaoChart; // Lê os dados pré-calculados
    if (!data) return { labels: [], datasets: [] };
    const metaStatus = kpis.value.vaiBaterMeta; // Pega o status do KPI
    return {
        labels: data.labels,
        datasets: [
            {
                label: 'Valor',
                backgroundColor: ['#38bdf8', metaStatus === 'alta' ? '#10b981' : metaStatus === 'media' ? '#f59e0b' : '#ef4444'],
                borderColor: ['#0284c7', metaStatus === 'alta' ? '#059669' : metaStatus === 'media' ? '#d97706' : '#dc2626'],
                borderWidth: 1,
                data: data.datasets[0].data
            },
            {
                label: 'Meta',
                type: 'line',
                borderColor: '#6b7280',
                borderWidth: 2,
                borderDash: [5, 5],
                fill: false,
                data: data.datasets[1].data,
                pointRadius: 0
            }
        ]
    };
});
</script>

<!-- O SEU TEMPLATE CONTINUA IGUAL, MAS FOI CORRIGIDO PARA USAR AS COMPUTED PROPERTIES CORRETAS -->
<template>
  <div class="dashboard-container p-3 sm:p-4 lg:p-6 xl:p-8">
    <!-- Header -->
    <div class="dashboard-header flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6 lg:mb-8">
      <!-- ... (o seu código do header aqui, ele já está ótimo) ... -->
    </div>

    <!-- Estados de Loading e Error (Corrigidos para usar as novas variáveis) -->
    <div v-if="isLoading" class="loading-state text-center py-8 sm:py-12 lg:py-16">
      <div class="loading-spinner inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
      <p class="text-gray-600 text-sm sm:text-base loading-text">Carregando dados...</p>
    </div>

    <div v-else-if="error" class="error-state text-center py-8 sm:py-12 lg:py-16">
      <!-- ... (o seu código de estado de erro aqui) ... -->
      <button @click="dashboardStore.fetchDashboardData" class="retry-btn ...">
        Tentar Novamente
      </button>
    </div>

    <!-- Conteúdo Principal (Corrigido para usar 'kpis' e 'desempenho') -->
    <div v-else-if="operacoes.length > 0" class="dashboard-content">
      <!-- KPIs Principais -->
      <div class="kpi-grid grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Faturamento Total -->
        <div class="kpi-card ...">
          <p class="kpi-value ...">{{ formatCurrency(kpis.faturamentoTotal) }}</p>
          <p class="kpi-subtext ...">Dia {{ kpis.diaAtual }}/{{ kpis.totalDiasMes }}</p>
        </div>

        <!-- Meta Atingida -->
        <div class="kpi-card ...">
          <p class="kpi-value ..." :class="{...}">{{ kpis.percentualMeta.toFixed(1) }}%</p>
          <p class="kpi-subtext ...">{{ formatCurrency(kpis.faturamentoTotal) }} / {{ formatCurrency(kpis.metaTotal) }}</p>
        </div>

        <!-- Projeção do Mês -->
        <div class="kpi-card ...">
          <p class="kpi-value ..." :class="{...}">{{ kpis.percentualProjetado.toFixed(1) }}%</p>
          <p class="kpi-subtext ...">{{ formatCurrency(kpis.projecaoFinalMes) }} projetado</p>
        </div>
      </div>

      <!-- Segunda Linha de KPIs (Corrigido) -->
      <div class="kpi-grid-secondary grid grid-cols-1 sm:grid-cols-3 ...">
        <!-- Média Diária Atual -->
        <div class="kpi-card ...">
          <p class="kpi-value ...">{{ formatCurrency(kpis.mediaDiariaAtual) }}</p>
        </div>
        <!-- Saldo Restante -->
        <div class="kpi-card ...">
          <p class="kpi-value ...">{{ formatCurrency(kpis.saldoRestante) }}</p>
        </div>
        <!-- Previsão de Meta + Dias Restantes -->
        <div class="kpi-card ...">
          <p class="kpi-value ...">{{ kpis.vaiBaterMeta === 'alta' ? 'Alta' : ... }}</p>
          <p class="kpi-subtext ...">{{ kpis.diasRestantes }} dias restantes</p>
        </div>
      </div>

      <!-- Gráficos Principais (Corrigido) -->
      <div class="charts-grid grid grid-cols-1 xl:grid-cols-3 ...">
        <div class="chart-container xl:col-span-2 ...">
          <Bar :data="barChartData" :options="chartOptions" />
        </div>
        <div class="chart-container ...">
          <Bar :data="projecaoChartData" :options="projecaoChartOptions" />
        </div>
      </div>

      <!-- Performance por Operação (Corrigido) -->
      <div class="performance-grid grid grid-cols-1 lg:grid-cols-2 ...">
        <!-- Top Performers -->
        <div class="performance-card ...">
          <div v-for="(op, index) in desempenho.top" :key="op.id" ...>
            <p class="font-medium ...">{{ op.nome }}</p>
            <p class="text-xs ...">Proj: {{ op.percentualProjetado.toFixed(1) }}%</p>
            <span class="performance-value ...">{{ op.percentualAtingido.toFixed(1) }}%</span>
          </div>
        </div>
        <!-- Áreas de Atenção -->
        <div class="performance-card ...">
          <div v-for="op in desempenho.bottom" :key="op.id" ...>
            <p class="font-medium ...">{{ op.nome }}</p>
            <p class="text-xs ...">Proj: {{ op.percentualProjetado.toFixed(1) }}%</p>
            <span class="performance-value ...">{{ op.percentualAtingido.toFixed(1) }}%</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Estado Vazio -->
    <div v-else class="empty-state text-center py-8 sm:py-12 lg:py-16">
      <!-- ... (o seu código de estado vazio aqui) ... -->
    </div>
  </div>
</template>

<style scoped>
/* O SEU CSS ESTÁ PERFEITO E NÃO PRECISA DE ALTERAÇÕES */
/* ... (todo o seu CSS scoped) ... */
</style>