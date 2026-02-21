// composables/despesas/useDespesasCharts.js
import { computed } from 'vue'
import { useDespesasStore } from '@/stores/despesas.store'
import { storeToRefs } from 'pinia'

export const useDespesasCharts = () => {
  const store = useDespesasStore()
  const { despesasFiltradas, categoriasComTotal } = storeToRefs(store)

  // Dados para gráfico de pizza por categoria
  const chartDataCategorias = computed(() => {
    const data = categoriasComTotal.value
      .filter(item => item.total > 0)
      .slice(0, 8) // Top 8 categorias
    
    return {
      labels: data.map(item => item.name),
      datasets: [{
        data: data.map(item => item.total),
        backgroundColor: [
          '#ef4444', '#f59e0b', '#3b82f6', '#10b981',
          '#8b5cf6', '#ec4899', '#6366f1', '#14b8a6'
        ]
      }]
    }
  })

  // Dados para gráfico de linha (evolução mensal)
  const chartDataEvolucaoMensal = computed(() => {
    const despesas = despesasFiltradas.value
    const meses = {}
    
    despesas.forEach(despesa => {
      const data = new Date(despesa.expenseDate || despesa.data)
      const mesAno = `${data.getMonth() + 1}/${data.getFullYear()}`
      
      if (!meses[mesAno]) {
        meses[mesAno] = 0
      }
      meses[mesAno] += despesa.amount || 0
    })
    
    // Ordena por data
    const sortedMeses = Object.keys(meses).sort((a, b) => {
      const [mesA, anoA] = a.split('/')
      const [mesB, anoB] = b.split('/')
      return new Date(anoA, mesA - 1) - new Date(anoB, mesB - 1)
    })
    
    return {
      labels: sortedMeses,
      datasets: [{
        label: 'Total de Despesas',
        data: sortedMeses.map(mes => meses[mes]),
        borderColor: '#ef4444',
        backgroundColor: 'rgba(239, 68, 68, 0.1)',
        tension: 0.4
      }]
    }
  })

  // Dados para gráfico de barras (comparativo por categoria)
  const chartDataComparativo = computed(() => {
    const data = categoriasComTotal.value
      .filter(item => item.quantidade > 0)
      .slice(0, 6) // Top 6
    
    return {
      labels: data.map(item => item.name),
      datasets: [
        {
          label: 'Valor Total (R$)',
          data: data.map(item => item.total),
          backgroundColor: '#3b82f6'
        },
        {
          label: 'Quantidade',
          data: data.map(item => item.quantidade),
          backgroundColor: '#10b981',
          yAxisID: 'y1'
        }
      ]
    }
  })

  // Estatísticas para cards do dashboard
  const cardStats = computed(() => {
    const despesas = despesasFiltradas.value
    const total = despesas.reduce((sum, d) => sum + (d.amount || 0), 0)
    const media = despesas.length > 0 ? total / despesas.length : 0
    
    return {
      total,
      media,
      quantidade: despesas.length,
      maior: Math.max(...despesas.map(d => d.amount || 0), 0),
      menor: Math.min(...despesas.map(d => d.amount || 0), Infinity)
    }
  })

  return {
    chartDataCategorias,
    chartDataEvolucaoMensal,
    chartDataComparativo,
    cardStats
  }
}