<script setup>
defineProps(['activeTab']);
defineEmits(['change']);

const tabs = [
  { id: 'visao-geral', label: 'Visão Geral', icon: '📊' },
  { id: 'faturamentos', label: 'Faturamentos', icon: '💰' },
  //{ id: 'mensalistas', label: 'Mensalistas', icon: '🚙' },
  { id: 'configuracoes', label: 'Configurações', icon: '⚙️' },
];
</script>

<template>
  <div class="flex space-x-1 border-b border-slate-200 dark:border-slate-700 mb-6 overflow-x-auto">
    <button
      v-for="tab in tabs"
      :key="tab.id"
      @click="$emit('change', tab.id)"
      class="px-4 py-2 text-sm font-medium rounded-t-lg transition-colors border-t border-l border-r relative -bottom-[1px] whitespace-nowrap"
      :class="[
        activeTab === tab.id 
          ? 'bg-white dark:bg-slate-800 text-blue-600 dark:text-blue-400 border-slate-200 dark:border-slate-700 border-b-white dark:border-b-slate-800 z-10' 
          : 'bg-slate-50 dark:bg-slate-900 text-slate-500 border-transparent hover:bg-slate-100 dark:hover:bg-slate-800'
      ]"
    >
      <span class="mr-2">{{ tab.icon }}</span>
      {{ tab.label }}
    </button>
  </div>
</template>