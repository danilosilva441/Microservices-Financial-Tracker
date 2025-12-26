<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  isVisible: Boolean,
  isLoading: Boolean,
  mensalistaParaEditar: Object
});

const emit = defineEmits(['close', 'save']);

const form = ref({
  nome: '',
  cpf: '',
  valorMensalidade: ''
});

// Preenche o formulário se for edição
watch(() => props.mensalistaParaEditar, (novoValor) => {
  if (novoValor) {
    form.value = { ...novoValor };
  } else {
    form.value = { nome: '', cpf: '', valorMensalidade: '' };
  }
}, { immediate: true });

const handleSubmit = () => {
  if (!form.value.nome || !form.value.valorMensalidade) {
    alert('Nome e Valor são obrigatórios.');
    return;
  }
  
  // Converter para número e emitir
  const payload = {
    ...form.value,
    valorMensalidade: parseFloat(form.value.valorMensalidade)
  };
  
  emit('save', payload);
};
</script>

<template>
  <div v-if="isVisible" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
    <div class="bg-white dark:bg-slate-800 w-full max-w-md rounded-xl shadow-2xl overflow-hidden animate-fade-in">
      
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white">
          {{ mensalistaParaEditar ? 'Editar Mensalista' : 'Novo Mensalista' }}
        </h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Nome Completo*</label>
          <input 
            v-model="form.nome" 
            type="text" 
            class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
            placeholder="Ex: João da Silva"
            required 
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">CPF (Opcional)</label>
          <input 
            v-model="form.cpf" 
            type="text" 
            class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
            placeholder="000.000.000-00"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Valor Mensal (R$)*</label>
          <input 
            v-model="form.valorMensalidade" 
            type="number" 
            step="0.01" 
            min="0"
            class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
            placeholder="0,00"
            required
          />
        </div>

        <div class="pt-4 flex gap-3">
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
            class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-70 flex justify-center items-center"
          >
            <span v-if="isLoading" class="animate-spin h-5 w-5 border-2 border-white border-t-transparent rounded-full mr-2"></span>
            Salvar
          </button>
        </div>
      </form>
    </div>
  </div>
</template>