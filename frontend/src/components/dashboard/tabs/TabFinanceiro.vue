<script setup>
import { computed } from 'vue';
import { useDashboardStore } from '@/stores/dashboardStore';
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  type: { type: String, required: true }, // 'receita', 'despesa', 'lucro'
  title: String,
  color: String // 'green', 'red', 'blue'
});

const store = useDashboardStore();

// Computa a lista ordenada baseada no tipo escolhido
const ranking = computed(() => {
  const lista = [...store.desempenho.todas];
  return lista.sort((a, b) => b[props.type] - a[props.type]);
});

const totalValue = computed(() => {
  if (props.type === 'receita') return store.kpis.receitaTotal;
  if (props.type === 'despesa') return store.kpis.despesaTotal;
  return store.kpis.lucroTotal;
});

const getBgColor = (val) => {
  if (props.color === 'green') return 'bg-green-100 text-green-800';
  if (props.color === 'red') return 'bg-red-100 text-red-800';
  return val >= 0 ? 'bg-blue-100 text-blue-800' : 'bg-red-100 text-red-800';
};
</script>

<template>
  <div class="animate-fade-in">
    <div class="flex items-center justify-between mb-6">
      <h2 class="text-lg font-bold text-gray-800">{{ title }}</h2>
      <div class="text-right">
        <span class="text-sm text-gray-500">Total Consolidado</span>
        <p class="text-2xl font-bold" :class="`text-${color}-600`">
          {{ formatCurrency(totalValue) }}
        </p>
      </div>
    </div>

    <div class="overflow-x-auto border rounded-lg">
      <table class="w-full text-sm text-left text-gray-500">
        <thead class="text-xs text-gray-700 uppercase bg-gray-50 border-b">
          <tr>
            <th class="px-6 py-3">Rank</th>
            <th class="px-6 py-3">Unidade</th>
            <th class="px-6 py-3 text-right">Valor ({{ type }})</th>
            <th class="px-6 py-3 text-right" v-if="type === 'receita'">Meta</th>
            <th class="px-6 py-3 text-right" v-if="type === 'receita'">% Atingido</th>
            <th class="px-6 py-3 text-center">Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(unidade, index) in ranking" :key="unidade.id" class="bg-white border-b hover:bg-gray-50">
            <td class="px-6 py-4 font-medium">{{ index + 1 }}º</td>
            <td class="px-6 py-4 font-medium text-gray-900">{{ unidade.nome }}</td>
            
            <td class="px-6 py-4 text-right font-bold">
              {{ formatCurrency(unidade[type]) }}
            </td>

            <td class="px-6 py-4 text-right text-gray-400" v-if="type === 'receita'">
              {{ formatCurrency(unidade.metaMensal) }}
            </td>

            <td class="px-6 py-4 text-right" v-if="type === 'receita'">
              <div class="w-full bg-gray-200 rounded-full h-2.5 dark:bg-gray-700">
                <div class="h-2.5 rounded-full" 
                     :class="unidade.percentualMeta >= 100 ? 'bg-green-600' : 'bg-yellow-400'"
                     :style="{ width: Math.min(unidade.percentualMeta, 100) + '%' }"></div>
              </div>
              <span class="text-xs">{{ unidade.percentualMeta }}%</span>
            </td>

            <td class="px-6 py-4 text-center">
              <span class="px-2 py-1 rounded text-xs font-semibold" :class="getBgColor(unidade.lucro)">
                {{ unidade.status === 'lucro' ? 'Lucrativa' : 'Prejuízo' }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>