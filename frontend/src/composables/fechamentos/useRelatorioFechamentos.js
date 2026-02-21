import { ref, computed } from 'vue';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useRelatorioFechamentos() {
  const fechamentosStore = useFechamentosStore();
  
  const relatorioData = ref({
    periodo: { dataInicio: null, dataFim: null },
    totalDias: 0,
    totalFaturamento: 0,
    mediaDiaria: 0,
    totalDiferencas: 0,
    fechamentos: [],
    resumoPorStatus: {
      abertos: 0,
      fechados: 0,
      conferidos: 0,
      pendentes: 0,
    },
  });

  const isGerandoRelatorio = ref(false);
  const tipoRelatorio = ref('resumido'); // 'resumido', 'detalhado', 'analitico'
  const formatoExportacao = ref('pdf'); // 'pdf', 'excel', 'csv'

  const tiposRelatorio = [
    { value: 'resumido', label: 'Resumido' },
    { value: 'detalhado', label: 'Detalhado' },
    { value: 'analitico', label: 'Analítico' },
  ];

  const formatosExportacao = [
    { value: 'pdf', label: 'PDF' },
    { value: 'excel', label: 'Excel' },
    { value: 'csv', label: 'CSV' },
  ];

  const gerarRelatorio = (unidadeId, dataInicio, dataFim) => {
    isGerandoRelatorio.value = true;
    
    try {
      const relatorio = fechamentosStore.gerarRelatorioCaixa(unidadeId, dataInicio, dataFim);
      relatorioData.value = relatorio;
      return relatorio;
    } finally {
      isGerandoRelatorio.value = false;
    }
  };

  const exportarRelatorio = async () => {
    // Implementar lógica de exportação
    console.log('Exportando relatório em', formatoExportacao.value);
  };

  const dadosParaTabela = computed(() => {
    return relatorioData.value.fechamentos.map(f => ({
      id: f.id,
      data: new Date(f.data).toLocaleDateString('pt-BR'),
      valorTotal: f.valorTotal,
      valorConferido: f.valorConferido,
      diferenca: f.diferenca,
      status: f.statusCaixa,
      fechadoPor: f.fechadoPorNome || '-',
      conferidoPor: f.conferidoPorNome || '-',
    }));
  });

  const resumoRelatorio = computed(() => {
    const { periodo, totalDias, totalFaturamento, mediaDiaria, totalDiferencas } = relatorioData.value;
    
    return {
      periodo: periodo.dataInicio && periodo.dataFim 
        ? `${new Date(periodo.dataInicio).toLocaleDateString('pt-BR')} - ${new Date(periodo.dataFim).toLocaleDateString('pt-BR')}`
        : 'Todos os períodos',
      totalDias,
      totalFaturamento: new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalFaturamento),
      mediaDiaria: new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(mediaDiaria),
      totalDiferencas: new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalDiferencas),
    };
  });

  return {
    // State
    relatorioData,
    isGerandoRelatorio,
    tipoRelatorio,
    formatoExportacao,
    tiposRelatorio,
    formatosExportacao,
    
    // Computed
    dadosParaTabela,
    resumoRelatorio,
    
    // Actions
    gerarRelatorio,
    exportarRelatorio,
  };
}