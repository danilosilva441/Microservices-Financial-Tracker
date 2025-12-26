<script setup>
import { ref, computed } from 'vue';
import { formatCurrency } from '@/utils/formatters';
import FaturamentoForm from '@/components/FaturamentoForm.vue'; // Reutilizando seu componente existente

const props = defineProps({
  faturamentos: { type: Array, default: () => [] }
});

const emit = defineEmits(['add', 'delete']);
const showForm = ref(false);

const sortedFaturamentos = computed(() => {
  // Cria uma cópia para não mutar a prop diretamente
  return [...props.faturamentos].sort((a, b) => new Date(b.data) - new Date(a.data));
});

const handleAdd = (data) => {
  emit('add', data);
  showForm.value = false;
};
</script>

<template>
  <div class="space-y-6 animate-fade-in">
    <div class="flex justify-between items-center">
      <h3 class="text-lg font-bold text-slate-800 dark:text-white">Histórico de Lançamentos</h3>
      <button 
        @click="showForm = !showForm"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors shadow-sm"
      >
        {{ showForm ? 'Cancelar' : 'Novo Lançamento' }}
      </button>
    </div>

    <div v-if="showForm" class="bg-slate-50 dark:bg-slate-700/30 p-4 rounded-xl border border-blue-100 dark:border-slate-600 mb-6">
      <h4 class="text-sm font-bold text-slate-700 dark:text-slate-200 mb-4">Adicionar Faturamento</h4>
      <FaturamentoForm @submit="handleAdd" />
    </div>

    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden">
      <table class="w-full">
        <thead class="bg-slate-50 dark:bg-slate-700/50 border-b dark:border-slate-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-bold text-slate-500 dark:text-slate-300 uppercase tracking-wider">Data</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-slate-500 dark:text-slate-300 uppercase tracking-wider">Valor</th>
            <th class="px-6 py-3 text-left text-xs font-bold text-slate-500 dark:text-slate-300 uppercase tracking-wider">Origem</th>
            <th class="px-6 py-3 text-right text-xs font-bold text-slate-500 dark:text-slate-300 uppercase tracking-wider">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
          <tr v-for="fat in sortedFaturamentos" :key="fat.id" class="hover:bg-slate-50 dark:hover:bg-slate-700/30 transition-colors">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-slate-200">
              {{ new Date(fat.data).toLocaleDateString() }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-green-600 dark:text-green-400">
              {{ formatCurrency(fat.valor) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
              <span class="px-2 py-1 bg-blue-50 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300 rounded-full text-xs font-medium border border-blue-100 dark:border-blue-800">
                {{ fat.origem }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button @click="$emit('delete', fat.id)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300 transition-colors">
                Excluir
              </button>
            </td>
          </tr>
          <tr v-if="sortedFaturamentos.length === 0">
            <td colspan="4" class="px-6 py-12 text-center text-slate-500 dark:text-slate-400">
              Nenhum faturamento registrado.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>