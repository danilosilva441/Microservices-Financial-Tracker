// composables/mensalistas/useMensalistasFilters.js
import { computed } from 'vue';

export function useMensalistasFilters(store) {
  const mensalistasFiltrados = computed(() => store.mensalistasFiltrados);
  const mensalistasAtivos = computed(() => store.mensalistasAtivos);
  const mensalistasInativos = computed(() => store.mensalistasInativos);
  const mensalistasAtencao = computed(() => store.mensalistasAtencao);
  const proximosVencimentos = computed(() => store.proximosVencimentos);
  const novosCadastrosEsteMes = computed(() => store.novosCadastrosEsteMes);

  const aplicarFiltros = (filtros) => {
    store.aplicarFiltros(filtros);
  };

  const limparFiltros = () => {
    store.limparFiltros();
  };

  const filtrarPorStatus = (ativos = true, inativos = false) => {
    store.aplicarFiltros({ ativos, inativos });
  };

  const filtrarPorEmpresa = (empresaId) => {
    store.aplicarFiltros({ empresaId });
  };

  const filtrarPorValor = (valorMin, valorMax) => {
    store.aplicarFiltros({ valorMin, valorMax });
  };

  const buscarPorTexto = (busca) => {
    store.aplicarFiltros({ busca });
  };

  const ordenarPor = (ordenacao) => {
    store.aplicarFiltros({ ordenacao });
  };

  const ordenacaoOptions = [
    { value: 'nome_asc', label: 'Nome (A-Z)' },
    { value: 'nome_desc', label: 'Nome (Z-A)' },
    { value: 'valor_asc', label: 'Valor (menor para maior)' },
    { value: 'valor_desc', label: 'Valor (maior para menor)' },
    { value: 'data_cadastro_desc', label: 'Mais recentes' },
  ];

  return {
    mensalistasFiltrados,
    mensalistasAtivos,
    mensalistasInativos,
    mensalistasAtencao,
    proximosVencimentos,
    novosCadastrosEsteMes,
    aplicarFiltros,
    limparFiltros,
    filtrarPorStatus,
    filtrarPorEmpresa,
    filtrarPorValor,
    buscarPorTexto,
    ordenarPor,
    ordenacaoOptions,
  };
}