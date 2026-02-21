// src/composables/analysis/useAnalysisExport.js

import { storeToRefs } from 'pinia';
import { computed } from 'vue';
import { useAnalysisStore } from '@/stores/analysis.store';

export function useAnalysisFilters() {
  const store = useAnalysisStore();
  const { filters } = storeToRefs(store);
  
  // Opções de período
  const periodoOptions = [
    { value: 'mes_atual', label: 'Mês Atual' },
    { value: 'mes_anterior', label: 'Mês Anterior' },
    { value: 'trimestre', label: 'Trimestre' },
    { value: 'ano', label: 'Ano' },
  ];
  
  // Opções de tipo de gráfico
  const tipoGraficoOptions = [
    { value: 'barra', label: 'Barra' },
    { value: 'pizza', label: 'Pizza' },
    { value: 'linha', label: 'Linha' },
  ];
  
  // Filtros ativos
  const activeFiltersCount = computed(() => {
    let count = 0;
    if (filters.value.startDate) count++;
    if (filters.value.endDate) count++;
    if (filters.value.unidadeId) count++;
    return count;
  });
  
  // Descrição dos filtros para exibição
  const filtersDescription = computed(() => {
    const desc = [];
    if (filters.value.periodo) {
      const periodo = periodoOptions.find(p => p.value === filters.value.periodo);
      if (periodo) desc.push(periodo.label);
    }
    if (filters.value.unidadeId) {
      desc.push('Unidade específica');
    }
    return desc.join(' • ') || 'Todos os filtros';
  });
  
  // Range de datas formatado
  const dateRangeFormatted = computed(() => {
    if (!filters.value.startDate || !filters.value.endDate) return null;
    
    const start = new Date(filters.value.startDate).toLocaleDateString('pt-BR');
    const end = new Date(filters.value.endDate).toLocaleDateString('pt-BR');
    return `${start} - ${end}`;
  });
  
  return {
    // Estado
    filters,
    
    // Opções
    periodoOptions,
    tipoGraficoOptions,
    
    // Computados
    activeFiltersCount,
    filtersDescription,
    dateRangeFormatted,
    
    // Ações
    updateFilters: store.updateFilters,
    resetFilters: store.resetFilters,
  };
}