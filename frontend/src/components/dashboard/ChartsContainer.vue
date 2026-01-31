<script setup>
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

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, LineElement, PointElement);

const props = defineProps({
  graficos: {
    type: Object,
    default: () => ({})
  },
  kpis: {
    type: Object,
    default: () => ({})
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
};

const barChartData = computed(() => {
  const data = props.graficos.barChartData; 
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
</script>

<template>
  <div class="grid grid-cols-1 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
    <!-- Gráfico Principal -->
    <div class="modern-card xl:col-span-2 bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
      <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">Meta vs. Faturamento</h3>
      <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
        <Bar :data="barChartData" :options="chartOptions" />
      </div>
    </div>
    
    <!-- Gráfico de Projeção -->
    <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
      <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">Projeção do Mês</h3>
      <div class="h-32 sm:h-44 lg:h-56 xl:h-72">
        <Line 
          :data="graficos.projecaoChartData || { labels: [], datasets: [] }" 
          :options="chartOptions" 
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
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
</style>