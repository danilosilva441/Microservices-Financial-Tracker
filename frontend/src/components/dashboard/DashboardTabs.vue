<script setup>
const props = defineProps({
  activeTab: {
    type: String,
    default: 'geral'
  }
});

const emit = defineEmits(['change']);

const tabs = [
  { id: 'geral', label: 'Visão Geral', icon: '📊' },
  { id: 'financeiro', label: 'Financeiro', icon: '💰' },
  { id: 'operacoes', label: 'Operações', icon: '⚙️' },
  { id: 'analise', label: 'Análise BI', icon: '📈' }
];

const handleTabClick = (tabId) => {
  emit('change', tabId);
};
</script>

<template>
  <div class="dashboard-tabs mb-6">
    <div class="flex space-x-1 border-b border-gray-200">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="handleTabClick(tab.id)"
        :class="[
          'flex items-center px-4 py-2 text-sm font-medium rounded-t-lg transition-all duration-200',
          activeTab === tab.id
            ? 'bg-white border-t border-x border-gray-200 text-blue-600 shadow-sm'
            : 'text-gray-500 hover:text-gray-700 hover:bg-gray-50'
        ]"
      >
        <span class="mr-2">{{ tab.icon }}</span>
        {{ tab.label }}
      </button>
    </div>
  </div>
</template>