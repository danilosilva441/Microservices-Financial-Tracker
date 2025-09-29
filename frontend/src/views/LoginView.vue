<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

// Variáveis reativas para os campos do formulário
const email = ref('');
const password = ref('');
const errorMessage = ref('');
const isLoading = ref(false);

const authStore = useAuthStore();
const router = useRouter();

// Função que será chamada ao submeter o formulário
async function handleLogin() {
  try {
    errorMessage.value = ''; // Limpa mensagens de erro antigas
    isLoading.value = true;

    // A store cuida de tudo: chamada de API, salvar o token, etc.
    await authStore.login({
      email: email.value,
      password: password.value,
    });

    // Após o login bem-sucedido, redireciona para o dashboard
    router.push('/dashboard');

  } catch (error) {
    console.error('Erro no login:', error);
    errorMessage.value = 'Email ou senha inválidos. Por favor, tente novamente.';
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <div class="flex items-center justify-center min-h-screen bg-gray-100">
    <div class="px-8 py-6 mt-4 text-left bg-white shadow-lg rounded-lg w-full max-w-md">
      <h3 class="text-2xl font-bold text-center text-gray-800">Acessar Sistema</h3>

      <form @submit.prevent="handleLogin">
        <div class="mt-4">
          <div>
            <label class="block" for="email">Email</label>
            <input
              id="email"
              v-model="email"
              type="email"
              autocomplete="email" 
              placeholder="Email"
              class="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
              required
            />
          </div>
          
          <div class="mt-4">
            <label class="block" for="password">Senha</label>
            <input
              id="password"
              v-model="password"
              type="password"
              autocomplete="current-password" 
              placeholder="Senha"
              class="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
              required
            />
          </div>

          <div class="flex items-baseline justify-between">
            <button 
              type="submit"
              :disabled="isLoading"
              class="w-full px-6 py-2 mt-4 text-white bg-blue-600 rounded-lg hover:bg-blue-900 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isLoading ? 'Entrando...' : 'Entrar' }}
            </button>
          </div>

          <div v-if="errorMessage" class="mt-4 text-red-600 text-sm text-center">
            {{ errorMessage }}
          </div>
        </div>
      </form>
    </div>
  </div>
</template>