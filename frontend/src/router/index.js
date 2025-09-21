import { createRouter, createWebHistory } from 'vue-router'

// 1. Importa os arrays de rotas dos nossos novos arquivos
import authRoutes from './routes/auth.routes.js';
import dashboardRoutes from './routes/dashboard.routes.js';

// 2. Junta todos os arrays de rotas em um só
const allRoutes = [...authRoutes, ...dashboardRoutes];

// 3. Cria a instância do roteador com a lista completa de rotas
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: allRoutes
})

// 4. A guarda de navegação global continua aqui, pois ela se aplica a TODAS as rotas
router.beforeEach((to, from, next) => {
  const isLoggedIn = !!localStorage.getItem('authToken');

  if (to.meta.requiresAuth && !isLoggedIn) {
    next({ name: 'login' });
  } else {
    next();
  }
});

export default router