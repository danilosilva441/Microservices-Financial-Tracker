<script setup>
import { ref } from 'vue';

const props = defineProps({
  isVisible: Boolean,
  isLoading: Boolean,
  categorias: { type: Array, default: () => [] }
});

const emit = defineEmits(['close', 'save', 'create-category']);

// Estado do Formulário
const form = ref({
  description: '',
  amount: '',
  date: new Date().toISOString().split('T')[0], // Hoje
  categoryId: '',
  file: null
});

// Referência para o input de arquivo
const fileInput = ref(null);

const handleFileChange = (event) => {
  const file = event.target.files[0];
  if (file) {
    form.value.file = file;
  }
};

const handleSubmit = () => {
  if (!form.value.description || !form.value.amount || !form.value.categoryId) {
    alert('Preencha os campos obrigatórios.');
    return;
  }
  emit('save', { ...form.value });
};

const handleNewCategory = () => {
  const name = prompt('Nome da nova categoria:');
  if (name) {
    emit('create-category', name);
  }
};
</script>

<template>
  <div v-if="isVisible" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
    <div class="bg-white dark:bg-slate-800 w-full max-w-lg rounded-xl shadow-2xl overflow-hidden animate-fade-in">
      
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white">Nova Despesa</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Valor (R$)*</label>
            <input 
              v-model="form.amount" 
              type="number" step="0.01" min="0" 
              class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-red-500 outline-none"
              required 
              placeholder="0,00"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Data*</label>
            <input 
              v-model="form.date" 
              type="date" 
              class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-red-500 outline-none"
              required 
            />
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Descrição*</label>
          <input 
            v-model="form.description" 
            type="text" 
            class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-red-500 outline-none"
            placeholder="Ex: Compra de material de limpeza"
            required 
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Categoria*</label>
          <div class="flex gap-2">
            <select 
              v-model="form.categoryId" 
              class="flex-1 px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-red-500 outline-none"
              required
            >
              <option value="" disabled>Selecione...</option>
              <option v-for="cat in categorias" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
            
            <button 
              type="button"
              @click="handleNewCategory"
              class="px-3 py-2 bg-slate-100 dark:bg-slate-700 rounded-lg text-slate-600 dark:text-slate-300 hover:bg-slate-200 transition-colors"
              title="Nova Categoria"
            >
              +
            </button>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Comprovante</label>
          <div class="flex items-center justify-center w-full">
            <label class="flex flex-col items-center justify-center w-full h-24 border-2 border-slate-300 border-dashed rounded-lg cursor-pointer bg-slate-50 dark:bg-slate-700 hover:bg-slate-100 dark:border-slate-600 dark:hover:border-slate-500 transition-colors">
              <div class="flex flex-col items-center justify-center pt-5 pb-6">
                <p class="text-sm text-slate-500 dark:text-slate-400">
                  <span v-if="form.file" class="font-semibold text-green-600">{{ form.file.name }}</span>
                  <span v-else>Clique para fazer upload (Imagem ou PDF)</span>
                </p>
              </div>
              <input type="file" class="hidden" @change="handleFileChange" accept="image/*,application/pdf" />
            </label>
          </div>
        </div>

        <div class="pt-4 flex gap-3">
          <button 
            type="button" 
            @click="$emit('close')"
            class="flex-1 px-4 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
          >
            Cancelar
          </button>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-70 flex justify-center items-center transition-colors"
          >
            <span v-if="isLoading" class="animate-spin h-5 w-5 border-2 border-white border-t-transparent rounded-full mr-2"></span>
            Salvar Despesa
          </button>
        </div>

      </form>
    </div>
  </div>
</template>