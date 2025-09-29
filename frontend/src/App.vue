<script setup>
import { RouterView, RouterLink, useRoute } from 'vue-router'
import { computed } from 'vue'
import { useAuthStore } from './stores/authStore'

const route = useRoute()
const authStore = useAuthStore()

// Verifica se o usuário está logado (qualquer rota que não seja 'login')
const isUserLoggedIn = computed(() => route.name !== 'login')
// 1. Criamos uma computed property para saber se o usuário é Admin
const isAdmin = computed(() => authStore.isAdmin)

// Função de logout que chama a action da store
function handleLogout() {
  authStore.logout()
}
</script>

<template>
  <div v-if="isUserLoggedIn" class="flex h-screen bg-gray-100 font-sans">
    <aside class="w-64 bg-neutral-dark text-white flex flex-col flex-shrink-0">
      <div class="p-6 text-2xl font-heading border-b border-gray-700">
        Meu Painel
      </div>
      
      <nav class="flex-1 p-4 space-y-2">
        <RouterLink 
          to="/dashboard" 
          class="block px-4 py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors"
          :class="{ 'bg-neutral-light bg-opacity-25': route.path.startsWith('/dashboard') }"
        >
          Dashboard
        </RouterLink>
        <RouterLink 
          to="/operacoes" 
          class="block px-4 py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors"
          :class="{ 'bg-neutral-light bg-opacity-25': route.path.startsWith('/operacoes') }"
        >
          Operações
        </RouterLink>


      </nav>
      
      <div class="p-4 border-t border-gray-700">
        <button @click="handleLogout" class="w-full text-left px-4 py-2 rounded-md hover:bg-red-500 transition-colors">
          Sair
        </button>
      </div>
    </aside>

    <main class="flex-1 overflow-y-auto">
      <RouterView />
    </main>
  </div>

  <div v-else>
    <RouterView />
  </div>
</template>