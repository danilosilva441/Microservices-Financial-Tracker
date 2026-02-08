<!-- src/components/auth/LoginForm.vue -->
<template>
  <form @submit.prevent="handleSubmit" class="login-form">
    <!-- Mensagens de erro/sucesso -->
    <div v-if="error" class="alert alert-error">
      <i class="fas fa-exclamation-circle"></i>
      {{ error }}
    </div>

    <div v-if="successMessage" class="alert alert-success">
      <i class="fas fa-check-circle"></i>
      {{ successMessage }}
    </div>

    <!-- Campo Email -->
    <div class="form-group">
      <label for="email">Email</label>
      <div class="input-with-icon">
        <i class="fas fa-envelope"></i>
        <input id="email" v-model="form.email" type="email" placeholder="seu@email.com" required :disabled="isLoading"
          @input="clearError" />
      </div>
      <div v-if="errors.email" class="error-message">
        {{ errors.email }}
      </div>
    </div>

    <!-- Campo Senha -->
    <div class="form-group">
      <div class="label-row">
        <label for="password">Senha</label>
        <router-link to="/forgot-password" class="forgot-link">
          Esqueceu a senha?
        </router-link>
      </div>
      <div class="input-with-icon">
        <i class="fas fa-lock"></i>
        <input id="password" v-model="form.password" :type="showPassword ? 'text' : 'password'" placeholder="Sua senha"
          required :disabled="isLoading" @input="clearError" />
        <button type="button" class="toggle-password" @click="showPassword = !showPassword"
          :title="showPassword ? 'Ocultar senha' : 'Mostrar senha'" :disabled="isLoading">
          <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
        </button>
      </div>
      <div v-if="errors.password" class="error-message">
        {{ errors.password }}
      </div>
    </div>

    <!-- Lembrar de mim -->
    <!-- Lembrar de mim -->
    <div class="form-group checkbox-group">
      <div class="checkbox-wrapper">
        <label class="checkbox-label">
          <input type="checkbox" v-model="form.rememberMe" :disabled="isLoading" class="checkbox-input" />
          <span class="checkbox-custom">
            <svg v-if="form.rememberMe" class="checkbox-check" viewBox="0 0 12 10">
              <polyline points="1.5 6 4.5 9 10.5 1" />
            </svg>
          </span>
          <span class="checkbox-text">
            Lembrar de mim
          </span>
        </label>
      </div>
    </div>

    <!-- Botão de submit -->
    <button type="submit" class="btn-submit" :disabled="isLoading || !isFormValid">
      <span v-if="!isLoading">
        <i class="fas fa-sign-in-alt"></i>
        Entrar
      </span>
      <span v-else>
        <i class="fas fa-spinner fa-spin"></i>
        Entrando...
      </span>
    </button>

    <!-- Login com Google/GitHub -->
    <div class="social-buttons" v-if="showSocial">
      <button type="button" class="btn-google" @click="loginWithGoogle" :disabled="isLoading">
        <i class="fab fa-google"></i>
        Entrar com Google
      </button>

      <button type="button" class="btn-github" @click="loginWithGithub" :disabled="isLoading">
        <i class="fab fa-github"></i>
        Entrar com GitHub
      </button>
    </div>
  </form>
</template>

<script>
import { ref, computed, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'LoginForm',

  props: {
    showSocial: {
      type: Boolean,
      default: true
    },
    redirectTo: {
      type: String,
      default: '/dashboard'
    }
  },

  setup(props) {
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
      // Limpa erros anteriores
      clearErrors()
      clearError()

      // Validação simples
      if (!form.email.includes('@')) {
        errors.email = 'Email inválido'
        return
      }

      if (form.password.length < 6) {
        errors.password = 'A senha deve ter pelo menos 6 caracteres'
        return
      }

      try {
        // Faz login
        const result = await login(form)

        if (result.success) {
          successMessage.value = 'Login realizado com sucesso!'

          // Redireciona após sucesso
          setTimeout(() => {
            // Verifica se há redirect na query string
            const redirect = route.query.redirect || props.redirectTo
            router.push(redirect)
          }, 1500)
        } else {
          // Erro já é tratado pela store
          console.error('Login falhou:', result.error)
        }
      } catch (err) {
        console.error('Erro no login:', err)
        errors.general = 'Erro ao fazer login. Tente novamente.'
      }
    }

    // Login social (mock - implemente conforme sua API)
    const loginWithGoogle = async () => {
      console.log('Login com Google')
      // Implemente a integração com Google OAuth
    }

    const loginWithGithub = async () => {
      console.log('Login com GitHub')
      // Implemente a integração com GitHub OAuth
    }

    // Limpar erros
    const clearErrors = () => {
      Object.keys(errors).forEach(key => {
        errors[key] = ''
      })
    }

    // Limpar mensagens quando usuário digitar
    const handleInput = () => {
      clearErrors()
      clearError()
      successMessage.value = ''
    }

    // Auto-preenchimento para desenvolvimento
    const prefillForDevelopment = () => {
      if (process.env.NODE_ENV === 'development') {
        form.email = 'admin@exemplo.com'
        form.password = '123456'
      }
    }

    // Executa ao montar
    prefillForDevelopment()

    return {
      form,
      showPassword,
      errors,
      successMessage,
      isLoading,
      error,
      isFormValid,
      handleSubmit,
      loginWithGoogle,
      loginWithGithub,
      clearError,
      handleInput
    }
  }
}
</script>

<style scoped>
@import './CSS/index.css';
</style>