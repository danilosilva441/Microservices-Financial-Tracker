<script setup>
import { computed } from 'vue';
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  benchmark: {
    type: Object,
    required: true,
    default: () => ({ variacaoReceita: 0, variacaoLucro: 0, variacaoTicket: 0 })
  },
  kpis: {
    type: Object,
    required: true
  }
});

/**
 * Helper para determinar cor e ícone baseados na variação.
 * Positivo = Verde (Seta Cima), Negativo = Vermelho (Seta Baixo).
 */
const getStatus = (valor) => {
  const val = valor || 0; // Proteção contra undefined/null
  const isPositive = val >= 0;
  return {
    colorClass: isPositive
      ? 'text-emerald-600 bg-emerald-50 dark:bg-emerald-900/30 dark:text-emerald-400'
      : 'text-rose-600 bg-rose-50 dark:bg-rose-900/30 dark:text-rose-400',
    icon: isPositive ? '↑' : '↓'
  };
};

const metrics = computed(() => {
  // ✅ CORREÇÃO AQUI: Acesso seguro com ?. e valores default
  const ticketMedio = props.kpis.analise?.ticketMedio || 0; 

  return [
    { 
      label: 'Receita', 
      current: props.kpis.receitaTotal || 0, 
      variation: props.benchmark.variacaoReceita || 0 
    },
    { 
      label: 'Lucro', 
      current: props.kpis.lucroTotal || 0, 
      variation: props.benchmark.variacaoLucro || 0 
    },
    { 
      label: 'Ticket Médio', 
      current: ticketMedio, 
      variation: props.benchmark.variacaoTicket || 0 
    }
  ];
});
</script>

<template>
  <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
    <div v-for="(metric, idx) in metrics" :key="idx"
         class="p-4 bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm transition-colors">
      
      <p class="text-xs text-slate-500 dark:text-slate-400 font-medium uppercase tracking-wide">
        {{ metric.label }} vs Anterior
      </p>
      
      <div class="mt-2 flex items-end justify-between">
        <p class="text-2xl font-bold text-slate-800 dark:text-white">
          {{ formatCurrency(metric.current) }}
        </p>
        
        <div class="flex items-center px-2 py-1 rounded-lg text-xs font-bold"
             :class="getStatus(metric.variation).colorClass">
          <span class="mr-1">{{ getStatus(metric.variation).icon }}</span>
          {{ Math.abs(metric.variation) }}%
        </div>
      </div>
    </div>
  </div>
</template>