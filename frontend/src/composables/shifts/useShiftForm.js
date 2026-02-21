import { ref, computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'
import { useToast } from '../unidades/useToast'
import { useShiftTypes } from './useShiftTypes'
import { useFuncionarios } from './useFuncionarios'

export function useShiftForm() {
  const store = useShiftsStore()
  const toast = useToast()
  const { getTipoById, getAllTipos } = useShiftTypes()
  const { funcionariosAtivos } = useFuncionarios()
  
  const formData = ref({
    id: null,
    unidadeId: null,
    userId: null,
    templateId: null,
    data: new Date().toISOString().split('T')[0],
    type: 1,
    startTime: '08:00',
    endTime: '14:00',
    isAtivo: true,
    observacoes: ''
  })

  const errors = ref({})
  const isSubmitting = ref(false)
  const isEditing = ref(false)

  const isValid = computed(() => {
    return Object.keys(errors.value).length === 0
  })

  const resetForm = () => {
    formData.value = {
      id: null,
      unidadeId: null,
      userId: null,
      templateId: null,
      data: new Date().toISOString().split('T')[0],
      type: 1,
      startTime: '08:00',
      endTime: '14:00',
      isAtivo: true,
      observacoes: ''
    }
    errors.value = {}
    isEditing.value = false
  }

  const setEditShift = (shift) => {
    formData.value = {
      id: shift.id || null,
      unidadeId: shift.unidadeId || null,
      userId: shift.userId || null,
      templateId: shift.templateId || null,
      data: shift.data || shift.startDate?.split('T')[0] || new Date().toISOString().split('T')[0],
      type: shift.type || 1,
      startTime: shift.startTime || '08:00',
      endTime: shift.endTime || '14:00',
      isAtivo: shift.isAtivo !== false,
      observacoes: shift.observacoes || ''
    }
    isEditing.value = true
  }

  const validateForm = () => {
    const newErrors = {}

    if (!formData.value.unidadeId) {
      newErrors.unidadeId = 'Unidade é obrigatória'
    }

    if (!formData.value.userId) {
      newErrors.userId = 'Funcionário é obrigatório'
    }

    if (!formData.value.data) {
      newErrors.data = 'Data é obrigatória'
    }

    if (!formData.value.type) {
      newErrors.type = 'Tipo de turno é obrigatório'
    }

    if (formData.value.type !== 7) { // Não é folga
      if (!formData.value.startTime) {
        newErrors.startTime = 'Horário de início é obrigatório'
      }
      if (!formData.value.endTime) {
        newErrors.endTime = 'Horário de término é obrigatório'
      }
    }

    errors.value = newErrors
    return Object.keys(newErrors).length === 0
  }

  const salvarShift = async () => {
    if (!validateForm()) {
      toast.show('Preencha todos os campos obrigatórios', 'error')
      return { success: false }
    }

    isSubmitting.value = true

    try {
      // Simulação de salvamento (em produção, chamaria a API)
      await new Promise(resolve => setTimeout(resolve, 500))

      const shiftData = {
        ...formData.value,
        startDate: formData.value.data,
        id: formData.value.id || `shift-${Date.now()}`
      }

      if (isEditing.value) {
        // Atualizar shift existente
        const index = store.shifts.findIndex(s => s.id === shiftData.id)
        if (index !== -1) {
          store.shifts[index] = { ...store.shifts[index], ...shiftData }
        }
        toast.show('Turno atualizado com sucesso!', 'success')
      } else {
        // Criar novo shift
        store.shifts.push(shiftData)
        toast.show('Turno criado com sucesso!', 'success')
      }

      store.calcularEstatisticas()
      store.atualizarDashboard()
      
      return { success: true, data: shiftData }
    } catch (error) {
      console.error('Erro ao salvar turno:', error)
      toast.show('Erro ao salvar turno', 'error')
      return { success: false, error: error.message }
    } finally {
      isSubmitting.value = false
    }
  }

  const setTipoTurno = (tipoId) => {
    formData.value.type = tipoId
    const horarioPadrao = getTipoById(tipoId)?.horarioPadrao || { inicio: '08:00', fim: '14:00' }
    
    if (tipoId !== 7) { // Não é folga
      formData.value.startTime = horarioPadrao.inicio
      formData.value.endTime = horarioPadrao.fim
    } else {
      formData.value.startTime = null
      formData.value.endTime = null
    }
  }

  const setFuncionario = (userId) => {
    formData.value.userId = userId
    // Verificar disponibilidade
    const disponibilidade = store.verificarDisponibilidadeFuncionario(userId, formData.value.data)
    if (!disponibilidade.disponivel) {
      toast.show(disponibilidade.motivo, 'warning')
    }
  }

  return {
    formData,
    errors,
    isSubmitting,
    isEditing,
    isValid,
    resetForm,
    setEditShift,
    validateForm,
    salvarShift,
    setTipoTurno,
    setFuncionario
  }
}