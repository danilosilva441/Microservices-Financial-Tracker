<template>
  <div class="h-full flex flex-col">
    <!-- Topo -->
    <div class="px-5 py-5 border-b border-white/10">
      <h2 class="text-xl font-bold">Meu Painel</h2>
      <p class="text-xs text-white/70 mt-1">
        Navegue pelas seções do sistema
      </p>
    </div>

    <!-- Links -->
    <nav class="flex-1 px-3 py-3 space-y-1">
      <RouterLink
        v-for="item in menu"
        :key="item.to"
        :to="item.to"
        class="flex items-center gap-3 px-3 py-2 rounded-lg text-white/90 hover:bg-white/10"
        active-class="bg-white/10 text-white"
        @click="$emit('navigate')"
      >
        <span class="text-lg">{{ item.icon }}</span>
        <span class="font-medium">{{ item.label }}</span>
      </RouterLink>
    </nav>

    <!-- Rodapé -->
    <div class="px-5 py-4 border-t border-white/10">
      <p class="text-sm text-white/80 truncate">{{ auth.user?.email }}</p>
      <p class="text-xs text-white/60 mt-1">Role: {{ auth.user?.role || '-' }}</p>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'

const auth = useAuthStore()

/**
 * Menu por role (base).
 * A gente começa simples e depois refina:
 * - gerente/admin: vê tudo
 * - supervisor: vê unidades e dashboards (por enquanto)
 */
const menu = computed(() => {
  const role = (auth.user?.role || '').toLowerCase()

  const base = [
    { label: 'Dashboards', to: '/app/dashboards', icon: '📊' },
    { label: 'Unidades', to: '/app/unidades', icon: '🏢' },
  ]

  if (role === 'admin' || role === 'manager' || role === 'gerente') {
    base.push({ label: 'Usuários', to: '/app/usuarios', icon: '👥' })
  }

  return base
})
</script>
