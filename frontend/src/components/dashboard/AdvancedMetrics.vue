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
  advancedMetrics: {
    type: Object,
    default: () => ({})
  },
  operacoes: {
    type: Array,
    default: () => []
  }
});

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: { boxWidth: 10, font: { size: 12 }, padding: 12 }
    }
  }
};

const tendenciaChartData = computed(() => ({
  labels: ['Realizado', 'Necessário'],
  datasets: [
    {
      label: 'Média Diária',
      data: [
        props.advancedMetrics.tendencia?.realizado || 0, 
        props.advancedMetrics.tendencia?.necessario || 0
      ],
      backgroundColor: ['#10b981', props.advancedMetrics.tendencia >= 100 ? '#10b981' : '#ef4444'],
      borderColor: ['#059669', props.advancedMetrics.tendencia >= 100 ? '#059669' : '#dc2626'],
      borderWidth: 1
    }
  ]
}));
</script>

<template>
  <div class="modern-card bg-white rounded-xl shadow-card p-6 mb-6 lg:mb-8">
    <h3 class="text-lg font-bold text-gray-800 mb-6 border-b pb-3">Métricas Avançadas de Business Intelligence</h3>
    
    <!-- KPIs de BI -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-4 gap-6 mb-6">
      <div class="text-center p-4 bg-gradient-to-br from-green-50 to-blue-50 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-700 mb-2">ROI Operacional</h4>
        <p class="text-2xl font-bold text-green-600">{{ advancedMetrics.roi?.toFixed(1) || 0 }}%</p>
        <p class="text-xs text-gray-500 mt-1">Retorno sobre investimento</p>
      </div>

      <div class="text-center p-4 bg-gradient-to-br from-blue-50 to-purple-50 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-700 mb-2">Crescimento</h4>
        <p class="text-2xl font-bold text-blue-600">{{ advancedMetrics.crescimentoMensal?.toFixed(1) || 0 }}%</p>
        <p class="text-xs text-gray-500 mt-1">Vs mês anterior</p>
      </div>

      <div class="text-center p-4 bg-gradient-to-br from-orange-50 to-red-50 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-700 mb-2">Variação Semanal</h4>
        <p class="text-2xl font-bold text-orange-600">{{ advancedMetrics.variacaoSemanal?.toFixed(1) || 0 }}%</p>
        <p class="text-xs text-gray-500 mt-1">Performance semanal</p>
      </div>

      <div class="text-center p-4 bg-gradient-to-br from-red-50 to-pink-50 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-700 mb-2">Pontos de Atenção</h4>
        <p class="text-2xl font-bold text-red-600">{{ advancedMetrics.pontosCriticos || 0 }}</p>
        <p class="text-xs text-gray-500 mt-1">Unidades críticas</p>
      </div>
    </div>

    <!-- Gráficos de BI -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <div class="modern-card bg-gray-50 p-4 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-800 mb-3">Tendência de Performance</h4>
        <div class="h-48">
          <Bar :data="tendenciaChartData" :options="chartOptions" />
        </div>
      </div>

      <div class="modern-card bg-gray-50 p-4 rounded-lg">
        <h4 class="text-sm font-semibold text-gray-800 mb-3">Eficiência por Unidade</h4>
        <div class="h-48">
          <Bar 
            v-if="advancedMetrics.eficienciaOperacional"
            :data="{
              labels: advancedMetrics.eficienciaOperacional.map(op => op.nome),
              datasets: [{
                label: 'Eficiência (%)',
                data: advancedMetrics.eficienciaOperacional.map(op => op.contribuicao),
                backgroundColor: advancedMetrics.eficienciaOperacional.map(op => 
                  op.contribuicao >= 100 ? '#10b981' : 
                  op.contribuicao >= 70 ? '#f59e0b' : '#ef4444'
                )
              }]
            }" 
            :options="chartOptions" 
          />
        </div>
      </div>
    </div>

    <!-- Previsão para Próximos Meses -->
    <div class="mt-6">
      <h4 class="text-sm font-semibold text-gray-800 mb-3">Previsão para Próximos Meses</h4>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div v-for="(previsao, index) in advancedMetrics.previsaoMensal || []" :key="index" 
             class="bg-white p-4 rounded-lg border border-gray-200">
          <h5 class="font-semibold text-gray-700 mb-2">{{ previsao.mes }}</h5>
          <p class="text-lg font-bold text-blue-600">{{ formatCurrency(previsao.previsao) }}</p>
          <p class="text-xs text-gray-500">Previsão de faturamento</p>
          <div class="mt-2 text-xs">
            <span :class="previsao.previsao >= previsao.meta ? 'text-green-600' : 'text-orange-600'">
              {{ previsao.meta > 0 ? ((previsao.previsao / previsao.meta) * 100).toFixed(1) : 0 }}% da meta
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modern-card {
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
</style>