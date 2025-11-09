<script setup>
import { onMounted, computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useOperacoesStore } from '@/stores/operacoes';
import { formatCurrency } from '@/utils/formatters.js';
import FaturamentoForm from '@/components/FaturamentoForm.vue';
import { useAuthStore } from '@/stores/authStore';

// Importações otimizadas do Chart.js
import { Bar, Line, Pie } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  LineElement,
  PointElement,
  ArcElement
} from 'chart.js';

// Registra os componentes do Chart.js
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, LineElement, PointElement, ArcElement);

const route = useRoute();
const router = useRouter();
const operacoesStore = useOperacoesStore();
const authStore = useAuthStore();

// Estados e Cache
const cachedCalculations = ref(null);
const lastCalculationTime = ref(0);
const showFiltrosAvancados = ref(false);
const exportLoading = ref(false);

// Filtros para BI
const filtros = ref({
  dataInicio: '',
  dataFim: '',
  origem: '',
  valorMinimo: '',
  valorMaximo: '',
  periodo: 'mensal' // mensal, semanal, diário
});

onMounted(() => {
  operacoesStore.fetchOperacaoById(route.params.id);
});

// Watch para recarregar quando o ID mudar
watch(() => route.params.id, (newId) => {
  if (newId) {
    cachedCalculations.value = null;
    operacoesStore.fetchOperacaoById(newId);
  }
});

// Métodos
const handleSaveFaturamento = async (faturamentoData) => {
  const operacaoId = route.params.id;
  const dadosCompletos = {
    ...faturamentoData,
    origem: 'AVULSO'
  };
  await operacoesStore.addFaturamento(operacaoId, dadosCompletos);
  cachedCalculations.value = null;
};

const handleDeleteOperacao = async () => {
  const operacaoId = operacaoAtual.value.id;
  if (window.confirm(`Tem certeza que deseja excluir a operação "${operacaoAtual.value.nome}"? Esta ação não pode ser desfeita.`)) {
    const success = await operacoesStore.deleteOperacao(operacaoId);
    if (success) {
      router.push({ name: 'operacoes' });
    } else {
      alert(`Falha ao excluir a operação: ${operacoesStore.error}`);
    }
  }
};

const handleDeleteFaturamento = async (faturamentoId) => {
  if (window.confirm('Tem certeza que deseja excluir este faturamento?')) {
    await operacoesStore.deleteFaturamento(operacaoAtual.value.id, faturamentoId);
    cachedCalculations.value = null;
  }
};

const limparFiltros = () => {
  filtros.value = {
    dataInicio: '',
    dataFim: '',
    origem: '',
    valorMinimo: '',
    valorMaximo: '',
    periodo: 'mensal'
  };
};

const exportarDados = async () => {
  exportLoading.value = true;
  try {
    // Simular exportação - implementar conforme necessidade
    const dados = kpis.value;
    const blob = new Blob([JSON.stringify(dados, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `dados-operacao-${operacaoAtual.value.nome}-${new Date().toISOString().split('T')[0]}.json`;
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

// --- CÁLCULOS OTIMIZADOS COM CACHE E ANÁLISES BI ---
const operacaoAtual = computed(() => operacoesStore.operacaoAtual);

const calcularKPIs = () => {
  const now = Date.now();
  
  // Retorna cache se existir e for recente (30 segundos)
  if (cachedCalculations.value && (now - lastCalculationTime.value) < 30000) {
    return cachedCalculations.value;
  }

  const operacao = operacaoAtual.value;
  const faturamentos = operacao?.faturamentos?.$values || [];
  
  // Aplicar filtros
  const faturamentosFiltrados = faturamentos.filter(f => {
    const dataFaturamento = new Date(f.data);
    const valor = f.valor;
    
    if (filtros.value.dataInicio && dataFaturamento < new Date(filtros.value.dataInicio)) return false;
    if (filtros.value.dataFim && dataFaturamento > new Date(filtros.value.dataFim)) return false;
    if (filtros.value.origem && f.origem !== filtros.value.origem) return false;
    if (filtros.value.valorMinimo && valor < parseFloat(filtros.value.valorMinimo)) return false;
    if (filtros.value.valorMaximo && valor > parseFloat(filtros.value.valorMaximo)) return false;
    
    return true;
  });

  // Ordenar por data (mais recente primeiro para tabela)
  const faturamentosOrdenados = [...faturamentosFiltrados].sort((a, b) => 
    new Date(b.data) - new Date(a.data)
  );

  // Cálculos básicos
  const totalFaturado = faturamentosFiltrados.reduce((total, f) => total + f.valor, 0);
  const metaMensal = operacao?.metaMensal || 0;
  const percentualMeta = metaMensal > 0 ? (totalFaturado / metaMensal) * 100 : 0;
  const saldoRestante = Math.max(0, metaMensal - totalFaturado);
  
  // Cálculo de média diária inteligente
  const hoje = new Date();
  const diasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();
  const diasDecorridos = hoje.getDate();
  const diasRestantes = Math.max(1, diasNoMes - diasDecorridos);
  const mediaDiariaNecessaria = saldoRestante / diasRestantes;

  // Análises avançadas para BI
  const mediaFaturamentoDiario = diasDecorridos > 0 ? totalFaturado / diasDecorridos : 0;
  const tendencia = mediaFaturamentoDiario > 0 ? (mediaDiariaNecessaria / mediaFaturamentoDiario) * 100 : 0;
  
  // Análise por origem
  const faturamentoPorOrigem = {};
  faturamentosFiltrados.forEach(f => {
    faturamentoPorOrigem[f.origem] = (faturamentoPorOrigem[f.origem] || 0) + f.valor;
  });

  // Análise semanal
  const faturamentoSemanal = {};
  faturamentosFiltrados.forEach(f => {
    const data = new Date(f.data);
    const semana = `Semana ${Math.ceil(data.getDate() / 7)}`;
    faturamentoSemanal[semana] = (faturamentoSemanal[semana] || 0) + f.valor;
  });

  // Agrupar e ordenar faturamentos por dia para gráficos (ordem cronológica)
  const faturamentosPorDia = {};
  faturamentosFiltrados.forEach(faturamento => {
    const data = new Date(faturamento.data).toLocaleDateString('pt-BR');
    faturamentosPorDia[data] = (faturamentosPorDia[data] || 0) + faturamento.valor;
  });
  
  const faturamentoPorDia = Object.entries(faturamentosPorDia)
    .map(([data, valor]) => ({ data, valor }))
    .sort((a, b) => new Date(a.data) - new Date(b.data));

  // Progresso acumulado para gráfico de linha
  const progressoAcumulado = [];
  let acumulado = 0;
  
  faturamentoPorDia.forEach(({ data, valor }) => {
    acumulado += valor;
    progressoAcumulado.push({
      data,
      valor,
      acumulado,
      percentual: (acumulado / metaMensal) * 100
    });
  });

  // Dados para gráficos
  const barChartData = {
    labels: faturamentoPorDia.map(item => item.data),
    datasets: [
      {
        label: 'Faturamento Diário',
        backgroundColor: '#38bdf8',
        borderColor: '#0284c7',
        borderWidth: 1,
        data: faturamentoPorDia.map(item => item.valor)
      }
    ]
  };

  const lineChartData = {
    labels: progressoAcumulado.map(item => item.data),
    datasets: [
      {
        label: 'Progresso Acumulado',
        borderColor: '#10b981',
        backgroundColor: 'rgba(16, 185, 129, 0.1)',
        borderWidth: 2,
        fill: true,
        tension: 0.4,
        data: progressoAcumulado.map(item => item.acumulado)
      },
      {
        label: 'Meta Mensal',
        borderColor: '#ef4444',
        borderWidth: 2,
        borderDash: [5, 5],
        fill: false,
        data: progressoAcumulado.map(() => metaMensal)
      }
    ]
  };

  const pieChartData = {
    labels: Object.keys(faturamentoPorOrigem),
    datasets: [
      {
        data: Object.values(faturamentoPorOrigem),
        backgroundColor: [
          '#4ade80', '#38bdf8', '#f87171', '#fbbf24', 
          '#a78bfa', '#f472b6', '#60a5fa', '#34d399'
        ]
      }
    ]
  };

  const result = {
    // KPIs Básicos
    totalFaturado,
    percentualMeta,
    saldoRestante,
    mediaDiariaNecessaria,
    totalRegistros: faturamentosFiltrados.length,
    
    // Análises BI
    mediaFaturamentoDiario,
    tendencia,
    diasDecorridos,
    diasRestantes,
    diasNoMes,
    
    // Dados para tabelas e gráficos
    faturamentosOrdenados,
    faturamentoPorDia,
    progressoAcumulado,
    faturamentoPorOrigem,
    faturamentoSemanal,
    
    // Dados dos gráficos
    barChartData,
    lineChartData,
    pieChartData
  };

  // Armazena em cache
  cachedCalculations.value = result;
  lastCalculationTime.value = now;

  return result;
};

// Computed properties otimizadas
const kpis = computed(() => calcularKPIs());

// Origens únicas para o filtro
const origensUnicas = computed(() => {
  const origens = operacaoAtual.value?.faturamentos?.$values?.map(f => f.origem) || [];
  return [...new Set(origens)];
});

// Configurações responsivas dos gráficos
const getResponsiveFontSize = () => {
  const width = window.innerWidth;
  if (width < 640) return 9;
  if (width < 1024) return 10;
  return 12;
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
        padding: window.innerWidth < 640 ? 8 : 12
      }
    },
    tooltip: {
      callbacks: {
        label: function(context) {
          let label = context.dataset.label || '';
          if (label) label += ': ';
          if (context.parsed.y !== null) {
            label += formatCurrency(context.parsed.y);
          }
          return label;
        }
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: {
          size: getResponsiveFontSize()
        },
        maxRotation: window.innerWidth < 640 ? 45 : 0
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
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 10,
        font: {
          size: getResponsiveFontSize()
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
}));
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-4 sm:p-6 lg:p-8">
    <!-- Header Modernizado -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 lg:mb-8">
      <div class="header-content">
        <h1 class="text-2xl sm:text-3xl lg:text-4xl font-bold text-gray-900 mb-2 modern-title">
          Detalhes da Operação
        </h1>
        <p class="text-gray-600 text-sm sm:text-base hidden sm:block">
          Painel analítico completo com métricas avançadas
        </p>
      </div>
      <div class="flex flex-col sm:flex-row gap-3 mt-4 sm:mt-0">
        <button 
          @click="exportarDados"
          :disabled="exportLoading"
          class="flex items-center justify-center bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg transition-all duration-200 text-sm font-medium"
        >
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
          </svg>
          {{ exportLoading ? 'Exportando...' : 'Exportar Dados' }}
        </button>
        <button 
          @click="router.push({ name: 'operacoes' })"
          class="flex items-center justify-center bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition-all duration-200 text-sm font-medium"
        >
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
          </svg>
          Voltar para Operações
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="operacoesStore.isLoading" class="text-center py-12 lg:py-16 modern-loading">
      <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mb-4"></div>
      <p class="text-gray-600 text-lg">Carregando dados da operação...</p>
    </div>

    <div v-else-if="operacaoAtual" class="space-y-6 lg:space-y-8">
      <!-- Dashboard de KPIs -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 lg:gap-6">
        <!-- Meta Atingida -->
        <div class="modern-kpi-card bg-white p-4 lg:p-6 rounded-xl shadow-lg border-l-4 border-green-500 hover-lift">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-sm font-semibold text-gray-500 mb-2">Meta Atingida</h3>
              <p class="text-2xl lg:text-3xl font-bold kpi-value" 
                 :class="{
                   'text-green-500': kpis.percentualMeta >= 100,
                   'text-yellow-500': kpis.percentualMeta >= 70 && kpis.percentualMeta < 100,
                   'text-red-500': kpis.percentualMeta < 70
                 }">
                {{ kpis.percentualMeta.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-1 kpi-subtext truncate">
                {{ formatCurrency(kpis.totalFaturado) }} / {{ formatCurrency(operacaoAtual.metaMensal) }}
              </p>
            </div>
            <div class="kpi-icon w-10 h-10 lg:w-12 lg:h-12 bg-green-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 lg:w-6 lg:h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Saldo Restante -->
        <div class="modern-kpi-card bg-white p-4 lg:p-6 rounded-xl shadow-lg border-l-4 border-blue-500 hover-lift">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-sm font-semibold text-gray-500 mb-2">Saldo Restante</h3>
              <p class="text-2xl lg:text-3xl font-bold text-blue-600 kpi-value">
                {{ formatCurrency(kpis.saldoRestante) }}
              </p>
              <p class="text-xs text-gray-500 mt-1 kpi-subtext">para bater a meta</p>
            </div>
            <div class="kpi-icon w-10 h-10 lg:w-12 lg:h-12 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 lg:w-6 lg:h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Média Diária Necessária -->
        <div class="modern-kpi-card bg-white p-4 lg:p-6 rounded-xl shadow-lg border-l-4 border-orange-500 hover-lift">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-sm font-semibold text-gray-500 mb-2">Média Diária</h3>
              <p class="text-2xl lg:text-3xl font-bold text-orange-600 kpi-value">
                {{ formatCurrency(kpis.mediaDiariaNecessaria) }}
              </p>
              <p class="text-xs text-gray-500 mt-1 kpi-subtext">necessária por dia</p>
            </div>
            <div class="kpi-icon w-10 h-10 lg:w-12 lg:h-12 bg-orange-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 lg:w-6 lg:h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Total de Registros -->
        <div class="modern-kpi-card bg-white p-4 lg:p-6 rounded-xl shadow-lg border-l-4 border-purple-500 hover-lift">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <h3 class="text-sm font-semibold text-gray-500 mb-2">Registros</h3>
              <p class="text-2xl lg:text-3xl font-bold text-purple-600 kpi-value">
                {{ kpis.totalRegistros }}
              </p>
              <p class="text-xs text-gray-500 mt-1 kpi-subtext">faturamentos</p>
            </div>
            <div class="kpi-icon w-10 h-10 lg:w-12 lg:h-12 bg-purple-100 rounded-full flex items-center justify-center flex-shrink-0 ml-3">
              <svg class="w-5 h-5 lg:w-6 lg:h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Filtros Avançados -->
      <div class="bg-white rounded-xl shadow-lg p-6">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4">
          <h3 class="text-lg font-bold text-gray-900 mb-2 sm:mb-0">Filtros Avançados</h3>
          <div class="flex gap-3">
            <button 
              @click="showFiltrosAvancados = !showFiltrosAvancados"
              class="flex items-center text-blue-600 hover:text-blue-800 text-sm font-medium"
            >
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"/>
              </svg>
              {{ showFiltrosAvancados ? 'Ocultar Filtros' : 'Mostrar Filtros' }}
            </button>
            <button 
              @click="limparFiltros"
              class="flex items-center text-gray-600 hover:text-gray-800 text-sm font-medium"
            >
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
              </svg>
              Limpar Filtros
            </button>
          </div>
        </div>

        <div v-if="showFiltrosAvancados" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 border-t">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Data Início</label>
            <input 
              v-model="filtros.dataInicio"
              type="date" 
              class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Data Fim</label>
            <input 
              v-model="filtros.dataFim"
              type="date" 
              class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Origem</label>
            <select 
              v-model="filtros.origem"
              class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">Todas as Origens</option>
              <option v-for="origem in origensUnicas" :key="origem" :value="origem">
                {{ origem }}
              </option>
            </select>
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Valor Mín.</label>
              <input 
                v-model="filtros.valorMinimo"
                type="number" 
                placeholder="0,00"
                class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Valor Máx.</label>
              <input 
                v-model="filtros.valorMaximo"
                type="number" 
                placeholder="0,00"
                class="w-full border-gray-300 rounded-lg shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
            </div>
          </div>
        </div>
      </div>

      <!-- Conteúdo Principal -->
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6 lg:gap-8">
        <!-- Seção de Faturamentos -->
        <div class="xl:col-span-2 order-1 xl:order-2">
          <div class="modern-card bg-white rounded-xl shadow-lg hover-lift">
            <div class="p-6 border-b">
              <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                <h2 class="text-xl font-bold text-gray-900 mb-2 sm:mb-0">Faturamentos Registrados</h2>
                <span class="text-sm text-gray-500 bg-gray-100 px-3 py-1 rounded-full font-medium">
                  {{ kpis.totalRegistros }} registros
                </span>
              </div>
            </div>

            <!-- Tabela com altura fixa e scroll -->
            <div class="p-1">
              <div class="overflow-x-auto">
                <div class="max-h-96 overflow-y-auto">
                  <table class="w-full modern-table">
                    <thead class="bg-gray-50 sticky top-0">
                      <tr>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Data</th>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Valor</th>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Origem</th>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Ações</th>
                      </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-200">
                      <tr 
                        v-for="faturamento in kpis.faturamentosOrdenados" 
                        :key="faturamento.id"
                        class="hover:bg-gray-50 transition-colors duration-150"
                      >
                        <td class="px-4 py-3 text-sm whitespace-nowrap">
                          {{ new Date(faturamento.data).toLocaleDateString('pt-BR') }}
                        </td>
                        <td class="px-4 py-3 text-sm font-medium text-green-600">
                          {{ formatCurrency(faturamento.valor) }}
                        </td>
                        <td class="px-4 py-3 text-sm">
                          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                            {{ faturamento.origem }}
                          </span>
                        </td>
                        <td class="px-4 py-3 text-sm">
                          <button 
                            @click="handleDeleteFaturamento(faturamento.id)"
                            class="modern-delete-btn flex items-center text-red-500 hover:text-red-700 transition-all duration-200"
                          >
                            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                            </svg>
                            Excluir
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              
              <div v-if="kpis.totalRegistros === 0" class="text-center py-12 text-gray-500 border-t">
                <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
                <p class="text-lg font-medium text-gray-900 mb-2">Nenhum faturamento encontrado</p>
                <p class="text-gray-600">Os filtros aplicados não retornaram resultados.</p>
              </div>
            </div>

            <!-- Formulário de Faturamento -->
            <div class="p-6 border-t">
              <h3 class="text-lg font-bold text-gray-900 mb-4">Adicionar Faturamento</h3>
              <FaturamentoForm 
                @submit="handleSaveFaturamento" 
                :error-message="operacoesStore.error" 
              />
            </div>
          </div>
        </div>

        <!-- Card de Informações da Operação -->
        <div class="col-span-1 order-2 xl:order-1">
          <div class="modern-card bg-white rounded-xl shadow-lg hover-lift h-fit">
            <div class="p-6">
              <div class="flex items-start justify-between mb-4">
                <h2 class="text-xl font-bold text-gray-900 pr-2 truncate">{{ operacaoAtual.nome }}</h2>
                <div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                </div>
              </div>
              
              <p class="text-gray-600 mb-6 leading-relaxed">{{ operacaoAtual.descricao || 'Sem descrição.' }}</p>
              
              <hr class="my-6 border-gray-200">
              
              <div class="space-y-4">
                <div class="flex justify-between items-center">
                  <span class="font-medium text-gray-700">Meta Mensal:</span>
                  <span class="font-bold text-green-600">{{ formatCurrency(operacaoAtual.metaMensal) }}</span>
                </div>
                
                <div class="flex justify-between items-center">
                  <span class="font-medium text-gray-700">Faturamento Total:</span>
                  <span class="font-bold text-blue-600">
                    {{ formatCurrency(kpis.totalFaturado) }}
                  </span>
                </div>

                <!-- Métricas BI Adicionais -->
                <div class="pt-4 border-t border-gray-200 space-y-3">
                  <div class="flex justify-between items-center text-sm">
                    <span class="text-gray-600">Dias Decorridos:</span>
                    <span class="font-medium">{{ kpis.diasDecorridos }}/{{ kpis.diasNoMes }}</span>
                  </div>
                  <div class="flex justify-between items-center text-sm">
                    <span class="text-gray-600">Média Diária Real:</span>
                    <span class="font-medium text-green-600">{{ formatCurrency(kpis.mediaFaturamentoDiario) }}</span>
                  </div>
                  <div class="flex justify-between items-center text-sm">
                    <span class="text-gray-600">Tendência:</span>
                    <span class="font-medium" :class="kpis.tendencia <= 100 ? 'text-green-600' : 'text-orange-600'">
                      {{ kpis.tendencia.toFixed(1) }}%
                    </span>
                  </div>
                </div>
              </div>

              <div v-if="authStore.isAdmin" class="mt-8 pt-6 border-t border-gray-200">
                <button 
                  @click="handleDeleteOperacao" 
                  class="modern-delete-btn w-full flex items-center justify-center bg-red-600 hover:bg-red-700 text-white font-medium py-3 px-4 rounded-lg transition-all duration-200 transform hover:scale-105"
                >
                  <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                  Excluir Operação
                </button>
              </div>
            </div>
          </div>

          <!-- Gráfico de Distribuição por Origem -->
          <div class="modern-card bg-white rounded-xl shadow-lg hover-lift mt-6 lg:mt-8">
            <div class="p-6">
              <h3 class="text-lg font-bold text-gray-900 mb-4">Distribuição por Origem</h3>
              <div class="h-64">
                <Pie 
                  v-if="Object.keys(kpis.faturamentoPorOrigem).length > 0" 
                  :data="kpis.pieChartData" 
                  :options="pieChartOptions" 
                />
                <div v-else class="h-full flex items-center justify-center text-gray-500">
                  <p class="text-center">
                    <svg class="w-12 h-12 mx-auto mb-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z"/>
                    </svg>
                    Nenhum dado para exibir
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos de Análise -->
      <div class="grid grid-cols-1 xl:grid-cols-2 gap-6 lg:gap-8 order-3">
        <!-- Gráfico de Progresso Acumulado -->
        <div class="modern-card bg-white rounded-xl shadow-lg hover-lift">
          <div class="p-6">
            <h3 class="text-lg font-bold text-gray-900 mb-4">Progresso da Meta</h3>
            <div class="h-80">
              <Line 
                v-if="kpis.progressoAcumulado.length > 0" 
                :data="kpis.lineChartData" 
                :options="chartOptions" 
              />
              <div v-else class="h-full flex items-center justify-center text-gray-500">
                <p class="text-center">
                  <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                  </svg>
                  Nenhum dado para exibir
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Gráfico de Faturamento Diário -->
        <div class="modern-card bg-white rounded-xl shadow-lg hover-lift">
          <div class="p-6">
            <h3 class="text-lg font-bold text-gray-900 mb-4">Faturamento por Dia</h3>
            <div class="h-80">
              <Bar 
                v-if="kpis.faturamentoPorDia.length > 0" 
                :data="kpis.barChartData" 
                :options="chartOptions" 
              />
              <div v-else class="h-full flex items-center justify-center text-gray-500">
                <p class="text-center">
                  <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                  </svg>
                  Nenhum dado para exibir
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Estado de Operação Não Encontrada -->
    <div v-else class="text-center py-16 modern-empty">
      <svg class="w-24 h-24 mx-auto text-gray-400 mb-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
      </svg>
      <h3 class="text-2xl font-bold text-gray-900 mb-2">Operação não encontrada</h3>
      <p class="text-gray-600 text-lg mb-8">A operação que você está procurando não existe ou foi removida.</p>
      <button 
        @click="router.push({ name: 'operacoes' })"
        class="bg-blue-600 hover:bg-blue-700 text-white px-8 py-3 rounded-lg transition-all duration-200 transform hover:scale-105 font-medium"
      >
        Voltar para Operações
      </button>
    </div>
  </div>
</template>

<style scoped>
/* Design System Moderno */
.modern-title {
  background: linear-gradient(135deg, #1e293b 0%, #475569 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Cards com efeitos modernos */
.modern-card {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.hover-lift:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

/* KPIs Cards */
.modern-kpi-card {
  transition: all 0.3s ease;
}

.modern-kpi-card:hover {
  transform: translateY(-4px);
}

/* Tipografia responsiva para KPIs */
.kpi-value {
  font-feature-settings: 'tnum';
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
  word-break: break-word;
  overflow-wrap: break-word;
}

.kpi-subtext {
  opacity: 0.8;
  font-size: 0.75rem;
  line-height: 1.2;
}

/* Animações */
.modern-loading {
  animation: fadeInUp 0.6s ease-out;
}

.modern-empty {
  animation: fadeInUp 0.6s ease-out;
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

/* Tabela moderna */
.modern-table {
  border-collapse: separate;
  border-spacing: 0;
}

.modern-table thead {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

.modern-table th {
  border-bottom: 2px solid #e2e8f0;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* Botões interativos */
.modern-delete-btn {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Utilitários responsivos */
@media (max-width: 640px) {
  .kpi-value {
    font-size: 1.25rem !important;
  }
  
  .modern-kpi-card {
    padding: 1rem !important;
  }
}

@media (max-width: 480px) {
  .kpi-value {
    font-size: 1.1rem !important;
  }
}

/* Scroll personalizado */
.max-h-96 {
  scrollbar-width: thin;
  scrollbar-color: #cbd5e0 #f7fafc;
}

.max-h-96::-webkit-scrollbar {
  width: 6px;
}

.max-h-96::-webkit-scrollbar-track {
  background: #f7fafc;
  border-radius: 3px;
}

.max-h-96::-webkit-scrollbar-thumb {
  background: #cbd5e0;
  border-radius: 3px;
}

.max-h-96::-webkit-scrollbar-thumb:hover {
  background: #a0aec0;
}

/* Estados de foco para acessibilidade */
.modern-delete-btn:focus {
  outline: 2px solid #ef4444;
  outline-offset: 2px;
}

button:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}
</style>