<script setup>
import { RouterView, RouterLink, useRoute } from 'vue-router'
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from './stores/authStore'

const route = useRoute()
const authStore = useAuthStore()
const isMobileMenuOpen = ref(false)

// DEBUG - para verificar o estado
const debugInfo = computed(() => ({
  user: authStore.user,
  roles: authStore.user?.roles,
  isAdmin: authStore.isAdmin,
  hasToken: !!authStore.token
}))

onMounted(() => {
  console.log('🔍 DEBUG App:', debugInfo.value)
})

// CORREÇÃO: Verifica se está logado baseado no token da store
const isUserLoggedIn = computed(() => !!authStore.token)
const isAdmin = computed(() => authStore.isAdmin)

// Função de logout que chama a action da store
function handleLogout() {
  authStore.logout()
  isMobileMenuOpen.value = false // Fecha o menu ao fazer logout
}

// Fecha o menu mobile ao clicar em um link
function handleMobileLinkClick() {
  isMobileMenuOpen.value = false
}

// Fecha o menu ao redimensionar a tela para desktop
function handleResize() {
  if (window.innerWidth >= 1024) {
    isMobileMenuOpen.value = false
  }
}

// Adiciona listener de redimensionamento
onMounted(() => {
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
})
</script>

<template>
  <div v-if="isUserLoggedIn" class="min-h-screen bg-gray-100 font-sans flex flex-col lg:flex-row">
    <!-- Mobile Header -->
    <header class="lg:hidden bg-neutral-dark text-white p-4 flex items-center justify-between sticky top-0 z-50 shadow-md">
      <div class="flex items-center">
        <RouterLink to="/dashboard" class="text-xl font-heading font-bold" @click="handleMobileLinkClick">
          Meu Painel
        </RouterLink>
      </div>
      
      <button 
        @click="isMobileMenuOpen = !isMobileMenuOpen"
        class="p-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors"
        :class="{ 'bg-neutral-light bg-opacity-25': isMobileMenuOpen }"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path 
            v-if="!isMobileMenuOpen" 
            stroke-linecap="round" 
            stroke-linejoin="round" 
            stroke-width="2" 
            d="M4 6h16M4 12h16M4 18h16"
          />
          <path 
            v-else 
            stroke-linecap="round" 
            stroke-linejoin="round" 
            stroke-width="2" 
            d="M6 18L18 6M6 6l12 12"
          />
        </svg>
      </button>
    </header>

    <!-- Mobile Menu Overlay -->
    <div 
      v-if="isMobileMenuOpen" 
      class="lg:hidden fixed inset-0 bg-black bg-opacity-50 z-40"
      @click="isMobileMenuOpen = false"
    ></div>

    <!-- Sidebar -->
    <aside 
      class="lg:w-64 bg-neutral-dark text-white flex flex-col flex-shrink-0 fixed lg:sticky lg:top-0 inset-y-0 left-0 z-40 transform transition-transform duration-300 ease-in-out lg:transform-none"
      :class="[
        isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full',
        'lg:translate-x-0'
      ]"
    >
      <!-- Logo Area -->
      <div class="p-4 lg:p-6 border-b border-gray-700 flex items-center justify-between lg:block">
        <div class="text-xl lg:text-2xl font-heading font-bold">Meu Painel</div>
        <button 
          @click="isMobileMenuOpen = false"
          class="lg:hidden p-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
          </svg>
        </button>
      </div>
      
      <!-- Navigation -->
      <nav class="flex-1 p-4 space-y-1 lg:space-y-2">
        <RouterLink 
          to="/dashboard" 
          class="flex items-center px-4 py-3 lg:py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors group"
          :class="{ 'bg-neutral-light bg-opacity-25': route.path.startsWith('/dashboard') }"
          @click="handleMobileLinkClick"
        >
          <svg class="w-5 h-5 mr-3 text-gray-400 group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
          </svg>
          Dashboard
        </RouterLink>
        
        <RouterLink 
          to="/unidades" 
          class="flex items-center px-4 py-3 lg:py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors group"
          :class="{ 'bg-neutral-light bg-opacity-25': route.path.startsWith('/unidades') }"
          @click="handleMobileLinkClick"
        >
          <svg class="w-5 h-5 mr-3 text-gray-400 group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
          </svg>
          Operações
        </RouterLink>

        <!-- CORREÇÃO: Área Admin - Agora usando a computed property corrigida -->
        <div v-if="isAdmin" class="pt-4 mt-4 border-t border-gray-700">
          <p class="px-4 py-2 text-xs text-gray-400 uppercase tracking-wider">Administração</p>
          <RouterLink 
            to="/solicitacoes" 
            class="flex items-center px-4 py-3 lg:py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors group"
            :class="{ 'bg-neutral-light bg-opacity-25': route.path.startsWith('/solicitacoes') }"
            @click="handleMobileLinkClick"
          >
            <svg class="w-5 h-5 mr-3 text-gray-400 group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/>
            </svg>
            Solicitações
          </RouterLink>
        </div>
      </nav>
      
      <!-- User Area -->
      <div class="p-4 border-t border-gray-700 space-y-3">
        <div class="px-4 py-2 text-sm text-gray-300 flex items-center truncate">
          <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
          </svg>
          <span class="truncate">{{ authStore.user?.email || 'Utilizador' }}</span>
          <span v-if="isAdmin" class="ml-2 px-2 py-1 text-xs bg-blue-600 rounded-full">Admin</span>
        </div>
        
        <button 
          @click="handleLogout" 
          class="w-full flex items-center px-4 py-3 rounded-md hover:bg-red-600 transition-colors text-left group"
        >
          <svg class="w-5 h-5 mr-3 text-gray-400 group-hover:text-white transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
          </svg>
          Sair
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 lg:overflow-y-auto min-h-screen lg:min-h-0">
      <!-- Mobile Header Spacer -->
      <div class="lg:hidden h-16"></div>
      <RouterView />
    </main>
  </div>

  <!-- Login Page (sem sidebar) -->
  <div v-else class="min-h-screen">
    <RouterView />
  </div>
</template>

<style scoped>
/* Melhorias para scroll e animações */
.fixed {
  height: 100vh;
}

/* Ajustes para dark mode */
@media (prefers-color-scheme: dark) {
  .bg-gray-100 {
    background-color: #1f2937;
  }
}

/* Melhorias de acessibilidade */
@media (max-width: 1023px) {
  .fixed {
    width: 280px;
  }
}

/* Suporte a telas muito pequenas */
@media (max-width: 320px) {
  .lg\:w-64 {
    width: 260px;
  }
}

/* Previne scroll no body quando menu mobile está aberto */
body:has(.lg\\:hidden .fixed.translate-x-0) {
  overflow: hidden;
}

/* Garante que a sidebar fique sticky apenas em desktop */
@media (min-width: 1024px) {
  .lg\:sticky {
    position: sticky;
    align-self: flex-start;
    height: 100vh;
    overflow-y: auto;
  }
}
</style>