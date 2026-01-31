<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import LoadingState from '@/components/common/LoadingState.vue';

const props = defineProps({
  roles: {
    type: Array,
    default: () => []
  }
});

const router = useRouter();
const authStore = useAuthStore();

const user = computed(() => authStore.user);
const isLoading = computed(() => authStore.isLoading);
const isAuthenticated = computed(() => authStore.isAuthenticated);

// Verifica se o usuário tem permissão
const hasPermission = computed(() => {
  if (props.roles.length === 0) return true;
  
  const userRole = user.value?.role || 'user';
  return props.roles.includes(userRole);
});

// Redireciona baseado no status de autenticação
watchEffect(() => {
  if (!isLoading.value) {
    if (!isAuthenticated.value) {
      // Não autenticado - redireciona para login
      router.replace('/login');
    } else if (!hasPermission.value) {
      // Autenticado mas sem permissão - redireciona para dashboard
      router.replace('/dashboard');
    }
  }
});

// Mostra loading enquanto verifica autenticação
const showContent = computed(() => {
  return !isLoading.value && isAuthenticated.value && hasPermission.value;
});
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="isLoading" class="min-h-screen flex items-center justify-center">
      <LoadingState text="Verificando autenticação..." />
    </div>

    <!-- Conteúdo Protegido -->
    <div v-else-if="showContent">
      <slot></slot>
    </div>

    <!-- Acesso Negado -->
    <div v-else-if="isAuthenticated && !hasPermission" 
         class="min-h-screen flex items-center justify-center">
      <div class="text-center p-8">
        <div class="w-16 h-16 mx-auto text-red-400 mb-4">
          <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m0 0v2m0-2h2m-2 0H9m3-6a3 3 0 110-6 3 3 0 010 6z"/>
          </svg>
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Acesso Negado</h3>
        <p class="text-gray-500 mb-4">
          Você não tem permissão para acessar esta página.
        </p>
        <router-link
          to="/dashboard"
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Voltar ao Dashboard
        </router-link>
      </div>
    </div>
  </div>
</template>