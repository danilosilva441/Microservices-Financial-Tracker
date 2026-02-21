import { ref, computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'
import { useToast } from '../unidades/useToast'
import { BreakTypeEnum } from '@/stores/shifts.store'

export function useBreaks() {
  const store = useShiftsStore()
  const toast = useToast()
  
  const breakForm = ref({
    id: null,
    shiftId: null,
    type: 1,
    startTime: '12:00',
    endTime: '13:00',
    observacoes: ''
  })

  const breakTypes = computed(() => BreakTypeEnum.getAll())
  const isEditingBreak = ref(false)

  const getBreakTypeName = (typeId) => {
    return BreakTypeEnum.getNome(typeId)
  }

  const getBreaksByShift = (shiftId) => {
    return store.breaks.filter(b => b.shiftId === shiftId)
  }

  const resetBreakForm = () => {
    breakForm.value = {
      id: null,
      shiftId: null,
      type: 1,
      startTime: '12:00',
      endTime: '13:00',
      observacoes: ''
    }
    isEditingBreak.value = false
  }

  const setEditBreak = (breakItem) => {
    breakForm.value = { ...breakItem }
    isEditingBreak.value = true
  }

  const validateBreak = () => {
    if (!breakForm.value.shiftId) {
      toast.show('Turno não identificado', 'error')
      return false
    }

    if (!breakForm.value.startTime || !breakForm.value.endTime) {
      toast.show('Horários de início e fim são obrigatórios', 'error')
      return false
    }

    // Validar se horário de início é anterior ao fim
    const start = breakForm.value.startTime.split(':').map(Number)
    const end = breakForm.value.endTime.split(':').map(Number)
    const startMinutes = start[0] * 60 + start[1]
    const endMinutes = end[0] * 60 + end[1]

    if (startMinutes >= endMinutes) {
      toast.show('Horário de início deve ser anterior ao horário de fim', 'error')
      return false
    }

    return true
  }

  const salvarIntervalo = async () => {
    if (!validateBreak()) return { success: false }

    try {
      // Simulação de salvamento
      await new Promise(resolve => setTimeout(resolve, 500))

      if (isEditingBreak.value) {
        // Atualizar
        const index = store.breaks.findIndex(b => b.id === breakForm.value.id)
        if (index !== -1) {
          store.breaks[index] = { ...breakForm.value }
        }
        toast.show('Intervalo atualizado com sucesso!', 'success')
      } else {
        // Criar novo
        const novoIntervalo = {
          ...breakForm.value,
          id: `break-${Date.now()}`
        }
        store.breaks.push(novoIntervalo)
        toast.show('Intervalo registrado com sucesso!', 'success')
      }

      store.calcularEstatisticas()
      resetBreakForm()
      return { success: true }
    } catch (error) {
      console.error('Erro ao salvar intervalo:', error)
      toast.show('Erro ao salvar intervalo', 'error')
      return { success: false, error: error.message }
    }
  }

  const deleteBreak = async (breakId) => {
    try {
      const index = store.breaks.findIndex(b => b.id === breakId)
      if (index !== -1) {
        store.breaks.splice(index, 1)
        toast.show('Intervalo removido com sucesso!', 'success')
        store.calcularEstatisticas()
      }
      return { success: true }
    } catch (error) {
      console.error('Erro ao remover intervalo:', error)
      toast.show('Erro ao remover intervalo', 'error')
      return { success: false, error: error.message }
    }
  }

  return {
    breakForm,
    breakTypes,
    isEditingBreak,
    getBreakTypeName,
    getBreaksByShift,
    resetBreakForm,
    setEditBreak,
    salvarIntervalo,
    deleteBreak,
    BreakTypeEnum
  }
}