<!--
 * src/components/auth/RegisterForm.vue
 * RegisterForm.vue
 *
 * A Vue component that provides a registration form for new users to create an account.
 * It includes fields for name, email, password, and company information, as well as validation and error handling.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 * It also integrates with the authentication composable to handle the registration process and manage loading states.
 -->
<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <!-- Mensagens -->
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

    <!-- Nome Completo -->
    <div class="space-y-2">
      <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Nome Completo <span class="text-red-500">*</span>
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconUser class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="name"
          v-model="form.name"
          type="text"
          placeholder="Seu nome completo"
          required
          :disabled="isLoading"
          @input="handleInput"
          class="w-full pl-10 pr-3 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
      </div>
      <p v-if="errors.name" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.name }}
      </p>
    </div>

    <!-- Nome da Empresa -->
    <div class="space-y-2">
      <label for="companyName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Nome da Empresa <span class="text-red-500">*</span>
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconBuilding class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="companyName"
          v-model="form.companyName"
          type="text"
          placeholder="Nome da sua empresa"
          required
          :disabled="isLoading"
          @input="handleInput"
          class="w-full pl-10 pr-3 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
      </div>
      <p v-if="errors.companyName" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.companyName }}
      </p>
    </div>

    <!-- Email -->
    <div class="space-y-2">
      <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Email <span class="text-red-500">*</span>
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

    <!-- Telefone (opcional) -->
    <div class="space-y-2">
      <label for="phone" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Telefone <span class="text-xs text-gray-500 dark:text-gray-400">(Opcional)</span>
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconPhone class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="phone"
          v-model="form.phone"
          type="tel"
          placeholder="(81) 9 9999-9999"
          :disabled="isLoading"
          @input="handleInput"
          class="w-full pl-10 pr-3 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
      </div>
    </div>

    <!-- Senha -->
    <div class="space-y-2">
      <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Senha <span class="text-red-500">*</span>
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconLock class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="password"
          v-model="form.password"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Crie uma senha segura"
          required
          :disabled="isLoading"
          @input="validatePassword"
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

      <!-- Indicador de força da senha -->
      <div v-if="form.password" class="mt-3 space-y-3">
        <div class="flex items-center gap-2">
          <div class="flex-1 h-2 overflow-hidden bg-gray-200 rounded-full dark:bg-gray-700">
            <div 
              class="h-full transition-all duration-300"
              :class="passwordStrengthClass"
              :style="{ width: `${(passwordStrength / 5) * 100}%` }"
            ></div>
          </div>
          <span class="text-xs font-medium" :class="passwordStrengthTextClass">
            {{ passwordStrengthText }}
          </span>
        </div>

        <!-- Dicas para senha -->
        <ul class="grid grid-cols-1 gap-2 text-xs sm:grid-cols-2">
          <li class="flex items-center gap-2" :class="form.password.length >= 8 ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
            <IconCheck v-if="form.password.length >= 8" class="w-4 h-4" />
            <IconX v-else class="w-4 h-4" />
            <span>Mínimo 8 caracteres</span>
          </li>
          <li class="flex items-center gap-2" :class="hasUpperCase ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
            <IconCheck v-if="hasUpperCase" class="w-4 h-4" />
            <IconX v-else class="w-4 h-4" />
            <span>Letra maiúscula</span>
          </li>
          <li class="flex items-center gap-2" :class="hasLowerCase ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
            <IconCheck v-if="hasLowerCase" class="w-4 h-4" />
            <IconX v-else class="w-4 h-4" />
            <span>Letra minúscula</span>
          </li>
          <li class="flex items-center gap-2" :class="hasNumber ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
            <IconCheck v-if="hasNumber" class="w-4 h-4" />
            <IconX v-else class="w-4 h-4" />
            <span>Número</span>
          </li>
          <li class="flex items-center gap-2 sm:col-span-2" :class="hasSpecialChar ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
            <IconCheck v-if="hasSpecialChar" class="w-4 h-4" />
            <IconX v-else class="w-4 h-4" />
            <span>Caractere especial (!@#$%^&*)</span>
          </li>
        </ul>
      </div>
      
      <p v-if="errors.password" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.password }}
      </p>
    </div>

    <!-- Confirmar Senha -->
    <div class="space-y-2">
      <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
        Confirmar Senha <span class="text-red-500">*</span>
      </label>
      <div class="relative">
        <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
          <IconLock class="w-5 h-5 text-gray-400 dark:text-gray-500" />
        </div>
        <input
          id="confirmPassword"
          v-model="form.confirmPassword"
          :type="showConfirmPassword ? 'text' : 'password'"
          placeholder="Confirme sua senha"
          required
          :disabled="isLoading"
          @input="validateConfirmPassword"
          class="w-full pl-10 pr-10 py-2.5 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
        />
        <button
          type="button"
          @click="showConfirmPassword = !showConfirmPassword"
          :disabled="isLoading"
          class="absolute inset-y-0 right-0 flex items-center pr-3 text-gray-400 transition-colors duration-200 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300 disabled:opacity-50"
          :title="showConfirmPassword ? 'Ocultar senha' : 'Mostrar senha'"
        >
          <IconEye v-if="!showConfirmPassword" class="w-5 h-5" />
          <IconEyeSlash v-else class="w-5 h-5" />
        </button>
      </div>
      <p v-if="errors.confirmPassword" class="mt-1 text-sm text-red-600 dark:text-red-400">
        {{ errors.confirmPassword }}
      </p>
    </div>

    <!-- Termos e Condições -->
    <div class="space-y-2">
      <label class="flex items-start gap-3 cursor-pointer">
        <div class="relative flex-shrink-0 mt-0.5">
          <input
            type="checkbox"
            v-model="form.acceptTerms"
            :disabled="isLoading"
            class="sr-only peer"
          />
          <div class="w-5 h-5 transition-all duration-200 bg-white border-2 border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-800 peer-checked:bg-primary-600 peer-checked:border-primary-600 peer-focus:ring-2 peer-focus:ring-primary-500 peer-focus:ring-offset-2 dark:peer-focus:ring-offset-gray-900 hover:border-primary-500"></div>
          <IconCheck 
            v-if="form.acceptTerms" 
            class="absolute top-0.5 left-0.5 w-4 h-4 text-white pointer-events-none"
          />
        </div>
        <span class="text-sm text-gray-700 transition-colors duration-200 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400">
          Eu concordo com os
          <router-link to="/terms" class="font-medium text-primary-600 dark:text-primary-400 hover:underline">
            Termos de Uso
          </router-link>
          e
          <router-link to="/privacy" class="font-medium text-primary-600 dark:text-primary-400 hover:underline">
            Política de Privacidade
          </router-link>
          <span class="ml-1 text-red-500">*</span>
        </span>
      </label>
      <p v-if="errors.acceptTerms" class="text-sm text-red-600 dark:text-red-400">
        {{ errors.acceptTerms }}
      </p>
    </div>

    <!-- Newsletter (opcional) -->
    <div class="space-y-2">
      <label class="flex items-start gap-3 cursor-pointer">
        <div class="relative flex-shrink-0 mt-0.5">
          <input
            type="checkbox"
            v-model="form.newsletter"
            :disabled="isLoading"
            class="sr-only peer"
          />
          <div class="w-5 h-5 transition-all duration-200 bg-white border-2 border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-800 peer-checked:bg-primary-600 peer-checked:border-primary-600 peer-focus:ring-2 peer-focus:ring-primary-500 peer-focus:ring-offset-2 dark:peer-focus:ring-offset-gray-900 hover:border-primary-500"></div>
          <IconCheck 
            v-if="form.newsletter" 
            class="absolute top-0.5 left-0.5 w-4 h-4 text-white pointer-events-none"
          />
        </div>
        <span class="text-sm text-gray-700 transition-colors duration-200 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400">
          Desejo receber novidades e atualizações por email
          <span class="ml-1 text-xs text-gray-500 dark:text-gray-400">(opcional)</span>
        </span>
      </label>
    </div>

    <!-- Botão de submit -->
    <button
      type="submit"
      :disabled="isLoading || !isFormValid"
      class="justify-center w-full py-3 btn-primary disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-lg"
    >
      <IconUserPlus v-if="!isLoading" class="w-5 h-5 mr-2" />
      <IconSpinner v-else class="w-5 h-5 mr-2 animate-spin" />
      {{ isLoading ? 'Criando conta...' : 'Criar Conta' }}
    </button>

    <!-- Já tem conta -->
    <div class="text-center">
      <p class="text-sm text-gray-600 dark:text-gray-400">
        Já tem uma conta?
        <router-link 
          to="/login" 
          class="font-medium transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline"
        >
          Faça login aqui
        </router-link>
      </p>
    </div>
  </form>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

// Ícones personalizados
import IconUser from '@/components/icons/user.vue'
import IconBuilding from '@/components/icons/building.vue'
import IconEnvelope from '@/components/icons/envelope.vue'
import IconPhone from '@/components/icons/phone.vue'
import IconLock from '@/components/icons/lock.vue'
import IconEye from '@/components/icons/eye.vue'
import IconEyeSlash from '@/components/icons/eye-slash.vue'
import IconCheck from '@/components/icons/check.vue'
import IconX from '@/components/icons/x.vue'
import IconUserPlus from '@/components/icons/user-plus.vue'
import IconSpinner from '@/components/icons/spinner.vue'
import IconExclamationTriangle from '@/components/icons/exclamation-triangle.vue'
import IconCheckCircle from '@/components/icons/check-circle.vue'

const router = useRouter()
const { provisionTenant, isLoading, error, clearError } = useAuth()

// Estado do formulário
const form = reactive({
  name: '',
  email: '',
  phone: '',
  companyName: '',
  password: '',
  confirmPassword: '',
  acceptTerms: false,
  newsletter: false
})

// Estado da UI
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const errors = reactive({})
const successMessage = ref('')

// Validações de senha
const hasUpperCase = computed(() => /[A-Z]/.test(form.password))
const hasLowerCase = computed(() => /[a-z]/.test(form.password))
const hasNumber = computed(() => /[0-9]/.test(form.password))
const hasSpecialChar = computed(() => /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]/.test(form.password))

// Força da senha
const passwordStrength = computed(() => {
  let score = 0
  if (form.password.length >= 8) score++
  if (hasUpperCase.value) score++
  if (hasLowerCase.value) score++
  if (hasNumber.value) score++
  if (hasSpecialChar.value) score++
  return score
})

const passwordStrengthText = computed(() => {
  const strength = passwordStrength.value
  if (strength <= 1) return 'Muito fraca'
  if (strength === 2) return 'Fraca'
  if (strength === 3) return 'Média'
  if (strength === 4) return 'Forte'
  return 'Muito forte'
})

const passwordStrengthClass = computed(() => {
  const strength = passwordStrength.value
  if (strength <= 1) return 'bg-red-500'
  if (strength === 2) return 'bg-orange-500'
  if (strength === 3) return 'bg-yellow-500'
  if (strength === 4) return 'bg-green-500'
  return 'bg-green-600'
})

const passwordStrengthTextClass = computed(() => {
  const strength = passwordStrength.value
  if (strength <= 1) return 'text-red-600 dark:text-red-400'
  if (strength === 2) return 'text-orange-600 dark:text-orange-400'
  if (strength === 3) return 'text-yellow-600 dark:text-yellow-400'
  if (strength === 4) return 'text-green-600 dark:text-green-400'
  return 'text-green-700 dark:text-green-500'
})

// Validação do formulário
const isFormValid = computed(() => {
  return (
    form.name.length >= 2 &&
    form.companyName.length >= 2 &&
    form.email.includes('@') &&
    form.password.length >= 8 &&
    form.password === form.confirmPassword &&
    form.acceptTerms &&
    passwordStrength.value >= 3
  )
})

// Validação da senha
const validatePassword = () => {
  errors.password = ''

  if (form.password.length < 8) {
    errors.password = 'A senha deve ter pelo menos 8 caracteres'
  } else if (!hasUpperCase.value || !hasLowerCase.value) {
    errors.password = 'A senha deve conter letras maiúsculas e minúsculas'
  } else if (!hasNumber.value) {
    errors.password = 'A senha deve conter pelo menos um número'
  } else if (!hasSpecialChar.value) {
    errors.password = 'A senha deve conter pelo menos um caractere especial'
  }
}

// Validação da confirmação
const validateConfirmPassword = () => {
  errors.confirmPassword = ''
  if (form.password !== form.confirmPassword) {
    errors.confirmPassword = 'As senhas não coincidem'
  }
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

// Manipular envio
const handleSubmit = async () => {
  clearErrors()
  clearError()

  if (form.name.length < 2) {
    errors.name = 'Nome muito curto'
    return
  }

  if (form.companyName.length < 2) {
    errors.companyName = 'Nome da empresa muito curto'
    return
  }

  if (!form.email.includes('@')) {
    errors.email = 'Email inválido'
    return
  }

  validatePassword()
  validateConfirmPassword()

  if (!form.acceptTerms) {
    errors.acceptTerms = 'Você deve aceitar os termos de uso'
    return
  }

  if (Object.values(errors).some(error => error)) {
    return
  }

  try {
    const tenantData = {
      nomeCompletoGerente: form.name,
      nomeDaEmpresa: form.companyName,
      emailDoGerente: form.email,
      senhaDoGerente: form.password,
      phone: form.phone || undefined,
      newsletter: form.newsletter
    }

    const result = await provisionTenant(tenantData)

    if (result.success) {
      successMessage.value = 'Conta criada com sucesso! Redirecionando...'
      
      setTimeout(() => {
        router.push('/login')
      }, 2000)
    }
  } catch (err) {
    console.error('Erro no registro:', err)
    errors.general = 'Erro ao criar conta. Tente novamente.'
  }
}

// Watch para validação automática
watch(() => form.password, validatePassword)
watch(() => form.confirmPassword, validateConfirmPassword)

// Auto-preenchimento para desenvolvimento
if (import.meta.env.DEV) {
  form.name = 'Usuário Teste'
  form.companyName = 'Empresa Teste'
  form.email = 'teste@exemplo.com'
  form.password = 'Senha123!'
  form.confirmPassword = 'Senha123!'
  form.acceptTerms = true
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

/* Grid responsivo para dicas de senha */
@media (max-width: 640px) {
  .sm\:grid-cols-2 {
    grid-template-columns: 1fr;
  }
}
</style>