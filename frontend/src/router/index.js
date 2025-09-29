import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore';

// Importa os arrays de rotas dos nossos arquivos modulares
import authRoutes from './routes/auth.routes.js';
import dashboardRoutes from './routes/dashboard.routes.js';

// Junta todos os arrays de rotas em um só
const allRoutes = [...authRoutes, ...dashboardRoutes];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: allRoutes
})

// --- GUARDA DE NAVEGAÇÃO GLOBAL ATUALIZADA ---
// Este código é executado ANTES de cada mudança de rota
router.beforeEach((to, from, next) => {
  // Inicializa a store aqui para garantir que temos o estado mais recente
  const authStore = useAuthStore();
  const isAuthenticated = !!authStore.token;

  // Se a rota exige o perfil de Admin, mas o usuário não é Admin...
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    // ...redireciona para o dashboard (uma página segura que ele pode acessar).
    console.warn('Acesso de Admin negado. Redirecionando para /dashboard.');
    next({ name: 'dashboard' });
  } 
  // Se a rota exige autenticação, mas o usuário não está logado...
  else if (to.meta.requiresAuth && !isAuthenticated) {
    // ...redireciona para a página de login.
    next({ name: 'login' });
  } 
  // Em todos os outros casos...
  else {
    // ...permite que a navegação continue.
    next();
  }
});

export default router