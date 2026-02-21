import { ref, computed } from 'vue';
import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useFechamentoForm() {
  const fechamentosStore = useFechamentosStore();
  
  const formData = ref({
    data: new Date().toISOString().split('T')[0],
    valorTotal: 0,
    valorConferido: 0,
    diferenca: 0,
    observacoes: '',
    statusCaixa: 'Aberto',
    status: '',
    itens: [],
    movimentacoes: [],
    fechadoPorUserId: null,
    fechadoPorNome: null,
    fechadoEm: null,
    conferidoPorUserId: null,
    conferidoPorNome: null,
    conferidoEm: null,
  });

  const formErrors = ref({});
  const isSubmitting = ref(false);

  const isValid = computed(() => {
    return Object.keys(formErrors.value).length === 0;
  });

  const diferencaCalculada = computed(() => {
    return (formData.value.valorConferido || 0) - (formData.value.valorTotal || 0);
  });

  const hasDiferenca = computed(() => {
    return Math.abs(diferencaCalculada.value) > 0.01;
  });

  const statusOptions = [
    { value: 'Aberto', label: 'Aberto', color: 'blue' },
    { value: 'Fechado', label: 'Fechado', color: 'orange' },
    { value: 'Conferido', label: 'Conferido', color: 'green' },
    { value: 'Pendente', label: 'Pendente', color: 'red' },
  ];

  const setFormData = (fechamento) => {
    if (fechamento) {
      formData.value = {
        ...formData.value,
        ...fechamento,
        data: fechamento.data || new Date().toISOString().split('T')[0],
      };
    }
  };

  const resetForm = () => {
    formData.value = {
      data: new Date().toISOString().split('T')[0],
      valorTotal: 0,
      valorConferido: 0,
      diferenca: 0,
      observacoes: '',
      statusCaixa: 'Aberto',
      status: '',
      itens: [],
      movimentacoes: [],
      fechadoPorUserId: null,
      fechadoPorNome: null,
      fechadoEm: null,
      conferidoPorUserId: null,
      conferidoPorNome: null,
      conferidoEm: null,
    };
    formErrors.value = {};
  };

  const validateForm = () => {
    const errors = {};
    
    if (!formData.value.data) {
      errors.data = 'Data é obrigatória';
    }
    
    if (formData.value.valorTotal < 0) {
      errors.valorTotal = 'Valor total não pode ser negativo';
    }
    
    if (formData.value.valorConferido < 0) {
      errors.valorConferido = 'Valor conferido não pode ser negativo';
    }
    
    formErrors.value = errors;
    return Object.keys(errors).length === 0;
  };

  const abrirDia = async (unidadeId) => {
    if (!validateForm()) return { success: false, errors: formErrors.value };
    
    isSubmitting.value = true;
    try {
      const result = await fechamentosStore.abrirDia(unidadeId, formData.value);
      if (result.success) {
        resetForm();
      }
      return result;
    } finally {
      isSubmitting.value = false;
    }
  };

  const atualizarFechamento = async (unidadeId, id) => {
    if (!validateForm()) return { success: false, errors: formErrors.value };
    
    isSubmitting.value = true;
    try {
      return await fechamentosStore.atualizarFechamento(unidadeId, id, formData.value);
    } finally {
      isSubmitting.value = false;
    }
  };

  return {
    formData,
    formErrors,
    isSubmitting,
    isValid,
    diferencaCalculada,
    hasDiferenca,
    statusOptions,
    setFormData,
    resetForm,
    validateForm,
    abrirDia,
    atualizarFechamento,
  };
}