<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Overlay do mobile quando drawer aberto -->
    <div>
      v-if="sidebarOpen"
      class="fixed inset-0 z-40 bg-black/40 md:hidden"
      @click="sidebarOpen = false"
    </div>

    <!-- Sidebar (fixa no desktop, drawer no mobile) -->
    <aside class="fixed z-50 inset-y-0 left-0 w-72 bg-slate-900 text-white transform transition-transform
             md:translate-x-0" :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'">
      <AppSidebar @navigate="sidebarOpen = false" />
    </aside>

    <!-- Conteúdo principal -->
    <div class="md:pl-72">
      <AppHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />

      <main class="p-4 md:p-6">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AppSidebar from '@/components/layout/AppSidebar.vue'
import AppHeader from '@/components/layout/AppHeader.vue'

/**
 * sidebarOpen:
 * - no mobile controla o drawer
 * - no desktop fica sempre visível (md:translate-x-0)
 */
const sidebarOpen = ref(false)
</script>
