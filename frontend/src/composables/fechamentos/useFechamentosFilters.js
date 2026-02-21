import { ref, computed, watch } from 'vue';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useFechamentosFilters() {
  const fechamentosStore = useFechamentosStore();
  
  const localFilters = ref({
    dataInicio: null,
    dataFim: null,
    status: '',
    busca: '',
  });

  const statusOptions = [
    { value: '', label: 'Todos' },
    { value: 'aberto', label: 'Abertos', color: 'blue' },
    { value: 'fechado', label: 'Fechados', color: 'orange' },
    { value: 'conferido', label: 'Conferidos', color: 'green' },
    { value: 'pendente', label: 'Pendentes', color: 'red' },
  ];

  const dateRangeOptions = [
    { value: 'hoje', label: 'Hoje' },
    { value: 'ontem', label: 'Ontem' },
    { value: 'esta-semana', label: 'Esta semana' },
    { value: 'este-mes', label: 'Este mês' },
    { value: 'mes-passado', label: 'Mês passado' },
    { value: 'personalizado', label: 'Personalizado' },
  ];

  const selectedDateRange = ref('este-mes');
  const showDatePicker = computed(() => selectedDateRange.value === 'personalizado');

  const hasActiveFilters = computed(() => {
    return localFilters.value.status !== '' || 
           localFilters.value.busca !== '' ||
           localFilters.value.dataInicio !== null ||
           localFilters.value.dataFim !== null;
  });

  const aplicarFiltros = () => {
    fechamentosStore.aplicarFiltros(localFilters.value);
  };

  const limparFiltros = () => {
    localFilters.value = {
      dataInicio: null,
      dataFim: null,
      status: '',
      busca: '',
    };
    selectedDateRange.value = 'este-mes';
    fechamentosStore.limparFiltros();
  };

  const aplicarDateRange = (range) => {
    const hoje = new Date();
    let dataInicio = null;
    let dataFim = hoje.toISOString().split('T')[0];

    switch (range) {
      case 'hoje':
        dataInicio = hoje.toISOString().split('T')[0];
        break;
      case 'ontem':
        const ontem = new Date(hoje);
        ontem.setDate(ontem.getDate() - 1);
        dataInicio = ontem.toISOString().split('T')[0];
        dataFim = dataInicio;
        break;
      case 'esta-semana':
        const inicioSemana = new Date(hoje);
        inicioSemana.setDate(hoje.getDate() - hoje.getDay());
        dataInicio = inicioSemana.toISOString().split('T')[0];
        break;
      case 'este-mes':
        dataInicio = new Date(hoje.getFullYear(), hoje.getMonth(), 1)
          .toISOString().split('T')[0];
        break;
      case 'mes-passado':
        dataInicio = new Date(hoje.getFullYear(), hoje.getMonth() - 1, 1)
          .toISOString().split('T')[0];
        dataFim = new Date(hoje.getFullYear(), hoje.getMonth(), 0)
          .toISOString().split('T')[0];
        break;
      default:
        return;
    }

    localFilters.value.dataInicio = dataInicio;
    localFilters.value.dataFim = dataFim;
    aplicarFiltros();
  };

  const buscarPorData = async (unidadeId) => {
    if (localFilters.value.dataInicio && localFilters.value.dataFim) {
      return await fechamentosStore.buscarPorData(
        unidadeId,
        localFilters.value.dataInicio,
        localFilters.value.dataFim
      );
    }
  };

  return {
    filters: localFilters,
    statusOptions,
    dateRangeOptions,
    selectedDateRange,
    showDatePicker,
    hasActiveFilters,
    aplicarFiltros,
    limparFiltros,
    aplicarDateRange,
    buscarPorData,
  };
}