// composables/despesas/useDespesaForm.js
import { ref, computed } from 'vue'
import { useDespesasStore } from '@/stores/despesas.store'
import { storeToRefs } from 'pinia'

export const useDespesaForm = () => {
  const store = useDespesasStore()
  const { categorias, despesaAtual } = storeToRefs(store)

  // Estado do formulário
  const formData = ref({
    description: '',
    amount: null,
    categoryId: null,
    expenseDate: new Date().toISOString().split('T')[0],
    unidadeId: null,
    observacao: '',
    recorrente: false,
    periodicidade: 'mensal' // mensal, trimestral, anual
  })

  const errors = ref({})
  const isSubmitting = ref(false)
  const isEditing = ref(false)

  // Validações
  const validations = {
    description: (value) => value?.length >= 3 || 'Descrição deve ter pelo menos 3 caracteres',
    amount: (value) => value > 0 || 'Valor deve ser maior que zero',
    categoryId: (value) => value !== null || 'Selecione uma categoria',
    expenseDate: (value) => value !== null || 'Selecione uma data'
  }

  const validateField = (field, value) => {
    const validator = validations[field]
    if (validator) {
      const result = validator(value)
      if (result !== true) {
        errors.value[field] = result
        return false
      } else {
        delete errors.value[field]
        return true
      }
    }
    return true
  }

  const validateForm = () => {
    errors.value = {}
    let isValid = true

    Object.keys(validations).forEach(field => {
      if (!validateField(field, formData.value[field])) {
        isValid = false
      }
    })

    return isValid
  }

  const hasErrors = computed(() => Object.keys(errors.value).length > 0)

  // Reset form
  const resetForm = () => {
    formData.value = {
      description: '',
      amount: null,
      categoryId: null,
      expenseDate: new Date().toISOString().split('T')[0],
      unidadeId: null,
      observacao: '',
      recorrente: false,
      periodicidade: 'mensal'
    }
    errors.value = {}
  }

  // Carregar despesa para edição
  const carregarParaEdicao = (despesa) => {
    if (despesa) {
      formData.value = {
        description: despesa.description || '',
        amount: despesa.amount || null,
        categoryId: despesa.categoryId || null,
        expenseDate: despesa.expenseDate || despesa.data || new Date().toISOString().split('T')[0],
        unidadeId: despesa.unidadeId || null,
        observacao: despesa.observacao || '',
        recorrente: despesa.recorrente || false,
        periodicidade: despesa.periodicidade || 'mensal'
      }
      isEditing.value = true
    }
  }

  // Submit form
  const submitForm = async () => {
    if (!validateForm()) return false

    isSubmitting.value = true
    try {
      let result
      if (isEditing.value) {
        // Implementar edição
        // result = await store.atualizarDespesa(despesaAtual.value.id, formData.value)
      } else {
        result = await store.criarDespesa(formData.value)
      }
      
      if (result.success) {
        resetForm()
        isEditing.value = false
        return true
      } else {
        return false
      }
    } finally {
      isSubmitting.value = false
    }
  }

  return {
    formData,
    errors,
    isSubmitting,
    isEditing,
    hasErrors,
    categorias,
    validateField,
    validateForm,
    resetForm,
    carregarParaEdicao,
    submitForm
  }
}