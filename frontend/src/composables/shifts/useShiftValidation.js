import { ref, computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'

export function useShiftValidation() {
  const store = useShiftsStore()
  
  const validationRules = ref({
    horarioMinimoIntervalo: 60, // minutos entre turnos
    cargaHorariaMaximaDiaria: 12, // horas
    cargaHorariaMaximaSemanal: 44, // horas
    diasConsecutivosMaximo: 6
  })

  const validateShift = (shiftData) => {
    const errors = []
    
    // Validar horários
    if (shiftData.type !== 7 && shiftData.startTime && shiftData.endTime) {
      const inicio = store.parseTimeToMinutes(shiftData.startTime)
      const fim = store.parseTimeToMinutes(shiftData.endTime)
      let duracao = fim - inicio
      
      if (duracao < 0) duracao += 24 * 60 // Passou da meia-noite
      
      // Verificar duração mínima (4 horas)
      if (duracao < 4 * 60) {
        errors.push('Turno deve ter no mínimo 4 horas de duração')
      }
      
      // Verificar duração máxima
      if (duracao > validationRules.value.cargaHorariaMaximaDiaria * 60) {
        errors.push(`Turno não pode exceder ${validationRules.value.cargaHorariaMaximaDiaria} horas`)
      }
    }
    
    return errors
  }

  const checkFuncionarioDisponivel = (userId, data, tipoTurno) => {
    const dataObj = new Date(data)
    const dataStr = dataObj.toISOString().split('T')[0]
    
    // Verificar se já tem turno no mesmo dia
    const turnoMesmoDia = store.shifts.some(s => 
      s.userId === userId && 
      new Date(s.data || s.startDate).toISOString().split('T')[0] === dataStr
    )
    
    if (turnoMesmoDia) {
      return {
        disponivel: false,
        motivo: 'Funcionário já possui turno nesta data'
      }
    }
    
    // Verificar descanso mínimo entre turnos
    const dataAnterior = new Date(dataObj)
    dataAnterior.setDate(dataAnterior.getDate() - 1)
    const dataAnteriorStr = dataAnterior.toISOString().split('T')[0]
    
    const turnoAnterior = store.shifts.find(s => 
      s.userId === userId && 
      new Date(s.data || s.startDate).toISOString().split('T')[0] === dataAnteriorStr
    )
    
    if (turnoAnterior && turnoAnterior.type !== 7) {
      // Verificar se é turno noturno (precisa de 24h de descanso)
      if (turnoAnterior.type === 3) {
        return {
          disponivel: false,
          motivo: 'Funcionário trabalhou no turno noturno ontem e precisa de 24h de descanso'
        }
      }
      
      // Verificar intervalo mínimo
      if (turnoAnterior.endTime) {
        const fimTurnoAnterior = store.parseTimeToMinutes(turnoAnterior.endTime)
        let descanso = 24 * 60 - fimTurnoAnterior // Minutos até meia-noite
        
        if (descanso < validationRules.value.horarioMinimoIntervalo) {
          return {
            disponivel: false,
            motivo: `Intervalo mínimo entre turnos é de ${validationRules.value.horarioMinimoIntervalo} minutos`
          }
        }
      }
    }
    
    return { disponivel: true }
  }

  const checkCargaHorariaSemanal = (userId, dataInicio) => {
    const dataInicioObj = new Date(dataInicio)
    const inicioSemana = new Date(dataInicioObj)
    inicioSemana.setDate(dataInicioObj.getDate() - dataInicioObj.getDay())
    const fimSemana = new Date(inicioSemana)
    fimSemana.setDate(inicioSemana.getDate() + 6)
    
    const shiftsSemana = store.shifts.filter(s => 
      s.userId === userId &&
      s.type !== 7 && // Não contar folgas
      new Date(s.data || s.startDate) >= inicioSemana &&
      new Date(s.data || s.startDate) <= fimSemana
    )
    
    let totalHoras = 0
    shiftsSemana.forEach(shift => {
      if (shift.startTime && shift.endTime) {
        const inicio = store.parseTimeToMinutes(shift.startTime)
        const fim = store.parseTimeToMinutes(shift.endTime)
        let horas = (fim - inicio) / 60
        if (horas < 0) horas += 24
        totalHoras += horas
      }
    })
    
    return {
      dentroDoLimite: totalHoras <= validationRules.value.cargaHorariaMaximaSemanal,
      horasAtuais: totalHoras,
      limite: validationRules.value.cargaHorariaMaximaSemanal,
      horasRestantes: Math.max(0, validationRules.value.cargaHorariaMaximaSemanal - totalHoras)
    }
  }

  const checkDiasConsecutivos = (userId, data) => {
    const dataObj = new Date(data)
    let diasConsecutivos = 1
    
    // Verificar dias anteriores
    for (let i = 1; i <= 7; i++) {
      const dataCheck = new Date(dataObj)
      dataCheck.setDate(dataObj.getDate() - i)
      const dataCheckStr = dataCheck.toISOString().split('T')[0]
      
      const temTurno = store.shifts.some(s => 
        s.userId === userId &&
        s.type !== 7 && // Não contar folgas
        new Date(s.data || s.startDate).toISOString().split('T')[0] === dataCheckStr
      )
      
      if (temTurno) {
        diasConsecutivos++
      } else {
        break
      }
    }
    
    // Verificar dias posteriores
    for (let i = 1; i <= 7; i++) {
      const dataCheck = new Date(dataObj)
      dataCheck.setDate(dataObj.getDate() + i)
      const dataCheckStr = dataCheck.toISOString().split('T')[0]
      
      const temTurno = store.shifts.some(s => 
        s.userId === userId &&
        s.type !== 7 &&
        new Date(s.data || s.startDate).toISOString().split('T')[0] === dataCheckStr
      )
      
      if (temTurno) {
        diasConsecutivos++
      } else {
        break
      }
    }
    
    return {
      dentroDoLimite: diasConsecutivos <= validationRules.value.diasConsecutivosMaximo,
      diasAtuais: diasConsecutivos,
      limite: validationRules.value.diasConsecutivosMaximo
    }
  }

  return {
    validationRules,
    validateShift,
    checkFuncionarioDisponivel,
    checkCargaHorariaSemanal,
    checkDiasConsecutivos
  }
}