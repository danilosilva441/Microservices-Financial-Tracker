import { ref, computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'

export function useCalendar() {
  const store = useShiftsStore()
  
  const currentDate = ref(new Date())
  const viewType = ref('month') // 'month', 'week', 'day'
  const selectedDate = ref(new Date())

  const monthNames = [
    'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
  ]

  const weekDays = [
    'Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'
  ]

  const shortWeekDays = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb']

  const currentMonth = computed(() => currentDate.value.getMonth())
  const currentYear = computed(() => currentDate.value.getFullYear())
  const currentMonthName = computed(() => monthNames[currentMonth.value])

  const calendarWeeks = computed(() => {
    return store.calendarioMensal || []
  })

  const goToPreviousMonth = () => {
    store.navegarCalendario('anterior')
    currentDate.value = new Date(currentYear.value, currentMonth.value - 1, 1)
  }

  const goToNextMonth = () => {
    store.navegarCalendario('proximo')
    currentDate.value = new Date(currentYear.value, currentMonth.value + 1, 1)
  }

  const goToToday = () => {
    const today = new Date()
    store.calendario.mesAtual = today.getMonth()
    store.calendario.anoAtual = today.getFullYear()
    currentDate.value = today
    selectedDate.value = today
  }

  const selectDate = (date) => {
    selectedDate.value = date
    store.calendario.dataSelecionada = date
  }

  const getShiftsForDate = (date) => {
    if (!date) return []
    
    const dateStr = date.toISOString().split('T')[0]
    return store.shifts.filter(s => {
      const shiftDate = new Date(s.data || s.startDate).toISOString().split('T')[0]
      return shiftDate === dateStr
    })
  }

  const getDateClass = (date) => {
    if (!date) return ''
    
    const hoje = new Date().toDateString()
    const selecionado = selectedDate.value?.toDateString()
    const dataStr = date.toDateString()
    
    if (dataStr === hoje && dataStr === selecionado) return 'today selected'
    if (dataStr === hoje) return 'today'
    if (dataStr === selecionado) return 'selected'
    return ''
  }

  const formatDate = (date) => {
    if (!date) return ''
    return date.toLocaleDateString('pt-BR')
  }

  const formatDateLong = (date) => {
    if (!date) return ''
    return date.toLocaleDateString('pt-BR', {
      weekday: 'long',
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    })
  }

  return {
    // Estado
    currentDate,
    viewType,
    selectedDate,
    
    // Constantes
    monthNames,
    weekDays,
    shortWeekDays,
    
    // Computed
    currentMonth,
    currentYear,
    currentMonthName,
    calendarWeeks,
    
    // Métodos
    goToPreviousMonth,
    goToNextMonth,
    goToToday,
    selectDate,
    getShiftsForDate,
    getDateClass,
    formatDate,
    formatDateLong
  }
}