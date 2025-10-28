<script setup>
import { onMounted, computed, ref } from 'vue';
// 1. A sua store de operações (caminho está correto)
import { useOperacoesStore } from '@/stores/operacoes'; 
import { formatCurrency } from '@/utils/formatters';

// Importações do Chart.js (perfeitas)
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

const operacoesStore = useOperacoesStore();
const timeframe = ref('month');

// --- CORREÇÃO DA LÓGICA DE DADOS ---
// 2. Removemos os 'refs' locais, pois a store já os tem
// const dashboardData = ref(null); <-- Removido
// const isLoading = ref(false); <-- Removido
// const error = ref(null); <-- Removido

// 3. Buscamos os dados usando a função correta da store
onMounted(() => {
  operacoesStore.fetchOperacoes();
});
// 4. Removemos a função 'loadDashboardData' que estava a causar o erro
// async function loadDashboardData() { ... } <-- Removido

// 5. A 'computed' de operações agora lê diretamente da store
const operacoes = computed(() => operacoesStore.operacoes?.$values || operacoesStore.operacoes || []);

// --- O RESTANTE DO SEU CÓDIGO (CÁLCULOS E GRÁFICOS) ESTÁ PERFEITO ---
// Todos estes 'computeds' dependem de 'operacoes.value', e como 'operacoes'
// está agora correto, tudo abaixo irá funcionar automaticamente.

// Calcula o faturamento total
const faturamentoTotal = computed(() => 
  operacoes.value.reduce((total, op) => total + (op.projecaoFaturamento || 0), 0)
);

// Calcula a meta total
const metaTotal = computed(() =>
  operacoes.value.reduce((total, op) => total + op.metaMensal, 0)
);

// Calcula o percentual da meta atingida
const percentualMeta = computed(() =>
  metaTotal.value > 0 ? (faturamentoTotal.value / metaTotal.value) * 100 : 0
);

// --- NOVOS CÁLCULOS PARA PROJEÇÃO FUTURA ---
const diaAtual = computed(() => new Date().getDate());
const totalDiasMes = computed(() => new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate());
const mediaDiariaAtual = computed(() =>
  diaAtual.value > 0 ? faturamentoTotal.value / diaAtual.value : 0
);
const projecaoFinalMes = computed(() =>
  mediaDiariaAtual.value * totalDiasMes.value
);
const percentualProjetado = computed(() =>
  metaTotal.value > 0 ? (projecaoFinalMes.value / metaTotal.value) * 100 : 0
);
const vaiBaterMeta = computed(() => {
  if (percentualProjetado.value >= 95) return 'alta';
  if (percentualProjetado.value >= 70) return 'media';
  return 'baixa';
});
const desempenhoOperacoes = computed(() =>
  operacoes.value.map(op => ({
    ...op,
    percentualAtingido: op.metaMensal > 0 ? ((op.projecaoFaturamento || 0) / op.metaMensal) * 100 : 0,
    diferenca: (op.projecaoFaturamento || 0) - op.metaMensal,
    mediaDiaria: diaAtual.value > 0 ? (op.projecaoFaturamento || 0) / diaAtual.value : 0,
    projecaoFinal: diaAtual.value > 0 ? ((op.projecaoFaturamento || 0) / diaAtual.value) * totalDiasMes.value : 0,
    percentualProjetado: op.metaMensal > 0 ? (((op.projecaoFaturamento || 0) / diaAtual.value) * totalDiasMes.value / op.metaMensal) * 100 : 0
  })).sort((a, b) => b.percentualAtingido - a.percentualAtingido)
);
const topOperacoes = computed(() =>
  desempenhoOperacoes.value.slice(0, 3)
);
const bottomOperacoes = computed(() =>
  [...desempenhoOperacoes.value].reverse().slice(0, 3)
);
const statsOperacoes = computed(() => ({
  ativas: operacoes.value.filter(op => op.isAtivo).length,
  inativas: operacoes.value.filter(op => !op.isAtivo).length,
  total: operacoes.value.length
}));
const mediaAtingimento = computed(() => {
  const operacoesComMeta = desempenhoOperacoes.value.filter(op => op.metaMensal > 0);
  return operacoesComMeta.length > 0 
    ? operacoesComMeta.reduce((sum, op) => sum + op.percentualAtingido, 0) / operacoesComMeta.length
    : 0;
});

// --- DADOS PARA OS GRÁFICOS (Sem alterações, já estavam corretos) ---
const chartOptions = { /* ... o seu código ... */ };
const pieChartOptions = { /* ... o seu código ... */ };
const projecaoChartOptions = { /* ... o seu código ... */ };

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
const pieChartData = computed(() => ({
  labels: operacoes.value.map(op => op.nome),
  datasets: [
    {
      backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa', '#f472b6', '#60a5fa', '#34d399', '#f59e0b', '#ef4444'],
      data: operacoes.value.map(op => op.projecaoFaturamento || 0)
    }
  ]
}));
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
  <div class="p-3 sm:p-4 lg:p-6 xl:p-8 modern-dashboard">
    <!-- Header (O seu código original) -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
      <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0 modern-title">
        Dashboard Geral
      </h1>
      <div class="flex items-center space-x-2 mt-2 sm:mt-0">
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

    <!-- 6. CORREÇÃO DOS ESTADOS DE LOADING E ERROR -->
    <!-- Agora usam as variáveis da store -->
    <div v-if="operacoesStore.isLoading" class="text-center py-8 sm:py-12 modern-loading">
      <div class="inline-block animate-spin rounded-full h-8 sm:h-10 w-8 sm:w-10 border-b-2 border-blue-500 mb-3 sm:mb-4"></div>
      <p class="text-gray-600 text-sm sm:text-base">Carregando dados...</p>
    </div>

    <div v-else-if="operacoesStore.error" class="text-center py-8 sm:py-12 modern-error">
      <div class="w-12 h-12 sm:w-16 sm:h-16 mx-auto text-red-400 mb-3 sm:mb-4">
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"/>
        </svg>
      </div>
      <h3 class="text-base sm:text-lg lg:text-xl font-semibold text-gray-900 mb-2">Erro ao carregar dashboard</h3>
      <p class="text-gray-500 text-sm sm:text-base max-w-sm mx-auto mb-4">
        {{ operacoesStore.error }}
      </p>
      <button 
        @click="operacoesStore.fetchOperacoes" 
        class="modern-button px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
      >
        Tentar Novamente
      </button>
    </div>

    <!-- 7. CORREÇÃO DO ESTADO VAZIO E CONTEÚDO PRINCIPAL -->
    <!-- Agora usa 'operacoes.length' -->
    <div v-else-if="operacoes.length > 0" class="dashboard-content">
      
      <!-- KPIs Principais -->
      <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
        <!-- Faturamento Total (Seu código original, agora funcional) -->
        <div class="modern-card ...">
          <p class="... text-neutral-dark ...">{{ formatCurrency(faturamentoTotal) }}</p>
          <p class="... text-gray-500 ...">Dia {{ diaAtual }}/{{ totalDiasMes }}</p>
        </div>

        <!-- Meta Atingida (Seu código original, agora funcional) -->
        <div class="modern-card ...">
          <p class="... font-bold ..." :class="percentualMeta >= 100 ? 'text-green-500' : ...">
            {{ percentualMeta.toFixed(1) }}%
          </p>
          <p class="... text-gray-500 ...">{{ formatCurrency(faturamentoTotal) }} / {{ formatCurrency(metaTotal) }}</p>
        </div>

        <!-- Projeção do Mês (Seu código original, agora funcional) -->
        <div class="modern-card ...">
          <p class="... font-bold ..." :class="vaiBaterMeta === 'alta' ? 'text-green-500' : ...">
            {{ percentualProjetado.toFixed(1) }}%
          </p>
          <p class="... text-gray-500 ...">{{ formatCurrency(projecaoFinalMes) }} projetado</p>
        </div>
      </div>

      <!-- Segunda Linha de KPIs (Seu código original, agora funcional) -->
      <div class="grid grid-cols-1 sm:grid-cols-3 ...">
        <!-- Média Diária Atual -->
        <div class="modern-card ...">
          <p class="... text-blue-600 ...">{{ formatCurrency(mediaDiariaAtual) }}</p>
        </div>
        <!-- Saldo Restante -->
        <div class="modern-card ...">
          <p class="... font-bold ..." :class="(metaTotal - faturamentoTotal) > 0 ? 'text-red-500' : ...">
            {{ formatCurrency(metaTotal - faturamentoTotal) }}
          </p>
        </div>
        <!-- Previsão de Meta + Dias Restantes -->
        <div class="modern-card ...">
          <p class="... font-bold ..." :class="vaiBaterMeta === 'alta' ? 'text-green-500' : ...">
            {{ vaiBaterMeta === 'alta' ? 'Alta' : vaiBaterMeta === 'media' ? 'Média' : 'Baixa' }}
          </p>
          <p class="... text-gray-500 ...">{{ totalDiasMes - diaAtual }} dias restantes</p>
        </div>
      </div>

      <!-- Gráficos Principais (Seu código original, agora funcional) -->
      <div class="grid grid-cols-1 xl:grid-cols-3 ...">
        <div class="modern-card xl:col-span-2 ...">
          <Bar :data="barChartData" :options="chartOptions" />
        </div>
        <div class="modern-card ...">
          <Bar :data="projecaoChartData" :options="projecaoChartOptions" />
        </div>
      </div>

      <!-- Performance por Operação (Seu código original, agora funcional) -->
      <div class="grid grid-cols-1 lg:grid-cols-2 ...">
        <!-- Top Performers -->
        <div class="modern-card ...">
          <div v-for="(op, index) in topOperacoes" :key="op.id" ...>
            <p class="... truncate">{{ op.nome }}</p>
            <p class="... truncate">Proj: {{ op.percentualProjetado.toFixed(1) }}%</p>
            <span class="... font-bold ...">{{ op.percentualAtingido.toFixed(1) }}%</span>
          </div>
        </div>
        <!-- Áreas de Atenção -->
        <div class="modern-card ...">
          <div v-for="op in bottomOperacoes" :key="op.id" ...>
            <p class="... truncate">{{ op.nome }}</p>
            <p class="... truncate">Proj: {{ op.percentualProjetado.toFixed(1) }}%</p>
            <span class="... font-bold ...">{{ op.percentualAtingido.toFixed(1) }}%</span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Estado Vazio (Seu código original, agora funcional) -->
    <div v-else class="text-center py-8 sm:py-12 modern-empty">
      <!-- ... (o seu código de estado vazio) ... -->
    </div>
  </div>
</template>

<style scoped>
/* O SEU CSS ESTÁ PERFEITO E NÃO PRECISA DE ALTERAÇÕES */
/* ... (todo o seu CSS scoped) ... */
</style>
