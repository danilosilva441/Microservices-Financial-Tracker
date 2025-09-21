const dashboardRoutes = [
  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { requiresAuth: true } // Marca a rota como protegida
  },
  {
    path: '/operacoes',
    name: 'operacoes',
    component: () => import('@/views/OperacoesView.vue'),
    meta: { requiresAuth: true }
  },
  {
    // O ':id' indica que esta parte da URL é um parâmetro dinâmico
    path: '/operacoes/:id',
    name: 'operacao-detalhes',
    component: () => import('@/views/OperacaoDetalhesView.vue'),
    meta: { requiresAuth: true }
  }
];

export default dashboardRoutes;