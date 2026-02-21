import { computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useDashboardFechamentos() {
  const fechamentosStore = useFechamentosStore();
  
  const {
    dashboard,
    estatisticas,
    fechamentosAbertosHoje,
    fechamentosFechadosPendentes,
    faturamentoTotalMes,
    distribuicaoPorStatus,
  } = storeToRefs(fechamentosStore);

  const cardsResumo = computed(() => [
    {
      title: 'Caixas Abertos Hoje',
      value: dashboard.value.caixasAbertosHoje,
      icon: 'CashIcon',
      color: 'blue',
      change: null,
    },
    {
      title: 'Pendentes Conferência',
      value: dashboard.value.caixasPendentesConferencia,
      icon: 'ClockIcon',
      color: 'orange',
      change: null,
    },
    {
      title: 'Faturamento Hoje',
      value: dashboard.value.faturamentoHoje,
      icon: 'TrendingUpIcon',
      color: 'green',
      isCurrency: true,
      change: null,
    },
    {
      title: 'Faturamento Mês',
      value: dashboard.value.faturamentoMes,
      icon: 'CurrencyDollarIcon',
      color: 'purple',
      isCurrency: true,
      change: '+12.5%',
    },
  ]);

  const graficoDistribuicao = computed(() => ({
    labels: ['Abertos', 'Fechados', 'Conferidos', 'Pendentes'],
    datasets: [
      {
        data: [
          distribuicaoPorStatus.value.abertos,
          distribuicaoPorStatus.value.fechados,
          distribuicaoPorStatus.value.conferidos,
          distribuicaoPorStatus.value.pendentes,
        ],
        backgroundColor: ['#3B82F6', '#F97316', '#22C55E', '#EF4444'],
      },
    ],
  }));

  const estatisticasResumo = computed(() => [
    {
      label: 'Total Faturamento',
      value: estatisticas.value.totalFaturamento,
      isCurrency: true,
    },
    {
      label: 'Média Diária',
      value: estatisticas.value.mediaDiaria,
      isCurrency: true,
    },
    {
      label: 'Dias Úteis',
      value: estatisticas.value.diasUteis,
    },
    {
      label: 'Maior Faturamento',
      value: estatisticas.value.maiorFaturamento,
      isCurrency: true,
    },
    {
      label: 'Menor Faturamento',
      value: estatisticas.value.menorFaturamento,
      isCurrency: true,
    },
    {
      label: 'Total Diferenças',
      value: estatisticas.value.totalDiferencas,
      isCurrency: true,
      isNegative: true,
    },
  ]);

  const atualizarDashboard = () => {
    fechamentosStore.atualizarDashboard();
  };

  return {
    // Data
    dashboard,
    cardsResumo,
    graficoDistribuicao,
    estatisticasResumo,
    fechamentosAbertosHoje,
    fechamentosFechadosPendentes,
    
    // Actions
    atualizarDashboard,
  };
}