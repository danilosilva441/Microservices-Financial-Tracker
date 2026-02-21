<!-- src/App.vue (atualizado e otimizado) -->
<template>
  <div id="app">
    <router-view v-slot="{ Component, route }">
      
      <component :is="getLayout(route)" v-bind="getLayoutProps(route)">
        
        <transition
          :name="getTransition(route)"
          mode="out-in"
          @before-enter="onBeforeEnter"
          @after-enter="onAfterEnter"
        >
          <keep-alive :include="cachedComponents" :max="10">
            
            <suspense>
              <template #default>
                <component :is="Component" :key="route.fullPath" />
              </template>
              
              <template #fallback>
                <AppLoading />
              </template>
            </suspense>

          </keep-alive>
        </transition>

      </component>
    </router-view>
    
    <NotificationContainer />
    <ConfirmModal v-if="showGlobalModal" />
    <ToastContainer />
  </div>
</template>

<script>
import { defineAsyncComponent, ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

// ⚠️ IMPORTANTE: Layouts devem ser importados estaticamente. 
// Como eles envolvem a tela inteira, fazê-los assíncronos causa "piscar de tela" 
// e os loops de Suspense que você estava vivenciando.
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import AuthLayout from '@/components/auth/AuthLayout.vue'

export default {
  name: 'App',
  
  components: {
    DefaultLayout,
    AuthLayout,
    
    // Componentes Globais (estes podem continuar assíncronos)
    NotificationContainer: defineAsyncComponent(() => 
      import('@/components/ui/NotificationContainer.vue')
    ),
    ConfirmModal: defineAsyncComponent(() => 
      import('@/components/ui/ConfirmModal.vue')
    ),
    ToastContainer: defineAsyncComponent(() => 
      import('@/components/ui/ToastContainer.vue')
    ),
    AppLoading: defineAsyncComponent(() => 
      import('@/components/ui/AppLoading.vue')
    )
  },
  
  setup() {
    const route = useRoute()
    const router = useRouter()
    
    const showGlobalModal = ref(false)
    
    const cachedComponents = computed(() => [
      'UnidadesIndex',
      'UnidadeDetalhes',
      'DashboardIndex'
    ])
    
    const getLayout = (route) => {
      const layout = route.meta?.layout || 'default'
      return layout === 'auth' ? 'AuthLayout' : 'DefaultLayout'
    }
    
    const getLayoutProps = (route) => {
      const props = {
        showHeader: route.meta?.showHeader !== false,
        showFooter: route.meta?.showFooter !== false
      }
      
      if (route.meta?.layout === 'auth') {
        return {
          ...props,
          title: route.meta.title || 'Bem-vindo',
          subtitle: route.meta.subtitle || '',
          showSocial: route.name === 'login',
          backgroundImage: route.meta.backgroundImage
        }
      }
      
      return props
    }
    
    const getTransition = (route) => {
      return route.meta?.transition || 'fade'
    }
    
    const onBeforeEnter = () => {
      window.scrollTo(0, 0)
      document.body.classList.add('page-transitioning')
    }
    
    const onAfterEnter = () => {
      document.body.classList.remove('page-transitioning')
      
      const appName = 'DS SysTech'
      const pageTitle = route.meta?.title || ''
      document.title = pageTitle ? `${pageTitle} | ${appName}` : appName
    }
    
    watch(
      () => route.path,
      (newPath, oldPath) => {
        if (process.env.NODE_ENV === 'development') {
          console.log(`Navegação: ${oldPath} → ${newPath}`)
        }
        
        if (window.gtag) {
          window.gtag('config', 'GA_MEASUREMENT_ID', {
            page_path: newPath
          })
        }
      }
    )
    
    const handleError = (error) => {
      console.error('Erro global capturado:', error)
      if (error.response?.status === 500) {
        router.push({ name: 'Error500' })
      }
    }
    
    window.addEventListener('error', handleError)
    window.addEventListener('unhandledrejection', handleError)
    
    return {
      showGlobalModal,
      cachedComponents,
      getLayout,
      getLayoutProps,
      getTransition,
      onBeforeEnter,
      onAfterEnter
    }
  }
}
</script>

<style>
/* Estilos globais */
:root {
  --color-primary: #667eea;
  --color-primary-dark: #5a67d8;
  --color-secondary: #764ba2;
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-danger: #ef4444;
  --color-info: #3b82f6;
  
  --color-text: #1a202c;
  --color-text-light: #718096;
  --color-text-lighter: #a0aec0;
  
  --color-bg: #f8f9fa;
  --color-bg-light: #ffffff;
  --color-bg-dark: #2d3748;
  
  --color-border: #e2e8f0;
  --color-border-light: #edf2f7;
  --color-border-dark: #cbd5e0;
  
  --shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.1);
  --shadow-md: 0 4px 6px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px rgba(0, 0, 0, 0.1);
  --shadow-xl: 0 20px 25px rgba(0, 0, 0, 0.1);
  
  --border-radius-sm: 4px;
  --border-radius-md: 8px;
  --border-radius-lg: 12px;
  --border-radius-xl: 16px;
  
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  --spacing-2xl: 48px;
  --spacing-3xl: 64px;
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html {
  font-size: 16px;
  scroll-behavior: smooth;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
  font-size: 1rem;
  line-height: 1.5;
  color: var(--color-text);
  background: var(--color-bg);
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  overflow-x: hidden;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Tipografia */
h1, h2, h3, h4, h5, h6 {
  font-weight: 700;
  line-height: 1.2;
  margin-bottom: var(--spacing-md);
  color: var(--color-text);
}

h1 { font-size: 2.5rem; }
h2 { font-size: 2rem; }
h3 { font-size: 1.75rem; }
h4 { font-size: 1.5rem; }
h5 { font-size: 1.25rem; }
h6 { font-size: 1rem; }

p {
  margin-bottom: var(--spacing-md);
}

a {
  color: var(--color-primary);
  text-decoration: none;
  transition: color 0.3s ease;
}

a:hover {
  color: var(--color-primary-dark);
  text-decoration: underline;
}

/* Utilidades */
.text-center { text-align: center; }
.text-right { text-align: right; }
.text-left { text-align: left; }

.text-primary { color: var(--color-primary); }
.text-success { color: var(--color-success); }
.text-warning { color: var(--color-warning); }
.text-danger { color: var(--color-danger); }
.text-info { color: var(--color-info); }

.bg-primary { background-color: var(--color-primary); }
.bg-success { background-color: var(--color-success); }
.bg-warning { background-color: var(--color-warning); }
.bg-danger { background-color: var(--color-danger); }
.bg-info { background-color: var(--color-info); }

.d-flex { display: flex; }
.flex-column { flex-direction: column; }
.justify-center { justify-content: center; }
.justify-between { justify-content: space-between; }
.align-center { align-items: center; }
.align-start { align-items: flex-start; }
.align-end { align-items: flex-end; }

.w-100 { width: 100%; }
.h-100 { height: 100%; }

.m-0 { margin: 0; }
.mb-1 { margin-bottom: var(--spacing-xs); }
.mb-2 { margin-bottom: var(--spacing-sm); }
.mb-3 { margin-bottom: var(--spacing-md); }
.mb-4 { margin-bottom: var(--spacing-lg); }
.mb-5 { margin-bottom: var(--spacing-xl); }

.p-0 { padding: 0; }
.p-1 { padding: var(--spacing-xs); }
.p-2 { padding: var(--spacing-sm); }
.p-3 { padding: var(--spacing-md); }
.p-4 { padding: var(--spacing-lg); }
.p-5 { padding: var(--spacing-xl); }

.shadow-sm { box-shadow: var(--shadow-sm); }
.shadow-md { box-shadow: var(--shadow-md); }
.shadow-lg { box-shadow: var(--shadow-lg); }

.rounded-sm { border-radius: var(--border-radius-sm); }
.rounded-md { border-radius: var(--border-radius-md); }
.rounded-lg { border-radius: var(--border-radius-lg); }
.rounded-xl { border-radius: var(--border-radius-xl); }

.cursor-pointer { cursor: pointer; }
.user-select-none { user-select: none; }

/* Transições personalizadas */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-fade-enter-active,
.slide-fade-leave-active {
  transition: all 0.3s ease;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(20px);
}

.scale-enter-active,
.scale-leave-active {
  transition: all 0.3s ease;
}

.scale-enter-from,
.scale-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* Animação para página de loading */
.page-transitioning {
  overflow: hidden;
}

.page-transitioning::after {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  opacity: 0;
  z-index: 9998;
  animation: pageTransition 0.3s ease;
}

@keyframes pageTransition {
  0% { opacity: 0; }
  50% { opacity: 0.05; }
  100% { opacity: 0; }
}

/* Scrollbar personalizada */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: var(--color-bg-light);
}

::-webkit-scrollbar-thumb {
  background: var(--color-border-dark);
  border-radius: var(--border-radius-md);
}

::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-lighter);
}

/* Seleção de texto */
::selection {
  background-color: rgba(102, 126, 234, 0.2);
  color: var(--color-text);
}

/* Classes para estados de formulário */
.is-valid {
  border-color: var(--color-success) !important;
}

.is-invalid {
  border-color: var(--color-danger) !important;
}

/* Loader global */
.global-loader {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  backdrop-filter: blur(4px);
}

.global-loader .spinner {
  width: 50px;
  height: 50px;
  border: 3px solid var(--color-border);
  border-top: 3px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Responsividade */
@media (max-width: 768px) {
  html {
    font-size: 14px;
  }
  
  h1 { font-size: 2rem; }
  h2 { font-size: 1.75rem; }
  h3 { font-size: 1.5rem; }
  h4 { font-size: 1.25rem; }
  h5 { font-size: 1.125rem; }
  
  .mobile-hide {
    display: none !important;
  }
}

@media (max-width: 480px) {
  html {
    font-size: 13px;
  }
  
  .xs-hide {
    display: none !important;
  }
}

/* Impressão */
@media print {
  .no-print {
    display: none !important;
  }
  
  body {
    background: white;
    color: black;
  }
  
  a {
    color: black;
    text-decoration: none;
  }
}

/* Acessibilidade */
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}

/* Dark mode (se implementado futuramente) */
@media (prefers-color-scheme: dark) {
  :root {
    --color-text: #f7fafc;
    --color-text-light: #cbd5e0;
    --color-text-lighter: #a0aec0;
    
    --color-bg: #1a202c;
    --color-bg-light: #2d3748;
    
    --color-border: #4a5568;
    --color-border-light: #718096;
    --color-border-dark: #2d3748;
  }
  
  body {
    background: var(--color-bg);
    color: var(--color-text);
  }
}
</style>