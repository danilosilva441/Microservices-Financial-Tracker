// composables/despesas/useDespesas.js
import { storeToRefs } from 'pinia'
import { useDespesasStore } from '@/stores/despesas.store'
import { useDespesasActions } from './useDespesasActions'
import { useDespesasFilters } from './useDespesasFilters'
import { useDespesasUI } from './useDespesasUI'
import { useCategorias } from './useCategorias'
import { useDespesasCharts } from './useDespesasCharts'
import { useDespesasExport } from './useDespesasExport'

export const useDespesas = () => {
  const store = useDespesasStore()
  
  // Extrai refs do store
  const {
    despesas,
    despesaAtual,
    categorias,
    categoriaAtual,
    filtros,
    arquivoUpload,
    progressoUpload,
    resultadoUpload,
    isLoading,
    error,
    editando,
    estatisticas,
    dashboard,
    categoriasPadrao,
    despesasFiltradas,
    despesasEsteMes,
    despesasHoje,
    totalPorCategoria,
    categoriasComTotal,
    topCategorias,
    despesasRecorrentes,
    despesasVencimentoProximo
  } = storeToRefs(store)

  // Importa funcionalidades específicas
  const actions = useDespesasActions()
  const filters = useDespesasFilters()
  const ui = useDespesasUI()
  const categoriasModule = useCategorias()
  const charts = useDespesasCharts()
  const exportModule = useDespesasExport()

  return {
    // Dados
    despesas,
    despesaAtual,
    categorias,
    categoriaAtual,
    filtros,
    arquivoUpload,
    progressoUpload,
    resultadoUpload,
    isLoading,
    error,
    editando,
    estatisticas,
    dashboard,
    categoriasPadrao,

    // Getters
    despesasFiltradas,
    despesasEsteMes,
    despesasHoje,
    totalPorCategoria,
    categoriasComTotal,
    topCategorias,
    despesasRecorrentes,
    despesasVencimentoProximo,

    // Ações agrupadas
    ...actions,
    ...filters,
    ...ui,
    ...categoriasModule,
    ...charts,
    ...exportModule,

    // Store original (se necessário)
    store
  }
}