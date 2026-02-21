import { storeToRefs } from 'pinia';
import { useSolicitacoesStore } from '@/stores/solicitacoes.store';
import { computed } from 'vue';

export function useSolicitacoesFilters() {
  const store = useSolicitacoesStore();
  const { filtros } = storeToRefs(store);

  const aplicarFiltros = (novosFiltros) => {
    store.aplicarFiltros(novosFiltros);
  };

  const limparFiltros = () => {
    store.limparFiltros();
  };

  const toggleApenasMinhas = () => {
    store.toggleApenasMinhas();
  };

  const filtrosAtivos = computed(() => {
    const ativos = [];
    if (filtros.value.status) ativos.push(`Status: ${store.getNomeStatus(filtros.value.status)}`);
    if (filtros.value.tipo) ativos.push(`Tipo: ${store.getNomeTipo(filtros.value.tipo)}`);
    if (filtros.value.dataInicio) ativos.push(`De: ${new Date(filtros.value.dataInicio).toLocaleDateString()}`);
    if (filtros.value.dataFim) ativos.push(`Até: ${new Date(filtros.value.dataFim).toLocaleDateString()}`);
    if (filtros.value.busca) ativos.push(`Busca: ${filtros.value.busca}`);
    if (filtros.value.apenasMinhas) ativos.push('Apenas minhas solicitações');
    return ativos;
  });

  const temFiltrosAtivos = computed(() => {
    return Object.values(filtros.value).some(value => 
      value !== '' && value !== null && value !== false && value !== 'data_desc'
    );
  });

  return {
    filtros,
    filtrosAtivos,
    temFiltrosAtivos,
    aplicarFiltros,
    limparFiltros,
    toggleApenasMinhas,
  };
}