import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

// Views
import UnidadesView from '@/views/UnidadesView.vue'
import UnidadeDetalhesView from '@/views/UnidadeDetalhesView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    // Protegidas
    { path: '/unidades', name: 'unidades', component: UnidadesView, meta: { requiresAuth: true } },
    { path: '/unidades/:id', name: 'unidade-detalhes', component: UnidadeDetalhesView, meta: { requiresAuth: true } },
  ]
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()

  // inicializa sessão (busca /me se precisar)
  // (isso evita bug de dar refresh e perder user)
  if (auth.token && !auth.user) {
    await auth.init()
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'login' }
  }

  // se tentar ir pro login autenticado -> manda pro dashboards
  if (to.name === 'login' && auth.isAuthenticated) {
    return { name: 'dashboards' }
  }

  return true
})

export default router
