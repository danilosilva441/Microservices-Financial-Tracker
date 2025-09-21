const authRoutes = [
  {
    path: '/',
    name: 'login',
    component: () => import('@/views/LoginView.vue')
  },
  // No futuro, uma rota de registro '/register' entraria aqui.
];

export default authRoutes;