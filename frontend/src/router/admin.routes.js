import { useAuthStore } from '@/stores/auth.store'

const adminRoutes = [
  {
    path: '/admin',
    name: 'admin-root',
    component: () => import('@/layouts/AdminLayout.vue'),
    meta: {
      requiresAuth: true,
      requiresAdmin: true,
      breadcrumb: 'Administração',
    },
    redirect: { name: 'admin-dashboard' },
    children: [
      {
        path: 'dashboard',
        name: 'admin-dashboard',
        component: () => import('@/views/admin/DashboardView.vue'),
        meta: {
          title: 'Dashboard Administrativo',
          icon: 'dashboard',
          breadcrumb: 'Dashboard',
          permissions: ['view_admin_dashboard'],
        },
      },
      {
        path: 'usuarios',
        name: 'admin-usuarios',
        component: () => import('@/views/admin/UsuariosView.vue'),
        meta: {
          title: 'Gerenciar Usuários',
          icon: 'people',
          breadcrumb: 'Usuários',
          permissions: ['manage_users'],
        },
      },
      {
        path: 'usuarios/:id',
        name: 'admin-usuario-detalhe',
        component: () => import('@/views/admin/UsuarioDetalheView.vue'),
        meta: {
          title: 'Detalhe do Usuário',
          breadcrumb: 'Detalhe do Usuário',
          permissions: ['view_user_details'],
        },
        props: true,
      },
      {
        path: 'usuarios/:userId/vinculos',
        name: 'admin-usuario-vinculos',
        component: () => import('@/views/admin/UsuarioVinculosView.vue'),
        meta: {
          title: 'Vínculos do Usuário',
          breadcrumb: 'Vínculos',
          permissions: ['manage_user_vinculos'],
        },
        props: true,
      },
      {
        path: 'unidades',
        name: 'admin-unidades',
        component: () => import('@/views/admin/UnidadesView.vue'),
        meta: {
          title: 'Gerenciar Unidades',
          icon: 'business',
          breadcrumb: 'Unidades',
          permissions: ['manage_unidades'],
        },
      },
      {
        path: 'unidades/:id',
        name: 'admin-unidade-detalhe',
        component: () => import('@/views/admin/UnidadeDetalheView.vue'),
        meta: {
          title: 'Detalhe da Unidade',
          breadcrumb: 'Detalhe da Unidade',
          permissions: ['view_unidade_details'],
        },
        props: true,
      },
      {
        path: 'vinculos',
        name: 'admin-vinculos',
        component: () => import('@/views/admin/VinculosView.vue'),
        meta: {
          title: 'Gerenciar Vínculos',
          icon: 'link',
          breadcrumb: 'Vínculos',
          permissions: ['manage_vinculos'],
        },
      },
      {
        path: 'relatorios',
        name: 'admin-relatorios',
        component: () => import('@/views/admin/RelatoriosView.vue'),
        meta: {
          title: 'Relatórios',
          icon: 'assessment',
          breadcrumb: 'Relatórios',
          permissions: ['view_reports'],
        },
      },
      {
        path: 'logs',
        name: 'admin-logs',
        component: () => import('@/views/admin/LogsView.vue'),
        meta: {
          title: 'Logs de Atividades',
          icon: 'history',
          breadcrumb: 'Logs',
          permissions: ['view_logs'],
        },
      },
      {
        path: 'configuracoes',
        name: 'admin-configuracoes',
        component: () => import('@/views/admin/ConfiguracoesView.vue'),
        meta: {
          title: 'Configurações',
          icon: 'settings',
          breadcrumb: 'Configurações',
          permissions: ['manage_admin_settings'],
        },
      },
    ],
  },
]

// Guard de navegação específico para rotas admin
export function setupAdminGuard(router) {
  router.beforeEach((to, from, next) => {
    if (to.matched.some(record => record.meta.requiresAdmin)) {
      const authStore = useAuthStore()
      
      // Verifica se está autenticado
      if (!authStore.isAuthenticated) {
        next({ name: 'login', query: { redirect: to.fullPath } })
        return
      }
      
      // Verifica se tem permissão de admin
      const userRole = authStore.user?.role
      const isAdmin = ['Admin', 'Dev'].includes(userRole)
      
      if (!isAdmin) {
        next({ name: 'unauthorized' })
        return
      }
      
      // Verifica permissões específicas se necessário
      if (to.meta.permissions && to.meta.permissions.length > 0) {
        const hasPermission = to.meta.permissions.every(permission => 
          authStore.hasPermission(permission)
        )
        
        if (!hasPermission) {
          next({ name: 'unauthorized' })
          return
        }
      }
    }
    
    next()
  })
}

// Função auxiliar para gerar breadcrumbs
export function getAdminBreadcrumbs(route) {
  const breadcrumbs = []
  
  if (route.meta.breadcrumb !== 'Administração') {
    breadcrumbs.push({
      text: 'Administração',
      to: { name: 'admin-dashboard' },
      disabled: false,
    })
  }
  
  if (route.meta.breadcrumb) {
    breadcrumbs.push({
      text: route.meta.breadcrumb,
      disabled: true,
    })
  }
  
  return breadcrumbs
}

export default adminRoutes