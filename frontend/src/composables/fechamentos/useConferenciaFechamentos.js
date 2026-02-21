import { ref, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useConferenciaFechamentos() {
  const fechamentosStore = useFechamentosStore();
  
  const {
    fechamentosPendentes,
    fechamentosFechadosPendentes,
  } = storeToRefs(fechamentosStore);

  const conferenciaData = ref({
    aprovado: true,
    observacoes: '',
    diferencaAceita: false,
    valoresConferidos: {},
  });

  const isConferindo = ref(false);
  const stepAtual = ref('lista'); // 'lista', 'conferencia', 'revisao', 'finalizado'
  const fechamentoSelecionado = ref(null);

  const itemsConferencia = ref([]);

  const progressoConferencia = computed(() => {
    if (itemsConferencia.value.length === 0) return 0;
    const conferidos = itemsConferencia.value.filter(item => item.conferido).length;
    return Math.round((conferidos / itemsConferencia.value.length) * 100);
  });

  const totalDiferencasPendentes = computed(() => {
    return fechamentosPendentes.value.reduce((total, f) => total + Math.abs(f.diferenca || 0), 0);
  });

  const carregarFechamentosPendentes = async () => {
    return await fechamentosStore.carregarFechamentosPendentes();
  };

  const selecionarFechamento = (fechamento) => {
    fechamentoSelecionado.value = fechamento;
    stepAtual.value = 'conferencia';
    
    // Preparar itens para conferência
    itemsConferencia.value = [
      {
        id: 'valorTotal',
        label: 'Valor Total',
        valorEsperado: fechamento.valorTotal,
        valorConferido: fechamento.valorTotal,
        conferido: false,
        ok: true,
      },
      {
        id: 'quantidadeVendas',
        label: 'Quantidade de Vendas',
        valorEsperado: fechamento.quantidadeVendas || 0,
        valorConferido: fechamento.quantidadeVendas || 0,
        conferido: false,
        ok: true,
      },
      ...(fechamento.movimentacoes || []).map((mov, index) => ({
        id: `mov-${index}`,
        label: `Movimentação: ${mov.tipo}`,
        valorEsperado: mov.valor,
        valorConferido: mov.valor,
        conferido: false,
        ok: true,
      })),
    ];
  };

  const atualizarValorConferido = (itemId, valor) => {
    const item = itemsConferencia.value.find(i => i.id === itemId);
    if (item) {
      item.valorConferido = valor;
      item.ok = Math.abs(item.valorEsperado - valor) < 0.01;
    }
  };

  const marcarItemConferido = (itemId) => {
    const item = itemsConferencia.value.find(i => i.id === itemId);
    if (item) {
      item.conferido = true;
    }
  };

  const conferirFechamento = async () => {
    if (!fechamentoSelecionado.value) return;
    
    isConferindo.value = true;
    
    try {
      const diferencaTotal = itemsConferencia.value.reduce((total, item) => {
        return total + (item.valorEsperado - item.valorConferido);
      }, 0);
      
      const dadosConferencia = {
        aprovado: conferenciaData.value.aprovado,
        observacoes: conferenciaData.value.observacoes,
        diferenca: diferencaTotal,
        diferencaAceita: conferenciaData.value.diferencaAceita,
        itensConferidos: itemsConferencia.value.map(item => ({
          id: item.id,
          valorEsperado: item.valorEsperado,
          valorConferido: item.valorConferido,
          ok: item.ok,
        })),
      };
      
      const result = await fechamentosStore.conferirFechamento(
        fechamentoSelecionado.value.unidadeId,
        fechamentoSelecionado.value.id,
        dadosConferencia
      );
      
      if (result.success) {
        stepAtual.value = 'finalizado';
        await carregarFechamentosPendentes();
      }
      
      return result;
    } finally {
      isConferindo.value = false;
    }
  };

  const resetConferencia = () => {
    conferenciaData.value = {
      aprovado: true,
      observacoes: '',
      diferencaAceita: false,
      valoresConferidos: {},
    };
    stepAtual.value = 'lista';
    fechamentoSelecionado.value = null;
    itemsConferencia.value = [];
  };

  const avancarStep = () => {
    if (stepAtual.value === 'conferencia') stepAtual.value = 'revisao';
    else if (stepAtual.value === 'revisao') conferirFechamento();
  };

  const voltarStep = () => {
    if (stepAtual.value === 'revisao') stepAtual.value = 'conferencia';
    else if (stepAtual.value === 'conferencia') stepAtual.value = 'lista';
  };

  return {
    // State
    fechamentosPendentes,
    fechamentosFechadosPendentes,
    conferenciaData,
    isConferindo,
    stepAtual,
    fechamentoSelecionado,
    itemsConferencia,
    progressoConferencia,
    totalDiferencasPendentes,
    
    // Actions
    carregarFechamentosPendentes,
    selecionarFechamento,
    atualizarValorConferido,
    marcarItemConferido,
    conferirFechamento,
    resetConferencia,
    avancarStep,
    voltarStep,
  };
}