<script setup>
import { formatCurrency } from '@/utils/formatters';

/**
 * Componente de Cabeçalho das Despesas.
 * Exibe título, resumo financeiro e botão de ação.
 */
defineProps({
  total: { type: Number, default: 0 }
});

defineEmits(['new', 'back']);
</script>

<template>
  <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-6 bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
    
    <div class="flex items-center gap-4">
      <button 
        @click="$emit('back')"
        class="p-2 rounded-full hover:bg-slate-100 dark:hover:bg-slate-700 text-slate-500 transition-colors"
        title="Voltar"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
      </button>
      <div>
        <h1 class="text-2xl font-bold text-slate-800 dark:text-white">Gestão de Despesas</h1>
        <p class="text-sm text-slate-500 dark:text-slate-400">Controle de saídas e comprovantes</p>
      </div>
    </div>

    <div class="flex items-center gap-6">
      <div class="text-right hidden sm:block">
        <p class="text-xs text-slate-500 uppercase font-bold tracking-wider">Total no Período</p>
        <p class="text-2xl font-bold text-red-600">{{ formatCurrency(total) }}</p>
      </div>

      <button 
        @click="$emit('new')"
        class="flex items-center justify-center bg-red-600 hover:bg-red-700 text-white font-medium px-6 py-3 rounded-lg shadow-sm transition-all transform hover:scale-105"
      >
        <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path></svg>
        Nova Despesa
      </button>
    </div>
  </div>
</template>