// composables/despesas/useCategorias.js
import { useDespesasStore } from '@/stores/despesas.store'
import { storeToRefs } from 'pinia'

export const useCategorias = () => {
  const store = useDespesasStore()
  const { categorias, categoriaAtual } = storeToRefs(store)

  const carregarCategorias = async () => {
    return await store.carregarCategorias()
  }

  const criarCategoria = async (categoriaData) => {
    return await store.criarCategoria(categoriaData)
  }

  const getNomeCategoria = (categoriaId) => {
    return store.getNomeCategoria(categoriaId)
  }

  const getDescricaoCategoria = (categoriaId) => {
    return store.getDescricaoCategoria(categoriaId)
  }

  const setCategoriaAtual = (categoria) => {
    categoriaAtual.value = categoria
  }

  const limparCategoriaAtual = () => {
    categoriaAtual.value = null
  }

  const categoriasPorId = (id) => {
    return categorias.value.find(c => c.id === id)
  }

  const categoriasSelectOptions = () => {
    return categorias.value.map(c => ({
      value: c.id,
      label: c.name,
      description: c.description
    }))
  }

  return {
    categorias,
    categoriaAtual,
    carregarCategorias,
    criarCategoria,
    getNomeCategoria,
    getDescricaoCategoria,
    setCategoriaAtual,
    limparCategoriaAtual,
    categoriasPorId,
    categoriasSelectOptions
  }
}