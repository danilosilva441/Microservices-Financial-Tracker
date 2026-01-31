// src/composables/auth/useAuthUI.js
import { ref, computed } from 'vue'
import { useAuth } from './useAuth'

export function useAuthUI() {
  const { isLoading, error, clearError } = useAuth()
  
  // Estados para UI
  const showLoginModal = ref(false)
  const showRegisterModal = ref(false)
  const showForgotPasswordModal = ref(false)
  const activeAuthTab = ref('login') // 'login' ou 'register'
  
  // Estados de formulário
  const loginForm = ref({
    email: '',
    password: '',
    rememberMe: false
  })
  
  const registerForm = ref({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    termsAccepted: false
  })
  
  const forgotPasswordForm = ref({
    email: ''
  })

  // Reseta todos os formulários
  const resetForms = () => {
    loginForm.value = {
      email: '',
      password: '',
      rememberMe: false
    }
    
    registerForm.value = {
      name: '',
      email: '',
      password: '',
      confirmPassword: '',
      termsAccepted: false
    }
    
    forgotPasswordForm.value = {
      email: ''
    }
    
    clearError()
  }

  // Abre modais
  const openLoginModal = () => {
    resetForms()
    showLoginModal.value = true
    showRegisterModal.value = false
    showForgotPasswordModal.value = false
    activeAuthTab.value = 'login'
  }

  const openRegisterModal = () => {
    resetForms()
    showRegisterModal.value = true
    showLoginModal.value = false
    showForgotPasswordModal.value = false
    activeAuthTab.value = 'provision'
  }

  const openForgotPasswordModal = () => {
    resetForms()
    showForgotPasswordModal.value = true
    showLoginModal.value = false
    showRegisterModal.value = false
  }

  const closeAllModals = () => {
    showLoginModal.value = false
    showRegisterModal.value = false
    showForgotPasswordModal.value = false
    resetForms()
  }

  // Validação de formulários
  const isLoginFormValid = computed(() => {
    const form = loginForm.value
    return (
      form.email.includes('@') &&
      form.email.length > 5 &&
      form.password.length >= 6
    )
  })

  const isRegisterFormValid = computed(() => {
    const form = registerForm.value
    return (
      form.name.length >= 2 &&
      form.email.includes('@') &&
      form.email.length > 5 &&
      form.password.length >= 6 &&
      form.password === form.confirmPassword &&
      form.termsAccepted
    )
  })

  const isForgotPasswordFormValid = computed(() => {
    return forgotPasswordForm.value.email.includes('@')
  })

  return {
    // UI States
    showLoginModal,
    showRegisterModal,
    showForgotPasswordModal,
    activeAuthTab,
    
    // Form States
    loginForm,
    registerForm,
    forgotPasswordForm,
    
    // Form Validations
    isLoginFormValid,
    isRegisterFormValid,
    isForgotPasswordFormValid,
    
    // Actions
    openLoginModal,
    openRegisterModal,
    openForgotPasswordModal,
    closeAllModals,
    resetForms,
    
    // Auth States
    isLoading,
    error,
    clearError,
  }
}