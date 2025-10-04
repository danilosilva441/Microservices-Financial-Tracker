<script setup>
import { onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useOperacoesStore } from '@/stores/operacoes';
import { formatCurrency } from '@/utils/formatters.js';
import FaturamentoForm from '@/components/FaturamentoForm.vue';
import { useAuthStore } from '@/stores/authStore';

// Importações do Chart.js
import { Bar, Line } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  LineElement,
  PointElement
} from 'chart.js';

// Registra os componentes do Chart.js
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, LineElement, PointElement);

const route = useRoute();
const router = useRouter();
const operacoesStore = useOperacoesStore();
const operacaoAtual = computed(() => operacoesStore.operacaoAtual);
const authStore = useAuthStore();

onMounted(() => {
  operacoesStore.fetchOperacaoById(route.params.id);
});

async function handleSaveFaturamento(faturamentoData) {
  const operacaoId = route.params.id;
  const dadosCompletos = {
    ...faturamentoData,
    origem: 'AVULSO'
  };
  await operacoesStore.addFaturamento(operacaoId, dadosCompletos);
}

async function handleDeleteOperacao() {
  const operacaoId = operacaoAtual.value.id;
  if (window.confirm(`Tem certeza que deseja excluir a operação "${operacaoAtual.value.nome}"? Esta ação não pode ser desfeita.`)) {
    const success = await operacoesStore.deleteOperacao(operacaoId);
    if (success) {
      router.push({ name: 'operacoes' });
    } else {
      alert(`Falha ao excluir a operação: ${operacoesStore.error}`);
    }
  }
}

async function handleDeleteFaturamento(faturamentoId) {
  if (window.confirm('Tem certeza que deseja excluir este faturamento?')) {
    await operacoesStore.deleteFaturamento(operacaoAtual.value.id, faturamentoId);
  }
}

// --- CÁLCULOS PARA O DASHBOARD DE PROGRESSO ---

// Calcula o total faturado
const totalFaturado = computed(() => 
  operacaoAtual.value?.faturamentos?.$values?.reduce((total, f) => total + f.valor, 0) || 0
);

// Calcula o percentual da meta atingida
const percentualMeta = computed(() =>
  operacaoAtual.value?.metaMensal > 0 ? (totalFaturado.value / operacaoAtual.value.metaMensal) * 100 : 0
);

// Calcula o saldo restante para bater a meta
const saldoRestante = computed(() =>
  Math.max(0, (operacaoAtual.value?.metaMensal || 0) - totalFaturado.value)
);

// Calcula a média diária necessária (considerando 30 dias)
const mediaDiariaNecessaria = computed(() => {
  const diasRestantes = 30 - new Date().getDate();
  return diasRestantes > 0 ? saldoRestante.value / diasRestantes : saldoRestante.value;
});

// Agrupa faturamentos por dia para o gráfico
const faturamentoPorDia = computed(() => {
  if (!operacaoAtual.value?.faturamentos?.$values) return [];
  
  const faturamentosPorDia = {};
  
  operacaoAtual.value.faturamentos.$values.forEach(faturamento => {
    const data = new Date(faturamento.data).toLocaleDateString('pt-BR');
    if (!faturamentosPorDia[data]) {
      faturamentosPorDia[data] = 0;
    }
    faturamentosPorDia[data] += faturamento.valor;
  });
  
  return Object.entries(faturamentosPorDia)
    .map(([data, valor]) => ({ data, valor }))
    .sort((a, b) => new Date(a.data) - new Date(b.data));
});

// Calcula progresso acumulado diário
const progressoAcumulado = computed(() => {
  const dias = [];
  let acumulado = 0;
  
  faturamentoPorDia.value.forEach(({ data, valor }) => {
    acumulado += valor;
    dias.push({
      data,
      valor,
      acumulado,
      percentual: (acumulado / (operacaoAtual.value?.metaMensal || 1)) * 100
    });
  });
  
  return dias;
});

// --- CONFIGURAÇÕES DOS GRÁFICOS ---

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        boxWidth: 12,
        font: {
          size: window.innerWidth < 768 ? 10 : 12
        }
      }
    }
  },
  scales: {
    x: {
      ticks: {
        font: {
          size: window.innerWidth < 768 ? 10 : 12
        }
      }
    },
    y: {
      ticks: {
        font: {
          size: window.innerWidth < 768 ? 10 : 12
        },
        callback: function(value) {
          return formatCurrency(value);
        }
      }
    }
  }
};

// Dados para o gráfico de barras (faturamento diário)
const barChartData = computed(() => ({
  labels: faturamentoPorDia.value.map(item => item.data),
  datasets: [
    {
      label: 'Faturamento Diário',
      backgroundColor: '#38bdf8',
      borderColor: '#0284c7',
      borderWidth: 1,
      data: faturamentoPorDia.value.map(item => item.valor)
    }
  ]
}));

// Dados para o gráfico de linha (progresso acumulado)
const lineChartData = computed(() => ({
  labels: progressoAcumulado.value.map(item => item.data),
  datasets: [
    {
      label: 'Progresso Acumulado',
      borderColor: '#10b981',
      backgroundColor: 'rgba(16, 185, 129, 0.1)',
      borderWidth: 2,
      fill: true,
      tension: 0.4,
      data: progressoAcumulado.value.map(item => item.acumulado)
    },
    {
      label: 'Meta Mensal',
      borderColor: '#ef4444',
      borderWidth: 2,
      borderDash: [5, 5],
      fill: false,
      data: progressoAcumulado.value.map(() => operacaoAtual.value?.metaMensal || 0)
    }
  ]
}));
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 sm:mb-8">
      <h1 class="text-2xl sm:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0">Detalhes da Operação</h1>
      <button 
        @click="router.push({ name: 'operacoes' })"
        class="flex items-center text-blue-600 hover:text-blue-800 text-sm sm:text-base transition-colors"
      >
        <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
        </svg>
        Voltar para Operações
      </button>
    </div>

    <div v-if="operacoesStore.isLoading" class="text-center py-8 sm:py-12">
      <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
      <p class="mt-2 text-gray-600 text-sm sm:text-base">Carregando operação...</p>
    </div>

    <div v-else-if="operacaoAtual" class="space-y-6 sm:space-y-8">
      <!-- Dashboard de Progresso -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-4 sm:gap-6">
        <!-- Meta Atingida -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card border-l-4 border-green-500">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm sm:text-base text-gray-500 mb-2">Meta Atingida</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold" 
                 :class="percentualMeta >= 100 ? 'text-green-500' : percentualMeta >= 70 ? 'text-yellow-500' : 'text-red-500'">
                {{ percentualMeta.toFixed(1) }}%
              </p>
              <p class="text-xs text-gray-500 mt-1">
                {{ formatCurrency(totalFaturado) }} / {{ formatCurrency(operacaoAtual.metaMensal) }}
              </p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-green-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Saldo Restante -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card border-l-4 border-blue-500">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm sm:text-base text-gray-500 mb-2">Saldo Restante</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-blue-600">
                {{ formatCurrency(saldoRestante) }}
              </p>
              <p class="text-xs text-gray-500 mt-1">para bater a meta</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Média Diária Necessária -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card border-l-4 border-orange-500">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm sm:text-base text-gray-500 mb-2">Média Diária</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-orange-600">
                {{ formatCurrency(mediaDiariaNecessaria) }}
              </p>
              <p class="text-xs text-gray-500 mt-1">necessária por dia</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-orange-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Total de Registros -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card border-l-4 border-purple-500">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm sm:text-base text-gray-500 mb-2">Registros</h3>
              <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-purple-600">
                {{ operacaoAtual.faturamentos?.$values?.length || 0 }}
              </p>
              <p class="text-xs text-gray-500 mt-1">faturamentos</p>
            </div>
            <div class="w-10 h-10 sm:w-12 sm:h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos de Progresso -->
      <div class="grid grid-cols-1 xl:grid-cols-2 gap-4 sm:gap-6 lg:gap-8">
        <!-- Gráfico de Progresso Acumulado -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card h-80 sm:h-96">
          <h3 class="font-bold text-base sm:text-lg mb-3 sm:mb-4">Progresso da Meta</h3>
          <div class="h-56 sm:h-72">
            <Line v-if="progressoAcumulado.length > 0" :data="lineChartData" :options="chartOptions" />
            <div v-else class="h-full flex items-center justify-center text-gray-500">
              <p class="text-center">
                <svg class="w-12 h-12 mx-auto mb-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                </svg>
                Nenhum dado para exibir
              </p>
            </div>
          </div>
        </div>

        <!-- Gráfico de Faturamento Diário -->
        <div class="bg-white p-4 sm:p-6 rounded-lg shadow-card h-80 sm:h-96">
          <h3 class="font-bold text-base sm:text-lg mb-3 sm:mb-4">Faturamento por Dia</h3>
          <div class="h-56 sm:h-72">
            <Bar v-if="faturamentoPorDia.length > 0" :data="barChartData" :options="chartOptions" />
            <div v-else class="h-full flex items-center justify-center text-gray-500">
              <p class="text-center">
                <svg class="w-12 h-12 mx-auto mb-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                </svg>
                Nenhum dado para exibir
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Conteúdo Principal -->
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-4 sm:gap-6 lg:gap-8">
        <!-- Card de Informações da Operação -->
        <div class="col-span-1 bg-white p-4 sm:p-6 rounded-lg shadow-card h-fit">
          <div class="flex items-start justify-between mb-4">
            <h2 class="text-lg sm:text-xl font-bold text-gray-800 pr-2">{{ operacaoAtual.nome }}</h2>
            <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
              <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
          
          <p class="text-gray-600 text-sm sm:text-base mb-4">{{ operacaoAtual.descricao || 'Sem descrição.' }}</p>
          
          <hr class="my-4 border-gray-200">
          
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <span class="text-sm sm:text-base font-medium text-gray-700">Meta Mensal:</span>
              <span class="text-sm sm:text-base font-bold text-green-600">{{ formatCurrency(operacaoAtual.metaMensal) }}</span>
            </div>
            
            <div class="flex justify-between items-center">
              <span class="text-sm sm:text-base font-medium text-gray-700">Faturamento Total:</span>
              <span class="text-sm sm:text-base font-bold text-blue-600">
                {{ formatCurrency(totalFaturado) }}
              </span>
            </div>
          </div>

          <div v-if="authStore.isAdmin" class="mt-6 pt-4 border-t border-gray-200">
            <button 
              @click="handleDeleteOperacao" 
              class="w-full flex items-center justify-center bg-red-600 hover:bg-red-700 text-white font-medium py-2 px-4 rounded-lg transition-colors duration-200 text-sm sm:text-base"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
              </svg>
              Excluir Operação
            </button>
          </div>
        </div>

        <!-- Seção de Faturamentos -->
        <div class="col-span-1 xl:col-span-2 bg-white p-4 sm:p-6 rounded-lg shadow-card">
          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
            <h2 class="text-lg sm:text-xl font-bold text-gray-800 mb-2 sm:mb-0">Faturamentos Registrados</h2>
            <span class="text-xs sm:text-sm text-gray-500 bg-gray-100 px-2 py-1 rounded-full">
              {{ operacaoAtual.faturamentos?.$values?.length || 0 }} registros
            </span>
          </div>

          <!-- Tabela de Faturamentos -->
          <div v-if="operacaoAtual.faturamentos?.$values?.length > 0" class="overflow-x-auto mb-6">
            <table class="w-full min-w-[500px]">
              <thead class="bg-gray-50 border-b">
                <tr>
                  <th class="p-2 sm:p-3 text-left text-xs sm:text-sm font-medium text-gray-500 uppercase tracking-wider">Data</th>
                  <th class="p-2 sm:p-3 text-left text-xs sm:text-sm font-medium text-gray-500 uppercase tracking-wider">Valor</th>
                  <th class="p-2 sm:p-3 text-left text-xs sm:text-sm font-medium text-gray-500 uppercase tracking-wider">Origem</th>
                  <th class="p-2 sm:p-3 text-left text-xs sm:text-sm font-medium text-gray-500 uppercase tracking-wider">Ações</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200">
                <tr v-for="faturamento in operacaoAtual.faturamentos.$values" :key="faturamento.id" class="hover:bg-gray-50 transition-colors">
                  <td class="p-2 sm:p-3 text-xs sm:text-sm whitespace-nowrap">
                    {{ new Date(faturamento.data).toLocaleDateString('pt-BR') }}
                  </td>
                  <td class="p-2 sm:p-3 text-xs sm:text-sm font-medium text-green-600">
                    {{ formatCurrency(faturamento.valor) }}
                  </td>
                  <td class="p-2 sm:p-3 text-xs sm:text-sm">
                    <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {{ faturamento.origem }}
                    </span>
                  </td>
                  <td class="p-2 sm:p-3 text-xs sm:text-sm">
                    <button 
                      @click="handleDeleteFaturamento(faturamento.id)"
                      class="flex items-center text-red-500 hover:text-red-700 transition-colors text-xs sm:text-sm"
                    >
                      <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                      </svg>
                      Excluir
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div v-else class="text-center py-8 sm:py-12 border-2 border-dashed border-gray-300 rounded-lg mb-6">
            <svg class="w-12 h-12 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
            </svg>
            <p class="mt-2 text-gray-500 text-sm sm:text-base">Nenhum faturamento registrado para esta operação.</p>
          </div>

          <!-- Formulário de Faturamento -->
          <div class="border-t pt-6 mt-6">
            <h3 class="text-lg font-bold text-gray-800 mb-4">Adicionar Faturamento</h3>
            <FaturamentoForm @submit="handleSaveFaturamento" :error-message="operacoesStore.error" />
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-12">
      <svg class="w-16 h-16 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
      </svg>
      <p class="mt-4 text-gray-500 text-lg">Operação não encontrada.</p>
      <button 
        @click="router.push({ name: 'operacoes' })"
        class="mt-4 bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg transition-colors"
      >
        Voltar para Operações
      </button>
    </div>
  </div>
</template>

<style scoped>
/* Melhorias para tabela em mobile */
@media (max-width: 640px) {
  .min-w-[500px] {
    min-width: 500px;
  }
}

/* Ajustes para dark mode */
@media (prefers-color-scheme: dark) {
  .bg-white {
    background-color: #374151;
  }
  
  .bg-gray-50 {
    background-color: #4b5563;
  }
  
  .text-gray-800 {
    color: #f9fafb;
  }
  
  .text-gray-700 {
    color: #e5e7eb;
  }
  
  .text-gray-600 {
    color: #d1d5db;
  }
  
  .border-gray-200 {
    border-color: #4b5563;
  }
  
  .divide-gray-200 > * + * {
    border-color: #4b5563;
  }
  
  .hover\:bg-gray-50:hover {
    background-color: #4b5563;
  }
}
</style>s