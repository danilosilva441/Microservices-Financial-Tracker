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

    <!-- Nome Empresa -->
    <div class="form-group">
      <label for="companyName">Nome da Empresa</label>
      <div class="input-with-icon">
        <i class="fas fa-building"></i>
        <input id="companyName" v-model="form.companyName" type="text" placeholder="Nome da sua empresa" required
          :disabled="isLoading" @input="clearError" />
      </div>
      <div v-if="errors.companyName" class="error-message">
        {{ errors.companyName }}
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
        <input id="phone" v-model="form.phone" type="tel" placeholder="(81) 9 9999-9999" :disabled="isLoading"
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
        const tenantData = {
          nomeCompletoGerente: form.name,
          nomeDaEmpresa: form.companyName,
          emailDoGerente: form.email,
          senhaDoGerente: form.password,
          phone: form.phone || undefined,
          newsletter: form.newsletter
        }

        // Registra usuário
        const result = await provisionTenant(tenantData)

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
@import './CSS/index.css';
</style>
