<script setup>
import { formatCurrency } from '@/utils/formatters';

defineProps({
  unidades: { type: Array, required: true }
});

defineEmits(['select']);

const getStatusColor = (unidade) => {
  const meta = unidade.metaMensal || 1;
  const percentual = (unidade.projecaoFaturamento || 0) / meta * 100;
  
  if (percentual >= 100) return 'bg-green-100 text-green-800 border-green-200';
  if (percentual >= 80) return 'bg-blue-100 text-blue-800 border-blue-200';
  if (percentual >= 50) return 'bg-yellow-100 text-yellow-800 border-yellow-200';
  return 'bg-red-100 text-red-800 border-red-200';
};

const getProgressBarColor = (unidade) => {
  const meta = unidade.metaMensal || 1;
  const percentual = (unidade.projecaoFaturamento || 0) / meta * 100;
  
  if (percentual >= 100) return 'bg-green-500';
  if (percentual >= 80) return 'bg-blue-500';
  if (percentual >= 50) return 'bg-yellow-500';
  return 'bg-red-500';
};
</script>

<template>
  <div class="hidden lg:block bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden">
    <table class="w-full">
      <thead class="bg-slate-50 dark:bg-slate-700/50 border-b dark:border-slate-700">
        <tr>
          <th class="py-4 px-6 text-left text-sm font-semibold text-slate-600 dark:text-slate-300">Unidade</th>
          <th class="py-4 px-6 text-left text-sm font-semibold text-slate-600 dark:text-slate-300">Meta Mensal</th>
          <th class="py-4 px-6 text-left text-sm font-semibold text-slate-600 dark:text-slate-300">Projeção</th>
          <th class="py-4 px-6 text-left text-sm font-semibold text-slate-600 dark:text-slate-300">Status</th>
          <th class="py-4 px-6 text-left text-sm font-semibold text-slate-600 dark:text-slate-300 w-1/4">Progresso</th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
        <tr v-for="unidade in unidades" :key="unidade.id" 
            @click="$emit('select', unidade.id)"
            class="hover:bg-slate-50 dark:hover:bg-slate-700/50 cursor-pointer transition-colors group">
          
          <td class="py-4 px-6">
            <div class="flex items-center">
              <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/30 rounded-lg flex items-center justify-center mr-3 text-blue-600 dark:text-blue-400 group-hover:scale-110 transition-transform">
                <span class="text-xl">🏢</span>
              </div>
              <div>
                <div class="font-medium text-slate-900 dark:text-white">{{ unidade.nome }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-400 truncate max-w-[200px]">{{ unidade.descricao }}</div>
              </div>
            </div>
          </td>
          
          <td class="py-4 px-6 text-sm font-medium text-slate-600 dark:text-slate-300">
            {{ formatCurrency(unidade.metaMensal) }}
          </td>
          
          <td class="py-4 px-6 text-sm font-bold text-blue-600 dark:text-blue-400">
            {{ formatCurrency(unidade.projecaoFaturamento || 0) }}
          </td>
          
          <td class="py-4 px-6">
            <span :class="getStatusColor(unidade)" class="px-2.5 py-0.5 rounded-full text-xs font-medium border whitespace-nowrap">
              {{ ((unidade.projecaoFaturamento || 0) / (unidade.metaMensal || 1) * 100).toFixed(0) }}% da meta
            </span>
          </td>

          <td class="py-4 px-6">
            <div class="w-full bg-slate-200 dark:bg-slate-600 rounded-full h-2.5">
              <div class="h-2.5 rounded-full transition-all duration-500"
                   :class="getProgressBarColor(unidade)"
                   :style="{ width: `${Math.min(((unidade.projecaoFaturamento || 0) / (unidade.metaMensal || 1) * 100), 100)}%` }">
              </div>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>