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
  <div class="flex items-center justify-center min-h-screen bg-gray-100 px-4 sm:px-6 lg:px-8 py-6">
    <div class="px-6 py-8 sm:px-8 sm:py-10 md:px-10 md:py-12 mt-4 text-left bg-white shadow-lg rounded-lg w-full max-w-xs sm:max-w-sm md:max-w-md">
      <div class="flex justify-center mb-2">
        <div class="w-12 h-12 sm:w-14 sm:h-14 bg-blue-600 rounded-full flex items-center justify-center">
          <svg class="w-6 h-6 sm:w-7 sm:h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
          </svg>
        </div>
      </div>

      <h3 class="text-xl sm:text-2xl font-bold text-center text-gray-800 mt-4">Acessar Sistema</h3>
      <p class="text-sm sm:text-base text-gray-600 text-center mt-2">Faça login em sua conta</p>

      <form @submit.prevent="handleLogin" class="mt-6 sm:mt-8">
        <div class="space-y-4 sm:space-y-6">
          <div>
            <label class="block text-sm sm:text-base font-medium text-gray-700 mb-2" for="email">
              Email
            </label>
            <input
              id="email"
              v-model="email"
              type="email"
              autocomplete="email" 
              placeholder="seu@email.com"
              class="w-full px-4 py-3 sm:py-2 text-sm sm:text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
              :disabled="isLoading"
              required
            />
          </div>
          
          <div>
            <label class="block text-sm sm:text-base font-medium text-gray-700 mb-2" for="password">
              Senha
            </label>
            <input
              id="password"
              v-model="password"
              type="password"
              autocomplete="current-password" 
              placeholder="Sua senha"
              class="w-full px-4 py-3 sm:py-2 text-sm sm:text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
              :disabled="isLoading"
              required
            />
          </div>

          <div class="pt-2">
            <button 
              type="submit"
              :disabled="isLoading"
              class="w-full px-6 py-3 sm:py-3 text-sm sm:text-base font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transform hover:scale-[1.02] active:scale-[0.98]"
            >
              <span v-if="isLoading" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Entrando...
              </span>
              <span v-else>
                Entrar
              </span>
            </button>
          </div>

          <div v-if="errorMessage" class="mt-4 p-3 bg-red-50 border border-red-200 rounded-lg">
            <div class="flex items-center">
              <svg class="w-5 h-5 text-red-500 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              <span class="text-red-700 text-sm sm:text-base">{{ errorMessage }}</span>
            </div>
          </div>
        </div>
      </form>

      <!-- Informações adicionais para mobile -->
      <div class="mt-6 sm:mt-8 pt-4 sm:pt-6 border-t border-gray-200">
        <p class="text-xs sm:text-sm text-gray-500 text-center">
          Problemas para acessar? 
          <a href="#" class="text-blue-600 hover:text-blue-800 font-medium transition-colors">
            Contate o suporte
          </a>
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Melhorias para telas muito pequenas */
@media (max-width: 320px) {
  .max-w-xs {
    max-width: 280px;
  }
}

/* Melhora a experiência de toque em dispositivos móveis */
@media (max-width: 640px) {
  input, button {
    font-size: 16px; /* Previne zoom no iOS */
  }
  
  button {
    min-height: 48px; /* Tamanho mínimo para toque */
  }
}

/* Ajustes para dark mode */
@media (prefers-color-scheme: dark) {
  .bg-gray-100 {
    background-color: #1f2937;
  }
  
  .bg-white {
    background-color: #374151;
  }
  
  .text-gray-800 {
    color: #f9fafb;
  }
  
  .text-gray-700 {
    color: #e5e7eb;
  }
  
  .text-gray-600 {
    color: #d1d5db;
  }
  
  .border-gray-300 {
    border-color: #4b5563;
  }
}
</style>