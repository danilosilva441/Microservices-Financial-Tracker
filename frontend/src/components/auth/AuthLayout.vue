<!-- src/components/auth/AuthLayout.vue -->
<template>
  <div class="auth-layout">
    <div class="auth-container">
      <!-- Header - Desktop -->
      <header class="auth-header-desktop">
        <router-link to="/" class="logo">
          <LogoIcon class="logo-icon" />
          <span class="logo-text">{{ appName }}</span>
        </router-link>
        
        <div class="auth-actions">
          <span class="auth-prompt">
            {{ promptText }}
          </span>
          <router-link 
            :to="actionRoute" 
            class="auth-link"
            :aria-label="actionText"
          >
            {{ actionText }}
          </router-link>
        </div>
      </header>
      
      <!-- Conteúdo principal -->
      <main class="auth-main">
        <!-- Card de autenticação -->
        <div class="auth-card-container">
          <!-- Header mobile -->
          <header class="auth-header-mobile">
            <router-link to="/" class="logo-mobile" aria-label="Página inicial">
              <LogoIcon class="logo-icon-mobile" />
              <h1 class="app-name-mobile">{{ appName }}</h1>
            </router-link>
          </header>
          
          <div class="auth-card">
            <!-- Título do formulário -->
            <div class="auth-title">
              <h1>{{ title }}</h1>
              <p class="auth-subtitle" v-if="subtitle">{{ subtitle }}</p>
            </div>
            
            <!-- Slot para formulários -->
            <div class="auth-form">
              <slot></slot>
            </div>
            
            <!-- Links extras -->
            <div class="auth-extras" v-if="showExtras && !hideForgotPassword">
              <router-link 
                to="/forgot-password" 
                class="forgot-password-link"
                aria-label="Recuperar senha"
              >
                Esqueceu sua senha?
              </router-link>
            </div>
            
            <!-- Separador -->
            <div class="auth-separator" v-if="showSeparator">
              <span>OU</span>
            </div>
            
            <!-- Login social -->
            <div class="social-login" v-if="showSocial">
              <button 
                class="social-btn google" 
                @click="handleSocialLogin('google')"
                aria-label="Continuar com Google"
              >
                <i class="fab fa-google"></i>
                <span>Continuar com Google</span>
              </button>
              <button 
                class="social-btn github" 
                @click="handleSocialLogin('github')"
                aria-label="Continuar com GitHub"
              >
                <i class="fab fa-github"></i>
                <span>Continuar com GitHub</span>
              </button>
            </div>
            
            <!-- Footer mobile -->
            <div class="auth-card-footer-mobile">
              <span class="auth-prompt-mobile">
                {{ promptText }}
              </span>
              <router-link 
                :to="actionRoute" 
                class="auth-link-mobile"
                :aria-label="actionText"
              >
                {{ actionText }}
              </router-link>
            </div>
          </div>
        </div>
        
        <!-- Painel lateral com ilustração - Desktop -->
        <aside class="auth-sidebar">
          <div class="sidebar-content">
            <div class="sidebar-logo">
              <LogoIcon class="sidebar-logo-img" />
              <h2 class="sidebar-app-name">{{ appName }}</h2>
            </div>
            
            <div class="sidebar-illustration">
              <!-- Ilustração minimalista -->
              <div class="illustration">
                <div class="icon-container">
                  <i class="fas fa-shield-alt"></i>
                </div>
                <h3>Segurança Garantida</h3>
                <p>Sua informação está protegida com criptografia de ponta</p>
              </div>
            </div>
            
            <div class="sidebar-features">
              <div class="feature">
                <i class="fas fa-rocket"></i>
                <span>Fácil e rápido</span>
              </div>
              <div class="feature">
                <i class="fas fa-users"></i>
                <span>Para equipes</span>
              </div>
              <div class="feature">
                <i class="fas fa-chart-line"></i>
                <span>Dashboard completo</span>
              </div>
            </div>
            
            <div class="sidebar-quote">
              <p>"O primeiro passo para o sucesso é começar."</p>
            </div>
          </div>
        </aside>
      </main>
      
      <!-- Footer desktop -->
      <footer class="auth-global-footer-desktop">
        <p>&copy; {{ currentYear }} {{ appName }}. Todos os direitos reservados.</p>
        <div class="footer-links">
          <router-link to="/terms" aria-label="Termos de uso">Termos de Uso</router-link>
          <router-link to="/privacy" aria-label="Política de privacidade">Política de Privacidade</router-link>
          <router-link to="/help" aria-label="Ajuda">Ajuda</router-link>
          <router-link to="/contact" aria-label="Contato">Contato</router-link>
        </div>
      </footer>

      <!-- Footer mobile -->
      <footer class="auth-global-footer-mobile">
        <div class="footer-links-mobile">
          <router-link to="/terms" aria-label="Termos">Termos</router-link>
          <router-link to="/privacy" aria-label="Privacidade">Privacidade</router-link>
          <router-link to="/help" aria-label="Ajuda">Ajuda</router-link>
          <router-link to="/contact" aria-label="Contato">Contato</router-link>
        </div>
        <p>&copy; {{ currentYear }} {{ appName }}</p>
      </footer>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import LogoIcon from '@/components/icons/LogoIcon.vue'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  subtitle: {
    type: String,
    default: ''
  },
  showExtras: {
    type: Boolean,
    default: true
  },
  showSeparator: {
    type: Boolean,
    default: false
  },
  showSocial: {
    type: Boolean,
    default: false
  },
  hideForgotPassword: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['social-login'])

const route = useRoute()
const appName = 'DS SysTech'
const currentYear = ref(new Date().getFullYear())

// Computed properties
const isLoginRoute = computed(() => route.name === 'login')
const isRegisterRoute = computed(() => route.name === 'register')

const promptText = computed(() => 
  isLoginRoute.value ? 'Não tem uma conta?' : 'Já tem uma conta?'
)

const actionText = computed(() => 
  isLoginRoute.value ? 'Criar conta' : 'Fazer login'
)

const actionRoute = computed(() => 
  isLoginRoute.value ? '/provision' : '/login'
)

// Methods
const handleSocialLogin = (provider) => {
  emit('social-login', provider)
}

// Lifecycle
onMounted(() => {
  // Adiciona classes para carregamento suave
  document.documentElement.classList.add('auth-page-loaded')
})
</script>

<style scoped>
/* Estilos base */
.auth-layout {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.auth-container {
  width: 100%;
  max-width: 1200px;
  min-height: 800px;
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
}

/* Header Desktop */
.auth-header-desktop {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px 40px;
  border-bottom: 1px solid #e2e8f0;
}

.logo {
  display: flex;
  align-items: center;
  gap: 12px;
  text-decoration: none;
  transition: opacity 0.2s;
}

.logo:hover {
  opacity: 0.9;
}

.logo-icon {
  width: 32px;
  height: 32px;
}

.logo-text {
  font-size: 20px;
  font-weight: 700;
  color: #2d3748;
  letter-spacing: -0.5px;
}

.auth-actions {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 14px;
}

.auth-prompt {
  color: #718096;
}

.auth-link {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
  padding: 8px 16px;
  border: 2px solid #667eea;
  border-radius: 8px;
  transition: all 0.2s ease;
  display: inline-block;
  background: transparent;
}

.auth-link:hover {
  background: #667eea;
  color: white;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
}

.auth-link:active {
  transform: translateY(0);
}

/* Conteúdo principal */
.auth-main {
  flex: 1;
  display: flex;
  min-height: 600px;
}

/* Card de autenticação */
.auth-card-container {
  flex: 1;
  padding: 40px;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  min-height: 0;
}

.auth-header-mobile {
  display: none;
  margin-bottom: 40px;
  text-align: center;
}

.logo-mobile {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  text-decoration: none;
  transition: opacity 0.2s;
}

.logo-mobile:hover {
  opacity: 0.9;
}

.logo-icon-mobile {
  width: 32px;
  height: 32px;
}

.app-name-mobile {
  font-size: 24px;
  color: #2d3748;
  margin: 0;
  font-weight: 700;
  letter-spacing: -0.5px;
}

.auth-card {
  max-width: 400px;
  width: 100%;
  margin: 0 auto;
}

.auth-title {
  text-align: center;
  margin-bottom: 40px;
}

.auth-title h1 {
  font-size: 32px;
  color: #2d3748;
  margin: 0 0 8px 0;
  font-weight: 700;
  line-height: 1.2;
}

.auth-subtitle {
  color: #718096;
  font-size: 16px;
  margin: 0;
  line-height: 1.5;
  max-width: 320px;
  margin: 0 auto;
}

.auth-form {
  margin-bottom: 24px;
}

.auth-extras {
  text-align: right;
  margin-bottom: 24px;
}

.forgot-password-link {
  color: #667eea;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
  display: inline-block;
}

.forgot-password-link:hover {
  text-decoration: underline;
  color: #5a67d8;
}

.auth-separator {
  text-align: center;
  margin: 32px 0;
  position: relative;
}

.auth-separator::before {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  top: 50%;
  height: 1px;
  background: #e2e8f0;
  transform: translateY(-50%);
}

.auth-separator span {
  background: white;
  padding: 0 20px;
  color: #a0aec0;
  font-size: 14px;
  font-weight: 500;
  position: relative;
  z-index: 1;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.social-login {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 32px;
}

.social-btn {
  padding: 14px 20px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  font-weight: 500;
  color: #4a5568;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 15px;
  outline: none;
  position: relative;
  overflow: hidden;
}

.social-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: currentColor;
  opacity: 0;
  transition: opacity 0.2s;
}

.social-btn:hover {
  border-color: #cbd5e0;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.social-btn:hover::before {
  opacity: 0.05;
}

.social-btn:active {
  transform: translateY(0);
}

.social-btn i {
  font-size: 18px;
  position: relative;
  z-index: 1;
}

.social-btn span {
  position: relative;
  z-index: 1;
}

.social-btn.google:hover {
  color: #DB4437;
}

.social-btn.github:hover {
  color: #333;
}

.auth-card-footer-mobile {
  display: none;
  text-align: center;
  padding-top: 24px;
  border-top: 1px solid #e2e8f0;
  margin-top: 32px;
}

.auth-prompt-mobile {
  color: #718096;
  font-size: 14px;
  margin-right: 8px;
}

.auth-link-mobile {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.2s;
}

.auth-link-mobile:hover {
  text-decoration: underline;
  color: #5a67d8;
}

/* Painel lateral */
.auth-sidebar {
  flex: 1;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 60px 40px;
  color: white;
  display: flex;
  align-items: center;
  position: relative;
  overflow: hidden;
}

.auth-sidebar::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url("data:image/svg+xml,%3Csvg width='100' height='100' viewBox='0 0 100 100' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M11 18c3.866 0 7-3.134 7-7s-3.134-7-7-7-7 3.134-7 7 3.134 7 7 7zm48 25c3.866 0 7-3.134 7-7s-3.134-7-7-7-7 3.134-7 7 3.134 7 7 7zm-43-7c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zm63 31c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zM34 90c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zm56-76c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zM12 86c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm28-65c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm23-11c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm-6 60c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm29 22c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zM32 63c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm57-13c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm-9-21c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM60 91c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM35 41c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM12 60c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2z' fill='%23ffffff' fill-opacity='0.05' fill-rule='evenodd'/%3E%3C/svg%3E");
  opacity: 0.3;
}

.sidebar-content {
  max-width: 400px;
  margin: 0 auto;
  text-align: center;
  position: relative;
  z-index: 1;
}

.sidebar-logo {
  margin-bottom: 60px;
}

.sidebar-logo-img {
  width: 80px;
  height: 80px;
  margin-bottom: 20px;
}

.sidebar-app-name {
  font-size: 28px;
  font-weight: 700;
  margin: 0;
  color: white;
  letter-spacing: -0.5px;
}

.sidebar-illustration {
  margin: 60px 0;
}

.illustration {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 40px;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.icon-container {
  width: 80px;
  height: 80px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 24px;
  transition: transform 0.3s ease;
}

.illustration:hover .icon-container {
  transform: scale(1.05);
}

.icon-container i {
  font-size: 36px;
  color: white;
}

.illustration h3 {
  font-size: 20px;
  margin: 0 0 12px 0;
  font-weight: 600;
}

.illustration p {
  margin: 0;
  opacity: 0.9;
  font-size: 14px;
  line-height: 1.5;
}

.sidebar-features {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 40px;
}

.feature {
  display: flex;
  align-items: center;
  gap: 12px;
  justify-content: center;
  font-size: 16px;
  opacity: 0.9;
  transition: opacity 0.2s;
}

.feature:hover {
  opacity: 1;
}

.feature i {
  font-size: 18px;
}

.sidebar-quote p {
  font-style: italic;
  opacity: 0.9;
  font-size: 18px;
  line-height: 1.6;
  max-width: 320px;
  margin: 0 auto;
}

/* Footer desktop */
.auth-global-footer-desktop {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 40px;
  border-top: 1px solid #e2e8f0;
  font-size: 14px;
  color: #718096;
}

.auth-global-footer-desktop p {
  margin: 0;
}

.footer-links {
  display: flex;
  gap: 24px;
}

.footer-links a {
  color: #718096;
  text-decoration: none;
  transition: color 0.2s;
  font-size: 13px;
}

.footer-links a:hover {
  color: #667eea;
}

/* Footer mobile */
.auth-global-footer-mobile {
  display: none;
  padding: 20px;
  border-top: 1px solid #e2e8f0;
  text-align: center;
}

.footer-links-mobile {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.footer-links-mobile a {
  color: #718096;
  text-decoration: none;
  font-size: 12px;
  transition: color 0.2s;
}

.footer-links-mobile a:hover {
  color: #667eea;
}

.auth-global-footer-mobile p {
  margin: 0;
  font-size: 12px;
  color: #a0aec0;
}

/* Responsividade */
@media (max-width: 1024px) {
  .auth-container {
    min-height: auto;
    max-width: 1000px;
  }
  
  .auth-sidebar {
    padding: 40px 30px;
  }
  
  .sidebar-logo-img {
    width: 64px;
    height: 64px;
  }
  
  .sidebar-app-name {
    font-size: 24px;
  }
  
  .icon-container {
    width: 64px;
    height: 64px;
  }
  
  .icon-container i {
    font-size: 28px;
  }
  
  .illustration {
    padding: 32px;
  }
}

@media (max-width: 768px) {
  .auth-layout {
    padding: 0;
    background: white;
  }
  
  .auth-container {
    border-radius: 0;
    box-shadow: none;
    min-height: 100vh;
    max-width: 100%;
  }
  
  .auth-header-desktop,
  .auth-sidebar,
  .auth-global-footer-desktop {
    display: none;
  }
  
  .auth-header-mobile,
  .auth-card-footer-mobile,
  .auth-global-footer-mobile {
    display: block;
  }
  
  .auth-main {
    flex-direction: column;
  }
  
  .auth-card-container {
    padding: 24px;
    flex: 1;
  }
  
  .auth-card {
    max-width: 100%;
  }
  
  .auth-title h1 {
    font-size: 28px;
  }
  
  .auth-subtitle {
    font-size: 15px;
  }
  
  .social-btn {
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .auth-card-container {
    padding: 20px;
  }
  
  .auth-title h1 {
    font-size: 24px;
  }
  
  .auth-subtitle {
    font-size: 14px;
  }
  
  .social-btn {
    padding: 12px 16px;
  }
  
  .social-btn i {
    font-size: 16px;
  }
  
  .footer-links-mobile {
    gap: 12px;
  }
  
  .footer-links-mobile a {
    font-size: 11px;
  }
  
  .app-name-mobile {
    font-size: 22px;
  }
}

/* Animações */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.auth-card {
  animation: fadeIn 0.4s ease-out;
}

.auth-sidebar {
  animation: slideIn 0.5s ease-out;
}

/* Estado de carregamento */
.auth-page-loaded .auth-container {
  animation: fadeIn 0.3s ease-out;
}

/* Foco melhorado para acessibilidade */
.auth-link:focus-visible,
.forgot-password-link:focus-visible,
.social-btn:focus-visible,
.footer-links a:focus-visible {
  outline: 2px solid #667eea;
  outline-offset: 2px;
  border-radius: 4px;
}

/* Transições suaves */
.logo,
.auth-link,
.forgot-password-link,
.social-btn,
.footer-links a,
.feature {
  transition: all 0.2s ease;
}

/* Scrollbar customizada */
.auth-card-container::-webkit-scrollbar {
  width: 6px;
}

.auth-card-container::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.auth-card-container::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.auth-card-container::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}
</style>