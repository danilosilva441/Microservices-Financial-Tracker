<!-- frontend/src/components/unidades/detalhes/tabs/TabCaixaDiario.vue -->
<script setup>
import { ref } from 'vue';

const props = defineProps({
  unidade: {
    type: Object,
    required: true
  }
});

// Estado básico
const lancamentos = ref([]);
const totalParcial = ref(0);

console.log('TabCaixaDiario carregado para unidade:', props.unidade.id);
</script>

<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-blue-100 dark:border-blue-900 overflow-hidden">
    
    <!-- Header -->
    <div class="p-6 border-b border-slate-100 dark:border-slate-700">
      <h3 class="text-xl font-bold text-slate-800 dark:text-white">
        📋 Caixa Diário
      </h3>
      <p class="text-sm text-slate-500 dark:text-slate-400">
        {{ new Date().toLocaleDateString('pt-BR') }} - Lançamentos em aberto
      </p>
    </div>

    <!-- Conteúdo simplificado -->
    <div class="p-6">
      <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg mb-6">
        <p class="text-sm text-slate-600 dark:text-slate-400">Total Parcial</p>
        <p class="text-2xl font-bold text-slate-800 dark:text-white">
          R$ 0,00
        </p>
      </div>

      <div class="text-center py-12 text-slate-500">
        <p>Funcionalidade de caixa em desenvolvimento...</p>
        <p class="text-sm mt-2">Unidade: {{ unidade.nome }}</p>
      </div>
    </div>
  </div>
</template>