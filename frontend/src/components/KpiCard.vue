<script setup lang="ts">
import { computed } from 'vue';

// Definição das Interfaces para Tipagem Estrita
export interface KpiTrend {
  value: number;        // Ex: 12.5
  label: string;        // Ex: "vs mês anterior"
  direction: 'up' | 'down' | 'neutral'; // Para controlar a cor
}

export interface KpiCardProps {
  title: string;
  value: number; // Pode ser alterado para string se você já mandar formatado "R$ 1.000"
  trend?: KpiTrend;
}

// Definição das Props
const props = defineProps<KpiCardProps>();

// Definição dos Emits
const emit = defineEmits<{
  (e: 'click', payload: MouseEvent): void
}>();

// Lógica Computada para Estilos da Tendência
const trendClasses = computed(() => {
  if (!props.trend) return '';
  
  switch (props.trend.direction) {
    case 'up':
      return 'text-emerald-600 bg-emerald-50';
    case 'down':
      return 'text-rose-600 bg-rose-50';
    default:
      return 'text-gray-600 bg-gray-50';
  }
});

const trendIcon = computed(() => {
  if (!props.trend) return null;
  return props.trend.direction === 'up' ? '▲' : props.trend.direction === 'down' ? '▼' : '-';
});

// Handler de Clique
const handleClick = (event: MouseEvent) => {
  emit('click', event);
};
</script>

<template>
  <div 
    class="bg-white shadow rounded-lg p-6 border border-gray-100 cursor-pointer transition-all duration-200 hover:shadow-lg hover:-translate-y-1"
    @click="handleClick"
  >
    <div class="flex justify-between items-center mb-2">
      <h3 class="text-sm font-medium text-gray-500 uppercase tracking-wide truncate">
        {{ title }}
      </h3>
      <div v-if="$slots.icon" class="text-gray-400">
        <slot name="icon"></slot>
      </div>
    </div>

    <div class="text-3xl font-bold text-gray-900 mb-4">
      {{ value.toLocaleString('pt-BR') }}
    </div>

    <div class="flex items-center justify-between">
      
      <div v-if="trend" class="flex items-center text-sm">
        <span 
          :class="['px-2.5 py-0.5 rounded-full font-medium flex items-center gap-1', trendClasses]"
        >
          <span class="text-[10px]">{{ trendIcon }}</span>
          {{ trend.value }}%
        </span>
        <span class="ml-2 text-gray-400 text-xs">{{ trend.label }}</span>
      </div>

      <div v-if="$slots.default">
        <slot></slot>
      </div>
    </div>
  </div>
</template>