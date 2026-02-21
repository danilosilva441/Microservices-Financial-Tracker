// src/composables/analysis/useAnalysisKPIs.js

import { storeToRefs } from 'pinia';
import { computed } from 'vue';
import { useAnalysisStore } from '@/stores/analysis.store';

export function useAnalysisKPIs() {
  const store = useAnalysisStore();
  
  const {
    kpis,
    receitaTotal,
    lucroTotal,
    despesaTotal,
    metaTotal,
    percentualMeta,
    unidadesAtivas,
    melhorUnidade,
    piorUnidade,
    projecao,
    benchmark,
    analise,
    metaStatus,
    metaStatusText,
    unidadesComLucro,
    unidadesComPrejuizo,
    unidadesAcimaDaMeta,
    unidadesAbaixoDaMeta,
    ticketMedioGeral,
    crescimentoPeriodo,
  } = storeToRefs(store);
  
  // Margem de lucro
  const margemLucro = computed(() => {
    if (receitaTotal.value === 0) return 0;
    return (lucroTotal.value / receitaTotal.value) * 100;
  });
  
  // Taxa de conversão (exemplo)
  const taxaConversao = computed(() => {
    // Implementar lógica real baseada nos dados disponíveis
    return kpis.value.taxaConversao || 0;
  });
  
  // Comparação com período anterior
  const comparacaoPeriodoAnterior = computed(() => {
    return {
      receita: kpis.value.variacaoReceita || 0,
      lucro: kpis.value.variacaoLucro || 0,
      despesa: kpis.value.variacaoDespesa || 0,
      meta: kpis.value.variacaoMeta || 0,
    };
  });
  
  // Top unidades
  const topUnidades = computed(() => {
    return {
      melhor: {
        nome: melhorUnidade.value.nome || 'N/A',
        valor: melhorUnidade.value.lucro || 0,
        meta: melhorUnidade.value.percentualMeta || 0,
      },
      pior: {
        nome: piorUnidade.value.nome || 'N/A',
        valor: piorUnidade.value.lucro || 0,
        meta: piorUnidade.value.percentualMeta || 0,
      },
    };
  });
  
  // Resumo de unidades
  const unidadesResumo = computed(() => {
    return {
      total: unidadesAtivas.value,
      comLucro: unidadesComLucro.value,
      comPrejuizo: unidadesComPrejuizo.value,
      acimaMeta: unidadesAcimaDaMeta.value,
      abaixoMeta: unidadesAbaixoDaMeta.value,
    };
  });
  
  // Projeção formatada
  const projecaoFormatada = computed(() => {
    return {
      valor: projecao.value.valor || 0,
      status: projecao.value.status || 'neutro',
      probabilidade: projecao.value.probabilidade || 'Indefinida',
      dataEstimada: projecao.value.dataEstimada 
        ? new Date(projecao.value.dataEstimada).toLocaleDateString('pt-BR')
        : null,
    };
  });
  
  // Benchmark formatado
  const benchmarkFormatado = computed(() => {
    return {
      mediaMercado: benchmark.value.mediaMercado || 0,
      posicao: benchmark.value.posicao || 'N/A',
      melhorConcorrente: benchmark.value.melhorConcorrente || 'N/A',
      diferencial: benchmark.value.diferencial || 0,
    };
  });
  
  return {
    // KPIs básicos
    kpis,
    receitaTotal,
    lucroTotal,
    despesaTotal,
    metaTotal,
    percentualMeta,
    unidadesAtivas,
    
    // Métricas calculadas
    margemLucro,
    taxaConversao,
    comparacaoPeriodoAnterior,
    
    // Unidades
    topUnidades,
    unidadesResumo,
    
    // Projeção e benchmark
    projecao,
    projecaoFormatada,
    benchmark,
    benchmarkFormatado,
    analise,
    
    // Status
    metaStatus,
    metaStatusText,
    ticketMedioGeral,
    crescimentoPeriodo,
    
    // Utilitários de formatação
    formatCurrency: store.formatCurrency,
    getStatusColor: store.getStatusColor,
  };
}