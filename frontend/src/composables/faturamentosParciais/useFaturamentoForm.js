// composables/faturamentosParciais/useFaturamentoForm.js
import { ref, computed } from 'vue';
import { useFaturamentosParciaisStore } from '@/stores/faturamentos-parciais.store';

export function useFaturamentoForm() {
  const store = useFaturamentosParciaisStore();
  
  const formData = ref({
    valor: null,
    metodoPagamentoId: null,
    origem: '',
    observacao: '',
    horaInicio: new Date().toISOString().slice(0, 16),
    isAtivo: true,
  });
  
  const errors = ref({});
  const isSubmitting = ref(false);
  
  const isValid = computed(() => {
    return formData.value.valor > 0 
      && formData.value.metodoPagamentoId 
      && formData.value.origem.trim();
  });
  
  const validate = () => {
    errors.value = {};
    
    if (!formData.value.valor || formData.value.valor <= 0) {
      errors.value.valor = 'Valor deve ser maior que zero';
    }
    
    if (!formData.value.metodoPagamentoId) {
      errors.value.metodoPagamentoId = 'Selecione um método de pagamento';
    }
    
    if (!formData.value.origem?.trim()) {
      errors.value.origem = 'Informe a origem';
    }
    
    return Object.keys(errors.value).length === 0;
  };
  
  const resetForm = () => {
    formData.value = {
      valor: null,
      metodoPagamentoId: null,
      origem: '',
      observacao: '',
      horaInicio: new Date().toISOString().slice(0, 16),
      isAtivo: true,
    };
    errors.value = {};
  };
  
  const setFormData = (lancamento) => {
    if (!lancamento) return;
    
    formData.value = {
      valor: lancamento.valor,
      metodoPagamentoId: lancamento.metodoPagamentoId,
      origem: lancamento.origem || '',
      observacao: lancamento.observacao || '',
      horaInicio: lancamento.horaInicio?.slice(0, 16) || new Date().toISOString().slice(0, 16),
      isAtivo: lancamento.isAtivo !== false,
    };
  };
  
  const submitForm = async (unidadeId, lancamentoId = null) => {
    if (!validate()) return false;
    
    isSubmitting.value = true;
    
    try {
      const dados = {
        ...formData.value,
        horaInicio: new Date(formData.value.horaInicio).toISOString(),
        horaFim: new Date().toISOString(),
      };
      
      let resultado;
      if (lancamentoId) {
        resultado = await store.atualizarLancamento(unidadeId, lancamentoId, dados);
      } else {
        resultado = await store.criarLancamento(unidadeId, dados);
      }
      
      if (resultado.success) {
        resetForm();
        return true;
      } else {
        return false;
      }
    } finally {
      isSubmitting.value = false;
    }
  };
  
  return {
    formData,
    errors,
    isSubmitting,
    isValid,
    validate,
    resetForm,
    setFormData,
    submitForm,
  };
}