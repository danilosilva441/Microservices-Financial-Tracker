<script setup>
import { formatCurrency } from '@/utils/formatters';
import { Line, Radar } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  RadialLinearScale
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, LineElement, PointElement, LinearScale, CategoryScale, RadialLinearScale);

const props = defineProps({
  kpis: {
    type: Object,
    default: () => ({})
  },
  advancedMetrics: {
    type: Object,
    default: () => ({})
  },
  operacoes: {
    type: Array,
    default: () => []
  }
});

// Dados para análise de tendência
const trendChartData = computed(() => ({
  labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul'],
  datasets: [
    {
      label: 'Receita',
      data: [120000, 145000, 138000, 162000, 158000, 175000, 190000],
      borderColor: '#10b981',
      backgroundColor: 'rgba(16, 185, 129, 0.1)',
      tension: 0.4,
      fill: true
    },
    {
      label: 'Meta',
      data: [130000, 140000, 150000, 160000, 170000, 180000, 190000],
      borderColor: '#6b7280',
      borderDash: [5, 5],
      tension: 0.4
    }
  ]
}));

// Dados para radar de eficiência
const efficiencyRadarData = computed(() => {
  const top5 = props.operacoes.slice(0, 5);
  return {
    labels: top5.map(op => op.nome?.substring(0, 15) || 'Unidade'),
    datasets: [
      {
        label: 'Eficiência (%)',
        data: top5.map(op => op.percentualAtingido || 0),
        backgroundColor: 'rgba(59, 130, 246, 0.2)',
        borderColor: '#3b82f6',
        pointBackgroundColor: '#3b82f6',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: '#3b82f6'
      }
    ]
  };
});

// Indicadores de Performance
const performanceIndicators = computed(() => [
  {
    label: 'Crescimento Mensal',
    value: `${props.kpis.benchmark?.variacaoReceita?.toFixed(1) || 0}%`,
    trend: props.kpis.benchmark?.variacaoReceita || 0,
    icon: '📈'
  },
  {
    label: 'Lucratividade',
    value: `${props.kpis.lucroTotal > 0 ? ((props.kpis.lucroTotal / props.kpis.receitaTotal) * 100).toFixed(1) : 0}%`,
    trend: props.kpis.benchmark?.variacaoLucro || 0,
    icon: '💰'
  },
  {
    label: 'Estabilidade',
    value: props.kpis.analise?.estabilidade || 'N/A',
    trend: 0,
    icon: '⚖️'
  },
  {
    label: 'Eficiência Operacional',
    value: `${props.advancedMetrics.roi?.toFixed(1) || 0}%`,
    trend: 5.2,
    icon: '⚡'
  }
]);
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between mb-6">
      <div>
        <h3 class="text-xl font-bold text-gray-900">Análise de Business Intelligence</h3>
        <p class="text-gray-500 text-sm">Insights avançados e previsões estratégicas</p>
      </div>
      <div class="flex items-center space-x-2">
        <span class="px-3 py-1 bg-purple-100 text-purple-700 rounded-full text-xs font-medium">
          BI Dashboard
        </span>
      </div>
    </div>

    <!-- Indicadores Rápidos -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="(indicator, index) in performanceIndicators" :key="index" 
           class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <div class="flex items-center justify-between mb-3">
          <span class="text-2xl">{{ indicator.icon }}</span>
          <span :class="[
            'text-xs font-medium px-2 py-1 rounded',
            indicator.trend > 0 ? 'bg-green-100 text-green-800' :
            indicator.trend < 0 ? 'bg-red-100 text-red-800' :
            'bg-gray-100 text-gray-800'
          ]">
            {{ indicator.trend > 0 ? '+' : '' }}{{ indicator.trend.toFixed(1) }}%
          </span>
        </div>
        <h4 class="text-sm font-semibold text-gray-700 mb-1">{{ indicator.label }}</h4>
        <p class="text-2xl font-bold text-gray-900">{{ indicator.value }}</p>
      </div>
    </div>

    <!-- Gráficos Principais -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Tendência Temporal -->
      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <h4 class="text-sm font-semibold text-gray-700 mb-4">Tendência de Receita (Últimos 7 meses)</h4>
        <div class="h-64">
          <Line 
            :data="trendChartData" 
            :options="{
              responsive: true,
              maintainAspectRatio: false,
              plugins: {
                legend: { position: 'bottom' }
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

      <!-- Radar de Eficiência -->
      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <h4 class="text-sm font-semibold text-gray-700 mb-4">Eficiência por Unidade (Top 5)</h4>
        <div class="h-64">
          <Radar 
            v-if="operacoes.length > 0"
            :data="efficiencyRadarData" 
            :options="{
              responsive: true,
              maintainAspectRatio: false,
              plugins: {
                legend: { position: 'bottom' }
              },
              scales: {
                r: {
                  beginAtZero: true,
                  max: 100,
                  ticks: {
                    callback: function(value) {
                      return value + '%';
                    }
                  }
                }
              }
            }" 
          />
          <div v-else class="h-full flex items-center justify-center text-gray-500">
            Nenhuma unidade para análise
          </div>
        </div>
      </div>
    </div>

    <!-- Insights e Recomendações -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
      <h4 class="text-sm font-semibold text-gray-700 mb-4">Insights e Recomendações</h4>
      <div class="space-y-3">
        <div v-if="advancedMetrics.pontosCriticos > 0" 
             class="flex items-start p-3 bg-red-50 rounded-lg">
          <span class="text-red-500 mr-3">⚠️</span>
          <div>
            <p class="text-sm font-medium text-red-800">
              {{ advancedMetrics.pontosCriticos }} unidade(s) requerem atenção
            </p>
            <p class="text-xs text-red-600 mt-1">
              Unidades com desempenho abaixo de 70% da meta
            </p>
          </div>
        </div>

        <div v-if="kpis.projecao?.status === 'positivo'" 
             class="flex items-start p-3 bg-green-50 rounded-lg">
          <span class="text-green-500 mr-3">🎯</span>
          <div>
            <p class="text-sm font-medium text-green-800">
              Projeção positiva para o fim do mês
            </p>
            <p class="text-xs text-green-600 mt-1">
              Expectativa de atingir {{ kpis.projecao?.percentualEstimado?.toFixed(1) || 0 }}% da meta
            </p>
          </div>
        </div>

        <div v-if="kpis.benchmark?.variacaoReceita > 0" 
             class="flex items-start p-3 bg-blue-50 rounded-lg">
          <span class="text-blue-500 mr-3">📊</span>
          <div>
            <p class="text-sm font-medium text-blue-800">
              Crescimento positivo identificado
            </p>
            <p class="text-xs text-blue-600 mt-1">
              Variação de {{ kpis.benchmark.variacaoReceita.toFixed(1) }}% em relação ao período anterior
            </p>
          </div>
        </div>

        <div class="flex items-start p-3 bg-purple-50 rounded-lg">
          <span class="text-purple-500 mr-3">💡</span>
          <div>
            <p class="text-sm font-medium text-purple-800">
              Recomendação de BI
            </p>
            <p class="text-xs text-purple-600 mt-1">
              Considere focar em melhorar o ticket médio para aumentar a rentabilidade
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Previsões -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <h4 class="text-sm font-semibold text-gray-700 mb-3">Previsão do Mês</h4>
        <div class="text-center">
          <p class="text-3xl font-bold" 
             :class="kpis.projecao?.status === 'positivo' ? 'text-green-600' : 
                    kpis.projecao?.status === 'negativo' ? 'text-red-600' : 'text-yellow-600'">
            {{ kpis.projecao?.percentualEstimado?.toFixed(1) || 0 }}%
          </p>
          <p class="text-xs text-gray-500 mt-1">da meta</p>
          <p class="text-sm text-gray-700 mt-2">
            {{ formatCurrency(kpis.projecao?.valorEstimado || 0) }} projetados
          </p>
        </div>
      </div>

      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <h4 class="text-sm font-semibold text-gray-700 mb-3">Próximo Mês</h4>
        <div class="text-center">
          <p class="text-3xl font-bold text-blue-600">
            {{ (kpis.projecao?.percentualEstimado || 0 * 1.1).toFixed(1) }}%
          </p>
          <p class="text-xs text-gray-500 mt-1">projeção</p>
          <p class="text-sm text-gray-700 mt-2">
            {{ formatCurrency((kpis.projecao?.valorEstimado || 0) * 1.1) }} estimados
          </p>
        </div>
      </div>

      <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
        <h4 class="text-sm font-semibold text-gray-700 mb-3">Meta Atingida</h4>
        <div class="text-center">
          <p class="text-3xl font-bold" 
             :class="kpis.percentualMeta >= 100 ? 'text-green-600' : 
                    kpis.percentualMeta >= 70 ? 'text-yellow-600' : 'text-red-600'">
            {{ kpis.percentualMeta?.toFixed(1) || 0 }}%
          </p>
          <p class="text-xs text-gray-500 mt-1">atual</p>
          <p class="text-sm text-gray-700 mt-2">
            {{ formatCurrency(kpis.receitaTotal || 0) }} de {{ formatCurrency(kpis.metaTotal || 0) }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>