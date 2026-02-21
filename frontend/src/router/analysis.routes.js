// src/router/analysis.router.js
// Rotas para o módulo de Análise (Dashboard e BI)

/**
 * Módulo de Análise - Rotas
 * 
 * Base URL: /analytics
 * Serviço: AnalysisService na porta 3000
 * Função: Dashboard com KPIs, gráficos e análise de desempenho
 */

// Definição das rotas de análise
const analysisRoutes = [
  {
    path: '/analytics',
    name: 'analytics',
    component: () => import('@/views/analysis/AnalyticsView.vue'),
    meta: {
      title: 'Análise e BI',
      requiresAuth: true,
      requiredRole: ['admin', 'gerente', 'supervisor', 'dev'],
      layout: 'default',
      breadcrumb: 'Análise',
      icon: 'ChartBarIcon',
      description: 'Dashboard completo com KPIs, gráficos e análise de desempenho'
    }
  },

  // Detalhes de análise para uma unidade específica
  // {
  //   path: '/analytics/unidade/:id',
  //   name: 'analytics-unidade',
  //   component: () => import('@/views/analysis/UnidadeAnalyticsView.vue'),
  //   meta: {
  //     title: 'Análise por Unidade',
  //     requiresAuth: true,
  //     requiredRole: ['admin', 'manager', 'supervisor', 'dev'],
  //     layout: 'default',
  //     breadcrumb: 'Análise por Unidade',
  //     parentRoute: 'analytics',
  //     dynamicBreadcrumb: true
  //   },
  //   props: true
  // },

  // Relatórios e exportações
  // {
  //   path: '/analytics/relatorios',
  //   name: 'analytics-relatorios',
  //   component: () => import('@/views/analysis/ReportsView.vue'),
  //   meta: {
  //     title: 'Relatórios',
  //     requiresAuth: true,
  //     requiredRole: ['admin', 'manager', 'dev'],
  //     layout: 'default',
  //     breadcrumb: 'Relatórios',
  //     icon: 'DocumentReportIcon'
  //   }
  // },

  // Comparativo entre períodos
  // {
  //   path: '/analytics/comparativo',
  //   name: 'analytics-comparativo',
  //   component: () => import('@/views/analysis/ComparativeView.vue'),
  //   meta: {
  //     title: 'Comparativo',
  //     requiresAuth: true,
  //     requiredRole: ['admin', 'manager', 'dev'],
  //     layout: 'default',
  //     breadcrumb: 'Comparativo',
  //     icon: 'ScaleIcon'
  //   }
  // },

  //   // Projeções e metas
  //   {
  //     path: '/analytics/projecoes',
  //     name: 'analytics-projecoes',
  //     component: () => import('@/views/analysis/ProjectionsView.vue'),
  //     meta: {
  //       title: 'Projeções e Metas',
  //       requiresAuth: true,
  //       requiredRole: ['admin', 'manager', 'dev'],
  //       layout: 'default',
  //       breadcrumb: 'Projeções',
  //       icon: 'TrendingUpIcon'
  //     }
  //   },

  //   // Benchmarking
  //   {
  //     path: '/analytics/benchmark',
  //     name: 'analytics-benchmark',
  //     component: () => import('@/views/analysis/BenchmarkView.vue'),
  //     meta: {
  //       title: 'Benchmarking',
  //       requiresAuth: true,
  //       requiredRole: ['admin', 'dev'],
  //       layout: 'default',
  //       breadcrumb: 'Benchmarking',
  //       icon: 'ChipIcon'
  //     }
  
];

// Exportar as rotas para serem incluídas no router principal
export default {
  routes: analysisRoutes,

  // Prefixo para todas as rotas (já incluído nos paths)
  prefix: '/analytics',

  // Nome do módulo
  name: 'analysis',

  // Funções auxiliares para navegação programática
  navigation: {
    goToAnalytics: (router) => router.push({ name: 'analytics' }),
    goToUnidadeAnalytics: (router, id) => router.push({
      name: 'analytics-unidade',
      params: { id }
    }),
    goToReports: (router) => router.push({ name: 'analytics-relatorios' }),
    goToComparative: (router) => router.push({ name: 'analytics-comparativo' }),
    goToProjections: (router) => router.push({ name: 'analytics-projecoes' }),
    goToBenchmark: (router) => router.push({ name: 'analytics-benchmark' })
  },

  // Meta informações para menus e breadcrumbs
  menuInfo: {
    icon: 'ChartBarIcon',
    order: 2, // Ordem no menu principal (após Dashboard)
    permissions: ['admin', 'gerente', 'supervisor', 'dev'],
    children: [
      {
        name: 'analytics',
        label: 'Dashboard Análise',
        icon: 'ChartPieIcon'
      },
      {
        name: 'analytics-relatorios',
        label: 'Relatórios',
        icon: 'DocumentReportIcon'
      },
      {
        name: 'analytics-comparativo',
        label: 'Comparativo',
        icon: 'ScaleIcon'
      },
      {
        name: 'analytics-projecoes',
        label: 'Projeções',
        icon: 'TrendingUpIcon'
      },
      {
        name: 'analytics-benchmark',
        label: 'Benchmarking',
        icon: 'ChipIcon'
      }
    ]
  }
};