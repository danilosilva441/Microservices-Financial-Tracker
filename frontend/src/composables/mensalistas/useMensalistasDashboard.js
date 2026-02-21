// composables/mensalistas/useMensalistasDashboard.js
import { computed } from 'vue';

export function useMensalistasDashboard(store) {
  const estatisticas = computed(() => store.estatisticas);
  const dashboard = computed(() => store.dashboard);
  const receitaMensalProjetada = computed(() => store.receitaMensalProjetada);
  const mensalistasPorEmpresa = computed(() => store.mensalistasPorEmpresa);
  const distribuicaoPorValor = computed(() => store.distribuicaoPorValor);

  const calcularEstatisticas = () => {
    store.calcularEstatisticas();
  };

  const atualizarDashboard = () => {
    store.atualizarDashboard();
  };

  const cardsDashboard = computed(() => [
    {
      title: 'Mensalistas Ativos',
      value: dashboard.value.mensalistasAtivos,
      icon: 'people',
      color: 'primary',
      trend: '+12%',
    },
    {
      title: 'Receita Mensal',
      value: formatarValor(dashboard.value.receitaMensalProjetada),
      icon: 'attach-money',
      color: 'success',
      trend: '+8%',
    },
    {
      title: 'Vencimentos',
      value: dashboard.value.mensalidadesVencendo,
      icon: 'event',
      color: 'warning',
      trend: '-3',
    },
    {
      title: 'Novos Cadastros',
      value: dashboard.value.novosCadastrosMes,
      icon: 'person-add',
      color: 'info',
      trend: '+5',
    },
  ]);

  const formatarValor = (valor) => {
    return store.formatarValor(valor);
  };

  return {
    estatisticas,
    dashboard,
    receitaMensalProjetada,
    mensalistasPorEmpresa,
    distribuicaoPorValor,
    cardsDashboard,
    calcularEstatisticas,
    atualizarDashboard,
  };
}