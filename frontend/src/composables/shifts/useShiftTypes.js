import { computed } from 'vue'
import { ShiftTypeEnum } from '@/stores/shifts.store'

export function useShiftTypes() {
  const tipos = computed(() => ShiftTypeEnum.getAll())

  const getTipoById = (id) => {
    return tipos.value.find(t => t.id === id) || null
  }

  const getNomeTipo = (id) => {
    return ShiftTypeEnum.getNome(id)
  }

  const getCorTipo = (id) => {
    return ShiftTypeEnum.getCor(id)
  }

  const getCorTextoTipo = (id) => {
    return ShiftTypeEnum.getCorTexto(id)
  }

  const getHorarioPadrao = (id) => {
    return ShiftTypeEnum.getHorarioPadrao(id)
  }

  const getTiposPorCategoria = computed(() => {
    return {
      trabalho: tipos.value.filter(t => ![7, 8, 9].includes(t.id)),
      folga: tipos.value.filter(t => [7, 8, 9].includes(t.id)),
      outros: tipos.value.filter(t => t.id === 10)
    }
  })

  return {
    tipos,
    getTipoById,
    getNomeTipo,
    getCorTipo,
    getCorTextoTipo,
    getHorarioPadrao,
    getTiposPorCategoria,
    ShiftTypeEnum
  }
}