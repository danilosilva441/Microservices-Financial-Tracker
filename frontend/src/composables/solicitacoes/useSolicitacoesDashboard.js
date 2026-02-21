import { computed } from 'vue';
import { useSolicitacoesStore } from '@/stores/solicitacoes.store';
import { storeToRefs } from 'pinia';

export function useSolicitacoesDashboard() {
  const store = useSolicitacoesStore();
  const { dashboard, estatisticas } = storeToRefs(store);

  const dadosGraficoStatus = computed(() => {
    return {
      labels: ['Pendentes', 'Aprovadas', 'Rejeitadas'],
      datasets: [
        {
          data: [
            estatisticas.value.pendentes,
            estatisticas.value.aprovadas,
            estatisticas.value.rejeitadas,
          ],
          backgroundColor: ['#ff9800', '#4caf50', '#f44336'],
        },
      ],
    };
  });

  const dadosGraficoPorTipo = computed(() => {
    const tipos = store.solicitacoesPorTipo;
    return {
      labels: Object.keys(tipos),
      datasets: [
        {
          data: Object.values(tipos).map(t => t.total),
          backgroundColor: [
            '#2196f3',
            '#4caf50',
            '#ff9800',
            '#9c27b0',
            '#f44336',
          ],
        },
      ],
    };
  });

  const dadosGraficoTendencia = computed(() => {
    const ultimos7Dias = [];
    const datas = [];
    
    for (let i = 6; i >= 0; i--) {
      const data = new Date();
      data.setDate(data.getDate() - i);
      datas.push(data.toLocaleDateString('pt-BR'));
      
      const solicitacoesDia = store.solicitacoes.filter(s => {
        const dataSolicitacao = new Date(s.dataSolicitacao || s.createdAt);
        return dataSolicitacao.toDateString() === data.toDateString();
      }).length;
      
      ultimos7Dias.push(solicitacoesDia);
    }

    return {
      labels: datas,
      datasets: [
        {
          label: 'Solicitações por dia',
          data: ultimos7Dias,
          borderColor: '#2196f3',
          tension: 0.1,
        },
      ],
    };
  });

  const metricasResumo = computed(() => {
    return [
      {
        titulo: 'Total de Solicitações',
        valor: estatisticas.value.totalSolicitacoes,
        icone: 'pi pi-list',
        cor: 'bg-blue-500',
      },
      {
        titulo: 'Pendentes',
        valor: estatisticas.value.pendentes,
        icone: 'pi pi-clock',
        cor: 'bg-yellow-500',
      },
      {
        titulo: 'Aprovadas',
        valor: estatisticas.value.aprovadas,
        icone: 'pi pi-check-circle',
        cor: 'bg-green-500',
      },
      {
        titulo: 'Taxa de Aprovação',
        valor: `${estatisticas.value.taxaAprovacao.toFixed(1)}%`,
        icone: 'pi pi-chart-line',
        cor: 'bg-purple-500',
      },
    ];
  });

  const tempoMedioFormatado = computed(() => {
    const horas = estatisticas.value.tempoMedioResolucao;
    if (horas < 24) {
      return `${horas.toFixed(1)} horas`;
    } else {
      const dias = (horas / 24).toFixed(1);
      return `${dias} dias`;
    }
  });

  return {
    dashboard,
    estatisticas,
    dadosGraficoStatus,
    dadosGraficoPorTipo,
    dadosGraficoTendencia,
    metricasResumo,
    tempoMedioFormatado,
    
    // Métodos
    atualizarDashboard: store.atualizarDashboard,
    calcularEstatisticas: store.calcularEstatisticas,
  };
}