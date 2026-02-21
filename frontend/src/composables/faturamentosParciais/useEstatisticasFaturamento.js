// composables/faturamentosParciais/useEstatisticasFaturamento.js
import { computed } from 'vue';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useEstatisticasFaturamento() {
  const store = useFaturamentosParciaisStore();
  
  const estatisticas = computed(() => store.estatisticas);
  
  const calcularEstatisticas = () => {
    store.calcularEstatisticas();
  };
  
  // Estatísticas derivadas
  const percentualDinheiroPix = computed(() => {
    if (estatisticas.value.totalDia === 0) return 0;
    return (estatisticas.value.totalDinheiroPix / estatisticas.value.totalDia) * 100;
  });
  
  const percentualCartoes = computed(() => {
    if (estatisticas.value.totalDia === 0) return 0;
    return (estatisticas.value.totalCartoes / estatisticas.value.totalDia) * 100;
  });
  
  const percentualOutros = computed(() => {
    if (estatisticas.value.totalDia === 0) return 0;
    return (estatisticas.value.totalOutros / estatisticas.value.totalDia) * 100;
  });
  
  const totalCombinado = computed(() => {
    return estatisticas.value.totalDinheiroPix + 
           estatisticas.value.totalCartoes + 
           estatisticas.value.totalOutros;
  });
  
  const mediaPorHora = computed(() => {
    if (estatisticas.value.quantidadeLancamentos === 0) return 0;
    return estatisticas.value.totalDia / 24; // Média por hora do dia
  });
  
  // Dados para gráficos
  const dadosGraficoPizza = computed(() => {
    return [
      { name: 'Dinheiro/Pix', value: estatisticas.value.totalDinheiroPix },
      { name: 'Cartões', value: estatisticas.value.totalCartoes },
      { name: 'Outros', value: estatisticas.value.totalOutros },
    ].filter(item => item.value > 0);
  });
  
  return {
    estatisticas,
    percentualDinheiroPix,
    percentualCartoes,
    percentualOutros,
    totalCombinado,
    mediaPorHora,
    dadosGraficoPizza,
    calcularEstatisticas,
  };
}