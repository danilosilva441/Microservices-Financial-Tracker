<script setup>
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()
// Verifica se a rota atual NÃO é a de login
const isUserLoggedIn = computed(() => route.name !== 'login')

function handleLogout() {
  localStorage.removeItem('authToken');
  window.location.href = '/'; // Força recarregamento
}
</script>

<template>
  <div v-if="isUserLoggedIn" class="flex h-screen bg-gray-100 font-sans">
    <aside class="w-64 bg-neutral-dark text-white flex flex-col">
      <div class="p-6 text-2xl font-heading border-b border-gray-700">Meu Painel</div>
      <nav class="flex-1 p-4 space-y-2">
        <RouterLink to="/dashboard" class="block px-4 py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors">Dashboard</RouterLink>
        <RouterLink to="/operacoes" class="block px-4 py-2 rounded-md hover:bg-neutral-light hover:bg-opacity-25 transition-colors">Operações</RouterLink>
      </nav>
      <div class="p-4 border-t border-gray-700">
        <button @click="handleLogout" class="w-full text-left px-4 py-2 rounded-md hover:bg-red-500 transition-colors">Sair</button>
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