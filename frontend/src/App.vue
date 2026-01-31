<!-- src/App.vue (atualizado) -->
<template>
  <div id="app">
    <Suspense>
      <template #default>
        <router-view v-slot="{ Component, route }">
          <transition
            :name="route.meta.transition || 'fade'"
            mode="out-in"
          >
            <component
              :is="getLayout(route)"
              :key="route.fullPath"
              v-bind="getLayoutProps(route)"
            >
              <component :is="Component" />
            </component>
          </transition>
        </router-view>
      </template>
      
      <template #fallback>
        <div class="app-loading">
          <div class="loading-spinner"></div>
          <p>Carregando...</p>
        </div>
      </template>
    </Suspense>
    
    <NotificationContainer />
  </div>
</template>

<script>
import { defineAsyncComponent } from 'vue'
import { useRoute } from 'vue-router'

export default {
  name: 'App',
  
  components: {
    DefaultLayout: defineAsyncComponent(() => 
      import('@/layouts/DefaultLayout.vue')
    ),
    AuthLayout: defineAsyncComponent(() => 
      import('@/components/auth/AuthLayout.vue')
    ),
    NotificationContainer: defineAsyncComponent(() => 
      import('@/components/ui/NotificationContainer.vue')
    )
  },
  
  methods: {
    getLayout(route) {
      const layout = route.meta?.layout || 'default'
      return layout === 'auth' ? 'AuthLayout' : 'DefaultLayout'
    },
    
    getLayoutProps(route) {
      if (route.meta?.layout === 'auth') {
        return {
          title: route.meta.title || 'Bem-vindo',
          subtitle: route.meta.subtitle || '',
          showSocial: route.name === 'login'
        }
      }
      return {}
    }
  }
}
</script>

<style>
/* Estilos globais */
#app {
  min-height: 100vh;
  background: #f8f9fa;
}

/* Transições */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s ease;
}

.slide-enter-from {
  transform: translateX(100%);
}

.slide-leave-to {
  transform: translateX(-100%);
}

/* Loading inicial */
.app-loading {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: white;
  z-index: 9999;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 3px solid #f3f3f3;
  border-top: 3px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.app-loading p {
  color: #666;
  font-size: 16px;
}
</style>