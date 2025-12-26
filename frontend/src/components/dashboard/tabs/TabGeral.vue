<script setup>
import { computed } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore';
import { formatCurrency } from '@/utils/formatters';
import { Bar, Chart } from 'vue-chartjs'; // Importando Chart genérico para tipos mistos
import { useChartConfig } from '@/composables/useChartConfig';
import { 
  Chart as ChartJS, 
  CategoryScale, 
  LinearScale, 
  PointElement, 
  LineElement, 
  BarElement, 
  Title, 
  Tooltip, 
  Legend, 
  Filler 
} from 'chart.js';

// Registrar todos os componentes necessários do Chart.js
ChartJS.register(
  CategoryScale, LinearScale, PointElement, LineElement, BarElement, 
  Title, Tooltip, Legend, Filler
);

const store = useDashboardStore();
const { baseChartOptions, trendChartOptions } = useChartConfig();

const kpis = computed(() => store.kpis);
const projecao = computed(() => store.kpis.projecao || {});
const analise = computed(() => store.kpis.analise || {});
const graficos = computed(() => store.graficos || {});

// Helpers de Estilo
const getStatusColor = (status) => {
  const map = {
    success: 'bg-emerald-50 border-emerald-200 text-emerald-900',
    warning: 'bg-amber-50 border-amber-200 text-amber-900',
    danger: 'bg-rose-50 border-rose-200 text-rose-900',
    neutro: 'bg-gray-50 border-gray-200 text-gray-900'
  };
  return map[status] || map.neutro;
};

const getGrowthColor = (val) => {
  if (val > 0) return 'text-emerald-600';
  if (val < 0) return 'text-rose-600';
  return 'text-gray-600';
};
</script>

<template>
  <div class="space-y-6 animate-fade-in pb-8">
    
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      
      <div class="rounded-xl border p-5 shadow-sm transition-all duration-300 hover:shadow-md"
           :class="getStatusColor(projecao.status)">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-sm font-bold uppercase tracking-wider opacity-70">Previsão de Fechamento</h3>
            <p class="text-3xl font-bold mt-1">{{ formatCurrency(projecao.valorEstimado) }}</p>
          </div>
          <span class="px-3 py-1 rounded-full text-xs font-bold bg-white/60 border border-black/5 shadow-sm">
            Probabilidade: {{ projecao.probabilidade }}
          </span>
        </div>

        <div class="mb-4">
          <div class="flex justify-between text-xs mb-1 font-medium">
            <span>Progresso Atual: {{ kpis.percentualMetaTotal }}%</span>
            <span>Estimado: {{ projecao.percentualEstimado }}%</span>
          </div>
          <div class="w-full bg-black/10 rounded-full h-2.5">
            <div class="h-2.5 rounded-full transition-all duration-1000 relative"
                 :class="projecao.status === 'success' ? 'bg-emerald-500' : (projecao.status === 'warning' ? 'bg-amber-500' : 'bg-rose-500')"
                 :style="{ width: `${Math.min(projecao.percentualEstimado, 100)}%` }">
            </div>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4 pt-4 border-t border-black/10 text-sm">
          <div>
            <p class="text-xs opacity-70 mb-1">Ritmo Atual</p>
            <p class="font-semibold">{{ formatCurrency(projecao.mediaDiariaAtual) }} / dia</p>
          </div>
          <div>
            <p class="text-xs opacity-70 mb-1">Ritmo Necessário</p>
            <p v-if="projecao.mediaDiariaNecessaria > 0" class="font-semibold text-rose-700">
              {{ formatCurrency(projecao.mediaDiariaNecessaria) }} / dia
            </p>
            <p v-else class="font-semibold text-emerald-700">Meta Garantida! 🚀</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl border border-slate-200 p-5 shadow-sm hover:shadow-md transition-all">
        <h3 class="text-sm font-bold text-slate-500 uppercase tracking-wider mb-4">Raio-X do Período</h3>
        
        <div class="grid grid-cols-2 gap-y-6 gap-x-4">
          <div class="flex items-start gap-3">
            <div class="p-2 rounded-lg bg-blue-50 text-blue-600">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>
            </div>
            <div>
              <p class="text-xs text-slate-500">Crescimento</p>
              <p class="text-lg font-bold" :class="getGrowthColor(analise.crescimentoPeriodo)">
                {{ analise.crescimentoPeriodo > 0 ? '+' : ''}}{{ analise.crescimentoPeriodo }}%
              </p>
            </div>
          </div>

          <div class="flex items-start gap-3">
            <div class="p-2 rounded-lg bg-purple-50 text-purple-600">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
            </div>
            <div>
              <p class="text-xs text-slate-500">Estabilidade</p>
              <p class="text-lg font-bold text-slate-700">{{ analise.estabilidade }}</p>
            </div>
          </div>

          <div class="flex items-start gap-3">
            <div class="p-2 rounded-lg bg-amber-50 text-amber-600">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
            </div>
            <div>
              <p class="text-xs text-slate-500">Melhor Dia</p>
              <p class="text-lg font-bold text-slate-700">{{ analise.diaMelhorPerformance || '-' }}</p>
            </div>
          </div>

          <div class="flex items-start gap-3">
            <div class="p-2 rounded-lg bg-emerald-50 text-emerald-600">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z"></path></svg>
            </div>
            <div>
              <p class="text-xs text-slate-500">Ticket Médio</p>
              <p class="text-lg font-bold text-slate-700">{{ formatCurrency(analise.ticketMedio) }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
      <div class="bg-white p-4 rounded-xl border shadow-sm">
        <p class="text-xs text-slate-500 mb-1">Receita Total</p>
        <p class="text-xl font-bold text-slate-800">{{ formatCurrency(kpis.receitaTotal) }}</p>
      </div>
      <div class="bg-white p-4 rounded-xl border shadow-sm">
        <p class="text-xs text-slate-500 mb-1">Despesa Total</p>
        <p class="text-xl font-bold text-red-600">{{ formatCurrency(kpis.despesaTotal) }}</p>
      </div>
      <div class="bg-white p-4 rounded-xl border shadow-sm">
        <p class="text-xs text-slate-500 mb-1">Lucro Líquido</p>
        <p class="text-xl font-bold text-blue-600">{{ formatCurrency(kpis.lucroTotal) }}</p>
      </div>
      <div class="bg-white p-4 rounded-xl border shadow-sm">
        <p class="text-xs text-slate-500 mb-1">Unidades Ativas</p>
        <p class="text-xl font-bold text-slate-800">{{ kpis.unidadesAtivas }}</p>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[400px]">
      
      <div class="lg:col-span-2 bg-white p-5 rounded-xl border shadow-sm flex flex-col">
        <h3 class="text-slate-700 font-bold mb-4 flex items-center gap-2">
          <span class="w-2 h-6 bg-blue-500 rounded-sm"></span>
          Tendência de Vendas (Diário vs Acumulado)
        </h3>
        <div class="flex-1 relative w-full min-h-0">
          <Chart 
            v-if="graficos.trendChartData && graficos.trendChartData.labels?.length"
            type="bar" 
            :data="graficos.trendChartData" 
            :options="trendChartOptions" 
          />
          <div v-else class="h-full flex items-center justify-center text-slate-400 text-sm">
            Sem dados de tendência
          </div>
        </div>
      </div>

      <div class="bg-white p-5 rounded-xl border shadow-sm flex flex-col">
        <h3 class="text-slate-700 font-bold mb-4 flex items-center gap-2">
          <span class="w-2 h-6 bg-indigo-500 rounded-sm"></span>
          Performance por Dia
        </h3>
        <div class="flex-1 relative w-full min-h-0">
          <Bar 
            v-if="graficos.sazonalidadeChartData && graficos.sazonalidadeChartData.labels?.length"
            :data="graficos.sazonalidadeChartData" 
            :options="baseChartOptions" 
          />
          <div v-else class="h-full flex items-center justify-center text-slate-400 text-sm">
            Sem dados sazonais
          </div>
        </div>
      </div>
    </div>
  </div>
</template>