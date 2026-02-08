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

  // Registro Tenant
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

  // {
  //   path: '/register',
  //   name: 'register',
  //   component: () => import('@/views/auth/RegisterDevView.vue'),
  //   meta: {
  //     title: 'Criar Conta',
  //     layout: 'auth',
  //     authLayout: {
  //       pageTitle: 'Crie usuario Gerente',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },

  // {
  //   path: '/promote-to-admin',
  //   name: 'promote-to-admin',
  //   component: () => import('@/views/auth/PromoteToAdminView.vue'),
  //   meta: {
  //     title: 'Promover Usuário a Administrador',
  //     layout: 'auth',
  //     authLayout: {
  //       pageTitle: 'Promover Usuário a Administrador',
  //       subtitle: 'Conceder permissões de administrador',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },

  // {
  //   path: '/me',
  //   name: 'me',
  //   component: () => import('@/views/auth/PerfilUsuarioView.vue'),
  //   meta: {
  //     title: 'Meu Perfil',
  //     layout: 'auth',
  //     authLayout: {
  //       pageTitle: 'Meu Perfil',
  //       subtitle: 'Visualize suas informações pessoais',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },

  // {
  //   path: '/tenant-user',
  //   name: 'tenant-user',
  //   component: () => import('@/views/auth/GerenteCriaUsuariosView.vue'),
  //   meta: {
  //     title: 'Criar Usuários',
  //     layout: 'auth',
  //     authLayout: {
  //       pageTitle: 'Criar Usuários',
  //       subtitle: 'Gerencie os usuários da sua organização',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },

  // {
  //   path: '/system-user',
  //   name: 'system-user',
  //   component: () => import('@/views/auth/CriarUserSistemView.vue'),
  //   meta: {
  //     title: 'Criar Usuario System',
  //     layout: 'auth',
  //     authLayout: {
  //       pageTitle: 'Criar Usuário System',
  //       subtitle: 'Gerencie os usuários do sistema',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },


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