// composables/faturamentosParciais/useMetodosPagamento.js
import { computed } from 'vue';
import { MetodoPagamentoEnum } from '@/stores/faturamentos-parciais.store';

export function useMetodosPagamento() {
  const metodosPagamento = computed(() => MetodoPagamentoEnum.getAll());
  
  const getNomeMetodo = (id) => {
    return MetodoPagamentoEnum.getNome(id);
  };
  
  const isDinheiroOuPix = (id) => {
    return MetodoPagamentoEnum.isDinheiroOuPix(id);
  };
  
  const isEletronico = (id) => {
    return MetodoPagamentoEnum.isEletronico(id);
  };
  
  const getMetodoPorId = (id) => {
    return metodosPagamento.value.find(m => m.id === id);
  };
  
  const metodosDinheiroPix = computed(() => {
    return metodosPagamento.value.filter(m => isDinheiroOuPix(m.id));
  });
  
  const metodosCartoes = computed(() => {
    return metodosPagamento.value.filter(m => m.id === 3 || m.id === 4);
  });
  
  const metodosEletronicos = computed(() => {
    return metodosPagamento.value.filter(m => isEletronico(m.id));
  });
  
  return {
    metodosPagamento,
    metodosDinheiroPix,
    metodosCartoes,
    metodosEletronicos,
    getNomeMetodo,
    isDinheiroOuPix,
    isEletronico,
    getMetodoPorId,
  };
}