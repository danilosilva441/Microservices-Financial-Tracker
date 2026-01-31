// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import authRoutes from './auth.routes'
// Importe outras rotas aqui depois

// Rotas principais
const routes = [
  // Rota inicial
  {
    path: '/',
    name: 'home',
    component: () => import('@/views/HomeView.vue'),
    meta: { title: 'Home' }
  },
  
  // Rotas de autenticação (vêm do auth.routes.js)
  ...authRoutes,
  
  // Rota 404
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
    meta: { title: 'Página não encontrada' }
  }
]

// Criar router
const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// Middleware global (guards)
router.beforeEach(async (to, from, next) => {
  // Título da página
  document.title = to.meta.title || 'DS SysTech'
  
  // Verifica se a rota requer autenticação
  if (to.meta.requiresAuth) {
    // Aqui você pode adicionar lógica de verificação de token
    const token = localStorage.getItem('token')
    
    if (!token) {
      // Redireciona para login com redirect
      next({
        name: 'login',
        query: { redirect: to.fullPath }
      })
      return
    }
    
    // Se tem token, prossegue
    next()
  } else {
    next()
  }
})

// Depois de cada navegação
router.afterEach((to, from) => {
  // Aqui você pode adicionar analytics ou outras lógicas
  console.log(`Navegou de ${from.name} para ${to.name}`)
})

export default router