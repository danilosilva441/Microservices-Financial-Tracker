import { ref, computed, watch } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'
import { useUnidades } from '../unidades'
import { useFuncionarios } from './useFuncionarios'

export function useShiftsFilters() {
  const store = useShiftsStore()
  const { unidades } = useUnidades()
  const { funcionarios } = useFuncionarios()
  
  const filters = ref({
    unidadeId: null,
    dataInicio: null,
    dataFim: null,
    userId: null,
    tipoTurno: null,
    apenasAtivos: true,
    busca: ''
  })

  const filterOptions = ref({
    unidades: [],
    funcionarios: [],
    tiposTurno: []
  })

  const activeFiltersCount = computed(() => {
    let count = 0
    if (filters.value.unidadeId) count++
    if (filters.value.dataInicio) count++
    if (filters.value.dataFim) count++
    if (filters.value.userId) count++
    if (filters.value.tipoTurno) count++
    if (!filters.value.apenasAtivos) count++
    if (filters.value.busca) count++
    return count
  })

  const hasFilters = computed(() => activeFiltersCount.value > 0)

  const loadFilterOptions = async () => {
    filterOptions.value = {
      unidades: unidades.value || [],
      funcionarios: funcionarios.value || [],
      tiposTurno: store.ShiftTypeEnum?.getAll() || []
    }
  }

  const applyFilters = () => {
    store.aplicarFiltros(filters.value)
  }

  const clearFilters = () => {
    filters.value = {
      unidadeId: filters.value.unidadeId || null,
      dataInicio: null,
      dataFim: null,
      userId: null,
      tipoTurno: null,
      apenasAtivos: true,
      busca: ''
    }
    store.limparFiltros()
  }

  const setPeriodo = (periodo) => {
    const hoje = new Date()
    const inicio = new Date()
    const fim = new Date()

    switch (periodo) {
      case 'hoje':
        filters.value.dataInicio = hoje.toISOString().split('T')[0]
        filters.value.dataFim = hoje.toISOString().split('T')[0]
        break
      case 'semana':
        inicio.setDate(hoje.getDate() - hoje.getDay())
        fim.setDate(inicio.getDate() + 6)
        filters.value.dataInicio = inicio.toISOString().split('T')[0]
        filters.value.dataFim = fim.toISOString().split('T')[0]
        break
      case 'mes':
        inicio.setDate(1)
        fim.setMonth(inicio.getMonth() + 1)
        fim.setDate(0)
        filters.value.dataInicio = inicio.toISOString().split('T')[0]
        filters.value.dataFim = fim.toISOString().split('T')[0]
        break
      case 'proximos30':
        inicio.setDate(hoje.getDate() + 1)
        fim.setDate(hoje.getDate() + 30)
        filters.value.dataInicio = inicio.toISOString().split('T')[0]
        filters.value.dataFim = fim.toISOString().split('T')[0]
        break
    }
    
    applyFilters()
  }

  // Watch para aplicar filtros automaticamente quando mudarem
  watch(filters, () => {
    applyFilters()
  }, { deep: true })

  return {
    filters,
    filterOptions,
    activeFiltersCount,
    hasFilters,
    loadFilterOptions,
    applyFilters,
    clearFilters,
    setPeriodo
  }
}