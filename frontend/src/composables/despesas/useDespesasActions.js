// composables/despesas/useDespesasActions.js
import { useDespesasStore } from '@/stores/despesas.store'

export const useDespesasActions = () => {
  const store = useDespesasStore()

  const carregarDespesas = async (unidadeId) => {
    return await store.carregarDespesas(unidadeId)
  }

  const criarDespesa = async (despesaData) => {
    return await store.criarDespesa(despesaData)
  }

  const removerDespesa = async (id) => {
    return await store.removerDespesa(id)
  }

  const uploadArquivo = async (formData) => {
    return await store.uploadArquivo(formData)
  }

  const resetarStore = () => {
    store.resetarStore()
  }

  const clearError = () => {
    store.clearError()
  }

  return {
    carregarDespesas,
    criarDespesa,
    removerDespesa,
    uploadArquivo,
    resetarStore,
    clearError
  }
}