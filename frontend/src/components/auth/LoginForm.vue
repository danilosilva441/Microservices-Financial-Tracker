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
        <button type="button" class="toggle-password" @click="showPassword = !showPassword">
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
.login-form {
  width: 100%;
}

.alert {
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.alert-error {
  background: #fee;
  color: #c33;
  border: 1px solid #fcc;
}

.alert-success {
  background: #efe;
  color: #393;
  border: 1px solid #cfc;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
}

.label-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.forgot-link {
  font-size: 14px;
  color: #667eea;
  text-decoration: none;
}

.forgot-link:hover {
  text-decoration: underline;
}

.input-with-icon {
  position: relative;
  display: flex;
  align-items: center;
}

.input-with-icon i {
  position: absolute;
  left: 15px;
  color: #999;
  z-index: 1;
}

.input-with-icon input {
  width: 100%;
  padding: 14px 14px 14px 45px;
  border: 2px solid #eee;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.3s;
}

.input-with-icon input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.input-with-icon input:disabled {
  background: #f5f5f5;
  cursor: not-allowed;
}

.toggle-password {
  position: absolute;
  right: 15px;
  background: none;
  border: none;
  color: #999;
  cursor: pointer;
  padding: 5px;
}

.toggle-password:hover {
  color: #667eea;
}

.error-message {
  color: #c33;
  font-size: 14px;
  margin-top: 5px;
}

.checkbox-group {
  display: flex;
  align-items: center;
}

.checkbox-label {
  display: flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}

.checkbox-label input {
  display: none;
}

.checkbox-custom {
  width: 20px;
  height: 20px;
  border: 2px solid #ddd;
  border-radius: 4px;
  margin-right: 10px;
  position: relative;
  transition: all 0.3s;
}

.checkbox-label input:checked+.checkbox-custom {
  background: #667eea;
  border-color: #667eea;
}

.checkbox-label input:checked+.checkbox-custom::after {
  content: '✓';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 12px;
}

.btn-submit {
  width: 100%;
  padding: 16px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.btn-submit:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.social-buttons {
  margin-top: 30px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.btn-google,
.btn-github {
  width: 100%;
  padding: 14px;
  border: 2px solid #eee;
  border-radius: 8px;
  background: white;
  font-size: 15px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.btn-google:hover:not(:disabled),
.btn-github:hover:not(:disabled) {
  border-color: #ddd;
  transform: translateY(-2px);
}

.btn-google:disabled,
.btn-github:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Estilos específicos para o checkbox "Lembrar de mim" */
.remember-me-group {
  margin-top: 1rem;
  margin-bottom: 1rem;
}

.remember-me-label {
  padding: 0.75rem;
  border-radius: 10px;
  background: linear-gradient(135deg, rgba(249, 250, 251, 0.8) 0%, rgba(243, 244, 246, 0.8) 100%);
  border: 1px solid rgba(229, 231, 235, 0.5);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  margin-left: -0.75rem;
}

.remember-me-label:hover {
  background: linear-gradient(135deg, rgba(239, 246, 255, 0.9) 0%, rgba(219, 234, 254, 0.9) 100%);
  border-color: rgba(59, 130, 246, 0.3);
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.05);
}

.remember-me-label:active {
  transform: translateY(0);
}

.remember-me-icon {
  position: relative;
  height: 22px;
  min-width: 22px;
  border: 2px solid rgba(59, 130, 246, 0.4);
  border-radius: 6px;
  margin-right: 12px;
  margin-top: 2px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.remember-me-label:hover .remember-me-icon {
  border-color: #0B5FFF;
  box-shadow: 0 4px 10px rgba(11, 95, 255, 0.12);
}

.checkbox-input:checked+.remember-me-icon {
  background: linear-gradient(135deg, #0B5FFF 0%, #08B3A4 100%);
  border-color: transparent;
  animation: remember-check 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

/* Ícones */
.remember-icon {
  width: 14px;
  height: 14px;
  opacity: 0.8;
  transition: all 0.3s ease;
}

.remember-me-label:hover .remember-icon {
  opacity: 1;
  stroke: #0B5FFF;
}

.checkbox-check {
  width: 14px;
  height: 10px;
  stroke: white;
  stroke-width: 2;
  stroke-linecap: round;
  stroke-linejoin: round;
  fill: none;
}

/* Texto */
.remember-me-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.remember-main-text {
  font-size: 15px;
  font-weight: 600;
  color: #1f2937;
  line-height: 1.4;
}

.checkbox-input:checked~.remember-me-text .remember-main-text {
  color: #0B5FFF;
  font-weight: 600;
}

.remember-hint {
  font-size: 12px;
  color: #6b7280;
  line-height: 1.3;
  font-weight: 400;
}

.checkbox-input:checked~.remember-me-text .remember-hint {
  color: #475569;
}

/* Estado desabilitado */
.checkbox-input:disabled+.remember-me-icon {
  opacity: 0.5;
  background: #f3f4f6;
  border-color: #d1d5db;
}

.checkbox-input:disabled~.remember-me-text .remember-main-text {
  color: #9ca3af;
}

.checkbox-input:disabled~.remember-me-text .remember-hint {
  color: #d1d5db;
}

/* Animação específica */
@keyframes remember-check {
  0% {
    transform: scale(0.9) rotate(-10deg);
    opacity: 0.7;
  }

  50% {
    transform: scale(1.1) rotate(5deg);
  }

  100% {
    transform: scale(1) rotate(0deg);
    opacity: 1;
  }
}

/* Responsividade */
@media (max-width: 640px) {
  .remember-me-label {
    padding: 0.625rem;
    margin-left: -0.625rem;
  }

  .remember-me-icon {
    height: 20px;
    min-width: 20px;
    margin-right: 10px;
  }

  .remember-icon {
    width: 12px;
    height: 12px;
  }

  .remember-main-text {
    font-size: 14px;
  }

  .remember-hint {
    font-size: 11px;
  }
}
</style>