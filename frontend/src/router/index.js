import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

// Importa os arrays de rotas dos arquivos modulares
import authRoutes from './routes/auth.routes';
import dashboardRoutes from './routes/dashboard.routes';

// Junta todos os arrays em um só
const allRoutes = [
  ...authRoutes,
  ...dashboardRoutes,
  // Rota curinga para 404 (Redireciona para dashboard ou login)
  { path: '/:pathMatch(.*)*', redirect: '/dashboard' }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: allRoutes,
  // Opcional: Scroll para o topo ao mudar de rota
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) return savedPosition;
    return { top: 0 };
  }
});

// --- GUARDA DE NAVEGAÇÃO GLOBAL ---
router.beforeEach(async (to, from, next) => {
  // Inicializa a store dentro do guard para evitar problemas de ciclo de vida
  const authStore = useAuthStore();
  
  // Verifica se o token é válido (se tiver lógica de expiração, o checkAuth cuida disso)
  // Nota: Se você não chamar checkAuth na inicialização do app, pode chamar aqui:
  // await authStore.checkAuth(); 
  
  const isAuthenticated = !!authStore.token;

  // 1. Rota exige Admin e usuário NÃO é admin
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    console.warn('⛔ Acesso de Admin negado.');
    next({ name: 'dashboard' });
  } 
  
  // 2. Rota exige Autenticação e usuário NÃO está logado
  else if (to.meta.requiresAuth && !isAuthenticated) {
    console.log('🔒 Redirecionando para login.');
    next({ name: 'login', query: { redirect: to.fullPath } });
  } 
  
  // 3. (Opcional) Usuário logado tentando acessar Login
  else if (to.name === 'login' && isAuthenticated) {
    next({ name: 'dashboard' });
  }

  // 4. Permitir navegação
  else {
    next();
  }
});

export default router;