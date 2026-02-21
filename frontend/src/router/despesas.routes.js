// src/router/despesas.routes.js
export const despesasRoutes = {
  // Lista todas as despesas da unidade
  listByUnidade: (unidadeId) => ({
    url: `/api/Expenses/unidade/${unidadeId}`,
    method: 'GET',
    requiresAuth: true
  }),

  // Lança uma nova despesa manualmente
  create: () => ({
    url: '/api/Expenses',
    method: 'POST',
    requiresAuth: true
  }),

  // Importação em massa via Excel/CSV
  upload: () => ({
    url: '/api/Expenses/upload',
    method: 'POST',
    requiresAuth: true,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  }),

  // Remove uma despesa lançada errado
  delete: (id) => ({
    url: `/api/Expenses/${id}`,
    method: 'DELETE',
    requiresAuth: true
  }),

  // Lista categorias (Aluguel, Luz, Água)
  listCategories: () => ({
    url: '/api/Expenses/categories',
    method: 'GET',
    requiresAuth: true
  }),

  // Cria uma nova categoria de despesa
  createCategory: () => ({
    url: '/api/Expenses/categories',
    method: 'POST',
    requiresAuth: true
  })
}

// Exemplos de payloads para documentação
export const despesasPayloadExamples = {
  create: {
    amount: 0.01,
    expenseDate: "2026-01-17T11:34:23.725Z",
    description: "string",
    categoryId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    unidadeId: "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  
  createCategory: {
    name: "string",
    description: "string"
  }
}

// Helper function para construir URLs com parâmetros
export const buildDespesasUrl = (route, params = {}) => {
  let url = route.url
  
  // Substitui parâmetros na URL (ex: {unidadeId} -> 123)
  Object.keys(params).forEach(key => {
    url = url.replace(`{${key}}`, params[key])
  })
  
  return url
}

// Validações básicas para os payloads
export const validateDespesaPayload = (payload) => {
  const errors = []
  
  if (!payload.amount || payload.amount <= 0) {
    errors.push('Amount must be greater than 0')
  }
  
  if (!payload.expenseDate) {
    errors.push('Expense date is required')
  }
  
  if (!payload.description || payload.description.trim() === '') {
    errors.push('Description is required')
  }
  
  if (!payload.categoryId) {
    errors.push('Category ID is required')
  }
  
  if (!payload.unidadeId) {
    errors.push('Unidade ID is required')
  }
  
  return {
    isValid: errors.length === 0,
    errors
  }
}

export const validateCategoryPayload = (payload) => {
  const errors = []
  
  if (!payload.name || payload.name.trim() === '') {
    errors.push('Category name is required')
  }
  
  return {
    isValid: errors.length === 0,
    errors
  }
}