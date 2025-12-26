<script setup>
import { ref } from 'vue';
import { useTheme } from '@/composables/useTheme';

const props = defineProps({
  title: String,
  isLoading: Boolean
});

const emit = defineEmits(['filter-change', 'refresh', 'open-export']);
const periodo = ref('month');
const { isDark, toggleTheme } = useTheme(); // Hook do tema

const updateFilter = () => {
  emit('filter-change', { periodo: periodo.value });
};
</script>

<template>
  <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-6">
    <div>
      <h1 class="text-2xl font-bold text-slate-800 dark:text-white transition-colors">{{ title }}</h1>
      <p class="text-sm text-slate-500 dark:text-slate-400">Visão consolidada das unidades</p>
    </div>

    <div class="flex items-center gap-3">
      
      <button 
        @click="toggleTheme"
        class="p-2.5 rounded-lg bg-white dark:bg-slate-700 border border-slate-200 dark:border-slate-600 text-slate-600 dark:text-yellow-400 hover:shadow-md transition-all"
        title="Alternar Tema"
      >
        <span v-if="isDark">☀️</span>
        <span v-else>🌙</span>
      </button>

      <button 
        @click="$emit('open-export')"
        class="p-2.5 rounded-lg bg-white dark:bg-slate-700 border border-slate-200 dark:border-slate-600 text-slate-600 dark:text-slate-200 hover:shadow-md transition-all flex items-center gap-2"
      >
        <span>📥</span>
        <span class="hidden sm:inline text-sm font-medium">Exportar</span>
      </button>

      <select 
        v-model="periodo" 
        @change="updateFilter"
        class="bg-white dark:bg-slate-700 border border-slate-200 dark:border-slate-600 text-slate-700 dark:text-white text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block p-2.5"
      >
        <option value="month">Este Mês</option>
        <option value="last_month">Mês Passado</option>
        <option value="year">Este Ano</option>
      </select>

      <button 
        @click="$emit('refresh')"
        :disabled="isLoading"
        class="text-white bg-blue-600 hover:bg-blue-700 focus:ring-4 focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 transition-colors disabled:opacity-50"
      >
        <span v-if="isLoading">...</span>
        <span v-else>↻</span>
      </button>
    </div>
  </div>
</template>