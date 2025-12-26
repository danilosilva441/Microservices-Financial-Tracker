<script setup>
import { formatCurrency } from '@/utils/formatters';

defineProps({
  unidades: { type: Array, required: true }
});

defineEmits(['select']);

const getStatusColor = (unidade) => {
  const percentual = (unidade.projecaoFaturamento || 0) / (unidade.metaMensal || 1) * 100;
  if (percentual >= 100) return 'text-green-600 bg-green-50 border-green-100';
  if (percentual >= 80) return 'text-blue-600 bg-blue-50 border-blue-100';
  return 'text-yellow-600 bg-yellow-50 border-yellow-100';
};
</script>

<template>
  <div class="lg:hidden space-y-4">
    <div 
      v-for="unidade in unidades" 
      :key="unidade.id"
      @click="$emit('select', unidade.id)"
      class="bg-white dark:bg-slate-800 p-4 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 active:scale-[0.99] transition-transform"
    >
      <div class="flex justify-between items-start mb-3">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/30 rounded-lg flex items-center justify-center text-blue-600">
            🏢
          </div>
          <div>
            <h3 class="font-bold text-slate-800 dark:text-white">{{ unidade.nome }}</h3>
            <p class="text-xs text-slate-500 truncate max-w-[150px]">{{ unidade.descricao }}</p>
          </div>
        </div>
        <span 
          :class="getStatusColor(unidade)" 
          class="text-xs font-bold px-2 py-1 rounded-full border"
        >
          {{ ((unidade.projecaoFaturamento || 0) / (unidade.metaMensal || 1) * 100).toFixed(0) }}%
        </span>
      </div>

      <div class="grid grid-cols-2 gap-4 text-sm mb-3">
        <div>
          <p class="text-slate-500 text-xs uppercase">Meta</p>
          <p class="font-medium text-slate-700 dark:text-slate-200">{{ formatCurrency(unidade.metaMensal) }}</p>
        </div>
        <div class="text-right">
          <p class="text-slate-500 text-xs uppercase">Projeção</p>
          <p class="font-bold text-blue-600">{{ formatCurrency(unidade.projecaoFaturamento || 0) }}</p>
        </div>
      </div>

      <div class="w-full bg-slate-100 dark:bg-slate-700 rounded-full h-1.5">
        <div 
          class="h-1.5 rounded-full bg-blue-500"
          :style="{ width: `${Math.min(((unidade.projecaoFaturamento || 0) / (unidade.metaMensal || 1) * 100), 100)}%` }"
        ></div>
      </div>
    </div>
  </div>
</template>