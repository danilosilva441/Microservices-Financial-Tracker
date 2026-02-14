// src/router/auth.routes.js
import { useAuthStore } from '@/stores/auth.store';
import { ROLES } from '@/utils/roles';

/**
 * CONFIGURAÇÃO DAS ROTAS DE AUTENTICAÇÃO
 * 
 * ENDPOINTS CORRESPONDENTES:
 * - POST /api/token → Login
 * - POST /api/tenant/provision → Criar empresa + gerente
 * - POST /api/users/tenant-user → Criar usuário no tenant
 * - GET /api/users/me → Perfil do usuário
 * - GET /api/admin/users → Listar usuários (admin)
 */

const authRoutes = [
  // ==================== ROTAS PÚBLICAS ====================
  
  /**
   * ROTA: /login
   * ENDPOINT: POST /api/token
   * DESCRIÇÃO: Página de login para todos os usuários
   * USO: Autenticação de usuários existentes
   */
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: {
      title: 'Login - Acessar Sistema',
      layout: 'auth',
      requiresAuth: false,
      requiresGuest: true, // Apenas usuários não autenticados
      authLayout: {
        pageTitle: 'Bem-vindo de volta!',
        subtitle: 'Faça login para acessar sua conta',
        showExtras: true,
        showSocial: true,
        showSeparator: true,
        alternativeLink: {
          text: 'Não tem uma conta?',
          linkText: 'Criar empresa',
          to: '/provision'
        }
      }
    }
  },

  /**
   * ROTA: /provision
   * ENDPOINT: POST /api/tenant/provision
   * DESCRIÇÃO: Cadastro de nova empresa (tenant) + gerente
   * USO: Empresas que querem se cadastrar na plataforma
   * PAYLOAD: { nomeDaEmpresa, emailDoGerente, nomeCompletoGerente, senhaDoGerente }
   */
  {
    path: '/provision',
    name: 'provision',
    component: () => import('@/views/auth/RegisterView.vue'), // Renomeado para clareza
    meta: {
      title: 'Criar Nova Empresa',
      layout: 'auth',
      requiresAuth: false,
      requiresGuest: true,
      authLayout: {
        pageTitle: 'Crie sua empresa',
        subtitle: 'Comece a gerenciar seu negócio',
        showExtras: false,
        showSocial: false,
        hideForgotPassword: true,
        alternativeLink: {
          text: 'Já tem uma conta?',
          linkText: 'Fazer login',
          to: '/login'
        }
      }
    }
  },

  // ==================== ROTAS DE RECUPERAÇÃO DE SENHA ====================
  
  /**
   * ROTA: /forgot-password
   * ENDPOINT: POST /api/auth/forgot-password
   * DESCRIÇÃO: Solicitar recuperação de senha
   * NOTA: Endpoint a ser implementado no backend
   */
  // {
  //   path: '/forgot-password',
  //   name: 'forgot-password',
  //   component: () => import('@/views/auth/ForgotPasswordView.vue'),
  //   meta: { 
  //     title: 'Recuperar Senha',
  //     layout: 'auth',
  //     requiresAuth: false,
  //     requiresGuest: true,
  //     authLayout: {
  //       pageTitle: 'Recuperar senha',
  //       subtitle: 'Enviaremos instruções para seu email',
  //       showExtras: false,
  //       showSocial: false,
  //       backToLogin: true
  //     }
  //   }
  // },

  /**
   * ROTA: /reset-password/:token
   * ENDPOINT: POST /api/auth/reset-password
   * DESCRIÇÃO: Redefinir senha com token
   * NOTA: Endpoint a ser implementado no backend
   */
  // {
  //   path: '/reset-password/:token',
  //   name: 'reset-password',
  //   component: () => import('@/views/auth/ResetPasswordView.vue'),
  //   meta: { 
  //     title: 'Redefinir Senha',
  //     layout: 'auth',
  //     requiresAuth: false,
  //     requiresGuest: true,
  //     authLayout: {
  //       pageTitle: 'Redefinir senha',
  //       subtitle: 'Digite sua nova senha',
  //       showExtras: false,
  //       showSocial: false,
  //       hideForgotPassword: true
  //     }
  //   }
  // },

  // ==================== ROTAS PROTEGIDAS ====================
  
  /**
   * ROTA: /dashboard
   * DESCRIÇÃO: Dashboard principal do sistema
   * ACESSO: Todos os usuários autenticados
   */
  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: {
      title: 'Dashboard',
      requiresAuth: true,
      layout: 'default',
      breadcrumb: [
        { text: 'Home', to: '/dashboard' },
        { text: 'Dashboard' }
      ]
    }
  },

  /**
   * ROTA: /me
   * ENDPOINT: GET /api/users/me
   * DESCRIÇÃO: Perfil do usuário logado
   * ACESSO: Qualquer usuário autenticado
   */
  // {
  //   path: '/me',
  //   name: 'profile',
  //   component: () => import('@/views/auth/ProfileView.vue'),
  //   meta: {
  //     title: 'Meu Perfil',
  //     requiresAuth: true,
  //     layout: 'default',
  //     breadcrumb: [
  //       { text: 'Home', to: '/dashboard' },
  //       { text: 'Meu Perfil' }
  //     ]
  //   }
  // },

  // ==================== ROTAS DE GERENCIAMENTO (Gerente/Admin) ====================
  
  /**
   * ROTA: /users/create
   * ENDPOINT: POST /api/users/tenant-user
   * DESCRIÇÃO: Criar usuários no tenant (Operador, Líder, etc)
   * ACESSO: Gerentes e Admins
   */
  // {
  //   path: '/users/create',
  //   name: 'create-user',
  //   component: () => import('@/views/users/CreateUserView.vue'),
  //   meta: {
  //     title: 'Criar Usuário',
  //     requiresAuth: true,
  //     requiresRole: [ROLES.GERENTE, ROLES.ADMIN, 'Gerente', 'Admin'],
  //     layout: 'default',
  //     breadcrumb: [
  //       { text: 'Home', to: '/dashboard' },
  //       { text: 'Usuários', to: '/users' },
  //       { text: 'Criar Usuário' }
  //     ]
  //   }
  // },

  /**
   * ROTA: /users/list
   * ENDPOINT: GET /api/users/tenant
   * DESCRIÇÃO: Listar usuários do tenant
   * ACESSO: Gerentes e Admins
   */
  // {
  //   path: '/users',
  //   name: 'users-list',
  //   component: () => import('@/views/users/UsersListView.vue'),
  //   meta: {
  //     title: 'Gerenciar Usuários',
  //     requiresAuth: true,
  //     requiresRole: [ROLES.GERENTE, ROLES.ADMIN, 'Gerente', 'Admin'],
  //     layout: 'default',
  //     breadcrumb: [
  //       { text: 'Home', to: '/dashboard' },
  //       { text: 'Usuários' }
  //     ]
  //   }
  // },

  // ==================== ROTAS ADMINISTRATIVAS (Master Admin) ====================
  
  /**
   * ROTA: /admin/users/system
   * ENDPOINT: POST /api/admin/users
   * DESCRIÇÃO: Criar usuários do sistema (nível admin)
   * ACESSO: Apenas Master Admin
   */
  // {
  //   path: '/admin/users/create',
  //   name: 'create-system-user',
  //   component: () => import('@/views/admin/CreateSystemUserView.vue'),
  //   meta: {
  //     title: 'Criar Usuário do Sistema',
  //     requiresAuth: true,
  //     requiresRole: [ROLES.ADMIN, 'MasterAdmin'],
  //     layout: 'admin',
  //     breadcrumb: [
  //       { text: 'Admin', to: '/admin' },
  //       { text: 'Usuários', to: '/admin/users' },
  //       { text: 'Criar Usuário' }
  //     ]
  //   }
  // },

  /**
   * ROTA: /admin/users
   * ENDPOINT: GET /api/admin/users
   * DESCRIÇÃO: Listar todos os usuários do sistema
   * ACESSO: Apenas Master Admin
   */
  // {
  //   path: '/admin/users',
  //   name: 'system-users-list',
  //   component: () => import('@/views/admin/SystemUsersView.vue'),
  //   meta: {
  //     title: 'Usuários do Sistema',
  //     requiresAuth: true,
  //     requiresRole: [ROLES.ADMIN, 'MasterAdmin'],
  //     layout: 'admin',
  //     breadcrumb: [
  //       { text: 'Admin', to: '/admin' },
  //       { text: 'Usuários' }
  //     ]
  //   }
  // },

  /**
   * ROTA: /admin/tenants
   * ENDPOINT: GET /api/admin/tenants
   * DESCRIÇÃO: Listar todos os tenants (empresas)
   * ACESSO: Apenas Master Admin
   */
  // {
  //   path: '/admin/tenants',
  //   name: 'tenants-list',
  //   component: () => import('@/views/admin/TenantsListView.vue'),
  //   meta: {
  //     title: 'Empresas',
  //     requiresAuth: true,
  //     requiresRole: [ROLES.ADMIN, 'MasterAdmin'],
  //     layout: 'admin',
  //     breadcrumb: [
  //       { text: 'Admin', to: '/admin' },
  //       { text: 'Empresas' }
  //     ]
  //   }
  // },

  // ==================== ROTAS DE TESTE/DEV (Comentadas para referência) ====================
  
  // /**
  //  * ROTA: /promote-to-admin
  //  * ENDPOINT: POST /api/admin/promote
  //  * DESCRIÇÃO: Promover usuário a administrador (apenas desenvolvimento)
  //  */
  // {
  //   path: '/promote-to-admin',
  //   name: 'promote-to-admin',
  //   component: () => import('@/views/dev/PromoteToAdminView.vue'),
  //   meta: {
  //     title: 'Promover Usuário',
  //     layout: 'dev',
  //     requiresDev: true
  //   }
  // },

  // /**
  //  * ROTA: /test/register
  //  * ENDPOINT: POST /api/users/register (teste)
  //  * DESCRIÇÃO: Rota de teste para registro (apenas desenvolvimento)
  //  */
  // {
  //   path: '/test/register',
  //   name: 'test-register',
  //   component: () => import('@/views/dev/TestRegisterView.vue'),
  //   meta: {
  //     title: 'Teste de Registro',
  //     layout: 'dev',
  //     requiresDev: true
  //   }
  // }
];

export default authRoutes;