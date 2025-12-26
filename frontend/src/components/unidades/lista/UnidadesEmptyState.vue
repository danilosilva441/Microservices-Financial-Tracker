<script setup>
defineProps({
  isAdmin: Boolean
});

defineEmits(['create']);
</script>

<template>
  <div class="text-center py-16 px-4 bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
    <div class="w-16 h-16 mx-auto bg-slate-100 dark:bg-slate-700 rounded-full flex items-center justify-center mb-4">
      <span class="text-3xl">🏢</span>
    </div>
    
    <h3 class="text-lg font-medium text-slate-900 dark:text-white mb-2">Nenhuma unidade cadastrada</h3>
    <p class="text-slate-500 dark:text-slate-400 mb-6 max-w-sm mx-auto">
      Comece criando sua primeira unidade para acompanhar seus faturamentos e metas.
    </p>
    
    <button 
      v-if="isAdmin"
      @click="$emit('create')"
      class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-6 rounded-lg transition-colors inline-flex items-center shadow-sm"
    >
      Criar Primeira Unidade
    </button>
  </div>
</template>