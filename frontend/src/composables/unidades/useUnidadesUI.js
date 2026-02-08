// composables/unidades/useUnidadesUI.js
import { ref, computed } from 'vue';
import { useUnidadesStore } from '@/stores/unidades.store';

export function useUnidadesUI() {
  const store = useUnidadesStore();
  
  // UI State
  const showFilters = ref(false);
  const showDeleteModal = ref(false);
  const showDeactivateModal = ref(false);
  const showProjecaoModal = ref(false);
  const selectedForAction = ref(null);
  const viewMode = ref('grid'); // 'grid' or 'list'
  
  // Computed UI States
  const hasActiveFilters = computed(() => {
    return !store.filtros.ativas || 
           store.filtros.inativas || 
           store.filtros.busca.length > 0;
  });
  
  const filterStatusText = computed(() => {
    const { ativas, inativas } = store.filtros;
    if (ativas && inativas) return 'Todas';
    if (ativas) return 'Ativas';
    if (inativas) return 'Inativas';
    return 'Nenhuma';
  });
  
  const canCreateUnidade = computed(() => {
    // Aqui você pode adicionar lógica de permissão
    return true;
  });
  
  const canEditUnidade = computed(() => {
    // Lógica de permissão para edição
    return true;
  });
  
  const canDeleteUnidade = computed(() => {
    // Lógica de permissão para exclusão
    return true;
  });
  
  // Methods
  const toggleFilters = () => {
    showFilters.value = !showFilters.value;
  };
  
  const toggleViewMode = () => {
    viewMode.value = viewMode.value === 'grid' ? 'list' : 'grid';
  };
  
  const openDeleteModal = (unidade) => {
    selectedForAction.value = unidade;
    showDeleteModal.value = true;
  };
  
  const closeDeleteModal = () => {
    showDeleteModal.value = false;
    selectedForAction.value = null;
  };
  
  const openDeactivateModal = (unidade) => {
    selectedForAction.value = unidade;
    showDeactivateModal.value = true;
  };
  
  const closeDeactivateModal = () => {
    showDeactivateModal.value = false;
    selectedForAction.value = null;
  };
  
  const openProjecaoModal = (unidade) => {
    selectedForAction.value = unidade;
    showProjecaoModal.value = true;
  };
  
  const closeProjecaoModal = () => {
    showProjecaoModal.value = false;
    selectedForAction.value = null;
  };
  
  // Formatadores
  const formatCurrency = (value) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value || 0);
  };
  
  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString('pt-BR');
  };
  
  const getStatusBadge = (isAtivo) => {
    return {
      label: isAtivo ? 'Ativa' : 'Inativa',
      variant: isAtivo ? 'success' : 'danger',
      icon: isAtivo ? 'check-circle' : 'x-circle'
    };
  };
  
  const getDaysToExpire = (dataFim) => {
    if (!dataFim) return null;
    
    const hoje = new Date();
    const fim = new Date(dataFim);
    const diffTime = fim - hoje;
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    return diffDays;
  };
  
  const getExpirationStatus = (dataFim) => {
    const days = getDaysToExpire(dataFim);
    
    if (days === null) return null;
    
    if (days < 0) {
      return {
        label: 'Expirada',
        variant: 'danger',
        days
      };
    } else if (days <= 30) {
      return {
        label: 'Vence em breve',
        variant: 'warning',
        days
      };
    } else {
      return {
        label: 'Válida',
        variant: 'success',
        days
      };
    }
  };
  
  return {
    // State
    showFilters,
    showDeleteModal,
    showDeactivateModal,
    showProjecaoModal,
    selectedForAction,
    viewMode,
    
    // Computed
    hasActiveFilters,
    filterStatusText,
    canCreateUnidade,
    canEditUnidade,
    canDeleteUnidade,
    
    // Methods
    toggleFilters,
    toggleViewMode,
    openDeleteModal,
    closeDeleteModal,
    openDeactivateModal,
    closeDeactivateModal,
    openProjecaoModal,
    closeProjecaoModal,
    
    // Formatters
    formatCurrency,
    formatDate,
    getStatusBadge,
    getDaysToExpire,
    getExpirationStatus,
    
    // Convenience
    isGridView: computed(() => viewMode.value === 'grid'),
    isListView: computed(() => viewMode.value === 'list'),
  };
}