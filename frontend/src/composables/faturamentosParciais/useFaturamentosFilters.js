// composables/faturamentosParciais/useFaturamentosFilters.js
import { computed } from 'vue';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useFaturamentosFilters() {
  const store = useFaturamentosParciaisStore();
  
  const filtros = computed({
    get: () => store.filtros,
    set: (value) => store.aplicarFiltros(value)
  });
  
  const updateFilter = (key, value) => {
    store.aplicarFiltros({ [key]: value });
  };
  
  const limparFiltros = () => {
    store.limparFiltros();
  };
  
  const setPeriodo = (dataInicio, dataFim) => {
    store.aplicarFiltros({ dataInicio, dataFim });
  };
  
  const setMetodoPagamento = (metodoPagamento) => {
    store.aplicarFiltros({ metodoPagamento });
  };
  
  const setOrigem = (origem) => {
    store.aplicarFiltros({ origem });
  };
  
  const setBusca = (busca) => {
    store.aplicarFiltros({ busca });
  };
  
  const toggleStatus = (ativos, inativos) => {
    store.aplicarFiltros({ ativos, inativos });
  };
  
  const hasActiveFilters = computed(() => {
    const f = store.filtros;
    return !!(f.metodoPagamento || f.origem || f.busca || f.dataInicio || f.dataFim || !f.ativos || f.inativos);
  });
  
  return {
    filtros,
    updateFilter,
    limparFiltros,
    setPeriodo,
    setMetodoPagamento,
    setOrigem,
    setBusca,
    toggleStatus,
    hasActiveFilters,
  };
}