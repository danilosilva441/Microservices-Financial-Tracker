// src/router/unidades.router.js
export default {
  routes: [
    {
      path: '/unidades',
      name: 'Unidades',
      component: () => import('@/views/unidades/UnidadesIndexView.vue'),
      meta: {
        title: 'Unidades',
        requiresAuth: true,
        breadcrumb: [
          { text: 'Dashboard', to: '/' },
          { text: 'Unidades', active: true }
        ]
      }
    },
    {
      path: '/unidades/nova',
      name: 'NovaUnidade',
      component: () => import('@/views/unidades/NovaUnidadeView.vue'),
      meta: {
        title: 'Nova Unidade',
        requiresAuth: true,
        breadcrumb: [
          { text: 'Dashboard', to: '/' },
          { text: 'Unidades', to: '/unidades' },
          { text: 'Nova Unidade', active: true }
        ]
      }
    },
    {
      path: '/unidades/:id',
      name: 'UnidadeDetalhes',
      component: () => import('@/views/unidades/UnidadeDetalhesView.vue'),
      meta: {
        title: 'Detalhes da Unidade',
        requiresAuth: true,
        breadcrumb: [
          { text: 'Dashboard', to: '/' },
          { text: 'Unidades', to: '/unidades' },
          { text: 'Detalhes', active: true }
        ]
      },
      props: true
    },
    {
      path: '/unidades/:id/editar',
      name: 'EditarUnidade',
      component: () => import('@/views/unidades/EditarUnidadeView.vue'),
      meta: {
        title: 'Editar Unidade',
        requiresAuth: true,
        breadcrumb: [
          { text: 'Dashboard', to: '/' },
          { text: 'Unidades', to: '/unidades' },
          { text: 'Editar', active: true }
        ]
      },
      props: true
    }
  ]
};