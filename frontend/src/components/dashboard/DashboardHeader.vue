<script setup>
import { ref } from 'vue';

const props = defineProps({
  timeframe: {
    type: String,
    default: 'month'
  },
  isLoading: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['timeframe-change', 'export', 'refresh']);

const localTimeframe = ref(props.timeframe);

const handleTimeframeChange = () => {
  emit('timeframe-change', localTimeframe.value);
};
</script>

<template>
  <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
    <div class="header-content">
      <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark mb-2 sm:mb-0 modern-title">
        Dashboard Geral
      </h1>
      <p class="text-xs sm:text-sm text-gray-500 mt-1 hidden sm:block">
        Visão geral do desempenho e métricas avançadas de BI
      </p>
    </div>
    <div class="controls-container flex items-center space-x-2 mt-2 sm:mt-0">
      <select 
        v-model="localTimeframe" 
        @change="handleTimeframeChange"
        :disabled="isLoading"
        class="modern-select text-xs sm:text-sm border border-gray-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 bg-white shadow-sm hover:shadow-md w-full sm:w-auto"
      >
        <option value="month">Este Mês</option>
        <option value="quarter">Este Trimestre</option>
        <option value="year">Este Ano</option>
      </select>
      <button 
        @click="emit('refresh')"
        :disabled="isLoading"
        class="modern-button px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 text-xs sm:text-sm"
      >
        <svg v-if="isLoading" class="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span v-else>Atualizar</span>
      </button>
      <button 
        @click="emit('export')"
        :disabled="isLoading"
        class="modern-button px-3 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-all duration-200 transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-opacity-50 text-xs sm:text-sm"
      >
        Exportar BI
      </button>
    </div>
  </div>
</template>

<style scoped>
.modern-title {
  background: linear-gradient(135deg, #1e293b 0%, #475569 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.modern-select:focus {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}
</style>