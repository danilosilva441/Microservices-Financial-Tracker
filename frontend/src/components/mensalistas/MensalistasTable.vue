<script setup>
import { formatCurrency } from '@/utils/formatters';

defineProps({
  mensalistas: { type: Array, default: () => [] }
});

defineEmits(['edit', 'toggle-status']);
</script>

<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-hidden">
    <div class="overflow-x-auto">
      <table class="w-full text-sm text-left">
        <thead class="bg-slate-50 dark:bg-slate-700/50 text-slate-500 dark:text-slate-300 uppercase font-semibold">
          <tr>
            <th class="px-6 py-4">Nome</th>
            <th class="px-6 py-4">CPF</th>
            <th class="px-6 py-4 text-right">Valor Mensal</th>
            <th class="px-6 py-4 text-center">Status</th>
            <th class="px-6 py-4 text-right">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
          <tr v-for="item in mensalistas" :key="item.id" class="hover:bg-slate-50 dark:hover:bg-slate-700/30 transition-colors">
            <td class="px-6 py-4 font-medium text-slate-900 dark:text-white">
              {{ item.nome }}
            </td>
            <td class="px-6 py-4 text-slate-600 dark:text-slate-400">
              {{ item.cpf || '-' }}
            </td>
            <td class="px-6 py-4 text-right font-bold text-slate-700 dark:text-slate-200">
              {{ formatCurrency(item.valorMensalidade) }}
            </td>
            <td class="px-6 py-4 text-center">
              <span 
                class="px-2.5 py-1 rounded-full text-xs font-medium border"
                :class="item.isAtivo 
                  ? 'bg-green-50 text-green-700 border-green-200 dark:bg-green-900/30 dark:text-green-400 dark:border-green-800' 
                  : 'bg-gray-50 text-gray-600 border-gray-200 dark:bg-gray-700 dark:text-gray-400'"
              >
                {{ item.isAtivo ? 'Ativo' : 'Inativo' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right flex justify-end gap-3">
              <button 
                @click="$emit('edit', item)"
                class="text-blue-600 hover:text-blue-800 dark:text-blue-400 font-medium transition-colors"
              >
                Editar
              </button>
              <button 
                @click="$emit('toggle-status', item)"
                class="font-medium transition-colors"
                :class="item.isAtivo ? 'text-red-500 hover:text-red-700' : 'text-green-600 hover:text-green-800'"
              >
                {{ item.isAtivo ? 'Desativar' : 'Ativar' }}
              </button>
            </td>
          </tr>
          
          <tr v-if="mensalistas.length === 0">
            <td colspan="5" class="px-6 py-12 text-center text-slate-500 dark:text-slate-400">
              Nenhum mensalista cadastrado nesta unidade.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>