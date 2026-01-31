<script setup>
import { formatCurrency } from '@/utils/formatters';

defineProps({
  historico: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['ver-detalhes']);

const formatarData = (dataString) => {
  if (!dataString) return '-';
  return new Date(dataString).toLocaleDateString('pt-BR');
};

const verDetalhes = (fechamentoId) => {
  emit('ver-detalhes', fechamentoId);
};
</script>

<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-blue-100 dark:border-blue-900 overflow-hidden">
    <div class="p-6 border-b border-slate-100 dark:border-slate-700">
      <h3 class="text-xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
        <span class="text-purple-600">📜</span> Histórico de Fechamentos
      </h3>
      <p class="text-sm text-slate-500 dark:text-slate-400">
        Registro de todos os fechamentos realizados
      </p>
    </div>

    <div class="overflow-x-auto">
      <table class="w-full text-sm text-left">
        <thead class="bg-white dark:bg-slate-800 border-b dark:border-slate-700">
          <tr>
            <th class="px-6 py-3 text-slate-500">DATA FECHAMENTO</th>
            <th class="px-6 py-3 text-slate-500">RESPONSÁVEL</th>
            <th class="px-6 py-3 text-slate-500">VALOR TOTAL</th>
            <th class="px-6 py-3 text-slate-500">STATUS</th>
            <th class="px-6 py-3 text-right text-slate-500">AÇÕES</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100 dark:divide-slate-700">
          <tr 
            v-for="item in historico" 
            :key="item.id"
            class="hover:bg-slate-50 dark:hover:bg-slate-700/30"
          >
            <td class="px-6 py-3 text-slate-700 dark:text-slate-300 font-medium">
              {{ formatarData(item.dataFechamento || item.data) }}
            </td>
            <td class="px-6 py-3 text-slate-600 dark:text-slate-400">
              {{ item.responsavel || 'Admin' }}
            </td>
            <td class="px-6 py-3 font-medium text-slate-900 dark:text-white">
              {{ formatCurrency(item.valorTotal || 0) }}
            </td>
            <td class="px-6 py-3">
              <span 
                :class="[
                  'px-2 py-1 text-xs font-medium rounded-full',
                  item.status === 'enviado' 
                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300' 
                    : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300'
                ]"
              >
                {{ item.status === 'enviado' ? '✅ Enviado' : '⏳ Pendente' }}
              </span>
            </td>
            <td class="px-6 py-3 text-right">
              <button 
                @click="verDetalhes(item.id)"
                class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 text-sm font-medium flex items-center gap-1 ml-auto"
              >
                <span>🔍</span> Ver Detalhes
              </button>
            </td>
          </tr>
          <tr v-if="historico.length === 0">
            <td colspan="5" class="px-6 py-8 text-center text-slate-500">
              Nenhum fechamento registrado no histórico.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>