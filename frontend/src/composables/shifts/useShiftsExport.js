import { ref } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'
import { useToast } from '../unidades/useToast'

export function useShiftsExport() {
  const store = useShiftsStore()
  const toast = useToast()
  
  const exporting = ref(false)
  const exportFormat = ref('csv') // 'csv', 'excel', 'pdf'
  const exportPeriod = ref('mes') // 'mes', 'semana', 'custom'
  const exportDateRange = ref({
    inicio: null,
    fim: null
  })

  const exportData = async () => {
    exporting.value = true

    try {
      let dataToExport = store.shiftsFiltrados

      // Filtrar por período se necessário
      if (exportPeriod.value === 'mes') {
        const hoje = new Date()
        const inicio = new Date(hoje.getFullYear(), hoje.getMonth(), 1)
        const fim = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0)
        dataToExport = dataToExport.filter(s => {
          const data = new Date(s.data || s.startDate)
          return data >= inicio && data <= fim
        })
      } else if (exportPeriod.value === 'semana') {
        const hoje = new Date()
        const inicio = new Date(hoje)
        inicio.setDate(hoje.getDate() - hoje.getDay())
        const fim = new Date(inicio)
        fim.setDate(inicio.getDate() + 6)
        dataToExport = dataToExport.filter(s => {
          const data = new Date(s.data || s.startDate)
          return data >= inicio && data <= fim
        })
      } else if (exportPeriod.value === 'custom' && exportDateRange.value.inicio && exportDateRange.value.fim) {
        dataToExport = dataToExport.filter(s => {
          const data = new Date(s.data || s.startDate)
          return data >= new Date(exportDateRange.value.inicio) && 
                 data <= new Date(exportDateRange.value.fim)
        })
      }

      // Formatar dados para exportação
      const formattedData = dataToExport.map(shift => ({
        data: store.formatarDataCompleta(shift.data || shift.startDate),
        funcionario: shift.user?.nome || 'Não atribuído',
        tipo: store.getNomeTipo(shift.type),
        horario: shift.type !== 7 ? `${shift.startTime} - ${shift.endTime}` : '-',
        unidade: shift.unidadeId,
        status: shift.isAtivo ? 'Ativo' : 'Inativo',
        observacoes: shift.observacoes || ''
      }))

      // Simular exportação baseada no formato
      await new Promise(resolve => setTimeout(resolve, 1000))

      if (exportFormat.value === 'csv') {
        await exportToCSV(formattedData)
      } else if (exportFormat.value === 'excel') {
        await exportToExcel(formattedData)
      } else if (exportFormat.value === 'pdf') {
        await exportToPDF(formattedData)
      }

      toast.show('Exportação concluída com sucesso!', 'success')
      return { success: true }
    } catch (error) {
      console.error('Erro ao exportar dados:', error)
      toast.show('Erro ao exportar dados', 'error')
      return { success: false, error: error.message }
    } finally {
      exporting.value = false
    }
  }

  const exportToCSV = async (data) => {
    if (data.length === 0) {
      toast.show('Nenhum dado para exportar', 'warning')
      return
    }

    const headers = Object.keys(data[0]).join(',')
    const rows = data.map(row => Object.values(row).join(',')).join('\n')
    const csv = `${headers}\n${rows}`

    const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)
    
    link.href = url
    link.setAttribute('download', `escalas_${new Date().toISOString().split('T')[0]}.csv`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
  }

  const exportToExcel = async (data) => {
    // Em produção, usaria uma biblioteca como xlsx
    // Simulação
    console.log('Exportando para Excel:', data)
    toast.show('Exportação para Excel simulada', 'info')
  }

  const exportToPDF = async (data) => {
    // Em produção, usaria uma biblioteca como jspdf
    // Simulação
    console.log('Exportando para PDF:', data)
    toast.show('Exportação para PDF simulada', 'info')
  }

  const printSchedule = () => {
    window.print()
  }

  return {
    exporting,
    exportFormat,
    exportPeriod,
    exportDateRange,
    exportData,
    printSchedule
  }
}