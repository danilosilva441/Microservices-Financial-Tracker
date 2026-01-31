<script setup>
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  kpis: {
    type: Object,
    default: () => ({})
  },
  benchmark: {
    type: Object,
    default: () => ({})
  }
});

const getTrendIcon = (value) => {
  if (value > 0) return '↗️';
  if (value < 0) return '↘️';
  return '➡️';
};

const getTrendColor = (value) => {
  if (value > 0) return 'text-green-600';
  if (value < 0) return 'text-red-600';
  return 'text-gray-600';
};
</script>

<template>
  <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
    <!-- Benchmark de Receita -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
      <div class="flex items-center justify-between mb-2">
        <h4 class="text-sm font-semibold text-gray-700">Receita vs Benchmark</h4>
        <span :class="['text-xs font-medium', getTrendColor(kpis.benchmark?.variacaoReceita || 0)]">
          {{ getTrendIcon(kpis.benchmark?.variacaoReceita || 0) }}
        </span>
      </div>
      <p class="text-2xl font-bold text-gray-900">
        {{ (kpis.benchmark?.variacaoReceita || 0).toFixed(1) }}%
      </p>
      <p class="text-xs text-gray-500 mt-1">Variação em relação ao período anterior</p>
    </div>

    <!-- Benchmark de Lucro -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
      <div class="flex items-center justify-between mb-2">
        <h4 class="text-sm font-semibold text-gray-700">Lucro vs Benchmark</h4>
        <span :class="['text-xs font-medium', getTrendColor(kpis.benchmark?.variacaoLucro || 0)]">
          {{ getTrendIcon(kpis.benchmark?.variacaoLucro || 0) }}
        </span>
      </div>
      <p class="text-2xl font-bold text-gray-900">
        {{ (kpis.benchmark?.variacaoLucro || 0).toFixed(1) }}%
      </p>
      <p class="text-xs text-gray-500 mt-1">Eficiência operacional</p>
    </div>

    <!-- Benchmark de Ticket Médio -->
    <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-200">
      <div class="flex items-center justify-between mb-2">
        <h4 class="text-sm font-semibold text-gray-700">Ticket Médio</h4>
        <span :class="['text-xs font-medium', getTrendColor(kpis.benchmark?.variacaoTicket || 0)]">
          {{ getTrendIcon(kpis.benchmark?.variacaoTicket || 0) }}
        </span>
      </div>
      <p class="text-2xl font-bold text-gray-900">
        {{ formatCurrency(kpis.analise?.ticketMedio || 0) }}
      </p>
      <p class="text-xs text-gray-500 mt-1">
        {{ (kpis.benchmark?.variacaoTicket || 0).toFixed(1) }}% vs anterior
      </p>
    </div>
  </div>
</template>