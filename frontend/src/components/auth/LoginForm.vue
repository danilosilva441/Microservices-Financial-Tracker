<script setup>
/**
 * Componente de Formulário.
 * Responsabilidade: Coletar dados e emitir evento de envio.
 */
defineProps({
  loading: Boolean,
  error: String
});

// Define os modelos para comunicação bidirecional com o pai
const email = defineModel('email');
const password = defineModel('password');

const emit = defineEmits(['submit']);
</script>

<template>
  <form @submit.prevent="$emit('submit')" class="space-y-6">
    
    <div>
      <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2" for="email">
        Email Corporativo
      </label>
      <input
        id="email"
        v-model="email"
        type="email"
        autocomplete="email"
        placeholder="ex: gerente@empresa.com"
        class="input-field"
        :disabled="loading"
        required
      />
    </div>

    <div>
      <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2" for="password">
        Senha
      </label>
      <input
        id="password"
        v-model="password"
        type="password"
        autocomplete="current-password"
        placeholder="••••••••"
        class="input-field"
        :disabled="loading"
        required
      />
    </div>

    <button 
      type="submit"
      :disabled="loading"
      class="btn-primary w-full"
    >
      <span v-if="loading" class="flex items-center justify-center gap-2">
        <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        Validando...
      </span>
      
      <span v-else>Entrar</span>
    </button>

    <div v-if="error" class="error-box animate-shake">
      <div class="flex items-center gap-2">
        <span class="text-xl">⚠️</span>
        <span class="text-sm font-medium">{{ error }}</span>
      </div>
    </div>

  </form>
</template>

<style scoped>
/* Classes utilitárias encapsuladas para manter o template limpo */
.input-field {
  @apply w-full px-4 py-3 text-sm border border-gray-300 rounded-lg outline-none transition-all duration-200;
  @apply focus:ring-2 focus:ring-blue-500 focus:border-transparent;
  @apply disabled:bg-gray-100 disabled:cursor-not-allowed;
  @apply dark:bg-slate-700 dark:border-slate-600 dark:text-white dark:placeholder-gray-400;
}

.btn-primary {
  @apply px-6 py-3 text-sm font-medium text-white bg-blue-600 rounded-lg shadow-sm transition-all duration-200;
  @apply hover:bg-blue-700 hover:shadow-md hover:-translate-y-0.5;
  @apply focus:ring-4 focus:ring-blue-500/50;
  @apply disabled:opacity-70 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-none;
}

.error-box {
  @apply mt-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg dark:bg-red-900/20 dark:border-red-800 dark:text-red-300;
}

/* Animação de erro */
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}
.animate-shake {
  animation: shake 0.3s ease-in-out;
}
</style>