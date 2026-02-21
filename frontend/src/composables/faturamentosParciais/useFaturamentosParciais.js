// composables/faturamentosParciais/useFaturamentosParciais.js
import { storeToRefs } from 'pinia';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';
import { useFaturamentosActions } from './useFaturamentosActions';
import { useFaturamentosFilters } from './useFaturamentosFilters';
import { useFaturamentosUI } from './useFaturamentosUI';
import { useCarrinhoFaturamento } from './useCarrinhoFaturamento';
import { useEstatisticasFaturamento } from './useEstatisticasFaturamento';
import { useMetodosPagamento } from './useMetodosPagamento';

export function useFaturamentosParciais() {
  const store = useFaturamentosParciaisStore();
  const state = storeToRefs(store);
  
  return {
    // Estado
    ...state,
    
    // Composables especializados
    ...useFaturamentosActions(),
    ...useFaturamentosFilters(),
    ...useFaturamentosUI(),
    ...useCarrinhoFaturamento(),
    ...useEstatisticasFaturamento(),
    ...useMetodosPagamento(),
    
    // Getters
    lancamentosFiltrados: store.lancamentosFiltrados,
    totalPorMetodoPagamento: store.totalPorMetodoPagamento,
    totalPorHora: store.totalPorHora,
    lancamentosCaixaFisico: store.lancamentosCaixaFisico,
    totalCaixaFisico: store.totalCaixaFisico,
    lancamentosCartoes: store.lancamentosCartoes,
    totalCartoes: store.totalCartoes,
    valorMedioTransacao: store.valorMedioTransacao,
    ultimasTransacoes: store.ultimasTransacoes,
    
    // Métodos utilitários
    formatarMetodoPagamento: store.formatarMetodoPagamento.bind(store),
    formatarValor: store.formatarValor.bind(store),
    formatarHora: store.formatarHora.bind(store),
    resetarStore: store.resetarStore.bind(store),
    clearError: store.clearError.bind(store),
  };
}