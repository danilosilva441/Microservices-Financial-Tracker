// composables/unidades/useUnidadesFilters.js
import { useUnidadesStore } from '@/stores/unidades.store';
import { computed, ref, watch } from 'vue';

export function useUnidadesFilters() {
  const store = useUnidadesStore();
  
  // Local filter state
  const localFilters = ref({
    ...store.filtros
  });
  
  // Apply filters to store
  const applyFilters = () => {
    store.aplicarFiltros(localFilters.value);
  };
  
  // Set individual filters
  const setStatusFilter = (ativas, inativas) => {
    localFilters.value.ativas = ativas;
    localFilters.value.inativas = inativas;
    applyFilters();
  };
  
  const setSearchFilter = (searchTerm) => {
    localFilters.value.busca = searchTerm;
    applyFilters();
  };
  
  const clearFilters = () => {
    store.limparFiltros();
    localFilters.value = { ...store.filtros };
  };
  
  const toggleAtivas = () => {
    localFilters.value.ativas = !localFilters.value.ativas;
    applyFilters();
  };
  
  const toggleInativas = () => {
    localFilters.value.inativas = !localFilters.value.inativas;
    applyFilters();
  };
  
  // Filter presets
  const showAll = () => {
    setStatusFilter(true, true);
  };
  
  const showAtivas = () => {
    setStatusFilter(true, false);
  };
  
  const showInativas = () => {
    setStatusFilter(false, true);
  };
  
  // Computed filter states
  const filterSummary = () => {
    const summary = [];
    
    if (!localFilters.value.ativas && !localFilters.value.inativas) {
      summary.push('Nenhum status selecionado');
    } else if (localFilters.value.ativas && localFilters.value.inativas) {
      summary.push('Todos os status');
    } else if (localFilters.value.ativas) {
      summary.push('Apenas ativas');
    } else if (localFilters.value.inativas) {
      summary.push('Apenas inativas');
    }
    
    if (localFilters.value.busca) {
      summary.push(`Busca: "${localFilters.value.busca}"`);
    }
    
    return summary.length > 0 ? summary.join(' • ') : 'Sem filtros';
  };
  
  // Watch for store filter changes
  watch(
    () => store.filtros,
    (newFilters) => {
      localFilters.value = { ...newFilters };
    },
    { deep: true }
  );
  
  return {
    // State
    localFilters,
    
    // Methods
    applyFilters,
    setStatusFilter,
    setSearchFilter,
    clearFilters,
    toggleAtivas,
    toggleInativas,
    showAll,
    showAtivas,
    showInativas,
    
    // Computed
    filterSummary,
    
    // Convenience getters
    hasFilters: computed(() => 
      !store.filtros.ativas || 
      store.filtros.inativas || 
      store.filtros.busca.length > 0
    ),
    isShowingAll: computed(() => 
      store.filtros.ativas && store.filtros.inativas
    ),
    isShowingAtivas: computed(() => 
      store.filtros.ativas && !store.filtros.inativas
    ),
    isShowingInativas: computed(() => 
      !store.filtros.ativas && store.filtros.inativas
    ),
  };
}