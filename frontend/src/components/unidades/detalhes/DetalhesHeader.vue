<script setup>
import { useRouter } from 'vue-router';

const props = defineProps({
  unidade: { type: Object, required: true },
  isAdmin: Boolean
});

const emit = defineEmits(['delete']);
const router = useRouter();

const goToDespesas = () => {
  router.push({ name: 'unidade-despesas', params: { id: props.unidade.id } });
};
</script>

<template>
  <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-4 mb-6 bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
    
    <div class="flex items-center gap-3">
      <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
        <span class="text-2xl">🏢</span>
      </div>
      <div>
        <h1 class="text-xl sm:text-2xl font-bold text-slate-800 dark:text-white">
          {{ unidade.nome }}
        </h1>
        <p class="text-slate-500 dark:text-slate-400 text-sm">
          {{ unidade.descricao || 'Sem descrição.' }}
        </p>
      </div>
    </div>
    
    <div class="flex flex-wrap gap-2 sm:gap-3 w-full xl:w-auto">
      
      <button 
        @click="goToDespesas"
        class="flex-1 sm:flex-none flex items-center justify-center gap-2 bg-white hover:bg-slate-50 dark:bg-slate-700 dark:hover:bg-slate-600 text-slate-700 dark:text-white border border-slate-300 dark:border-slate-500 px-4 py-2 rounded-lg transition-colors text-sm font-medium shadow-sm"
      >
        <span>💸</span>
        Despesas
      </button>

      <button 
        @click="router.push({ name: 'unidades' })"
        class="flex-1 sm:flex-none bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:hover:bg-slate-600 text-slate-700 dark:text-white px-4 py-2 rounded-lg transition-colors text-sm font-medium"
      >
        Voltar
      </button>

      <button 
        v-if="isAdmin"
        @click="$emit('delete')"
        class="flex-1 sm:flex-none bg-red-50 hover:bg-red-100 dark:bg-red-900/20 dark:hover:bg-red-900/40 text-red-600 dark:text-red-400 px-4 py-2 rounded-lg transition-colors text-sm font-medium border border-red-200 dark:border-red-800"
      >
        Excluir
      </button>
    </div>
  </div>
</template>