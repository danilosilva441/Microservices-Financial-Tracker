<!--
 * src/components/auth/LoginForm.vue
 * LoginForm.vue
 *
 * A Vue component that provides a login form for users to authenticate with their email and password.
 * It includes features such as error handling, success messages, password visibility toggle, and social login options.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 * It also integrates with the authentication composable to handle the login process and manage loading states.
 -->
<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <!-- Mensagens de erro/sucesso -->
    <div v-if="error" class="p-4 border border-red-200 rounded-lg bg-red-50 dark:bg-red-900/30 dark:border-red-800">
      <div class="flex items-center gap-3">
        <IconExclamationTriangle class="flex-shrink-0 w-5 h-5 text-red-600 dark:text-red-400" />
        <p class="text-sm text-red-800 dark:text-red-200">{{ error }}</p>
      </div>
    </div>

    <div v-if="successMessage" class="p-4 border border-green-200 rounded-lg bg-green-50 dark:bg-green-900/30 dark:border-green-800">
      <div class="flex items-center gap-3">
        <IconCheckCircle class="flex-shrink-0 w-5 h-5 text-green-600 dark:text-green-400" />
        <p class="text-sm text-green-800 dark:text-green-200">{{ successMessage }}</p>
      </div>
    </div>

    <!-- Campo Email -->
    <div class="space-y-2">
      <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Email
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconEnvelope class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="email"
          v-model="form.email"
          type="email"
          placeholder="seu@email.com"
          required
          :disabled="isLoading"
          @input="handleInput"
          class="w-full pl-10 pr-3 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
      </div>
      <p v-if="errors.email" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.email }}
      </p>
    </div>

    <!-- Campo Senha -->
    <div class="space-y-2">
      <div class="flex items-center justify-between">
        <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
          Senha
        </label>
        <router-link 
          to="/forgot-password" 
          class="text-sm transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline"
        >
          Esqueceu a senha?
        </router-link>
      </div>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconLock class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="password"
          v-model="form.password"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Sua senha"
          required
          :disabled="isLoading"
          @input="handleInput"
          class="w-full pl-10 pr-10 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
        <button
          type="button"
          @click="showPassword = !showPassword"
          :disabled="isLoading"
          class="absolute inset-y-0 right-0 flex items-center pr-3 text-gray-400 transition-colors duration-200 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300 disabled:opacity-50"
          :title="showPassword ? 'Ocultar senha' : 'Mostrar senha'"
        >
          <IconEye v-if="!showPassword" class="w-5 h-5" />
          <IconEyeSlash v-else class="w-5 h-5" />
        </button>
      </div>
      <p v-if="errors.password" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.password }}
      </p>
    </div>

    <!-- Lembrar de mim -->
    <div class="flex items-center">
      <label class="flex items-center gap-3 cursor-pointer">
        <div class="relative">
          <input
            type="checkbox"
            v-model="form.rememberMe"
            :disabled="isLoading"
            class="sr-only peer"
          />
          <div class="w-5 h-5 transition-all duration-200 bg-white border-2 border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-800 peer-checked:bg-primary-600 peer-checked:border-primary-600 peer-focus:ring-2 peer-focus:ring-primary-500 peer-focus:ring-offset-2 dark:peer-focus:ring-offset-gray-900 hover:border-primary-500"></div>
          <IconCheck 
            v-if="form.rememberMe" 
            class="absolute top-0.5 left-0.5 w-4 h-4 text-white pointer-events-none"
          />
        </div>
        <span class="text-sm text-gray-700 transition-colors duration-200 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400">
          Lembrar de mim
        </span>
      </label>
    </div>

    <!-- Botão de submit -->
    <button
      type="submit"
      :disabled="isLoading || !isFormValid"
      class="justify-center w-full py-3 btn-primary disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-lg"
    >
      <IconSignInAlt v-if="!isLoading" class="w-5 h-5 mr-2" />
      <IconSpinner v-else class="w-5 h-5 mr-2 animate-spin" />
      {{ isLoading ? 'Entrando...' : 'Entrar' }}
    </button>

    <!-- Login Social -->
    <div v-if="showSocial" class="space-y-3">
      <div class="relative">
        <div class="absolute inset-0 flex items-center">
          <div class="w-full border-t border-gray-300 dark:border-gray-600"></div>
        </div>
        <div class="relative flex justify-center text-sm">
          <span class="px-4 text-gray-500 bg-white dark:bg-gray-800 dark:text-gray-400">
            Ou continue com
          </span>
        </div>
      </div>

      <div class="grid grid-cols-2 gap-3">
        <button
          type="button"
          @click="loginWithGoogle"
          :disabled="isLoading"
          class="flex items-center justify-center gap-2 px-4 py-2.5 border-2 border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:border-primary-500 dark:hover:border-primary-400 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-gray-50 dark:hover:bg-gray-700/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
        >
          <IconGoogle class="w-5 h-5" />
          <span class="hidden text-sm font-medium sm:inline">Google</span>
        </button>

        <button
          type="button"
          @click="loginWithGithub"
          :disabled="isLoading"
          class="flex items-center justify-center gap-2 px-4 py-2.5 border-2 border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:border-primary-500 dark:hover:border-primary-400 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-gray-50 dark:hover:bg-gray-700/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
        >
          <IconGithub class="w-5 h-5" />
          <span class="hidden text-sm font-medium sm:inline">GitHub</span>
        </button>
      </div>
    </div>

    <!-- Link para registro/cadastro -->
    <div class="text-center">
      <p class="text-sm text-gray-600 dark:text-gray-400">
        Não tem uma conta?
        <router-link 
          to="/provision" 
          class="font-medium transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline"
        >
          Criar conta
        </router-link>
      </p>
    </div>
  </form>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

// Ícones personalizados
import IconEnvelope from '@/components/icons/envelope.vue'
import IconLock from '@/components/icons/lock.vue'
import IconEye from '@/components/icons/eye.vue'
import IconEyeSlash from '@/components/icons/eye-slash.vue'
import IconCheck from '@/components/icons/check.vue'
import IconSignInAlt from '@/components/icons/sign-in-alt.vue'
import IconSpinner from '@/components/icons/spinner.vue'
import IconExclamationTriangle from '@/components/icons/exclamation-triangle.vue'
import IconCheckCircle from '@/components/icons/check-circle.vue'
import IconGoogle from '@/components/icons/google.vue'
import IconGithub from '@/components/icons/github.vue'

const props = defineProps({
  showSocial: {
    type: Boolean,
    default: true
  },
  redirectTo: {
    type: String,
    default: '/dashboard'
  }
})

const router = useRouter()
const route = useRoute()
const { login, isLoading, error, clearError } = useAuth()

// Estado do formulário
const form = reactive({
  email: '',
  password: '',
  rememberMe: false
})

// Estado da UI
const showPassword = ref(false)
const errors = reactive({})
const successMessage = ref('')

// Validação do formulário
const isFormValid = computed(() => {
  return (
    form.email.includes('@') &&
    form.email.length > 5 &&
    form.password.length >= 6
  )
})

// Manipular envio do formulário
const handleSubmit = async () => {
  clearErrors()
  clearError()

  if (!form.email.includes('@')) {
    errors.email = 'Email inválido'
    return
  }

  if (form.password.length < 6) {
    errors.password = 'A senha deve ter pelo menos 6 caracteres'
    return
  }

  try {
    const result = await login(form)

    if (result.success) {
      successMessage.value = 'Login realizado com sucesso!'
      
      setTimeout(() => {
        const redirect = route.query.redirect || props.redirectTo
        router.push(redirect)
      }, 1500)
    }
  } catch (err) {
    console.error('Erro no login:', err)
    errors.general = 'Erro ao fazer login. Tente novamente.'
  }
}

// Login social
const loginWithGoogle = () => {
  console.log('Login com Google')
}

const loginWithGithub = () => {
  console.log('Login com GitHub')
}

// Limpar erros
const clearErrors = () => {
  Object.keys(errors).forEach(key => {
    errors[key] = ''
  })
}

const handleInput = () => {
  clearErrors()
  clearError()
  successMessage.value = ''
}

// Auto-preenchimento para desenvolvimento
if (import.meta.env.DEV) {
  form.email = 'admin@exemplo.com'
  form.password = '123456'
}
</script>

<style scoped>
@import '@/assets/default.css';

/* Animações */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.alert {
  animation: fadeIn 0.3s ease-out;
}

/* Transições suaves */
input, button {
  transition: all 0.2s ease;
}

/* Focus styles personalizados */
input:focus {
  outline: none;
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(2px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow, 0 0 #0000);
  --tw-ring-color: #667eea;
  --tw-ring-offset-width: 2px;
  --tw-ring-offset-color: #fff;
}

.dark input:focus {
  --tw-ring-offset-color: #111827;
}
</style>