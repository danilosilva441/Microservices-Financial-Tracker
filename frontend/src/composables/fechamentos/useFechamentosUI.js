import { ref, computed } from 'vue';

export function useFechamentosUI() {
  // Estados de UI
  const showCreateModal = ref(false);
  const showEditModal = ref(false);
  const showViewModal = ref(false);
  const showDeleteModal = ref(false);
  const showConferirModal = ref(false);
  const showReabrirModal = ref(false);
  const showRelatorioModal = ref(false);
  
  const selectedFechamento = ref(null);
  const activeTab = ref('lista');
  const viewMode = ref('table'); // 'table' ou 'cards'
  
  // Estados de loading por ação
  const loadingStates = ref({
    create: false,
    update: false,
    delete: false,
    conferir: false,
    reabrir: false,
    export: false,
  });

  // Mensagens de feedback
  const toast = ref({
    show: false,
    type: 'info',
    message: '',
    duration: 3000,
  });

  // Modais
  const openCreateModal = () => {
    selectedFechamento.value = null;
    showCreateModal.value = true;
  };

  const openEditModal = (fechamento) => {
    selectedFechamento.value = fechamento;
    showEditModal.value = true;
  };

  const openViewModal = (fechamento) => {
    selectedFechamento.value = fechamento;
    showViewModal.value = true;
  };

  const openConferirModal = (fechamento) => {
    selectedFechamento.value = fechamento;
    showConferirModal.value = true;
  };

  const openReabrirModal = (fechamento) => {
    selectedFechamento.value = fechamento;
    showReabrirModal.value = true;
  };

  const openRelatorioModal = () => {
    showRelatorioModal.value = true;
  };

  const closeAllModals = () => {
    showCreateModal.value = false;
    showEditModal.value = false;
    showViewModal.value = false;
    showDeleteModal.value = false;
    showConferirModal.value = false;
    showReabrirModal.value = false;
    showRelatorioModal.value = false;
    selectedFechamento.value = null;
  };

  // Toast
  const showToast = (type, message, duration = 3000) => {
    toast.value = {
      show: true,
      type,
      message,
      duration,
    };
    
    setTimeout(() => {
      toast.value.show = false;
    }, duration);
  };

  const hideToast = () => {
    toast.value.show = false;
  };

  // Loading states
  const setLoading = (action, isLoading) => {
    loadingStates.value[action] = isLoading;
  };

  const isLoading = computed(() => {
    return Object.values(loadingStates.value).some(state => state === true);
  });

  // Cores por status
  const getStatusColor = (status) => {
    const colors = {
      'Aberto': 'blue',
      'Fechado': 'orange',
      'Conferido': 'green',
      'Pendente': 'red',
    };
    return colors[status] || 'gray';
  };

  const getStatusBadgeClass = (status) => {
    const classes = {
      'Aberto': 'bg-blue-100 text-blue-800',
      'Fechado': 'bg-orange-100 text-orange-800',
      'Conferido': 'bg-green-100 text-green-800',
      'Pendente': 'bg-red-100 text-red-800',
    };
    return classes[status] || 'bg-gray-100 text-gray-800';
  };

  // Formatação
  const formatCurrency = (value) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value || 0);
  };

  const formatDate = (date) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  };

  const formatDateTime = (date) => {
    if (!date) return '-';
    return new Date(date).toLocaleString('pt-BR');
  };

  return {
    // Modais
    showCreateModal,
    showEditModal,
    showViewModal,
    showDeleteModal,
    showConferirModal,
    showReabrirModal,
    showRelatorioModal,
    selectedFechamento,
    activeTab,
    viewMode,
    
    // Loading
    loadingStates,
    isLoading,
    
    // Toast
    toast,
    
    // Actions UI
    openCreateModal,
    openEditModal,
    openViewModal,
    openConferirModal,
    openReabrirModal,
    openRelatorioModal,
    closeAllModals,
    
    // Toast actions
    showToast,
    hideToast,
    
    // Loading actions
    setLoading,
    
    // Helpers
    getStatusColor,
    getStatusBadgeClass,
    formatCurrency,
    formatDate,
    formatDateTime,
  };
}