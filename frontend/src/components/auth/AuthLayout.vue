<!-- src/components/auth/AuthLayout.vue -->
<template>
  <div class="min-h-screen transition-colors duration-200 bg-gray-50 dark:bg-gray-900" :class="{ 'dark': isDarkMode }">
    <!-- Header Mobile com Theme Toggle -->
    <header class="fixed top-0 left-0 right-0 z-50 bg-white border-b border-gray-200 lg:hidden dark:bg-gray-800 dark:border-gray-700">
      <div class="flex items-center justify-between px-4 py-3">
        <router-link to="/" class="flex items-center gap-2 group">
          <div class="p-2 transition-transform duration-200 rounded-lg bg-gradient-to-r from-primary-500 to-secondary-500 group-hover:scale-105">
            <IconRocket class="w-5 h-5 text-white" />
          </div>
          <span class="text-xl font-bold text-transparent bg-gradient-to-r from-primary-600 to-secondary-600 bg-clip-text">
            DS SysTech
          </span>
        </router-link>

        <button 
          @click="toggleTheme"
          class="action-button"
          aria-label="Alternar tema"
        >
          <IconSun v-if="isDarkMode" class="w-5 h-5 text-yellow-500" />
          <IconMoon v-else class="w-5 h-5 text-gray-600 dark:text-gray-300" />
        </button>
      </div>
    </header>

    <div class="flex min-h-screen pt-16 lg:pt-0">
      <!-- Painel Lateral - Desktop -->
      <aside class="relative hidden overflow-hidden lg:flex lg:w-1/2 bg-gradient-to-br from-primary-600 via-primary-500 to-secondary-500">
        <!-- Padrão geométrico -->
        <div class="absolute inset-0 opacity-10">
          <svg class="w-full h-full" viewBox="0 0 100 100" preserveAspectRatio="none">
            <pattern id="grid" x="0" y="0" width="20" height="20" patternUnits="userSpaceOnUse">
              <path d="M 20 0 L 0 0 0 20" fill="none" stroke="white" stroke-width="0.5" />
            </pattern>
            <rect x="0" y="0" width="100%" height="100%" fill="url(#grid)" />
          </svg>
        </div>

        <div class="relative flex flex-col items-center justify-between w-full p-12 text-white">
          <!-- Logo e Título -->
          <div class="text-center">
            <div class="flex justify-center mb-6">
              <div class="p-4 bg-white/10 rounded-2xl backdrop-blur-sm">
                <IconStore class="w-16 h-16 text-white" />
              </div>
            </div>
            <h1 class="mb-2 text-4xl font-bold">DS SysTech</h1>
            <p class="text-lg text-white/80">Soluções inteligentes para seu negócio</p>
          </div>

          <!-- Features -->
          <div class="w-full max-w-md space-y-8">
            <div class="grid gap-4">
              <div v-for="(feature, index) in features" :key="index" 
                   class="flex items-center gap-4 p-4 transition-all duration-300 transform bg-white/10 rounded-xl backdrop-blur-sm hover:bg-white/20 hover:scale-105">
                <div class="p-3 rounded-lg bg-white/20">
                  <component :is="feature.icon" class="w-6 h-6" />
                </div>
                <div>
                  <h3 class="font-semibold">{{ feature.title }}</h3>
                  <p class="text-sm text-white/70">{{ feature.description }}</p>
                </div>
              </div>
            </div>

            <!-- Citação -->
            <div class="text-center">
              <div class="inline-block p-4 bg-white/5 rounded-2xl backdrop-blur-sm">
                <p class="text-lg italic text-white/90">"O primeiro passo para o sucesso é começar."</p>
              </div>
            </div>
          </div>

          <!-- Footer do Sidebar -->
          <div class="text-sm text-center text-white/60">
            <p>&copy; {{ currentYear }} DS SysTech. Todos os direitos reservados.</p>
          </div>
        </div>
      </aside>

      <!-- Conteúdo Principal -->
      <main class="flex items-center justify-center flex-1 p-4 lg:p-8">
        <div class="w-full max-w-md">
          <!-- Header Desktop (Theme Toggle) -->
          <div class="justify-end hidden mb-6 lg:flex">
            <button 
              @click="toggleTheme"
              class="action-button"
              aria-label="Alternar tema"
            >
              <IconSun v-if="isDarkMode" class="w-5 h-5 text-yellow-500" />
              <IconMoon v-else class="w-5 h-5 text-gray-600 dark:text-gray-300" />
            </button>
          </div>

          <!-- Card Principal -->
          <div class="p-6 transition-all duration-300 bg-white shadow-xl dark:bg-gray-800 rounded-2xl sm:p-8 hover:shadow-2xl">
            <!-- Título -->
            <div class="mb-8 text-center">
              <h2 class="mb-2 text-3xl font-bold text-gray-900 dark:text-white">{{ title }}</h2>
              <p v-if="subtitle" class="text-gray-600 dark:text-gray-400">{{ subtitle }}</p>
            </div>

            <!-- Slot do Formulário -->
            <div class="space-y-6">
              <slot></slot>

              <!-- Links Extras -->
              <div v-if="showExtras && !hideForgotPassword" class="text-right">
                <router-link 
                  to="/forgot-password"
                  class="text-sm transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline"
                >
                  Esqueceu sua senha?
                </router-link>
              </div>

              <!-- Separador -->
              <div v-if="showSeparator" class="relative">
                <div class="absolute inset-0 flex items-center">
                  <div class="w-full border-t border-gray-300 dark:border-gray-600"></div>
                </div>
                <div class="relative flex justify-center text-sm">
                  <span class="px-4 text-gray-500 bg-white dark:bg-gray-800 dark:text-gray-400">OU</span>
                </div>
              </div>

              <!-- Login Social -->
              <div v-if="showSocial" class="space-y-3">
                <button 
                  v-for="provider in socialProviders" 
                  :key="provider.name"
                  @click="handleSocialLogin(provider.name)"
                  class="flex items-center justify-center w-full gap-3 px-4 py-3 text-gray-700 transition-all duration-200 border-2 border-gray-300 rounded-lg dark:border-gray-600 dark:text-gray-300 hover:border-primary-500 dark:hover:border-primary-400 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-gray-50 dark:hover:bg-gray-700/50 focus:outline-none focus:ring-2 focus:ring-primary-500"
                >
                  <component :is="provider.icon" class="w-5 h-5" />
                  <span class="font-medium">Continuar com {{ provider.label }}</span>
                </button>
              </div>

              <!-- Footer Mobile -->
              <div class="pt-6 text-center border-t border-gray-200 dark:border-gray-700 lg:hidden">
                <span class="mr-2 text-gray-600 dark:text-gray-400">{{ promptText }}</span>
                <router-link 
                  :to="actionRoute"
                  class="font-semibold transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline"
                >
                  {{ actionText }}
                </router-link>
              </div>
            </div>
          </div>

          <!-- Footer com Links -->
          <div class="mt-8 text-center">
            <!-- Links Desktop -->
            <div class="justify-center hidden gap-6 text-sm text-gray-500 lg:flex dark:text-gray-400">
              <router-link to="/terms" class="footer-link">
                <IconShieldAlt class="w-4 h-4 mr-1" />
                Termos
              </router-link>
              <router-link to="/privacy" class="footer-link">
                <IconBook class="w-4 h-4 mr-1" />
                Privacidade
              </router-link>
              <router-link to="/help" class="footer-link">
                <IconQuestionCircle class="w-4 h-4 mr-1" />
                Ajuda
              </router-link>
            </div>

            <!-- Links Mobile -->
            <div class="flex justify-center gap-4 text-xs text-gray-500 lg:hidden dark:text-gray-400">
              <router-link to="/terms" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Termos</router-link>
              <router-link to="/privacy" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Privacidade</router-link>
              <router-link to="/help" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Ajuda</router-link>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useTheme } from '@/composables/useTheme'

// Ícones personalizados
import IconRocket from '@/components/icons/rocket.vue'
import IconStore from '@/components/icons/store.vue'
import IconSun from '@/components/icons/sun.vue'
import IconMoon from '@/components/icons/moon.vue'
import IconGoogle from '@/components/icons/google.vue'
import IconGithub from '@/components/icons/github.vue'
import IconShieldAlt from '@/components/icons/shield-alt.vue'
import IconUsers from '@/components/icons/users.vue'
import IconChartBar from '@/components/icons/chart-bar.vue'
import IconBook from '@/components/icons/book.vue'
import IconQuestionCircle from '@/components/icons/question-circle.vue'

const props = defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
  showExtras: { type: Boolean, default: true },
  showSeparator: { type: Boolean, default: false },
  showSocial: { type: Boolean, default: false },
  hideForgotPassword: { type: Boolean, default: false }
})

const emit = defineEmits(['social-login'])

const route = useRoute()
const { isDarkMode, toggleTheme } = useTheme()
const currentYear = ref(new Date().getFullYear())

// Features do sidebar
const features = [
  {
    icon: IconShieldAlt,
    title: 'Segurança Garantida',
    description: 'Criptografia de ponta a ponta para seus dados'
  },
  {
    icon: IconRocket,
    title: 'Alta Performance',
    description: 'Sistema rápido e otimizado para sua produtividade'
  },
  {
    icon: IconUsers,
    title: 'Colaboração em Equipe',
    description: 'Trabalhe junto com sua equipe em tempo real'
  },
  {
    icon: IconChartBar,
    title: 'Análises Detalhadas',
    description: 'Relatórios e métricas para decisões estratégicas'
  }
]

// Providers de login social
const socialProviders = [
  { name: 'google', label: 'Google', icon: IconGoogle },
  { name: 'github', label: 'GitHub', icon: IconGithub }
]

// Computed properties
const isLoginRoute = computed(() => route.name === 'login')

const promptText = computed(() =>
  isLoginRoute.value ? 'Não tem uma conta?' : 'Já tem uma conta?'
)

const actionText = computed(() =>
  isLoginRoute.value ? 'Criar conta' : 'Fazer login'
)

const actionRoute = computed(() =>
  isLoginRoute.value ? '/register' : '/login'
)

// Methods
const handleSocialLogin = (provider) => {
  emit('social-login', provider)
}
</script>

<style scoped>
@import '@/assets/default.css';

/* Animações personalizadas */
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

.auth-card {
  animation: fadeIn 0.4s ease-out;
}

/* Scrollbar customizada */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100 dark:bg-gray-800;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-300 dark:bg-gray-600 rounded-full;
}

::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400 dark:bg-gray-500;
}

/* Responsive adjustments */
@media (max-width: 1024px) {
  .lg\:flex {
    display: none;
  }
}

@media (max-width: 640px) {
  .sm\:p-8 {
    padding: 1.5rem;
  }
}
</style>