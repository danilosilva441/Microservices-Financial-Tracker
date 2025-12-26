<script setup>
import { computed } from 'vue';
import { formatCurrency } from '@/utils/formatters';
import { useFechamentoCaixa } from '@/composables/faturamento/useFechamentoCaixa';

// Componentes
import FaturamentoForm from '@/components/FaturamentoForm.vue';
import FechamentoModal from '@/components/faturamento/FechamentoModal.vue';
import TabelaHistoricoCaixa from '@/components/faturamento/TabelaHistoricoCaixa.vue';

const props = defineProps({
  unidade: { type: Object, required: true }
});

const emit = defineEmits(['add', 'delete']);

// Inicializa a lógica de fechamento
const { 
  fechamentos, faturamentosHoje, resumoHoje, isLoading, isModalOpen, 
  realizarFechamento 
} = useFechamentoCaixa(props.unidade.id);

// Ordena os lançamentos de hoje
const lancamentosOrdenados = computed(() => {
  return [...faturamentosHoje.value].sort((a, b) => new Date(b.horaInicio || b.data) - new Date(a.horaInicio || a.data));
});
</script>

<template>
  <div class="space-y-8 animate-fade-in">
    
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-blue-100 dark:border-blue-900 overflow-hidden">
      <div class="p-6 border-b border-slate-100 dark:border-slate-700 flex flex-col md:flex-row justify-between items-center gap-4">
        <div>
          <h3 class="text-xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
            <span class="text-blue-600">📅</span> Caixa de Hoje
          </h3>
          <p class="text-sm text-slate-500 dark:text-slate-400">
            {{ new Date().toLocaleDateString() }} - Lançamentos em aberto
          </p>
        </div>
        
        <div class="flex items-center gap-4 bg-slate-50 dark:bg-slate-700/50 px-4 py-2 rounded-lg">
          <div class="text-right">
            <p class="text-xs text-slate-500 uppercase">Total Parcial</p>
            <p class="text-xl font-bold text-slate-800 dark:text-white">{{ formatCurrency(resumoHoje.total) }}</p>
          </div>
          <button 
            @click="isModalOpen = true"
            :disabled="faturamentosHoje.length === 0"
            class="bg-green-600 hover:bg-green-700 text-white px-5 py-2.5 rounded-lg font-medium shadow-md transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Fechar Caixa
          </button>
        </div>
      </div>

      <div class="p-6 bg-slate-50 dark:bg-slate-700/20 border-b border-slate-100 dark:border-slate-700">
        <h4 class="text-sm font-bold text-slate-700 dark:text-slate-300 mb-3">Novo Lançamento</h4>
        <FaturamentoForm @submit="(data) => $emit('add', data)" />
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead class="bg-white dark:bg-slate-800 border-b dark:border-slate-700">
            <tr>
              <th class="px-6 py-3 text-slate-500">Hora</th>
              <th class="px-6 py-3 text-slate-500">Origem</th>
              <th class="px-6 py-3 text-slate-500 text-right">Valor</th>
              <th class="px-6 py-3 text-right">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100 dark:divide-slate-700">
            <tr v-for="item in lancamentosOrdenados" :key="item.id" class="hover:bg-slate-50 dark:hover:bg-slate-700/30">
              <td class="px-6 py-3 text-slate-700 dark:text-slate-300">
                {{ item.horaInicio ? new Date(item.horaInicio).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) : '-' }}
              </td>
              <td class="px-6 py-3">
                <span class="px-2 py-1 bg-blue-50 text-blue-700 text-xs rounded-md border border-blue-100 dark:bg-blue-900/30 dark:text-blue-300 dark:border-blue-800">
                  {{ item.origem }}
                </span>
              </td>
              <td class="px-6 py-3 text-right font-medium text-slate-900 dark:text-white">
                {{ formatCurrency(item.valor) }}
              </td>
              <td class="px-6 py-3 text-right">
                <button @click="$emit('delete', item.id)" class="text-red-500 hover:text-red-700 text-xs font-bold uppercase">
                  Excluir
                </button>
              </td>
            </tr>
            <tr v-if="lancamentosOrdenados.length === 0">
              <td colspan="4" class="px-6 py-8 text-center text-slate-400 italic">
                Nenhum movimento registrado hoje.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <TabelaHistoricoCaixa :historico="fechamentos" />

    <FechamentoModal 
      :is-visible="isModalOpen" 
      :resumo="resumoHoje"
      :is-loading="isLoading"
      @close="isModalOpen = false"
      @confirm="realizarFechamento"
    />
  </div>
</template>