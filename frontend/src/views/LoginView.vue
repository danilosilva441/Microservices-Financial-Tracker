<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '../services/api'; // Nosso comunicador da API!

// Variáveis reativas para os campos do formulário
const email = ref('');
const password = ref('');
const errorMessage = ref('');
const router = useRouter();

// Função que será chamada ao submeter o formulário
async function handleLogin() {
  try {
    errorMessage.value = ''; // Limpa mensagens de erro antigas

    // Faz a chamada POST para /api/token usando nosso serviço 'api'
    const response = await api.post('/token', {
      email: email.value,
      password: password.value,
    });

    // Se o login for bem-sucedido:
    // 1. Salva o token no armazenamento local do navegador
    localStorage.setItem('authToken', response.data.token);

    // 2. Redireciona o usuário para o dashboard (que criaremos depois)
    router.push('/dashboard');

  } catch (error) {
    console.error('Erro no login:', error);
    errorMessage.value = 'Email ou senha inválidos. Por favor, tente novamente.';
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
    autocomplete="email" placeholder="Email"
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
    autocomplete="current-password" placeholder="Senha"
    class="w-full px-4 py-2 mt-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600"
    required
  />
</div>

          <div class="flex items-baseline justify-between">
            <button type="submit"
              class="w-full px-6 py-2 mt-4 text-white bg-blue-600 rounded-lg hover:bg-blue-900 transition-colors">
              Entrar
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