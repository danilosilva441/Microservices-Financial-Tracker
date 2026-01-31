<!-- src/components/auth/RegisterForm.vue -->
<template>
  <form @submit.prevent="handleSubmit" class="register-form">
    <!-- Mensagens -->
    <div v-if="error" class="alert alert-error">
      <i class="fas fa-exclamation-circle"></i>
      {{ error }}
    </div>

    <div v-if="successMessage" class="alert alert-success">
      <i class="fas fa-check-circle"></i>
      {{ successMessage }}
    </div>

    <!-- Nome Completo -->
    <div class="form-group">
      <label for="name">Nome Completo</label>
      <div class="input-with-icon">
        <i class="fas fa-user"></i>
        <input id="name" v-model="form.name" type="text" placeholder="Seu nome completo" required :disabled="isLoading"
          @input="clearError" />
      </div>
      <div v-if="errors.name" class="error-message">
        {{ errors.name }}
      </div>
    </div>

    <!-- Email -->
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

    <!-- Telefone (opcional) -->
    <div class="form-group">
      <label for="phone">Telefone <span class="optional">(Opcional)</span></label>
      <div class="input-with-icon">
        <i class="fas fa-phone"></i>
        <input id="phone" v-model="form.phone" type="tel" placeholder="(11) 99999-9999" :disabled="isLoading"
          @input="clearError" />
      </div>
    </div>

    <!-- Senha -->
    <div class="form-group">
      <label for="password">Senha</label>
      <div class="input-with-icon">
        <i class="fas fa-lock"></i>
        <input id="password" v-model="form.password" :type="showPassword ? 'text' : 'password'"
          placeholder="Crie uma senha segura" required :disabled="isLoading" @input="validatePassword" />
        <button type="button" class="toggle-password" @click="showPassword = !showPassword">
          <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
        </button>
      </div>
      <div v-if="errors.password" class="error-message">
        {{ errors.password }}
      </div>

      <!-- Indicador de força da senha -->
      <div class="password-strength" v-if="form.password">
        <div class="strength-bar" :class="passwordStrengthClass"></div>
        <div class="strength-text">
          {{ passwordStrengthText }}
        </div>
      </div>

      <!-- Dicas para senha -->
      <ul class="password-hints" v-if="form.password">
        <li :class="{ valid: form.password.length >= 8 }">
          <i :class="form.password.length >= 8 ? 'fas fa-check' : 'fas fa-times'"></i>
          Pelo menos 8 caracteres
        </li>
        <li :class="{ valid: hasUpperCase }">
          <i :class="hasUpperCase ? 'fas fa-check' : 'fas fa-times'"></i>
          Letra maiúscula
        </li>
        <li :class="{ valid: hasLowerCase }">
          <i :class="hasLowerCase ? 'fas fa-check' : 'fas fa-times'"></i>
          Letra minúscula
        </li>
        <li :class="{ valid: hasNumber }">
          <i :class="hasNumber ? 'fas fa-check' : 'fas fa-times'"></i>
          Número
        </li>
        <li :class="{ valid: hasSpecialChar }">
          <i :class="hasSpecialChar ? 'fas fa-check' : 'fas fa-times'"></i>
          Caractere especial
        </li>
      </ul>
    </div>

    <!-- Confirmar Senha -->
    <div class="form-group">
      <label for="confirmPassword">Confirmar Senha</label>
      <div class="input-with-icon">
        <i class="fas fa-lock"></i>
        <input id="confirmPassword" v-model="form.confirmPassword" :type="showConfirmPassword ? 'text' : 'password'"
          placeholder="Confirme sua senha" required :disabled="isLoading" @input="validateConfirmPassword" />
        <button type="button" class="toggle-password" @click="showConfirmPassword = !showConfirmPassword">
          <i :class="showConfirmPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
        </button>
      </div>
      <div v-if="errors.confirmPassword" class="error-message">
        {{ errors.confirmPassword }}
      </div>
    </div>

    <!-- Termos e Condições -->
    <div class="form-group checkbox-group" :class="{ 'has-error': errors.acceptTerms }">
      <div class="checkbox-wrapper">
        <label class="checkbox-label">
          <input type="checkbox" v-model="form.acceptTerms" :disabled="isLoading" required class="checkbox-input" />
          <span class="checkbox-custom">
            <svg v-if="form.acceptTerms" class="checkbox-check" viewBox="0 0 12 10">
              <polyline points="1.5 6 4.5 9 10.5 1" />
            </svg>
          </span>
          <span class="checkbox-text">
            Eu concordo com os
            <router-link to="/terms" class="terms-link">Termos de Uso</router-link>
            e
            <router-link to="/privacy" class="terms-link">Política de Privacidade</router-link>
            <span class="required-asterisk">*</span>
          </span>
        </label>
        <div v-if="errors.acceptTerms" class="error-message">
          <svg class="error-icon" viewBox="0 0 24 24">
            <path
              d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z" />
          </svg>
          {{ errors.acceptTerms }}
        </div>
      </div>
    </div>

    <!-- Newsletter (opcional) -->
    <div class="form-group checkbox-group">
      <div class="checkbox-wrapper">
        <label class="checkbox-label">
          <input type="checkbox" v-model="form.newsletter" :disabled="isLoading" class="checkbox-input" />
          <span class="checkbox-custom">
            <svg v-if="form.newsletter" class="checkbox-check" viewBox="0 0 12 10">
              <polyline points="1.5 6 4.5 9 10.5 1" />
            </svg>
          </span>
          <span class="checkbox-text">
            Desejo receber novidades e atualizações por email
            <span class="optional-label">(opcional)</span>
          </span>
        </label>
      </div>
    </div>

    <!-- Botão de submit -->
    <button type="submit" class="btn-submit" :disabled="isLoading || !isFormValid">
      <span v-if="!isLoading">
        <i class="fas fa-user-plus"></i>
        Criar Conta
      </span>
      <span v-else>
        <i class="fas fa-spinner fa-spin"></i>
        Criando conta...
      </span>
    </button>

    <!-- Já tem conta -->
    <div class="already-have-account">
      <p>Já tem uma conta?
        <router-link to="/login" class="login-link">
          Faça login aqui
        </router-link>
      </p>
    </div>
  </form>
</template>

<script>
import { ref, computed, reactive, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'RegisterForm',

  setup() {
    const router = useRouter()
    const { registerTeamUser, isLoading, error, clearError } = useAuth()

    // Estado do formulário
    const form = reactive({
      name: '',
      email: '',
      phone: '',
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
    const hasSpecialChar = computed(() => /[^A-Za-z0-9]/.test(form.password))

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
      if (strength <= 1) return 'very-weak'
      if (strength === 2) return 'weak'
      if (strength === 3) return 'medium'
      if (strength === 4) return 'strong'
      return 'very-strong'
    })

    // Validação do formulário
    const isFormValid = computed(() => {
      return (
        form.name.length >= 2 &&
        form.email.includes('@') &&
        form.password.length >= 8 &&
        form.password === form.confirmPassword &&
        form.acceptTerms &&
        passwordStrength.value >= 3 // Senha pelo menos média
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

    // Manipular envio
    const handleSubmit = async () => {
      clearErrors()
      clearError()

      // Validações
      if (form.name.length < 2) {
        errors.name = 'Nome muito curto'
        return
      }

      if (!form.email.includes('@')) {
        errors.email = 'Email inválido'
        return
      }

      validatePassword()
      validateConfirmPassword()

      if (!form.acceptTerms) {
        errors.acceptTerms = 'Você deve aceitar os termos'
        return
      }

      // Verifica se há erros
      if (Object.values(errors).some(error => error)) {
        return
      }

      try {
        // Prepara dados para registro
        const userData = {
          name: form.name,
          email: form.email,
          password: form.password,
          phone: form.phone || undefined,
          newsletter: form.newsletter
        }

        // Registra usuário
        const result = await registerTeamUser(userData)

        if (result.success) {
          successMessage.value = 'Conta criada com sucesso! Redirecionando...'

          // Redireciona após sucesso
          setTimeout(() => {
            router.push('/login')
          }, 2000)
        } else {
          console.error('Registro falhou:', result.error)
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
    const prefillForDevelopment = () => {
      if (process.env.NODE_ENV === 'development') {
        form.name = 'Usuário Teste'
        form.email = 'teste@exemplo.com'
        form.password = 'Senha123!'
        form.confirmPassword = 'Senha123!'
        form.acceptTerms = true
      }
    }

    prefillForDevelopment()

    return {
      form,
      showPassword,
      showConfirmPassword,
      errors,
      successMessage,
      isLoading,
      error,
      isFormValid,
      hasUpperCase,
      hasLowerCase,
      hasNumber,
      hasSpecialChar,
      passwordStrength,
      passwordStrengthText,
      passwordStrengthClass,
      handleSubmit,
      clearError,
      validatePassword,
      validateConfirmPassword
    }
  }
}
</script>

<style scoped>
.register-form {
  width: 100%;
}

.optional {
  color: #999;
  font-size: 12px;
  font-weight: normal;
}

.password-strength {
  margin-top: 10px;
}

.strength-bar {
  height: 4px;
  border-radius: 2px;
  margin-bottom: 5px;
  transition: all 0.3s;
}

.strength-bar.very-weak {
  width: 20%;
  background: #ff4d4d;
}

.strength-bar.weak {
  width: 40%;
  background: #ff944d;
}

.strength-bar.medium {
  width: 60%;
  background: #ffd24d;
}

.strength-bar.strong {
  width: 80%;
  background: #8cff4d;
}

.strength-bar.very-strong {
  width: 100%;
  background: #4dff88;
}

.strength-text {
  font-size: 12px;
  color: #666;
}

.password-hints {
  list-style: none;
  padding: 0;
  margin: 10px 0 0 0;
}

.password-hints li {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: #999;
  margin-bottom: 5px;
}

.password-hints li.valid {
  color: #393;
}

.password-hints li i.fa-check {
  color: #393;
}

.password-hints li i.fa-times {
  color: #c33;
}

.terms-link {
  color: #667eea;
  text-decoration: none;
}

.terms-link:hover {
  text-decoration: underline;
}

.already-have-account {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #eee;
  text-align: center;
  color: #666;
}

.login-link {
  color: #667eea;
  text-decoration: none;
  font-weight: 500;
}

.login-link:hover {
  text-decoration: underline;
}

/* Estilos herdados do LoginForm */
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
  font-size: 14px;
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
  flex-shrink: 0;
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

.checkbox-group {
  margin-bottom: 1.5rem;
}

.checkbox-group.has-error .checkbox-custom {
  border-color: #ff4757;
}

.checkbox-group.has-error .checkbox-text {
  color: #ff4757;
}

.checkbox-wrapper {
  position: relative;
}

.checkbox-label {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  user-select: none;
  transition: all 0.2s ease;
  padding: 0.5rem;
  border-radius: 8px;
  margin-left: -0.5rem;
}

.checkbox-label:hover {
  background: rgba(59, 130, 246, 0.05);
}

.checkbox-label:active {
  transform: translateY(1px);
}

.checkbox-input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

.checkbox-custom {
  position: relative;
  height: 22px;
  min-width: 22px;
  border: 2px solid #d1d5db;
  border-radius: 6px;
  margin-right: 12px;
  margin-top: 2px;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.checkbox-label:hover .checkbox-custom {
  border-color: #9ca3af;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.checkbox-input:checked + .checkbox-custom {
  background: linear-gradient(135deg, #0B5FFF 0%, #08B3A4 100%);
  border-color: transparent;
  animation: checkmark-pop 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.checkbox-input:disabled + .checkbox-custom {
  opacity: 0.5;
  cursor: not-allowed;
  background: #f3f4f6;
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

.checkbox-text {
  font-size: 15px;
  line-height: 1.5;
  color: #374151;
  flex: 1;
  padding-top: 1px;
}

.terms-link {
  color: #0B5FFF;
  text-decoration: none;
  font-weight: 600;
  position: relative;
  transition: all 0.2s ease;
}

.terms-link:hover {
  color: #08B3A4;
  text-decoration: underline;
}

.terms-link::after {
  content: '';
  position: absolute;
  width: 100%;
  height: 1px;
  bottom: -1px;
  left: 0;
  background: linear-gradient(90deg, #0B5FFF, #08B3A4);
  opacity: 0;
  transition: opacity 0.2s ease;
}

.terms-link:hover::after {
  opacity: 1;
}

.required-asterisk {
  color: #ef4444;
  font-weight: bold;
  margin-left: 2px;
}

.optional-label {
  font-size: 13px;
  color: #6b7280;
  font-style: italic;
  margin-left: 6px;
  font-weight: 400;
}

.error-message {
  display: flex;
  align-items: center;
  color: #ff4757;
  font-size: 13px;
  margin-top: 6px;
  margin-left: 34px;
  animation: shake 0.5s ease;
  font-weight: 500;
}

.error-icon {
  width: 16px;
  height: 16px;
  margin-right: 6px;
  fill: #ff4757;
  flex-shrink: 0;
}

@keyframes checkmark-pop {
  0% {
    transform: scale(0.8);
    opacity: 0.5;
  }
  50% {
    transform: scale(1.1);
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes shake {
  0%, 100% {
    transform: translateX(0);
  }
  10%, 30%, 50%, 70%, 90% {
    transform: translateX(-2px);
  }
  20%, 40%, 60%, 80% {
    transform: translateX(2px);
  }
}

/* Responsividade */
@media (max-width: 640px) {
  .checkbox-text {
    font-size: 14px;
  }
  
  .checkbox-custom {
    height: 20px;
    min-width: 20px;
    margin-right: 10px;
  }
  
  .checkbox-check {
    width: 12px;
    height: 9px;
  }
  
  .error-message {
    font-size: 12px;
    margin-left: 30px;
  }
}
</style>