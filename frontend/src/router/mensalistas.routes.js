// router/mensalistas.routes.js
export const mensalistasRoutes = [
  {
    path: '/unidades/:unidadeId/mensalistas',
    name: 'mensalistas-list',
    component: () => import('@/views/mensalistas/MensalistasList.vue'),
    meta: {
      title: 'Mensalistas',
      requiresAuth: true,
      permission: 'view_mensalistas',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/novo',
    name: 'mensalistas-create',
    component: () => import('@/views/mensalistas/MensalistasForm.vue'),
    meta: {
      title: 'Novo Mensalista',
      requiresAuth: true,
      permission: 'create_mensalista',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Novo' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/:mensalistaId',
    name: 'mensalistas-detail',
    component: () => import('@/views/mensalistas/MensalistasDetail.vue'),
    meta: {
      title: 'Detalhes do Mensalista',
      requiresAuth: true,
      permission: 'view_mensalista',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Detalhes' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/:mensalistaId/editar',
    name: 'mensalistas-edit',
    component: () => import('@/views/mensalistas/MensalistasForm.vue'),
    meta: {
      title: 'Editar Mensalista',
      requiresAuth: true,
      permission: 'edit_mensalista',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Editar' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/importar',
    name: 'mensalistas-import',
    component: () => import('@/views/mensalistas/MensalistasImport.vue'),
    meta: {
      title: 'Importar Mensalistas',
      requiresAuth: true,
      permission: 'import_mensalistas',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Importar' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/relatorios',
    name: 'mensalistas-reports',
    component: () => import('@/views/mensalistas/MensalistasReports.vue'),
    meta: {
      title: 'Relatórios de Mensalistas',
      requiresAuth: true,
      permission: 'view_mensalistas_reports',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Relatórios' }
      ]
    },
    props: true
  },
  {
    path: '/unidades/:unidadeId/mensalistas/cobrancas/gerar',
    name: 'mensalistas-generate-charges',
    component: () => import('@/views/mensalistas/GenerateCharges.vue'),
    meta: {
      title: 'Gerar Cobranças',
      requiresAuth: true,
      permission: 'generate_charges',
      breadcrumb: [
        { label: 'Dashboard', to: { name: 'dashboard' } },
        { label: 'Unidades', to: { name: 'unidades-list' } },
        { label: 'Mensalistas', to: { name: 'mensalistas-list' } },
        { label: 'Gerar Cobranças' }
      ]
    },
    props: true
  }
];