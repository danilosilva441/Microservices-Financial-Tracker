const dashboardRoutes = [
  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { requiresAuth: true }
  },
  {
    // ✅ Renomeado de /operacoes para /unidades
    path: '/unidades',
    name: 'unidades',
    // ✅ Importando a View refatorada
    component: () => import('@/views/UnidadesView.vue'),
    meta: { requiresAuth: true }
  },
  {
    // ✅ Renomeado parâmetro e nome da rota
    path: '/unidades/:id',
    name: 'unidade-detalhes',
    // ✅ Importando a View de detalhes refatorada
    component: () => import('@/views/UnidadeDetalhesView.vue'),
    meta: { requiresAuth: true }
  },

  {
    path: '/unidades/:id/despesas',
    name: 'unidade-despesas',
    component: () => import('@/views/DespesasView.vue'),
    meta: { requiresAuth: true }
  },
  // // Rota de configuração (placeholder para o futuro)
  // {
  //   path: '/configuracoes',
  //   name: 'configuracoes',
  //   component: () => import('@/views/ConfiguracoesView.vue'), // Crie este arquivo depois se não existir
  //   meta: { requiresAuth: true, requiresAdmin: true } // Exemplo de rota só para Admin
  // }
];

export default dashboardRoutes;