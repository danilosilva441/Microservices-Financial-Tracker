import { ref } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'
import { useToast } from '../unidades/useToast'
import { useConfirmDialog } from '../unidades/useConfirmDialog'

export function useShiftsActions() {
  const store = useShiftsStore()
  const toast = useToast()
  const confirmDialog = useConfirmDialog()
  
  const actionLoading = ref(false)
  const selectedShifts = ref([])

  const deleteShift = async (shiftId) => {
    const confirm = await confirmDialog.show({
      title: 'Excluir turno',
      message: 'Tem certeza que deseja excluir este turno?',
      confirmText: 'Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    })

    if (!confirm) return { success: false, cancelled: true }

    actionLoading.value = true

    try {
      // Simulação de exclusão (em produção, chamaria a API)
      await new Promise(resolve => setTimeout(resolve, 500))

      const index = store.shifts.findIndex(s => s.id === shiftId)
      if (index !== -1) {
        store.shifts.splice(index, 1)
      }

      store.calcularEstatisticas()
      store.atualizarDashboard()
      
      toast.show('Turno excluído com sucesso!', 'success')
      return { success: true }
    } catch (error) {
      console.error('Erro ao excluir turno:', error)
      toast.show('Erro ao excluir turno', 'error')
      return { success: false, error: error.message }
    } finally {
      actionLoading.value = false
    }
  }

  const deleteMultipleShifts = async () => {
    if (selectedShifts.value.length === 0) {
      toast.show('Selecione pelo menos um turno', 'warning')
      return { success: false }
    }

    const confirm = await confirmDialog.show({
      title: 'Excluir turnos',
      message: `Tem certeza que deseja excluir ${selectedShifts.value.length} turno(s)?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    })

    if (!confirm) return { success: false, cancelled: true }

    actionLoading.value = true

    try {
      // Simulação de exclusão
      await new Promise(resolve => setTimeout(resolve, 500))

      store.shifts = store.shifts.filter(s => !selectedShifts.value.includes(s.id))
      
      store.calcularEstatisticas()
      store.atualizarDashboard()
      selectedShifts.value = []
      
      toast.show('Turnos excluídos com sucesso!', 'success')
      return { success: true }
    } catch (error) {
      console.error('Erro ao excluir turnos:', error)
      toast.show('Erro ao excluir turnos', 'error')
      return { success: false, error: error.message }
    } finally {
      actionLoading.value = false
    }
  }

  const duplicarShift = async (shift) => {
    actionLoading.value = true

    try {
      const novoShift = {
        ...shift,
        id: `shift-${Date.now()}`,
        data: new Date().toISOString().split('T')[0]
      }
      
      delete novoShift.id_original
      
      store.shifts.push(novoShift)
      
      store.calcularEstatisticas()
      store.atualizarDashboard()
      
      toast.show('Turno duplicado com sucesso!', 'success')
      return { success: true, data: novoShift }
    } catch (error) {
      console.error('Erro ao duplicar turno:', error)
      toast.show('Erro ao duplicar turno', 'error')
      return { success: false, error: error.message }
    } finally {
      actionLoading.value = false
    }
  }

  const toggleAtivo = async (shift) => {
    actionLoading.value = true

    try {
      const novoStatus = !shift.isAtivo
      
      // Simulação de atualização
      await new Promise(resolve => setTimeout(resolve, 300))
      
      shift.isAtivo = novoStatus
      
      toast.show(`Turno ${novoStatus ? 'ativado' : 'desativado'} com sucesso!`, 'success')
      return { success: true }
    } catch (error) {
      console.error('Erro ao alterar status:', error)
      toast.show('Erro ao alterar status', 'error')
      return { success: false, error: error.message }
    } finally {
      actionLoading.value = false
    }
  }

  const registrarPresenca = async (shiftId, presente) => {
    actionLoading.value = true

    try {
      const shift = store.shifts.find(s => s.id === shiftId)
      if (!shift) throw new Error('Turno não encontrado')

      shift.registrouPresenca = presente
      shift.horaRegistro = new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })

      toast.show(`Presença ${presente ? 'registrada' : 'marcada como ausente'}!`, 'success')
      return { success: true }
    } catch (error) {
      console.error('Erro ao registrar presença:', error)
      toast.show('Erro ao registrar presença', 'error')
      return { success: false, error: error.message }
    } finally {
      actionLoading.value = false
    }
  }

  const toggleSelection = (shiftId) => {
    const index = selectedShifts.value.indexOf(shiftId)
    if (index === -1) {
      selectedShifts.value.push(shiftId)
    } else {
      selectedShifts.value.splice(index, 1)
    }
  }

  const selectAll = (shifts) => {
    selectedShifts.value = shifts.map(s => s.id)
  }

  const clearSelection = () => {
    selectedShifts.value = []
  }

  return {
    actionLoading,
    selectedShifts,
    deleteShift,
    deleteMultipleShifts,
    duplicarShift,
    toggleAtivo,
    registrarPresenca,
    toggleSelection,
    selectAll,
    clearSelection
  }
}