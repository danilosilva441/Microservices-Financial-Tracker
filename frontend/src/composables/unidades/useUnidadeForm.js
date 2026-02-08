// composables/unidades/useUnidadeForm.js
import { ref, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useUnidadesActions } from './useUnidadesActions';

export function useUnidadeForm(unidadeData = null, isEdit = false) {
  const router = useRouter();
  const actions = useUnidadesActions();
  
  // Form state
  const form = reactive({
    nome: unidadeData?.nome || '',
    descricao: unidadeData?.descricao || '',
    endereco: unidadeData?.endereco || '',
    metaMensal: unidadeData?.metaMensal || 0,
    dataInicio: unidadeData?.dataInicio || new Date().toISOString().split('T')[0],
    dataFim: unidadeData?.dataFim || '',
    isAtivo: unidadeData?.isAtivo !== undefined ? unidadeData.isAtivo : true,
  });
  
  // UI State
  const isLoading = ref(false);
  const errors = ref({});
  const serverError = ref(null);
  
  // Validation
  const validateField = () => {
    const newErrors = {};
    
    if (!form.nome.trim()) {
      newErrors.nome = 'Nome é obrigatório';
    } else if (form.nome.length < 3) {
      newErrors.nome = 'Nome deve ter pelo menos 3 caracteres';
    }
    
    if (!form.endereco.trim()) {
      newErrors.endereco = 'Endereço é obrigatório';
    }
    
    if (form.metaMensal < 0) {
      newErrors.metaMensal = 'Meta mensal não pode ser negativa';
    }
    
    if (form.dataFim && new Date(form.dataFim) < new Date(form.dataInicio)) {
      newErrors.dataFim = 'Data de fim não pode ser anterior à data de início';
    }
    
    errors.value = newErrors;
    return Object.keys(newErrors).length === 0;
  };
  
  // Form submission
  const submit = async () => {
    if (!validateField()) {
      return false;
    }
    
    isLoading.value = true;
    serverError.value = null;
    
    try {
      if (isEdit && initialData?.id) {
        await actions.updateUnidade(initialData.id, form);
        router.push({ name: 'UnidadeDetalhes', params: { id: initialData.id } });
      } else {
        const novaUnidade = await actions.createUnidade(form);
        router.push({ name: 'UnidadeDetalhes', params: { id: novaUnidade.id } });
      }
      return true;
    } catch (error) {
      serverError.value = error.message;
      return false;
    } finally {
      isLoading.value = false;
    }
  };
  
  const reset = () => {
    if (initialData) {
      Object.keys(form).forEach(key => {
        form[key] = initialData[key] || '';
      });
    } else {
      Object.keys(form).forEach(key => {
        if (key === 'isAtivo') {
          form[key] = true;
        } else if (key === 'dataInicio') {
          form[key] = new Date().toISOString().split('T')[0];
        } else if (key === 'metaMensal') {
          form[key] = 0;
        } else {
          form[key] = '';
        }
      });
    }
    errors.value = {};
    serverError.value = null;
  };
  
  // Computed
  const hasErrors = computed(() => Object.keys(errors.value).length > 0);
  const isFormValid = computed(() => validateField());
  
  return {
    // Form state
    form,
    
    // UI State
    isLoading,
    errors,
    serverError,
    
    // Methods
    validateField,
    submit,
    reset,
    
    // Computed
    hasErrors,
    isFormValid,
    
    // Convenience
    isEdit,
    title: computed(() => isEdit ? 'Editar Unidade' : 'Nova Unidade'),
    submitText: computed(() => isEdit ? 'Salvar Alterações' : 'Criar Unidade'),
  };
}