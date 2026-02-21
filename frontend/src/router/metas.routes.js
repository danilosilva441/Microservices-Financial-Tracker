// router/metas.routes.js
export const metasRoutes = [
  {
    path: '/metas',
    name: 'metas',
    component: () => import('@/views/metas/MetasView.vue'),
    meta: {
      title: 'Metas',
      requiresAuth: true,
      permission: 'visualizar_metas',
    },
    children: [
      {
        path: '',
        name: 'metas-lista',
        component: () => import('@/views/metas/MetasLista.vue'),
        meta: {
          title: 'Lista de Metas',
          requiresAuth: true,
          permission: 'visualizar_metas',
        },
      },
      {
        path: 'dashboard',
        name: 'metas-dashboard',
        component: () => import('@/views/metas/MetasDashboard.vue'),
        meta: {
          title: 'Dashboard de Metas',
          requiresAuth: true,
          permission: 'visualizar_metas',
        },
      },
      {
        path: 'nova',
        name: 'metas-nova',
        component: () => import('@/views/metas/MetasForm.vue'),
        meta: {
          title: 'Nova Meta',
          requiresAuth: true,
          permission: 'criar_metas',
        },
      },
      {
        path: ':id/editar',
        name: 'metas-editar',
        component: () => import('@/views/metas/MetasForm.vue'),
        meta: {
          title: 'Editar Meta',
          requiresAuth: true,
          permission: 'editar_metas',
        },
      },
      {
        path: ':id',
        name: 'metas-detalhe',
        component: () => import('@/views/metas/MetasDetalhe.vue'),
        meta: {
          title: 'Detalhes da Meta',
          requiresAuth: true,
          permission: 'visualizar_metas',
        },
      },
      {
        path: 'analises',
        name: 'metas-analises',
        component: () => import('@/views/metas/MetasAnalises.vue'),
        meta: {
          title: 'Análises de Metas',
          requiresAuth: true,
          permission: 'visualizar_metas',
        },
      },
      {
        path: 'relatorios',
        name: 'metas-relatorios',
        component: () => import('@/views/metas/MetasRelatorios.vue'),
        meta: {
          title: 'Relatórios de Metas',
          requiresAuth: true,
          permission: 'exportar_metas',
        },
      },
    ],
  },
];

// Constantes com os nomes das rotas para facilitar o uso em links e redirecionamentos
export const METAS_ROUTES_NAMES = {
  LISTA: 'metas-lista',
  DASHBOARD: 'metas-dashboard',
  NOVA: 'metas-nova',
  EDITAR: 'metas-editar',
  DETALHE: 'metas-detalhe',
  ANALISES: 'metas-analises',
  RELATORIOS: 'metas-relatorios',
};

// Configuração de breadcrumbs para as rotas de metas
export const metasBreadcrumbs = {
  [METAS_ROUTES_NAMES.LISTA]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
  ],
  [METAS_ROUTES_NAMES.DASHBOARD]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Dashboard', to: { name: METAS_ROUTES_NAMES.DASHBOARD } },
  ],
  [METAS_ROUTES_NAMES.NOVA]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Nova Meta' },
  ],
  [METAS_ROUTES_NAMES.EDITAR]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Editar Meta' },
  ],
  [METAS_ROUTES_NAMES.DETALHE]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Detalhes da Meta' },
  ],
  [METAS_ROUTES_NAMES.ANALISES]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Análises', to: { name: METAS_ROUTES_NAMES.ANALISES } },
  ],
  [METAS_ROUTES_NAMES.RELATORIOS]: [
    { title: 'Dashboard', to: { name: 'dashboard' } },
    { title: 'Metas', to: { name: METAS_ROUTES_NAMES.LISTA } },
    { title: 'Relatórios', to: { name: METAS_ROUTES_NAMES.RELATORIOS } },
  ],
};

// Menu de navegação para o módulo de metas
export const metasMenu = {
  title: 'Metas',
  icon: 'trending_up',
  order: 4,
  children: [
    {
      title: 'Lista de Metas',
      routeName: METAS_ROUTES_NAMES.LISTA,
      icon: 'list',
      permission: 'visualizar_metas',
    },
    {
      title: 'Dashboard',
      routeName: METAS_ROUTES_NAMES.DASHBOARD,
      icon: 'dashboard',
      permission: 'visualizar_metas',
    },
    {
      title: 'Análises',
      routeName: METAS_ROUTES_NAMES.ANALISES,
      icon: 'analytics',
      permission: 'visualizar_metas',
    },
    {
      title: 'Relatórios',
      routeName: METAS_ROUTES_NAMES.RELATORIOS,
      icon: 'description',
      permission: 'exportar_metas',
    },
  ],
};