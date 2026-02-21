import { computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'

export function useShiftsDashboard() {
  const store = useShiftsStore()

  const dashboard = computed(() => store.dashboard)

  const proximosEventos = computed(() => {
    const eventos = []
    const hoje = new Date()
    
    // Próximas folgas
    store.shifts
      .filter(s => s.type === 7 && new Date(s.data || s.startDate) >= hoje)
      .slice(0, 5)
      .forEach(shift => {
        eventos.push({
          tipo: 'folga',
          data: shift.data || shift.startDate,
          funcionario: shift.user,
          descricao: 'Folga'
        })
      })
    
    // Próximas férias
    store.shifts
      .filter(s => s.type === 8 && new Date(s.data || s.startDate) >= hoje)
      .slice(0, 5)
      .forEach(shift => {
        eventos.push({
          tipo: 'ferias',
          data: shift.data || shift.startDate,
          funcionario: shift.user,
          descricao: 'Férias'
        })
      })
    
    return eventos.sort((a, b) => new Date(a.data) - new Date(b.data))
  })

  const alertas = computed(() => {
    const alerts = []
    const hoje = new Date()
    const amanha = new Date(hoje)
    amanha.setDate(hoje.getDate() + 1)
    
    // Escalas sem funcionário
    const pendentes = store.escalasPendentes
    if (pendentes.length > 0) {
      alerts.push({
        tipo: 'warning',
        titulo: 'Escalas pendentes',
        mensagem: `${pendentes.length} escala(s) sem funcionário atribuído`,
        acao: 'visualizar'
      })
    }
    
    // Funcionários próximos de atingir limite de horas
    const horasPorFunc = store.calcularHorasFuncionario
    store.funcionariosAtivos?.forEach(func => {
      const horas = horasPorFunc(func.id)
      if (horas.totalHoras > 40) { // Mais de 40 horas no mês
        alerts.push({
          tipo: 'info',
          titulo: 'Limite de horas próximo',
          mensagem: `${func.nome} já acumulou ${horas.totalHoras.toFixed(1)} horas`,
          acao: 'ver_detalhes'
        })
      }
    })
    
    // Turnos não preenchidos para amanhã
    const turnosAmanha = store.shifts.filter(s => {
      const dataShift = new Date(s.data || s.startDate).toDateString()
      return dataShift === amanha.toDateString() && !s.userId
    })
    
    if (turnosAmanha.length > 0) {
      alerts.push({
        tipo: 'error',
        titulo: 'Turnos críticos',
        mensagem: `${turnosAmanha.length} turno(s) para amanhã sem funcionário`,
        acao: 'atribuir'
      })
    }
    
    return alerts
  })

  const resumoMensal = computed(() => {
    return {
      totalTurnos: store.estatisticas.totalTurnosMes,
      totalHoras: store.estatisticas.totalHorasTrabalhadas,
      mediaDiaria: (store.estatisticas.totalHorasTrabalhadas / 30).toFixed(1),
      coberturaMedia: store.estatisticas.coberturaMedia.toFixed(1),
      funcionariosAtivos: store.estatisticas.funcionariosAtivos
    }
  })

  return {
    dashboard,
    proximosEventos,
    alertas,
    resumoMensal
  }
}