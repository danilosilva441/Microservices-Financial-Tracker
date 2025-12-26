const authRoutes = [
  {
    path: '/',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
    meta: { guestOnly: true } // Opcional: impedir usuário logado de ver login
  },
  // Se tiver rota de "Esqueci minha senha" ou "Cadastro", coloque aqui
];

export default authRoutes;