<script setup>
const props = defineProps({
  desempenho: {
    type: Object,
    default: () => ({ top: [], bottom: [] })
  }
});
</script>

<template>
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-3 sm:gap-4 lg:gap-6">
    <!-- Melhores Desempenhos -->
    <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
      <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">
        <span class="truncate">Melhores Desempenhos</span>
      </h3>
      <div class="space-y-2 sm:space-y-3">
        <div v-for="(op, index) in desempenho.top || []" :key="op.id" 
             class="modern-list-item flex items-center justify-between p-2 sm:p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-all duration-200">
          <div class="flex items-center space-x-2 sm:space-x-3 flex-1 min-w-0">
            <span class="rank-indicator flex-shrink-0 w-6 h-6 bg-green-500 text-white rounded-full flex items-center justify-center text-xs font-bold">
              {{ index + 1 }}
            </span>
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm font-medium text-gray-800 truncate">{{ op.nome || 'N/A' }}</p>
              <p class="text-xs text-gray-500 truncate">Proj: {{ (op.percentualProjetado || 0).toFixed(1) }}%</p>
            </div>
          </div>
          <span class="flex-shrink-0 text-sm sm:text-base font-bold text-green-500 ml-2">
            {{ (op.percentualAtingido || 0).toFixed(1) }}%
          </span>
        </div>
      </div>
    </div>

    <!-- Necessitam de Atenção -->
    <div class="modern-card bg-white p-3 sm:p-4 lg:p-5 rounded-xl shadow-card hover:shadow-lg transition-all duration-300">
      <h3 class="text-sm sm:text-base lg:text-lg font-semibold text-gray-800 mb-3 sm:mb-4">
        <span class="truncate">Necessitam de Atenção</span>
      </h3>
      <div class="space-y-2 sm:space-y-3">
        <div v-for="op in desempenho.bottom || []" :key="op.id" 
             class="modern-list-item flex items-center justify-between p-2 sm:p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-all duration-200">
          <div class="flex items-center space-x-2 sm:space-x-3 flex-1 min-w-0">
            <span class="warning-indicator flex-shrink-0 text-red-500 text-lg">
              ⚠️
            </span>
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm font-medium text-gray-800 truncate">{{ op.nome || 'N/A' }}</p>
              <p class="text-xs text-gray-500 truncate">Proj: {{ (op.percentualProjetado || 0).toFixed(1) }}%</p>
            </div>
          </div>
          <span class="flex-shrink-0 text-sm sm:text-base font-bold text-red-500 ml-2">
            {{ (op.percentualAtingido || 0).toFixed(1) }}%
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.shadow-card {
  box-shadow: 
    0 1px 3px 0 rgba(0, 0, 0, 0.1),
    0 1px 2px 0 rgba(0, 0, 0, 0.06),
    0 0 0 1px rgba(0, 0, 0, 0.02);
}

.modern-list-item {
  transition: all 0.2s ease;
}

.modern-list-item:hover {
  transform: translateX(2px);
}

.rank-indicator {
  transition: all 0.3s ease;
}

.rank-indicator:hover {
  transform: scale(1.1);
}

.truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.min-w-0 {
  min-width: 0;
}
</style>