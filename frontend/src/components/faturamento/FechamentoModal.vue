<script setup>
import { ref, computed } from 'vue';
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  isVisible: Boolean,
  resumo: { type: Object, required: true }, // Total calculado pelo sistema
  isLoading: Boolean
});

const emit = defineEmits(['close', 'confirm']);

const form = ref({
  fundoDeCaixa: '', // Dinheiro que sobra para o dia seguinte
  observacoes: ''
});

// Diferença (Apenas visual, se tivéssemos o valor em dinheiro separado)
const totalEsperado = computed(() => props.resumo.total);

const handleSubmit = () => {
  emit('confirm', {
    fundoDeCaixa: parseFloat(form.value.fundoDeCaixa || 0),
    observacoes: form.value.observacoes
  });
};
</script>

<template>
  <div v-if="isVisible" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
    <div class="bg-white dark:bg-slate-800 w-full max-w-md rounded-xl shadow-2xl overflow-hidden animate-fade-in">
      
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white">Fechar Caixa do Dia</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-red-500">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-5">
        
        <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg border border-blue-100 dark:border-blue-800">
          <p class="text-sm text-blue-600 dark:text-blue-300 font-medium mb-1">Total Lançado Hoje</p>
          <p class="text-2xl font-bold text-blue-800 dark:text-blue-100">{{ formatCurrency(totalEsperado) }}</p>
          <p class="text-xs text-blue-500 mt-1">Este valor será consolidado ao confirmar.</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            Fundo de Caixa (Troco Final)
          </label>
          <div class="relative">
            <span class="absolute left-3 top-2.5 text-slate-400">R$</span>
            <input 
              v-model="form.fundoDeCaixa" 
              type="number" step="0.01" min="0"
              class="w-full pl-10 pr-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-green-500 outline-none"
              placeholder="0,00"
              required 
            />
          </div>
          <p class="text-xs text-slate-500 mt-1">Valor em dinheiro que ficará na gaveta para amanhã.</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Observações</label>
          <textarea 
            v-model="form.observacoes" 
            rows="3"
            class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none resize-none"
            placeholder="Alguma divergência ou nota importante..."
          ></textarea>
        </div>

        <div class="flex gap-3 pt-2">
          <button 
            type="button" 
            @click="$emit('close')"
            class="flex-1 px-4 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700"
          >
            Cancelar
          </button>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="flex-1 px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium flex justify-center items-center gap-2"
          >
            <span v-if="isLoading">Processando...</span>
            <span v-else>Confirmar Fechamento</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>