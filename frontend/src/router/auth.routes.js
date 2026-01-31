// src/router/auth.routes.js
const authRoutes = [
  // Login
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: {
      title: 'Login',
      layout: 'auth',
      authLayout: {
        pageTitle: 'Bem-vindo de volta!',
        subtitle: 'Faça login para acessar sua conta',
        showExtras: false,
        showSocial: true,
        showSeparator: true
      }
    }
  },

  // Registro
  {
    path: '/provision',
    name: 'provision',
    component: () => import('@/views/auth/RegisterView.vue'),
    meta: {
      title: 'Criar Conta',
      layout: 'auth',
      authLayout: {
        pageTitle: 'Crie sua conta',
        subtitle: 'Junte-se à nossa plataforma',
        showExtras: false,
        showSocial: false,
        hideForgotPassword: true
      }
    }
  },

  // // Esqueci a senha
  // {
  //   path: '/forgot-password',
  //   name: 'forgot-password',
  //   component: () => import('@/views/auth/ForgotPasswordView.vue'),
  //   meta: { 
  //     title: 'Recuperar Senha',
  //     guestOnly: true,
  //     layout: 'auth'
  //   }
  // },

  // // Redefinir senha (com token)
  // {
  //   path: '/reset-password/:token',
  //   name: 'reset-password',
  //   component: () => import('@/views/auth/ResetPasswordView.vue'),
  //   meta: { 
  //     title: 'Redefinir Senha',
  //     guestOnly: true,
  //     layout: 'auth'
  //   }
  // },

  // Dashboard (protegido)
  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: {
      title: 'Dashboard',
      requiresAuth: true,
      layout: 'default'
    }
  },

  // // Perfil do usuário
  // {
  //   path: '/profile',
  //   name: 'profile',
  //   component: () => import('@/views/ProfileView.vue'),
  //   meta: { 
  //     title: 'Meu Perfil',
  //     requiresAuth: true
  //   }
  // }
]

export default authRoutes