// composables/despesas/useDespesasUI.js
import { useDespesasStore } from '@/stores/despesas.store'
import { storeToRefs } from 'pinia'

export const useDespesasUI = () => {
  const store = useDespesasStore()
  const { editando, error, isLoading } = storeToRefs(store)

  const formatarValor = (valor) => {
    return store.formatarValor(valor)
  }

  const formatarData = (data) => {
    return store.formatarData(data)
  }

  const formatarDataCompleta = (data) => {
    return store.formatarDataCompleta(data)
  }

  const getCorPorCategoria = (categoriaId) => {
    const cores = {
      1: '#ef4444', // Aluguel - Vermelho
      2: '#f59e0b', // Luz - Laranja
      3: '#3b82f6', // Água - Azul
      4: '#10b981', // Internet - Verde
      99: '#6b7280' // Outros - Cinza
    }
    return cores[categoriaId] || '#6b7280'
  }

  const getIconePorCategoria = (categoriaId) => {
    const icones = {
      1: 'mdi-home',
      2: 'mdi-lightning-bolt',
      3: 'mdi-water',
      4: 'mdi-wifi',
      99: 'mdi-cash'
    }
    return icones[categoriaId] || 'mdi-cash'
  }

  const getStatusBadge = (despesa) => {
    const hoje = new Date()
    const dataDespesa = new Date(despesa.expenseDate || despesa.data)
    
    if (dataDespesa < hoje) {
      return { text: 'Vencida', color: 'danger' }
    }
    
    const diffDias = Math.ceil((dataDespesa - hoje) / (1000 * 60 * 60 * 24))
    
    if (diffDias <= 3) {
      return { text: 'Próximo ao vencimento', color: 'warning' }
    }
    
    if (diffDias <= 7) {
      return { text: 'A vencer', color: 'info' }
    }
    
    return { text: 'Futura', color: 'success' }
  }

  const toggleEditMode = (valor) => {
    if (typeof valor === 'boolean') {
      editando.value = valor
    } else {
      editando.value = !editando.value
    }
  }

  return {
    editando,
    error,
    isLoading,
    formatarValor,
    formatarData,
    formatarDataCompleta,
    getCorPorCategoria,
    getIconePorCategoria,
    getStatusBadge,
    toggleEditMode
  }
}