// composables/faturamentosParciais/useFaturamentosActions.js
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useFaturamentosActions() {
  const store = useFaturamentosParciaisStore();
  
  const carregarLancamentosDia = async (unidadeId) => {
    return await store.carregarLancamentosDia(unidadeId);
  };
  
  const criarLancamento = async (unidadeId, lancamentoData) => {
    return await store.criarLancamento(unidadeId, lancamentoData);
  };
  
  const atualizarLancamento = async (unidadeId, faturamentoId, dadosAtualizados) => {
    return await store.atualizarLancamento(unidadeId, faturamentoId, dadosAtualizados);
  };
  
  const removerLancamento = async (unidadeId, faturamentoId) => {
    return await store.removerLancamento(unidadeId, faturamentoId);
  };
  
  const desativarLancamento = async (unidadeId, faturamentoId) => {
    return await store.desativarLancamento(unidadeId, faturamentoId);
  };
  
  const buscarLancamentoPorId = async (unidadeId, faturamentoId) => {
    return await store.buscarLancamentoPorId(unidadeId, faturamentoId);
  };
  
  return {
    carregarLancamentosDia,
    criarLancamento,
    atualizarLancamento,
    removerLancamento,
    desativarLancamento,
    buscarLancamentoPorId,
  };
}