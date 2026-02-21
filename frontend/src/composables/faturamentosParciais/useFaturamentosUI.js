// composables/faturamentosParciais/useFaturamentosUI.js
import { ref, computed } from 'vue';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useFaturamentosUI() {
  const store = useFaturamentosParciaisStore();
  
  // Estado local da UI
  const modalState = ref({
    create: false,
    edit: false,
    view: false,
    delete: false,
    carrinho: false,
    filtros: false,
  });
  
  const selectedLancamento = ref(null);
  const viewMode = ref('lista'); // 'lista', 'cards', 'graficos'
  
  const openModal = (modalName, lancamento = null) => {
    if (lancamento) selectedLancamento.value = lancamento;
    modalState.value[modalName] = true;
  };
  
  const closeModal = (modalName) => {
    modalState.value[modalName] = false;
    if (!Object.values(modalState.value).some(v => v)) {
      selectedLancamento.value = null;
    }
  };
  
  const closeAllModals = () => {
    Object.keys(modalState.value).forEach(key => {
      modalState.value[key] = false;
    });
    selectedLancamento.value = null;
  };
  
  const toggleViewMode = (mode) => {
    viewMode.value = mode;
  };
  
  const isLoading = computed(() => store.isLoading);
  const error = computed(() => store.error);
  const editando = computed(() => store.editando);
  
  return {
    // Estado UI
    modalState,
    selectedLancamento,
    viewMode,
    isLoading,
    error,
    editando,
    
    // Métodos UI
    openModal,
    closeModal,
    closeAllModals,
    toggleViewMode,
  };
}