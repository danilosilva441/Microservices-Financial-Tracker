// composables/despesas/useDespesasFilters.js
import { useDespesasStore } from '@/stores/despesas.store'
import { storeToRefs } from 'pinia'

export const useDespesasFilters = () => {
  const store = useDespesasStore()
  const { filtros } = storeToRefs(store)

  const aplicarFiltros = (novosFiltros) => {
    store.aplicarFiltros(novosFiltros)
  }

  const limparFiltros = () => {
    store.limparFiltros()
  }

  const setFiltroUnidade = (unidadeId) => {
    store.aplicarFiltros({ unidadeId })
  }

  const setFiltroPeriodo = (dataInicio, dataFim) => {
    store.aplicarFiltros({ dataInicio, dataFim })
  }

  const setFiltroCategoria = (categoriaId) => {
    store.aplicarFiltros({ categoriaId })
  }

  const setBusca = (busca) => {
    store.aplicarFiltros({ busca })
  }

  const setOrdenacao = (ordenacao) => {
    store.aplicarFiltros({ ordenacao })
  }

  const getFiltrosAtivos = () => {
    const ativos = {}
    Object.entries(filtros.value).forEach(([key, value]) => {
      if (value && value !== '') {
        ativos[key] = value
      }
    })
    return ativos
  }

  const temFiltrosAtivos = () => {
    return Object.values(getFiltrosAtivos()).length > 0
  }

  return {
    filtros,
    aplicarFiltros,
    limparFiltros,
    setFiltroUnidade,
    setFiltroPeriodo,
    setFiltroCategoria,
    setBusca,
    setOrdenacao,
    getFiltrosAtivos,
    temFiltrosAtivos
  }
}