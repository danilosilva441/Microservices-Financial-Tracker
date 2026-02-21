<!--
 * src/components/auth/AuthLayout.vue
 * AuthLayout.vue
 *
 * A Vue component that provides a consistent layout for authentication-related pages such as login and registration.
 * It includes a responsive design with a header, main content area, and footer. The component also supports social login buttons and extra links.
 * The layout is designed to be flexible and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <DefaultLayout>
    <div class="min-h-screen transition-colors duration-200 bg-gray-50 dark:bg-gray-900"
      :class="{ 'dark': isDarkMode }">
      <!-- Header Mobile - APENAS LOGO, SEM BOTÃO DE TEMA -->
      <header
        class="fixed top-0 left-0 right-0 z-50 bg-white border-b border-gray-200 lg:hidden dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center justify-between px-4 py-3">
          <router-link to="/" class="flex items-center gap-2 group">
            <div
              class="p-2 transition-transform duration-200 rounded-lg bg-gradient-to-r from-primary-500 to-secondary-500 group-hover:scale-105">
              <IconRocket class="w-5 h-5 text-white" />
            </div>
            <span
              class="text-xl font-bold text-transparent bg-gradient-to-r from-primary-600 to-secondary-600 bg-clip-text">
              DS SysTech
            </span>
          </router-link>

          <!-- Espaço vazio para manter o layout, mas sem botão -->
          <div class="w-10"></div>
        </div>
      </header>

      <div class="flex min-h-screen pt-16 lg:pt-0">
        <!-- Conteúdo Principal -->
        <main class="flex items-center justify-center flex-1 p-4 lg:p-8">
          <div class="w-full max-w-md">
            <!-- Card Principal -->
            <div
              class="p-6 transition-all duration-300 bg-white shadow-xl dark:bg-gray-800 rounded-2xl sm:p-8 hover:shadow-2xl">
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
                  <router-link to="/forgot-password"
                    class="text-sm transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline">
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
                  <button v-for="provider in socialProviders" :key="provider.name"
                    @click="handleSocialLogin(provider.name)"
                    class="flex items-center justify-center w-full gap-3 px-4 py-3 text-gray-700 transition-all duration-200 border-2 border-gray-300 rounded-lg dark:border-gray-600 dark:text-gray-300 hover:border-primary-500 dark:hover:border-primary-400 hover:text-primary-600 dark:hover:text-primary-400 hover:bg-gray-50 dark:hover:bg-gray-700/50 focus:outline-none focus:ring-2 focus:ring-primary-500">
                    <component :is="provider.icon" class="w-5 h-5" />
                    <span class="font-medium">Continuar com {{ provider.label }}</span>
                  </button>
                </div>

                <!-- Footer Mobile -->
                <div class="pt-6 text-center border-t border-gray-200 dark:border-gray-700 lg:hidden">
                  <span class="mr-2 text-gray-600 dark:text-gray-400">{{ promptText }}</span>
                  <router-link :to="actionRoute"
                    class="font-semibold transition-colors duration-200 text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 hover:underline">
                    {{ actionText }}
                  </router-link>
                </div>
              </div>
            </div>

            <!-- Footer com Links -->
            <div class="mt-8 text-center">
              <!-- Links Desktop -->
              <div class="justify-center hidden gap-6 text-sm text-gray-500 lg:flex dark:text-gray-400">
                <router-link to="/terms"
                  class="flex items-center text-gray-600 transition-colors duration-200 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400">
                  <IconShieldAlt class="w-4 h-4 mr-1" />
                  Termos
                </router-link>
                <router-link to="/privacy"
                  class="flex items-center text-gray-600 transition-colors duration-200 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400">
                  <IconBook class="w-4 h-4 mr-1" />
                  Privacidade
                </router-link>
                <router-link to="/help"
                  class="flex items-center text-gray-600 transition-colors duration-200 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400">
                  <IconQuestionCircle class="w-4 h-4 mr-1" />
                  Ajuda
                </router-link>
              </div>

              <!-- Links Mobile -->
              <div class="flex justify-center gap-4 text-xs text-gray-500 lg:hidden dark:text-gray-400">
                <router-link to="/terms"
                  class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Termos</router-link>
                <router-link to="/privacy"
                  class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Privacidade</router-link>
                <router-link to="/help"
                  class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">Ajuda</router-link>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  </DefaultLayout>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useTheme } from '@/composables/useTheme'
import DefaultLayout from '@/layouts/DefaultLayout.vue'

// Ícones personalizados
import IconRocket from '@/components/icons/rocket.vue'
import IconGoogle from '@/components/icons/google.vue'
import IconGithub from '@/components/icons/github.vue'
import IconShieldAlt from '@/components/icons/shield-alt.vue'
import IconBook from '@/components/icons/book.vue'
import IconQuestionCircle from '@/components/icons/question-circle.vue'

// Props
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
const { isDarkMode } = useTheme()

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
  isLoginRoute.value ? '/provision' : '/login'
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

/* Scrollbar customizada - sem @apply */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  background-color: #f3f4f6;
}

.dark ::-webkit-scrollbar-track {
  background-color: #1f2937;
}

::-webkit-scrollbar-thumb {
  background-color: #d1d5db;
  border-radius: 9999px;
}

.dark ::-webkit-scrollbar-thumb {
  background-color: #4b5563;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #9ca3af;
}

.dark ::-webkit-scrollbar-thumb:hover {
  background-color: #6b7280;
}

/* Transições suaves - sem @apply */
* {
  transition-property: background-color, border-color, color, fill, stroke;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Focus styles - sem @apply */
*:focus-visible {
  outline: none;
  --tw-ring-offset-shadow: var(--tw-ring-inset) 0 0 0 var(--tw-ring-offset-width) var(--tw-ring-offset-color);
  --tw-ring-shadow: var(--tw-ring-inset) 0 0 0 calc(2px + var(--tw-ring-offset-width)) var(--tw-ring-color);
  box-shadow: var(--tw-ring-offset-shadow), var(--tw-ring-shadow), var(--tw-shadow, 0 0 #0000);
  --tw-ring-color: #667eea;
  --tw-ring-offset-width: 2px;
  --tw-ring-offset-color: #fff;
}

.dark *:focus-visible {
  --tw-ring-offset-color: #111827;
}

/* Responsive adjustments */
@media (max-width: 1024px) {
  .auth-sidebar {
    display: none;
  }
}

@media (max-width: 640px) {
  .auth-card {
    padding: 1.5rem;
  }

  .auth-title h2 {
    font-size: 1.5rem;
    line-height: 2rem;
  }
}

/* Estado de carregamento */
.auth-page-loaded .auth-container {
  animation: fadeIn 0.3s ease-out;
}
</style>