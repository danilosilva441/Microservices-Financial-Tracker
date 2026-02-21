import { computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'

export function useShiftsStats() {
  const store = useShiftsStore()

  const estatisticas = computed(() => store.estatisticas)

  const horasPorTipoTurno = computed(() => {
    const stats = {}
    
    store.shiftsEsteMes.forEach(shift => {
      const tipo = shift.type
      if (!stats[tipo]) {
        stats[tipo] = {
          tipo,
          nome: store.getNomeTipo(tipo),
          cor: store.getCorTipo(tipo),
          totalShifts: 0,
          totalHoras: 0
        }
      }
      
      stats[tipo].totalShifts++
      
      if (shift.startTime && shift.endTime && shift.type !== 7) {
        const inicio = store.parseTimeToMinutes(shift.startTime)
        const fim = store.parseTimeToMinutes(shift.endTime)
        let horas = (fim - inicio) / 60
        if (horas < 0) horas += 24
        
        // Subtrai intervalos
        const intervalos = store.breaks.filter(b => b.shiftId === shift.id)
        intervalos.forEach(intervalo => {
          const inicioIntervalo = store.parseTimeToMinutes(intervalo.startTime)
          const fimIntervalo = store.parseTimeToMinutes(intervalo.endTime)
          horas -= (fimIntervalo - inicioIntervalo) / 60
        })
        
        stats[tipo].totalHoras += horas
      }
    })
    
    return Object.values(stats)
  })

  const horasPorFuncionario = computed(() => {
    const stats = []
    
    store.funcionariosAtivos?.forEach(func => {
      const horas = store.calcularHorasFuncionario(func.id)
      stats.push({
        funcionario: func,
        ...horas
      })
    })
    
    return stats.sort((a, b) => b.totalHoras - a.totalHoras)
  })

  const distribuicaoTurnos = computed(() => {
    const distribuicao = {
      manha: 0,
      tarde: 0,
      noite: 0,
      integral: 0,
      escala: 0,
      folga: 0
    }
    
    store.shiftsEsteMes.forEach(shift => {
      switch (shift.type) {
        case 1: distribuicao.manha++; break
        case 2: distribuicao.tarde++; break
        case 3: distribuicao.noite++; break
        case 4: distribuicao.integral++; break
        case 5:
        case 6: distribuicao.escala++; break
        case 7:
        case 8:
        case 9: distribuicao.folga++; break
      }
    })
    
    return distribuicao
  })

  const diasComMaisTurnos = computed(() => {
    const turnosPorDia = {}
    
    store.shiftsEsteMes.forEach(shift => {
      const data = new Date(shift.data || shift.startDate).toISOString().split('T')[0]
      turnosPorDia[data] = (turnosPorDia[data] || 0) + 1
    })
    
    return Object.entries(turnosPorDia)
      .map(([data, total]) => ({ data, total }))
      .sort((a, b) => b.total - a.total)
      .slice(0, 5)
  })

  return {
    estatisticas,
    horasPorTipoTurno,
    horasPorFuncionario,
    distribuicaoTurnos,
    diasComMaisTurnos
  }
}