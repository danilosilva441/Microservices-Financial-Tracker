<script setup>
import { formatCurrency } from '@/utils/formatters';
import { Bar, Pie } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  ArcElement
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement);

const props = defineProps({
  type: {
    type: String,
    default: 'receita'
  },
  title: {
    type: String,
    default: 'Detalhamento Financeiro'
  },
  color: {
    type: String,
    default: 'blue'
  },
  operacoes: {
    type: Array,
    default: () => []
  }
});

const getColorScheme = () => {
  switch (props.color) {
    case 'green': return { bg: 'bg-green-50', text: 'text-green-700', border: 'border-green-200' };
    case 'red': return { bg: 'bg-red-50', text: 'text-red-700', border: 'border-red-200' };
    case 'blue': return { bg: 'bg-blue-50', text: 'text-blue-700', border: 'border-blue-200' };
    default: return { bg: 'bg-gray-50', text: 'text-gray-700', border: 'border-gray-200' };
  }
};

const colorScheme = getColorScheme();

const summaryData = computed(() => {
  let total = 0;
  let avg = 0;
  let count = 0;
  
  if (props.operacoes.length > 0) {
    total = props.operacoes.reduce((sum, op) => sum + (op.projecaoFaturamento || 0), 0);
    count = props.operacoes.length;
    avg = total / count;
  }
  
  return { total, avg, count };
});

const financeChartData = computed(() => ({
  labels: props.operacoes.map(op => op.nome).slice(0, 8),
  datasets: [{
    label: props.type === 'receita' ? 'Faturamento' : 
           props.type === 'despesa' ? 'Despesas' : 'Lucro',
    data: props.operacoes.map(op => op.projecaoFaturamento || 0).slice(0, 8),
    backgroundColor: props.type === 'receita' ? '#10b981' : 
                    props.type === 'despesa' ? '#ef4444' : '#3b82f6',
    borderColor: props.type === 'receita' ? '#059669' : 
                 props.type === 'despesa' ? '#dc2626' : '#1d4ed8',
    borderWidth: 1
  }]
}));
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h3 class="text-xl font-bold text-gray-900">{{ title }}</h3>
        <p class="text-gray-500 text-sm">Análise detalhada por unidade</p>
      </div>
      <div class="flex items-center space-x-2">
        <span class="px-3 py-1 rounded-full text-xs font-medium" 
              :class="[colorScheme.bg, colorScheme.text]">
          {{ operacoes.length }} unidades
        </span>
      </div>
    </div>

    <!-- Cards de Resumo -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <div class="flex items-center justify-between mb-2">
          <h4 class="text-sm font-semibold text-gray-700">Total</h4>
          <span class="text-xs text-gray-500">{{ type === 'receita' ? 'Receita' : 'Valor' }}</span>
        </div>
        <p class="text-2xl font-bold" :class="colorScheme.text">
          {{ formatCurrency(summaryData.total) }}
        </p>
      </div>

      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <div class="flex items-center justify-between mb-2">
          <h4 class="text-sm font-semibold text-gray-700">Média por Unidade</h4>
          <span class="text-xs text-gray-500">Média</span>
        </div>
        <p class="text-2xl font-bold" :class="colorScheme.text">
          {{ formatCurrency(summaryData.avg) }}
        </p>
      </div>

      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <div class="flex items-center justify-between mb-2">
          <h4 class="text-sm font-semibold text-gray-700">Unidades</h4>
          <span class="text-xs text-gray-500">Ativas</span>
        </div>
        <p class="text-2xl font-bold" :class="colorScheme.text">
          {{ summaryData.count }}
        </p>
      </div>
    </div>

    <!-- Gráfico -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
      <h4 class="text-sm font-semibold text-gray-700 mb-4">
        Distribuição por Unidade (Top 8)
      </h4>
      <div class="h-64">
        <Bar 
          :data="financeChartData" 
          :options="{
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
              legend: {
                position: 'bottom'
              }
            },
            scales: {
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
          }" 
        />
      </div>
    </div>

    <!-- Tabela Detalhada -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="px-4 py-3 border-b border-gray-200 bg-gray-50">
        <h4 class="text-sm font-semibold text-gray-700">Detalhamento por Unidade</h4>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Unidade
              </th>
              <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Meta Mensal
              </th>
              <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Projeção
              </th>
              <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                % Atingido
              </th>
              <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="(op, index) in operacoes.slice(0, 10)" :key="op.id">
              <td class="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ op.nome || 'Unidade ' + (index + 1) }}
              </td>
              <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                {{ formatCurrency(op.metaMensal || 0) }}
              </td>
              <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                {{ formatCurrency(op.projecaoFaturamento || 0) }}
              </td>
              <td class="px-4 py-3 whitespace-nowrap text-sm">
                <span :class="[
                  'font-medium',
                  (op.percentualAtingido || 0) >= 100 ? 'text-green-600' :
                  (op.percentualAtingido || 0) >= 70 ? 'text-yellow-600' : 'text-red-600'
                ]">
                  {{ (op.percentualAtingido || 0).toFixed(1) }}%
                </span>
              </td>
              <td class="px-4 py-3 whitespace-nowrap">
                <span :class="[
                  'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                  (op.percentualAtingido || 0) >= 100 ? 'bg-green-100 text-green-800' :
                  (op.percentualAtingido || 0) >= 70 ? 'bg-yellow-100 text-yellow-800' : 'bg-red-100 text-red-800'
                ]">
                  {{ (op.percentualAtingido || 0) >= 100 ? 'No Alvo' :
                     (op.percentualAtingido || 0) >= 70 ? 'Em Andamento' : 'Atenção' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>