<script setup>
import { formatCurrency } from '@/utils/formatters';

/**
 * Tabela de listagem de despesas.
 * Suporta visualização de lista vazia e ações por linha.
 */
defineProps({
  despesas: { type: Array, default: () => [] }
});

defineEmits(['delete', 'view-receipt']);
</script>

<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden">
    
    <div v-if="despesas.length === 0" class="p-12 text-center">
      <div class="w-16 h-16 bg-slate-100 dark:bg-slate-700 rounded-full flex items-center justify-center mx-auto mb-4">
        <span class="text-3xl">💸</span>
      </div>
      <h3 class="text-lg font-medium text-slate-900 dark:text-white">Nenhuma despesa lançada</h3>
      <p class="text-slate-500 dark:text-slate-400">Clique em "Nova Despesa" para registrar saídas.</p>
    </div>

    <div v-else class="overflow-x-auto">
      <table class="w-full text-sm text-left">
        <thead class="bg-slate-50 dark:bg-slate-700/50 text-slate-500 dark:text-slate-300 uppercase font-semibold">
          <tr>
            <th class="px-6 py-4">Data</th>
            <th class="px-6 py-4">Descrição</th>
            <th class="px-6 py-4">Categoria</th>
            <th class="px-6 py-4 text-right">Valor</th>
            <th class="px-6 py-4 text-center">Comprovante</th>
            <th class="px-6 py-4 text-right">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
          <tr v-for="item in despesas" :key="item.id" class="hover:bg-slate-50 dark:hover:bg-slate-700/30 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap text-slate-700 dark:text-slate-300">
              {{ new Date(item.expenseDate).toLocaleDateString() }}
            </td>
            <td class="px-6 py-4 font-medium text-slate-900 dark:text-white">
              {{ item.description }}
            </td>
            <td class="px-6 py-4">
              <span class="px-2.5 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-600">
                {{ item.category?.name || 'Geral' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right font-bold text-red-600 dark:text-red-400">
              - {{ formatCurrency(item.amount) }}
            </td>
            <td class="px-6 py-4 text-center">
              <button 
                v-if="item.receiptPath" 
                @click="$emit('view-receipt', item.receiptPath)"
                class="text-blue-600 hover:text-blue-800 dark:text-blue-400"
                title="Ver Comprovante"
              >
                📎
              </button>
              <span v-else class="text-slate-300 dark:text-slate-600">-</span>
            </td>
            <td class="px-6 py-4 text-right">
              <button 
                @click="$emit('delete', item.id)"
                class="text-red-500 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300 transition-colors"
              >
                Excluir
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>