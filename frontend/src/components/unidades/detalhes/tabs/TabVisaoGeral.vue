<script setup>
import { computed } from 'vue';
import { formatCurrency } from '@/utils/formatters';
import { Bar } from 'vue-chartjs';
import { useChartConfig } from '@/composables/useChartConfig'; // Reutilizando config do dashboard
import { Chart as ChartJS, registerables } from 'chart.js';

ChartJS.register(...registerables);

const props = defineProps({
  unidade: { type: Object, required: true }
});

const { baseChartOptions } = useChartConfig();

// Cálculos Locais (Simplificados para a view)
const totalFaturado = computed(() => 
  props.unidade.faturamentos?.$values?.reduce((sum, f) => sum + f.valor, 0) || 0
);

const percentual = computed(() => 
  props.unidade.metaMensal > 0 ? (totalFaturado.value / props.unidade.metaMensal) * 100 : 0
);

// Gráfico Simples
const chartData = computed(() => {
  const faturamentos = props.unidade.faturamentos?.$values || [];
  // Agrupa por dia
  const porDia = {};
  faturamentos.forEach(f => {
    const dia = new Date(f.data).toLocaleDateString();
    porDia[dia] = (porDia[dia] || 0) + f.valor;
  });
  
  return {
    labels: Object.keys(porDia),
    datasets: [{
      label: 'Faturamento Diário',
      data: Object.values(porDia),
      backgroundColor: '#3b82f6',
      borderRadius: 4
    }]
  };
});
</script>

<template>
  <div class="space-y-6 animate-fade-in">
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="p-5 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
        <p class="text-sm text-slate-500 uppercase font-bold tracking-wider mb-1">Meta Mensal</p>
        <p class="text-2xl font-bold text-slate-800 dark:text-white">{{ formatCurrency(unidade.metaMensal) }}</p>
      </div>
      
      <div class="p-5 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
        <p class="text-sm text-slate-500 uppercase font-bold tracking-wider mb-1">Realizado</p>
        <p class="text-2xl font-bold text-blue-600">{{ formatCurrency(totalFaturado) }}</p>
        
        <div class="w-full bg-slate-100 rounded-full h-1.5 mt-3">
          <div class="h-1.5 rounded-full transition-all duration-1000"
               :class="percentual >= 100 ? 'bg-green-500' : 'bg-blue-500'"
               :style="{ width: `${Math.min(percentual, 100)}%` }">
          </div>
        </div>
        <p class="text-xs text-right mt-1 text-slate-400">{{ percentual.toFixed(1) }}% atingido</p>
      </div>

      <div class="p-5 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
        <p class="text-sm text-slate-500 uppercase font-bold tracking-wider mb-1">Saldo Restante</p>
        <p class="text-2xl font-bold text-slate-800 dark:text-white">
          {{ formatCurrency(Math.max(0, unidade.metaMensal - totalFaturado)) }}
        </p>
      </div>
    </div>

    <div class="p-6 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
      <h3 class="font-bold text-slate-700 dark:text-slate-200 mb-4">Evolução Diária</h3>
      <div class="h-72">
        <Bar v-if="chartData.labels.length" :data="chartData" :options="baseChartOptions" />
        <div v-else class="h-full flex items-center justify-center text-slate-400">
          Sem dados para exibir no gráfico
        </div>
      </div>
    </div>
  </div>
</template>