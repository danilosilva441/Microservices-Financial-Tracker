import { storeToRefs } from 'pinia'
import { useShiftsStore } from '@/stores/shifts.store'

export function useShifts() {
  const store = useShiftsStore()
  
  const {
    shifts,
    templates,
    breaks,
    shiftAtual,
    filtros,
    calendario,
    estatisticas,
    dashboard,
    isLoading,
    error,
    config
  } = storeToRefs(store)

  const {
    shiftsFiltrados,
    shiftsHoje,
    shiftsEstaSemana,
    shiftsEsteMes,
    shiftsPorFuncionario,
    calendarioMensal
  } = storeToRefs(store)

  const carregarShifts = async (unidadeId) => {
    return await store.carregarShifts(unidadeId)
  }

  const criarTemplate = async (templateData) => {
    return await store.criarTemplate(templateData)
  }

  const gerarEscala = async (dadosEscala) => {
    return await store.gerarEscala(dadosEscala)
  }

  const gerarEscalaAutomatica = async (unidadeId, mes, ano, funcionariosIds) => {
    return await store.gerarEscalaAutomatica(unidadeId, mes, ano, funcionariosIds)
  }

  const resetarStore = () => {
    store.resetarStore()
  }

  const clearError = () => {
    store.clearError()
  }

  return {
    // Estado
    shifts,
    templates,
    breaks,
    shiftAtual,
    filtros,
    calendario,
    estatisticas,
    dashboard,
    isLoading,
    error,
    config,
    
    // Getters
    shiftsFiltrados,
    shiftsHoje,
    shiftsEstaSemana,
    shiftsEsteMes,
    shiftsPorFuncionario,
    calendarioMensal,
    
    // Ações
    carregarShifts,
    criarTemplate,
    gerarEscala,
    gerarEscalaAutomatica,
    resetarStore,
    clearError
  }
}