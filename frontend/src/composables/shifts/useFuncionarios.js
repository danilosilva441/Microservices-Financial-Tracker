import { ref, computed } from 'vue'
import { useShiftsStore } from '@/stores/shifts.store'

export function useFuncionarios() {
  const store = useShiftsStore()
  
  const searchTerm = ref('')
  const selectedFuncionario = ref(null)

  const funcionarios = computed(() => store.funcionarios || [])

  const funcionariosAtivos = computed(() => {
    return funcionarios.value.filter(f => f.isAtivo !== false)
  })

  const funcionariosInativos = computed(() => {
    return funcionarios.value.filter(f => f.isAtivo === false)
  })

  const funcionariosFiltrados = computed(() => {
    if (!searchTerm.value) return funcionariosAtivos.value
    
    const term = searchTerm.value.toLowerCase()
    return funcionariosAtivos.value.filter(f => 
      f.nome?.toLowerCase().includes(term) ||
      f.email?.toLowerCase().includes(term) ||
      f.cargo?.toLowerCase().includes(term)
    )
  })

  const getFuncionarioById = (id) => {
    return funcionarios.value.find(f => f.id === id) || null
  }

  const getShiftsByFuncionario = (userId) => {
    return store.shifts.filter(s => s.userId === userId)
  }

  const getHorasTrabalhadas = (userId, periodo) => {
    return store.calcularHorasFuncionario(userId, periodo?.inicio, periodo?.fim)
  }

  const getDisponibilidade = (userId, data) => {
    return store.verificarDisponibilidadeFuncionario(userId, data)
  }

  const carregarFuncionarios = async (unidadeId) => {
    return await store.carregarFuncionarios(unidadeId)
  }

  const selectFuncionario = (funcionario) => {
    selectedFuncionario.value = funcionario
  }

  return {
    funcionarios,
    funcionariosAtivos,
    funcionariosInativos,
    funcionariosFiltrados,
    searchTerm,
    selectedFuncionario,
    getFuncionarioById,
    getShiftsByFuncionario,
    getHorasTrabalhadas,
    getDisponibilidade,
    carregarFuncionarios,
    selectFuncionario
  }
}