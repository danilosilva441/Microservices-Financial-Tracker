<script setup>
import { useExport } from '@/composables/useExport';

const props = defineProps({
  isOpen: Boolean,
  dataToExport: Object // { kpis, tableData }
});

const emit = defineEmits(['close']);
const { exportToCSV, exportToExcel, exportToPDF } = useExport();

const handleExport = (type) => {
  const { kpis, tableData } = props.dataToExport;
  
  if (type === 'csv') exportToCSV(tableData, 'dashboard_data');
  if (type === 'excel') exportToExcel(tableData, 'dashboard_data');
  if (type === 'pdf') exportToPDF(kpis, tableData, 'dashboard_report');
  
  emit('close');
};
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
    <div class="bg-white dark:bg-slate-800 rounded-xl shadow-2xl w-full max-w-md overflow-hidden transform transition-all">
      
      <div class="px-6 py-4 border-b border-slate-100 dark:border-slate-700 flex justify-between items-center">
        <h3 class="text-lg font-bold text-slate-800 dark:text-white">Exportar Relatório</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">
          ✕
        </button>
      </div>

      <div class="p-6 space-y-3">
        <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">
          Escolha o formato para baixar os dados atuais do dashboard (KPIs + Tabela).
        </p>

        <button @click="handleExport('excel')" 
          class="w-full flex items-center justify-center gap-3 p-3 rounded-lg border border-slate-200 hover:bg-green-50 hover:border-green-200 dark:border-slate-600 dark:hover:bg-green-900/20 transition-all group">
          <span class="text-xl">📊</span>
          <span class="font-medium text-slate-700 dark:text-slate-200 group-hover:text-green-700 dark:group-hover:text-green-400">Excel (.xlsx)</span>
        </button>

        <button @click="handleExport('csv')" 
          class="w-full flex items-center justify-center gap-3 p-3 rounded-lg border border-slate-200 hover:bg-blue-50 hover:border-blue-200 dark:border-slate-600 dark:hover:bg-blue-900/20 transition-all group">
          <span class="text-xl">📝</span>
          <span class="font-medium text-slate-700 dark:text-slate-200 group-hover:text-blue-700 dark:group-hover:text-blue-400">CSV (Texto)</span>
        </button>

        <button @click="handleExport('pdf')" 
          class="w-full flex items-center justify-center gap-3 p-3 rounded-lg border border-slate-200 hover:bg-red-50 hover:border-red-200 dark:border-slate-600 dark:hover:bg-red-900/20 transition-all group">
          <span class="text-xl">📄</span>
          <span class="font-medium text-slate-700 dark:text-slate-200 group-hover:text-red-700 dark:group-hover:text-red-400">PDF (Relatório)</span>
        </button>
      </div>
    </div>
  </div>
</template>