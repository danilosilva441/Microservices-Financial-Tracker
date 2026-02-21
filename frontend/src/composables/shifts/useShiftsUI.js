import { ref, computed } from 'vue'

export function useShiftsUI() {
  const viewMode = ref('calendar') // 'calendar', 'list', 'grid'
  const showFilters = ref(false)
  const showForm = ref(false)
  const showDetails = ref(false)
  const showStats = ref(false)
  const showExport = ref(false)
  
  const expandedCards = ref([])
  const activeTab = ref('geral')
  
  const sortBy = ref('data')
  const sortDirection = ref('asc')
  
  const itemsPerPage = ref(20)
  const currentPage = ref(1)

  const viewOptions = computed(() => [
    { value: 'calendar', label: 'Calendário', icon: 'Calendar' },
    { value: 'list', label: 'Lista', icon: 'List' },
    { value: 'grid', label: 'Grade', icon: 'Grid' }
  ])

  const tabOptions = computed(() => [
    { value: 'geral', label: 'Geral' },
    { value: 'funcionarios', label: 'Funcionários' },
    { value: 'estatisticas', label: 'Estatísticas' },
    { value: 'configuracoes', label: 'Configurações' }
  ])

  const toggleCard = (id) => {
    const index = expandedCards.value.indexOf(id)
    if (index === -1) {
      expandedCards.value.push(id)
    } else {
      expandedCards.value.splice(index, 1)
    }
  }

  const isExpanded = (id) => expandedCards.value.includes(id)

  const setSort = (field) => {
    if (sortBy.value === field) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortBy.value = field
      sortDirection.value = 'asc'
    }
  }

  const closeAllModals = () => {
    showFilters.value = false
    showForm.value = false
    showDetails.value = false
    showStats.value = false
    showExport.value = false
  }

  return {
    // Estado da UI
    viewMode,
    showFilters,
    showForm,
    showDetails,
    showStats,
    showExport,
    expandedCards,
    activeTab,
    sortBy,
    sortDirection,
    itemsPerPage,
    currentPage,
    
    // Opções
    viewOptions,
    tabOptions,
    
    // Métodos
    toggleCard,
    isExpanded,
    setSort,
    closeAllModals
  }
}