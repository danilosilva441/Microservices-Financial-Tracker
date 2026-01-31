<script setup>
import { ref } from 'vue';
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  dataToExport: {
    type: Object,
    default: () => ({})
  }
});

const emit = defineEmits(['close', 'export']);

const exportFormat = ref('json');
const includeAdvanced = ref(true);

const handleExport = () => {
  emit('export', {
    format: exportFormat.value,
    includeAdvanced: includeAdvanced.value
  });
};
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-semibold text-gray-900">Exportar Dados</h3>
        <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600">
          ✕
        </button>
      </div>
      
      <div class="space-y-4 mb-6">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Formato</label>
          <select v-model="exportFormat" class="w-full border border-gray-300 rounded-lg px-3 py-2">
            <option value="json">JSON</option>
            <option value="csv">CSV</option>
            <option value="excel">Excel</option>
          </select>
        </div>
        
        <div class="flex items-center">
          <input 
            type="checkbox" 
            v-model="includeAdvanced" 
            id="includeAdvanced"
            class="h-4 w-4 text-blue-600 rounded"
          />
          <label for="includeAdvanced" class="ml-2 text-sm text-gray-700">
            Incluir métricas avançadas de BI
          </label>
        </div>
        
        <div class="text-xs text-gray-500 bg-gray-50 p-3 rounded">
          <p>Serão exportados: KPIs principais, desempenho por unidade e dados de análise.</p>
        </div>
      </div>
      
      <div class="flex justify-end space-x-3">
        <button 
          @click="$emit('close')"
          class="px-4 py-2 text-gray-700 hover:text-gray-900"
        >
          Cancelar
        </button>
        <button 
          @click="handleExport"
          class="px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
        >
          Exportar
        </button>
      </div>
    </div>
  </div>
</template>