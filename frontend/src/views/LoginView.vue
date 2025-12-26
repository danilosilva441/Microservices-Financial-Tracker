<script setup>
/**
 * View Principal de Login.
 * Responsabilidade: Layout e integração entre Lógica (Composable) e UI (Componentes).
 */
import { useLogin } from '@/composables/auth/useLogin';

// Componentes
import LoginHeader from '@/components/auth/LoginHeader.vue';
import LoginForm from '@/components/auth/LoginForm.vue';
import LoginFooter from '@/components/auth/LoginFooter.vue';

// Extraindo a lógica do Composable
const { email, password, errorMessage, isLoading, handleLogin } = useLogin();
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-slate-900 px-4 py-12 transition-colors duration-300">
    
    <div class="w-full max-w-md bg-white dark:bg-slate-800 rounded-2xl shadow-xl overflow-hidden transition-colors duration-300">
      
      <div class="h-2 bg-gradient-to-r from-blue-500 to-purple-600"></div>

      <div class="p-8 sm:p-10">
        <LoginHeader />

        <LoginForm
          v-model:email="email"
          v-model:password="password"
          :loading="isLoading"
          :error="errorMessage"
          @submit="handleLogin"
        />

        <LoginFooter />
      </div>
    </div>

  </div>
</template>