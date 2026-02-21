// src/router/dashboard.routes.js

import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

// Views
import DashboardView from '@/views/DashboardView.vue'


const router = createRouter({
  history: createWebHistory(),
  routes: [
    // Protegidas
    { path: '/dashboards', name: 'dashboards', component: DashboardView, meta: { requiresAuth: true } },

  ]
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()



  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'dashboards' }
  }

  // se tentar ir pro login autenticado -> manda pro dashboards
  if (to.name === 'login' && auth.isAuthenticated) {
    return { name: 'dashboards' }
  }

  return true
})

export default router
