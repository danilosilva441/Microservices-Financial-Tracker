// composables/unidades/useUnidades.js

import { computed, onMounted, ref, reactive } from 'vue';
import { defineStore } from 'pinia';

console.log('✅ computed importado corretamente:', typeof computed);

import { useUnidadesStore } from '@/stores/unidades.store';
import { useUnidadesUI } from './useUnidadesUI';
import { useUnidadesActions } from './useUnidadesActions';
import { useUnidadesFilters } from './useUnidadesFilters';

export function useUnidades() {
  const store = useUnidadesStore();
  const ui = useUnidadesUI();
  const actions = useUnidadesActions();
  const filters = useUnidadesFilters();

  // State reativo adicional se necessário
  const searchTerm = ref('');
  const selectedUnidades = ref([]);

  // Computed properties que combinam store e local state
  const filteredUnidades = computed(() => {
    if (!searchTerm.value) return store.unidadesFiltradas;

    const term = searchTerm.value.toLowerCase();
    return store.unidades.filter(unidade =>
      unidade.nome?.toLowerCase().includes(term) ||
      unidade.descricao?.toLowerCase().includes(term) ||
      unidade.endereco?.toLowerCase().includes(term)
    );
  });

  // Status da loja
  const isUnidadesLoading = computed(() => store.isLoading);
  const error = computed(() => store.error);

  // Métodos
  const loadUnidades = async () => {
    await actions.loadUnidades();
  };

  const searchUnidades = (term) => {
    searchTerm.value = term;
    filters.setSearchFilter(term);
  };

  const clearSearch = () => {
    searchTerm.value = '';
    filters.clearFilters();
  };

  const selectUnidade = (unidade) => {
    const index = selectedUnidades.value.findIndex(u => u.id === unidade.id);
    if (index === -1) {
      selectedUnidades.value.push(unidade);
    } else {
      selectedUnidades.value.splice(index, 1);
    }
  };

  const clearSelection = () => {
    selectedUnidades.value = [];
  };

  // Inicialização
  onMounted(async () => {
    if (store.unidades.length === 0) {
      await loadUnidades();
    }
  });

  return {
    // Store access
    store,

    // State
    unidades: computed(() => store.unidades),
    unidadeAtual: computed(() => store.unidadeAtual),
    unidadesFiltradas: filteredUnidades,
    unidadesAtivas: computed(() => store.unidadesAtivas),
    unidadesInativas: computed(() => store.unidadesInativas),
    unidadesComVencimentoProximo: computed(() => store.unidadesComVencimentoProximo),

    // Stats
    totalUnidades: computed(() => store.totalUnidades),
    totalAtivas: computed(() => store.totalAtivas),
    totalFaturamentoProjetado: computed(() => store.totalFaturamentoProjetado),
    mediaFaturamentoProjetado: computed(() => store.mediaFaturamentoProjetado),

    // UI
    isLoading: isUnidadesLoading,
    error,
    ui,

    // Actions
    actions,

    // Filters
    filters,
    filtros: computed(() => store.filtros),

    // Local state
    searchTerm,
    selectedUnidades: computed(() => selectedUnidades.value),
    hasSelected: computed(() => selectedUnidades.value.length > 0),

    // Methods
    loadUnidades,
    searchUnidades,
    clearSearch,
    selectUnidade,
    clearSelection,
    getUnidadeById: (id) => store.getUnidadeLocal(id),

    // Convenience getters
    hasUnidades: computed(() => store.unidades.length > 0),
    isEmpty: computed(() => store.unidades.length === 0),
    isLoadingData: computed(() => store.isLoading && store.unidades.length === 0),
    isRefreshing: computed(() => store.isLoading && store.unidades.length > 0),
  };
}