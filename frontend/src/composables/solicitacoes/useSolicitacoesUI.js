import { ref, computed } from 'vue';
import { useSolicitacoesStore } from '@/stores/solicitacoes.store';
import { storeToRefs } from 'pinia';

export function useSolicitacoesUI() {
  const store = useSolicitacoesStore();
  const { editando, isLoading } = storeToRefs(store);

  // Estado local da UI
  const modalAberto = ref(false);
  const modalTipo = ref(null); // 'criar', 'detalhes', 'revisar'
  const modalDados = ref(null);
  const modoVisualizacao = ref('lista'); // 'lista', 'grid', 'dashboard'
  const itensPorPagina = ref(10);
  const paginaAtual = ref(1);
  const solicitacaoSelecionada = ref(null);

  const abrirModal = (tipo, dados = null) => {
    modalTipo.value = tipo;
    modalDados.value = dados;
    modalAberto.value = true;
  };

  const fecharModal = () => {
    modalAberto.value = false;
    modalTipo.value = null;
    modalDados.value = null;
  };

  const selecionarSolicitacao = (solicitacao) => {
    solicitacaoSelecionada.value = solicitacao;
  };

  const getStatusClasses = (status) => {
    const classes = {
      PENDENTE: 'bg-yellow-100 text-yellow-800',
      APROVADA: 'bg-green-100 text-green-800',
      REJEITADA: 'bg-red-100 text-red-800',
      CANCELADA: 'bg-gray-100 text-gray-800',
      PROCESSANDO: 'bg-blue-100 text-blue-800',
    };
    return classes[status] || 'bg-gray-100 text-gray-800';
  };

  const getStatusIcon = (status) => {
    const icons = {
      PENDENTE: 'pi pi-clock',
      APROVADA: 'pi pi-check-circle',
      REJEITADA: 'pi pi-times-circle',
      CANCELADA: 'pi pi-ban',
      PROCESSANDO: 'pi pi-spinner pi-spin',
    };
    return icons[status] || 'pi pi-question-circle';
  };

  const getTipoIcon = (tipo) => {
    const icons = {
      AJUSTE_VALOR: 'pi pi-dollar',
      AJUSTE_METODO_PAGAMENTO: 'pi pi-credit-card',
      AJUSTE_ORIGEM: 'pi pi-map-marker',
      AJUSTE_HORARIO: 'pi pi-clock',
      EXCLUSAO_LANCAMENTO: 'pi pi-trash',
      OUTROS: 'pi pi-file',
    };
    return icons[tipo] || 'pi pi-file';
  };

  const formatarDataParaExibicao = (data) => {
    return store.formatarData(data);
  };

  const formatarDataRelativa = (data) => {
    return store.formatarDataRelativa(data);
  };

  // Paginação
  const totalPaginas = computed(() => {
    return Math.ceil(store.solicitacoesFiltradas.length / itensPorPagina.value);
  });

  const itensPaginados = computed(() => {
    const inicio = (paginaAtual.value - 1) * itensPorPagina.value;
    const fim = inicio + itensPorPagina.value;
    return store.solicitacoesFiltradas.slice(inicio, fim);
  });

  const mudarPagina = (pagina) => {
    if (pagina >= 1 && pagina <= totalPaginas.value) {
      paginaAtual.value = pagina;
    }
  };

  return {
    // Estado
    modalAberto,
    modalTipo,
    modalDados,
    modoVisualizacao,
    itensPorPagina,
    paginaAtual,
    solicitacaoSelecionada,
    editando,
    isLoading,
    
    // Getters
    totalPaginas,
    itensPaginados,
    
    // Métodos
    abrirModal,
    fecharModal,
    selecionarSolicitacao,
    getStatusClasses,
    getStatusIcon,
    getTipoIcon,
    formatarDataParaExibicao,
    formatarDataRelativa,
    mudarPagina,
  };
}