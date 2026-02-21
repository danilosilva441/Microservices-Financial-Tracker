// router/faturamentosParciais.routes.js
export const faturamentosParciaisRoutes = [
  {
    path: '/unidades/:unidadeId/faturamentos-parciais',
    name: 'faturamentosParciais',
    component: () => import('@/views/faturamentosParciais/FaturamentosParciaisView.vue'),
    meta: {
      title: 'Faturamentos Parciais',
      requiresAuth: true,
      permission: 'view_faturamentos_parciais'
    },
    children: [
      {
        path: '',
        name: 'faturamentosParciaisLista',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisLista.vue'),
        meta: {
          title: 'Lista de Faturamentos Parciais',
        }
      },
      {
        path: 'novo',
        name: 'faturamentosParciaisNovo',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisForm.vue'),
        meta: {
          title: 'Novo Lançamento Parcial',
          permission: 'create_faturamento_parcial'
        }
      },
      {
        path: ':faturamentoId',
        name: 'faturamentosParciaisDetalhe',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisDetalhe.vue'),
        meta: {
          title: 'Detalhe do Lançamento',
        }
      },
      {
        path: ':faturamentoId/editar',
        name: 'faturamentosParciaisEditar',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisForm.vue'),
        meta: {
          title: 'Editar Lançamento Parcial',
          permission: 'edit_faturamento_parcial'
        }
      },
      {
        path: 'carrinho',
        name: 'faturamentosParciaisCarrinho',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisCarrinho.vue'),
        meta: {
          title: 'Carrinho de Lançamentos',
          permission: 'create_faturamento_parcial'
        }
      },
      {
        path: 'estatisticas',
        name: 'faturamentosParciaisEstatisticas',
        component: () => import('@/views/faturamentosParciais/FaturamentosParciaisEstatisticas.vue'),
        meta: {
          title: 'Estatísticas de Faturamento',
        }
      }
    ]
  }
];

// Constantes com os endpoints da API
export const FATURAMENTOS_PARCIAIS_ENDPOINTS = {
  // POST /api/unidades/{unidadeId}/faturamentos-parciais
  CREATE: (unidadeId) => `/api/unidades/${unidadeId}/faturamentos-parciais`,
  
  // GET /api/unidades/{unidadeId}/faturamentos-parciais
  LIST: (unidadeId) => `/api/unidades/${unidadeId}/faturamentos-parciais`,
  
  // GET /api/unidades/{unidadeId}/faturamentos-parciais/{faturamentoId}
  GET_BY_ID: (unidadeId, faturamentoId) => `/api/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`,
  
  // PUT /api/unidades/{unidadeId}/faturamentos-parciais/{faturamentoId}
  UPDATE: (unidadeId, faturamentoId) => `/api/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`,
  
  // DELETE /api/unidades/{unidadeId}/faturamentos-parciais/{faturamentoId}
  DELETE: (unidadeId, faturamentoId) => `/api/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}`,
  
  // PATCH /api/unidades/{unidadeId}/faturamentos-parciais/{faturamentoId}/desativar
  DESATIVAR: (unidadeId, faturamentoId) => `/api/unidades/${unidadeId}/faturamentos-parciais/${faturamentoId}/desativar`,
};

// Permissões do módulo
export const FATURAMENTOS_PARCIAIS_PERMISSIONS = {
  VIEW: 'view_faturamentos_parciais',
  CREATE: 'create_faturamento_parcial',
  EDIT: 'edit_faturamento_parcial',
  DELETE: 'delete_faturamento_parcial',
  SUPERVISOR_DELETE: 'supervisor_delete_faturamento_parcial', // Para deleção que requer supervisor
  EXPORT: 'export_faturamentos_parciais',
};

// Títulos e breadcrumbs
export const FATURAMENTOS_PARCIAIS_BREADCRUMBS = {
  LISTA: [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: 'Lista', active: true }
  ],
  NOVO: [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: 'Novo Lançamento', active: true }
  ],
  DETALHE: (id) => [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: `Lançamento #${id}`, active: true }
  ],
  EDITAR: (id) => [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: `Editar #${id}`, active: true }
  ],
  CARRINHO: [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: 'Carrinho', active: true }
  ],
  ESTATISTICAS: [
    { text: 'Dashboard', to: { name: 'dashboard' } },
    { text: 'Faturamentos Parciais', to: { name: 'faturamentosParciaisLista' } },
    { text: 'Estatísticas', active: true }
  ],
};