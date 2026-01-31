<script setup>
import ChartsContainer from '@/components/dashboard/ChartsContainer.vue';

const props = defineProps({
  graficos: {
    type: Object,
    default: () => ({})
  },
  kpis: {
    type: Object,
    default: () => ({})
  }
});
</script>

<template>
  <div class="tab-content">
    <!-- Gráficos -->
    <ChartsContainer :graficos="graficos" :kpis="kpis" />
    
    <!-- Resumo de Indicadores -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <div class="bg-blue-50 p-4 rounded-lg">
        <h4 class="text-sm font-semibold text-blue-700 mb-2">Desempenho Geral</h4>
        <p class="text-2xl font-bold text-blue-600">{{ kpis.percentualMeta?.toFixed(1) || 0 }}%</p>
        <p class="text-xs text-blue-600">Meta total atingida</p>
      </div>
      
      <div class="bg-green-50 p-4 rounded-lg">
        <h4 class="text-sm font-semibold text-green-700 mb-2">Lucratividade</h4>
        <p class="text-2xl font-bold text-green-600">{{ kpis.lucroTotal?.toFixed(1) || 0 }}%</p>
        <p class="text-xs text-green-600">Margem de lucro</p>
      </div>
      
      <div class="bg-orange-50 p-4 rounded-lg">
        <h4 class="text-sm font-semibold text-orange-700 mb-2">Eficiência</h4>
        <p class="text-2xl font-bold text-orange-600">{{ kpis.analise?.estabilidade || 'N/A' }}</p>
        <p class="text-xs text-orange-600">Estabilidade operacional</p>
      </div>
    </div>
  </div>
</template>