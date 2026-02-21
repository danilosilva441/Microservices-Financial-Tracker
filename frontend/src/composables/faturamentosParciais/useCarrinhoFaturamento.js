// composables/faturamentosParciais/useCarrinhoFaturamento.js
import { computed } from 'vue';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useCarrinhoFaturamento() {
  const store = useFaturamentosParciaisStore();
  
  const carrinho = computed(() => store.carrinho);
  const totalCarrinho = computed(() => store.totalCarrinho);
  const quantidadeItens = computed(() => store.carrinho.length);
  
  const adicionarAoCarrinho = (lancamento) => {
    store.adicionarAoCarrinho(lancamento);
  };
  
  const removerDoCarrinho = (idTemporario) => {
    store.removerDoCarrinho(idTemporario);
  };
  
  const limparCarrinho = () => {
    store.limparCarrinho();
  };
  
  const finalizarCarrinho = async (unidadeId) => {
    return await store.finalizarCarrinho(unidadeId);
  };
  
  const calcularTotalCarrinho = () => {
    store.calcularTotalCarrinho();
  };
  
  const carrinhoVazio = computed(() => store.carrinho.length === 0);
  
  return {
    carrinho,
    totalCarrinho,
    quantidadeItens,
    carrinhoVazio,
    adicionarAoCarrinho,
    removerDoCarrinho,
    limparCarrinho,
    finalizarCarrinho,
    calcularTotalCarrinho,
  };
}