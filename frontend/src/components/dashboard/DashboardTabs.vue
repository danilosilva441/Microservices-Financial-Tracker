<script setup>
defineProps(['activeTab']);
defineEmits(['change']);

const tabs = [
  { id: 'geral', label: 'Visão Geral', icon: '📊' },
  { id: 'faturamento', label: 'Faturamento', icon: '💰' },
  { id: 'despesas', label: 'Despesas', icon: '💸' },
  { id: 'lucros', label: 'Lucratividade', icon: '📈' },
];
</script>

<template>
  <div class="flex space-x-1 overflow-x-auto">
    <button
      v-for="tab in tabs"
      :key="tab.id"
      @click="$emit('change', tab.id)"
      class="px-6 py-3 text-sm font-medium rounded-t-lg transition-all duration-200 border-t border-l border-r relative top-[1px]"
      :class="[
        activeTab === tab.id 
          ? 'bg-white text-blue-600 border-gray-200 border-b-white z-10' 
          : 'bg-slate-100 text-slate-500 border-transparent hover:bg-slate-200'
      ]"
    >
      <span class="mr-2">{{ tab.icon }}</span>
      {{ tab.label }}
    </button>
  </div>
</template>