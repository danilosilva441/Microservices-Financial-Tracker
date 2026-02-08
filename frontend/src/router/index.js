// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'
import authRoutes from './auth.routes'
// Importe as rotas de unidades
import unidadesRoutes from './unidades.routes'

// Rotas principais
const routes = [
  // Rota inicial
  {
    path: '/',
    name: 'home',
    component: () => import('@/views/HomeView.vue'),
    meta: { 
      title: 'Home',
      requiresAuth: false,
      breadcrumb: 'Home'
    }
  },
  
  // Dashboard principal
  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { 
      title: 'Dashboard',
      requiresAuth: true,
      layout: 'default',
      breadcrumb: 'Dashboard'
    }
  },
  
  // Rotas de autenticação (vêm do auth.routes.js)
  ...authRoutes,
  
  // Rotas de Unidades (importadas do unidades.router.js)
  ...unidadesRoutes.routes,
  
  // Rota 404
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
    meta: { 
      title: 'Página não encontrada',
      httpStatus: 404
    }
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
      return { top: 0, behavior: 'smooth' }
    }
  }
})

// Configuração de logs - MODO TESTE ATIVADO
const LOG_CONFIG = {
  enabled: process.env.NODE_ENV === 'development',
  debugMode: true, // ATIVADO PARA TESTES - MOSTRA TUDO
  colors: {
    success: '#4CAF50',
    error: '#F44336',
    warning: '#FF9800',
    info: '#2196F3',
    auth: '#9C27B0',
    route: '#607D8B',
    test: '#FF5722',
    unidades: '#667eea' // Nova cor para unidades
  }
}

// Estado de teste (pode alternar via localStorage)
const isInTestMode = () => {
  // Durante desenvolvimento, sempre mostra logs de teste
  if (process.env.NODE_ENV === 'development') {
    return true
  }
  // Em produção, você pode controlar via flag
  return localStorage.getItem('testMode') === 'true' || false
}

// Utilitários para logs
const log = {
  // Log de sucesso
  success: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.log(`%c✅ ${message}`, `color: ${LOG_CONFIG.colors.success}; font-weight: bold`, data)
    }
  },
  
  // Log de erro
  error: (message, error = {}) => {
    if (LOG_CONFIG.enabled) {
      console.error(`%c❌ ${message}`, `color: ${LOG_CONFIG.colors.error}; font-weight: bold`, error)
    }
  },
  
  // Log de warning
  warning: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.warn(`%c⚠️ ${message}`, `color: ${LOG_CONFIG.colors.warning}; font-weight: bold`, data)
    }
  },
  
  // Log de informação
  info: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.info(`%cℹ️ ${message}`, `color: ${LOG_CONFIG.colors.info}; font-weight: bold`, data)
    }
  },
  
  // Log específico para autenticação
  auth: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.log(`%c🔐 ${message}`, `color: ${LOG_CONFIG.colors.auth}; font-weight: bold`, data)
    }
  },
  
  // Log de navegação
  route: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.log(`%c🧭 ${message}`, `color: ${LOG_CONFIG.colors.route}; font-weight: bold`, data)
    }
  },
  
  // Log específico para Unidades
  unidades: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      console.log(`%c🏢 ${message}`, `color: ${LOG_CONFIG.colors.unidades}; font-weight: bold`, data)
    }
  },
  
  // Log para TESTES (sempre visível durante desenvolvimento)
  test: (message, data = {}) => {
    if (LOG_CONFIG.enabled && LOG_CONFIG.debugMode) {
      console.log(`%c🧪 [TESTE] ${message}`, `color: ${LOG_CONFIG.colors.test}; background: #FFF3E0; padding: 2px 6px; border-radius: 4px; font-weight: bold; border: 1px dashed ${LOG_CONFIG.colors.test}`, data)
    }
  },
  
  // Log para desenvolvedor (MODIFICADO: visível durante testes)
  dev: (message, data = {}) => {
    if (LOG_CONFIG.enabled && (isInTestMode() || LOG_CONFIG.debugMode)) {
      console.log(`%c👨‍💻 [DEV] ${message}`, `color: #FF5722; background: #FFF3E0; padding: 2px 6px; border-radius: 4px; font-weight: bold; border-left: 4px solid #FF5722`, data)
    }
  },
  
  // Log de usuário (para testar diferentes roles)
  user: (message, data = {}) => {
    if (LOG_CONFIG.enabled) {
      const userRole = localStorage.getItem('userRole') || 'guest'
      const roleColors = {
        admin: '#D32F2F',
        manager: '#1976D2',
        user: '#388E3C',
        dev: '#FF5722',
        guest: '#757575',
        supervisor: '#7B1FA2',
        lider: '#0288D1',
        operador: '#689F38'
      }
      const color = roleColors[userRole] || '#757575'
      
      console.log(`%c👤 [${userRole.toUpperCase()}] ${message}`, `color: ${color}; font-weight: bold; border-left: 3px solid ${color}; padding-left: 5px`, data)
    }
  }
}

// Função para simular diferentes usuários (para testes)
const simulateUser = (role = 'user') => {
  const validRoles = ['admin', 'manager', 'dev', 'supervisor', 'lider', 'operador', 'user']
  
  if (!validRoles.includes(role)) {
    log.error('Role inválido', { 
      role, 
      validRoles,
      dica: 'Use: admin, manager, dev, supervisor, lider, operador, user' 
    })
    return null
  }
  
  // Dados fictícios baseados no role
  const userProfiles = {
    admin: {
      name: 'Administrador Sistema',
      email: 'admin@sistema.com',
      permissions: ['*']
    },
    manager: {
      name: 'Gerente Regional',
      email: 'gerente@empresa.com',
      permissions: ['unidades.read', 'unidades.write', 'dashboard.access']
    },
    dev: {
      name: 'Desenvolvedor Sistema',
      email: 'dev@tech.com',
      permissions: ['*', 'system.config']
    },
    supervisor: {
      name: 'Supervisor Operações',
      email: 'supervisor@empresa.com',
      permissions: ['unidades.read', 'dashboard.access']
    },
    lider: {
      name: 'Líder de Equipe',
      email: 'lider@empresa.com',
      permissions: ['unidades.read']
    },
    operador: {
      name: 'Operador',
      email: 'operador@empresa.com',
      permissions: ['dashboard.access']
    },
    user: {
      name: 'Usuário Padrão',
      email: 'usuario@empresa.com',
      permissions: []
    }
  }
  
  const profile = userProfiles[role] || userProfiles.user
  
  // Salva dados no localStorage
  localStorage.setItem('userRole', role)
  localStorage.setItem('token', `fake-token-${role}-${Date.now()}`)
  localStorage.setItem('userId', `user-${role}-${Math.random().toString(36).substr(2, 9)}`)
  localStorage.setItem('userName', profile.name)
  localStorage.setItem('userEmail', profile.email)
  localStorage.setItem('userPermissions', JSON.stringify(profile.permissions))
  
  log.test('Usuário simulado para testes', {
    role,
    nome: profile.name,
    email: profile.email,
    permissoes: profile.permissions,
    token: localStorage.getItem('token'),
    userId: localStorage.getItem('userId')
  })
  
  // Mostra informações no console
  console.log('%c🎭 USUÁRIO SIMULADO 🎭', 'background: linear-gradient(90deg, #667eea, #764ba2); color: white; font-size: 14px; font-weight: bold; padding: 10px; border-radius: 5px')
  console.log(`%c👤 Nome: ${profile.name}`, 'color: #4CAF50; font-weight: bold')
  console.log(`%c🏷️ Role: ${role.toUpperCase()}`, 'color: #2196F3; font-weight: bold')
  console.log(`%c📧 Email: ${profile.email}`, 'color: #9C27B0; font-weight: bold')
  console.log(`%c🔑 Permissões: ${profile.permissions.join(', ') || 'Nenhuma'}`, 'color: #FF9800; font-weight: bold')
  
  return role
}

// Banner de informações de teste (aparece só no console)
const showTestBanner = () => {
  if (LOG_CONFIG.enabled && LOG_CONFIG.debugMode) {
    console.log('%c🧪 MODO DE TESTE ATIVADO 🧪', 'background: linear-gradient(90deg, #FF5722, #FF9800); color: white; font-size: 14px; font-weight: bold; padding: 10px; border-radius: 5px')
    console.log('%c📋 Comandos disponíveis:', 'color: #2196F3; font-weight: bold')
    console.log('%c- router.simulateUser("admin")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("manager")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("dev")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("supervisor")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("lider")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("operador")', 'color: #4CAF50')
    console.log('%c- router.simulateUser("user")', 'color: #4CAF50')
    console.log('%c- router.clearSimulation()', 'color: #4CAF50')
    console.log('%c- router.getUserInfo()', 'color: #4CAF50')
    console.log('%c\n🚀 Módulos disponíveis:', 'color: #667eea; font-weight: bold')
    console.log('%c- /unidades → Lista de unidades', 'color: #764ba2')
    console.log('%c- /unidades/nova → Nova unidade', 'color: #764ba2')
    console.log('%c- /unidades/:id → Detalhes da unidade', 'color: #764ba2')
    console.log('%c- /unidades/:id/editar → Editar unidade', 'color: #764ba2')
  }
}

// Função para verificar permissões (MODIFICADA: não bloqueia durante testes)
const checkPermissions = (to, userRole) => {
  const requiredRole = to.meta.requiredRole
  
  if (!requiredRole) {
    return { hasAccess: true, reason: 'Rota pública' }
  }
  
  if (!userRole) {
    return { 
      hasAccess: false, 
      reason: 'Usuário não autenticado',
      code: 'NO_AUTH',
      testOverride: true // Permite override durante testes
    }
  }
  
  // Durante testes, permite acesso mesmo sem role correto (mas loga)
  const hasAccess = !requiredRole || 
                   (Array.isArray(requiredRole) && requiredRole.includes(userRole)) ||
                   (typeof requiredRole === 'string' && userRole === requiredRole)
  
  if (!hasAccess) {
    const reason = requiredRole === 'dev' 
      ? 'Acesso restrito para desenvolvedores' 
      : Array.isArray(requiredRole)
        ? `Permissão necessária: ${requiredRole.join(' ou ')}`
        : `Permissão necessária: ${requiredRole}`
    
    log.test('⚠️ PERMISSÃO INSUFICIENTE (permitido em modo teste)', {
      rota: to.fullPath,
      roleAtual: userRole,
      roleNecessario: requiredRole,
      motivo: reason
    })
    
    return { 
      hasAccess: LOG_CONFIG.debugMode, // Permite acesso durante testes
      reason,
      code: requiredRole === 'dev' ? 'DEV_ONLY' : 'INSUFFICIENT_PERMISSIONS',
      testOverride: LOG_CONFIG.debugMode
    }
  }
  
  return { hasAccess: true, reason: 'Permissão concedida' }
}

// Função para obter dados do usuário
const getUserData = () => {
  try {
    const token = localStorage.getItem('token')
    const userRole = localStorage.getItem('userRole')
    const userId = localStorage.getItem('userId')
    const userName = localStorage.getItem('userName')
    const userEmail = localStorage.getItem('userEmail')
    const permissions = JSON.parse(localStorage.getItem('userPermissions') || '[]')
    
    const data = {
      isAuthenticated: !!token,
      role: userRole || 'guest',
      id: userId || null,
      name: userName || 'Usuário não identificado',
      email: userEmail || null,
      permissions: permissions,
      token: token || null,
      isTestUser: token && token.includes('fake-token-')
    }
    
    log.user('Dados do usuário obtidos', data)
    return data
  } catch (error) {
    log.error('Erro ao obter dados do usuário:', error)
    return {
      isAuthenticated: false,
      role: 'guest',
      id: null,
      name: 'Erro ao carregar',
      email: null,
      permissions: [],
      token: null,
      isTestUser: false
    }
  }
}

// Middleware global (guards) - MODIFICADO para testes
router.beforeEach(async (to, from, next) => {
  // Mostrar banner de teste na primeira navegação
  if (!window._testBannerShown) {
    showTestBanner()
    window._testBannerShown = true
  }
  
  // Log de início de navegação
  log.route('Iniciando navegação', {
    de: from.fullPath || '/',
    para: to.fullPath,
    nome: to.name || 'sem-nome',
    modulo: to.path.startsWith('/unidades') ? '🏢 Unidades' : '📊 Geral'
  })
  
  // Log específico para módulo de unidades
  if (to.path.startsWith('/unidades')) {
    log.unidades('Acessando módulo de Unidades', {
      rota: to.fullPath,
      nome: to.name,
      id: to.params.id || 'N/A'
    })
  }
  
  // Título da página
  const pageTitle = to.meta.title || 'DS SysTech'
  document.title = `${pageTitle} | DS SysTech`
  log.info(`Título da página: ${pageTitle}`)
  
  // Obter dados do usuário
  const userData = getUserData()
  log.auth('Status autenticação', {
    autenticado: userData.isAuthenticated,
    role: userData.role,
    userId: userData.id,
    modoTeste: userData.isTestUser ? 'SIM' : 'NÃO'
  })
  
  // Verifica se a rota requer autenticação
  if (to.meta.requiresAuth) {
    if (!userData.isAuthenticated) {
      log.warning('Acesso negado: autenticação necessária', {
        rota: to.fullPath,
        motivo: 'Token não encontrado'
      })
      
      // Redireciona para login com redirect
      next({
        name: 'login',
        query: { 
          redirect: to.fullPath,
          reason: 'auth_required'
        }
      })
      return
    }
    
    // Verificar permissões baseadas em role
    const permissionCheck = checkPermissions(to, userData.role)
    
    if (!permissionCheck.hasAccess && !permissionCheck.testOverride) {
      log.error('Acesso negado: permissão insuficiente', {
        rota: to.fullPath,
        roleAtual: userData.role,
        roleNecessario: to.meta.requiredRole,
        motivo: permissionCheck.reason,
        codigo: permissionCheck.code
      })
      
      // Redirecionar baseado no tipo de erro
      if (permissionCheck.code === 'DEV_ONLY') {
        next({
          name: 'dashboard',
          query: { 
            error: 'dev_only',
            message: 'Acesso restrito para desenvolvedores'
          }
        })
      } else {
        next({
          name: 'access-denied',
          query: { 
            from: to.fullPath,
            reason: permissionCheck.code
          }
        })
      }
      return
    } else if (permissionCheck.testOverride) {
      log.test('⚠️ ACESSO PERMITIDO (modo teste)', {
        rota: to.fullPath,
        roleAtual: userData.role,
        roleNecessario: to.meta.requiredRole,
        motivo: 'Modo de teste ativado - verificações de role ignoradas'
      })
    }
    
    // Log específico para unidades
    if (to.path.startsWith('/unidades')) {
      log.unidades('Acesso concedido ao módulo Unidades', {
        rota: to.fullPath,
        role: userData.role,
        permissoes: userData.permissions,
        modo: permissionCheck.testOverride ? 'TESTE' : 'PRODUÇÃO'
      })
    } else {
      log.success('Acesso concedido à rota protegida', {
        rota: to.fullPath,
        role: userData.role,
        modo: permissionCheck.testOverride ? 'TESTE' : 'PRODUÇÃO'
      })
    }
  } else {
    log.info('Rota pública acessada', { rota: to.fullPath })
  }
  
  // Logs de teste detalhados
  if (LOG_CONFIG.debugMode) {
    log.dev('🔍 Detalhes da navegação (modo teste)', {
      rota: to.fullPath,
      meta: to.meta,
      query: to.query,
      params: to.params,
      userData: getUserData(),
      timestamp: new Date().toISOString()
    })
    
    // Simular delay para ver logs melhor
    await new Promise(resolve => setTimeout(resolve, 50))
  }
  
  // Próximo middleware ou componente
  next()
})

// Depois de cada navegação
router.afterEach((to, from, failure) => {
  if (failure) {
    log.error('Falha na navegação', {
      de: from.fullPath,
      para: to.fullPath,
      falha: failure
    })
  } else {
    const userData = getUserData()
    
    // Log específico para unidades
    if (to.path.startsWith('/unidades')) {
      log.unidades('Navegação em Unidades concluída', {
        de: from.name || 'início',
        para: to.name || 'sem-nome',
        usuario: userData.role,
        autenticado: userData.isAuthenticated ? 'SIM' : 'NÃO'
      })
    } else {
      log.success('Navegação concluída com sucesso', {
        de: from.name || 'início',
        para: to.name || 'sem-nome',
        usuario: userData.role,
        autenticado: userData.isAuthenticated ? 'SIM' : 'NÃO'
      })
    }
    
    // Log específico por tipo de usuário
    if (userData.role === 'dev') {
      log.dev('Desenvolvedor navegou com sucesso', {
        destino: to.fullPath,
        userData
      })
    } else if (userData.role === 'admin') {
      log.test('Administrador navegou', {
        destino: to.fullPath
      })
    }
  }
  
  // Status HTTP para analytics
  const httpStatus = to.meta.httpStatus || 200
  log.info(`Status HTTP: ${httpStatus}`)
})

// Tratamento de erros globais
router.onError((error) => {
  log.error('Erro no router Vue', {
    mensagem: error.message,
    stack: error.stack,
    tipo: error.name
  })
})

// UTILIDADES PARA TESTES (acessíveis via console)
router.simulateUser = simulateUser
router.clearSimulation = () => {
  localStorage.removeItem('userRole')
  localStorage.removeItem('token')
  localStorage.removeItem('userId')
  localStorage.removeItem('userName')
  localStorage.removeItem('userEmail')
  localStorage.removeItem('userPermissions')
  log.test('Simulação de usuário removida')
  window.location.reload()
}
router.getUserInfo = () => {
  const info = getUserData()
  console.table([info])
  return info
}
router.toggleTestMode = (enable = true) => {
  localStorage.setItem('testMode', enable.toString())
  log.test(`Modo teste ${enable ? 'ativado' : 'desativado'}`)
  window.location.reload()
}

// Navegação rápida para Unidades (helpers para console)
router.goToUnidades = () => router.push('/unidades')
router.goToNovaUnidade = () => router.push('/unidades/nova')
router.goToUnidadeDetalhes = (id) => router.push(`/unidades/${id}`)
router.goToEditarUnidade = (id) => router.push(`/unidades/${id}/editar`)

// Anexar utilitários ao router
router.log = log
router.getUserData = getUserData
router.checkPermissions = checkPermissions

// Helper para navegação com logs
const originalPush = router.push
router.push = function(location, onResolve, onReject) {
  if (typeof location === 'string' && location.includes('/unidades')) {
    log.unidades('Navegação para Unidades solicitada', { location })
  } else {
    log.route('Navegação programática solicitada', { location })
  }
  return originalPush.call(this, location, onResolve, onReject)
}

const originalReplace = router.replace
router.replace = function(location, onResolve, onReject) {
  if (typeof location === 'string' && location.includes('/unidades')) {
    log.unidades('Redirecionamento para Unidades solicitado', { location })
  } else {
    log.route('Redirecionamento programático solicitado', { location })
  }
  return originalReplace.call(this, location, onResolve, onReject)
}

// Inicialização
if (LOG_CONFIG.enabled) {
  log.test('Router Vue inicializado em modo desenvolvimento')
  log.info('\n🚀 MÓDULO UNIDADES DISPONÍVEL 🚀')
  log.info('Rotas disponíveis:')
  log.info('  /unidades → Lista de unidades')
  log.info('  /unidades/nova → Nova unidade')
  log.info('  /unidades/:id → Detalhes da unidade')
  log.info('  /unidades/:id/editar → Editar unidade')
  
  log.info('\n🎭 Simulação de usuários (use no console):')
  log.info('  router.simulateUser("admin") → Acesso total')
  log.info('  router.simulateUser("manager") → Gerente')
  log.info('  router.simulateUser("supervisor") → Supervisor')
  log.info('  router.simulateUser("lider") → Líder')
  log.info('  router.simulateUser("operador") → Operador')
  log.info('  router.simulateUser("user") → Usuário básico')
  log.info('  router.simulateUser("dev") → Desenvolvedor')
  
  log.info('\n⚡ Atalhos rápidos para Unidades:')
  log.info('  router.goToUnidades()')
  log.info('  router.goToNovaUnidade()')
  log.info('  router.goToUnidadeDetalhes(1)')
  log.info('  router.goToEditarUnidade(1)')
}

export default router