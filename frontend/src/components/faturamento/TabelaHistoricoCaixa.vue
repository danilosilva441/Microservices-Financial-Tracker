<script setup>
import { formatCurrency } from '@/utils/formatters';

defineProps({
  historico: { type: Array, default: () => [] }
});
</script>

<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden mt-8">
    <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-700/50">
      <h3 class="font-bold text-slate-800 dark:text-white">Histórico de Fechamentos</h3>
    </div>
    
    <div class="overflow-x-auto">
      <table class="w-full text-sm text-left">
        <thead class="bg-white dark:bg-slate-800 text-slate-500 dark:text-slate-400 border-b dark:border-slate-700">
          <tr>
            <th class="px-6 py-3">Data</th>
            <th class="px-6 py-3 text-right">Faturamento Total</th>
            <th class="px-6 py-3 text-right">Fundo de Caixa</th>
            <th class="px-6 py-3">Status</th>
            <th class="px-6 py-3">Observações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
          <tr v-for="item in historico" :key="item.id" class="hover:bg-slate-50 dark:hover:bg-slate-700/30">
            <td class="px-6 py-4 font-medium text-slate-900 dark:text-white">
              {{ new Date(item.data).toLocaleDateString() }}
            </td>
            <td class="px-6 py-4 text-right font-bold text-green-600 dark:text-green-400">
              {{ formatCurrency(item.valorTotalParciais || item.valorAtm || 0) }}
            </td>
            <td class="px-6 py-4 text-right text-slate-600 dark:text-slate-300">
              {{ formatCurrency(item.fundoDeCaixa) }}
            </td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 text-xs rounded-full border border-green-200 dark:border-green-800">
                Fechado
              </span>
            </td>
            <td class="px-6 py-4 text-slate-500 dark:text-slate-400 italic truncate max-w-xs">
              {{ item.observacoes || '-' }}
            </td>
          </tr>
          <tr v-if="historico.length === 0">
            <td colspan="5" class="px-6 py-8 text-center text-slate-500">
              Nenhum histórico de fechamento encontrado.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>