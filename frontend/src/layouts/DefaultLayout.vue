<!-- src/layouts/DefaultLayout.vue -->
<template>
  <div 
    class="min-h-screen flex flex-col bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors duration-200"
    :class="{ 'dark': isDarkMode }"
  >
    <!-- Navbar -->
    <header v-if="showHeader" class="sticky top-0 z-50 bg-white dark:bg-gray-800 shadow-md dark:shadow-gray-900/30">
      <nav class="container mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <!-- Brand Logo -->
          <div class="flex-shrink-0">
            <router-link 
              to="/" 
              class="flex items-center space-x-2 group"
              @click="closeAllMenus"
            >
              <div class="p-2 bg-gradient-to-r from-primary-500 to-secondary-500 rounded-lg group-hover:scale-105 transition-transform duration-200">
                <i class="fas fa-rocket text-white text-lg"></i>
              </div>
              <div class="flex flex-col">
                <span class="text-xl font-bold bg-gradient-to-r from-primary-600 to-secondary-600 bg-clip-text text-transparent">
                  DS SysTech
                </span>
                <span class="text-xs text-gray-500 dark:text-gray-400">Management System</span>
              </div>
            </router-link>
          </div>

          <!-- Desktop Menu -->
          <div class="hidden md:flex flex-1 justify-center">
            <div class="flex space-x-1">
              <router-link
                v-for="item in menuItems"
                :key="item.to"
                :to="item.to"
                class="nav-link-desktop"
                :class="{ 'nav-link-desktop-active': isActive(item.to) }"
                @click="closeAllMenus"
              >
                <i :class="item.icon" class="mr-2 text-sm"></i>
                <span class="font-medium">{{ item.text }}</span>
                <span 
                  v-if="item.badge"
                  class="ml-2 px-1.5 py-0.5 text-xs rounded-full bg-primary-100 dark:bg-primary-900 text-primary-800 dark:text-primary-200"
                >
                  {{ item.badge }}
                </span>
              </router-link>
            </div>
          </div>

          <!-- Right Actions -->
          <div class="flex items-center space-x-4">
            <!-- Theme Toggle -->
            <button
              @click="toggleTheme"
              class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200"
              aria-label="Alternar tema"
            >
              <i class="fas text-lg text-gray-600 dark:text-gray-300" :class="themeIcon"></i>
            </button>

            <!-- User Menu -->
            <div class="relative" v-if="isAuthenticated">
              <button
                ref="userTriggerRef"
                @click="toggleUserMenu"
                class="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200"
                :aria-expanded="showUserMenu ? 'true' : 'false'"
              >
                <div class="relative">
                  <div class="w-10 h-10 rounded-full bg-gradient-to-r from-primary-500 to-secondary-500 flex items-center justify-center text-white font-bold shadow-lg">
                    {{ userInitials }}
                  </div>
                  <div class="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-white dark:border-gray-800"></div>
                </div>
                <div class="hidden lg:block text-left">
                  <p class="font-semibold text-sm">{{ userData?.fullName || 'Usuário' }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Admin</p>
                </div>
                <i class="fas fa-chevron-down text-gray-400 text-xs transition-transform duration-200" :class="{ 'rotate-180': showUserMenu }"></i>
              </button>

              <!-- User Dropdown -->
              <div
                v-if="showUserMenu"
                class="absolute right-0 mt-2 w-56 rounded-xl shadow-2xl bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 overflow-hidden z-50"
                role="menu"
              >
                <div class="p-4 border-b border-gray-100 dark:border-gray-700">
                  <p class="font-semibold">{{ userData?.fullName || 'Usuário' }}</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">{{ userData?.email || 'user@example.com' }}</p>
                </div>
                
                <div class="py-1">
                  <router-link
                    v-for="item in userMenuItems"
                    :key="item.to"
                    :to="item.to"
                    class="user-menu-item"
                    @click="closeAllMenus"
                    role="menuitem"
                  >
                    <i :class="item.icon" class="mr-3 text-gray-400"></i>
                    <span>{{ item.text }}</span>
                    <i v-if="item.arrow" class="fas fa-chevron-right ml-auto text-xs text-gray-400"></i>
                  </router-link>
                  
                  <div class="border-t border-gray-100 dark:border-gray-700 my-1"></div>
                  
                  <button
                    @click="handleLogout"
                    class="user-menu-item text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300"
                    role="menuitem"
                  >
                    <i class="fas fa-sign-out-alt mr-3"></i>
                    <span>Sair</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Login Button -->
            <router-link
              v-else
              to="/login"
              class="btn-primary"
              @click="closeAllMenus"
            >
              <i class="fas fa-sign-in-alt mr-2"></i>
              Entrar
            </router-link>

            <!-- Mobile Menu Toggle -->
            <button
              @click="toggleMobileMenu"
              class="md:hidden p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200"
              :aria-expanded="showMobileMenu ? 'true' : 'false'"
              aria-label="Abrir menu"
            >
              <i class="fas text-xl" :class="showMobileMenu ? 'fa-times' : 'fa-bars'"></i>
            </button>
          </div>
        </div>
      </nav>

      <!-- Mobile Menu Overlay -->
      <div
        v-if="showMobileMenu"
        class="fixed inset-0 bg-black bg-opacity-50 z-40 md:hidden"
        @click="closeMobileMenu"
        aria-hidden="true"
      ></div>

      <!-- Mobile Menu Panel -->
      <div
        class="fixed inset-y-0 right-0 w-full max-w-sm bg-white dark:bg-gray-800 shadow-2xl transform transition-transform duration-300 ease-in-out z-40 md:hidden"
        :class="{ 'translate-x-0': showMobileMenu, 'translate-x-full': !showMobileMenu }"
      >
        <div class="flex flex-col h-full">
          <!-- Mobile Header -->
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between mb-6">
              <router-link to="/" @click="closeAllMenus" class="flex items-center space-x-2">
                <div class="p-2 bg-gradient-to-r from-primary-500 to-secondary-500 rounded-lg">
                  <i class="fas fa-rocket text-white"></i>
                </div>
                <span class="text-xl font-bold">DS SysTech</span>
              </router-link>
              <button @click="closeMobileMenu" class="p-2">
                <i class="fas fa-times text-xl"></i>
              </button>
            </div>

            <!-- User Info -->
            <div v-if="isAuthenticated" class="flex items-center space-x-4 p-4 rounded-xl bg-gray-50 dark:bg-gray-900">
              <div class="w-14 h-14 rounded-full bg-gradient-to-r from-primary-500 to-secondary-500 flex items-center justify-center text-white font-bold text-lg shadow-lg">
                {{ userInitials }}
              </div>
              <div>
                <p class="font-bold">{{ userData?.fullName || 'Usuário' }}</p>
                <p class="text-sm text-gray-500 dark:text-gray-400">Conta verificada</p>
              </div>
            </div>

            <!-- Mobile Login -->
            <div v-else class="space-y-3">
              <router-link
                to="/login"
                class="btn-primary w-full justify-center"
                @click="closeAllMenus"
              >
                <i class="fas fa-sign-in-alt mr-2"></i>
                Entrar
              </router-link>
              <router-link
                to="/register"
                class="btn-outline w-full justify-center"
                @click="closeAllMenus"
              >
                <i class="fas fa-user-plus mr-2"></i>
                Cadastrar
              </router-link>
            </div>
          </div>

          <!-- Mobile Menu Items -->
          <div class="flex-1 overflow-y-auto p-4">
            <div class="space-y-1">
              <router-link
                v-for="item in menuItems"
                :key="`mobile-${item.to}`"
                :to="item.to"
                class="mobile-nav-link"
                :class="{ 'mobile-nav-link-active': isActive(item.to) }"
                @click="closeAllMenus"
              >
                <div class="flex items-center justify-between w-full">
                  <div class="flex items-center">
                    <i :class="item.icon" class="mr-3 w-6 text-center"></i>
                    <span class="font-medium">{{ item.text }}</span>
                  </div>
                  <i class="fas fa-chevron-right text-xs text-gray-400"></i>
                </div>
                <span 
                  v-if="item.badge"
                  class="absolute right-12 px-2 py-1 text-xs rounded-full bg-primary-500 text-white"
                >
                  {{ item.badge }}
                </span>
              </router-link>
            </div>

            <!-- User Menu Mobile -->
            <div v-if="isAuthenticated" class="mt-8">
              <p class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider px-4 mb-2">Minha Conta</p>
              <div class="space-y-1">
                <router-link
                  v-for="item in userMenuItems"
                  :key="`mobile-user-${item.to}`"
                  :to="item.to"
                  class="mobile-nav-link"
                  @click="closeAllMenus"
                >
                  <i :class="item.icon" class="mr-3 w-6 text-center"></i>
                  <span>{{ item.text }}</span>
                </router-link>
              </div>
              
              <div class="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
                <button
                  @click="handleLogout"
                  class="mobile-nav-link text-red-600 dark:text-red-400"
                >
                  <i class="fas fa-sign-out-alt mr-3 w-6 text-center"></i>
                  <span>Sair da Conta</span>
                </button>
              </div>
            </div>
          </div>

          <!-- Mobile Footer -->
          <div class="p-4 border-t border-gray-200 dark:border-gray-700">
            <div class="flex justify-between items-center">
              <button
                @click="toggleTheme"
                class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200"
              >
                <i class="fas text-lg" :class="themeIcon"></i>
                <span class="ml-2 text-sm">Tema {{ isDarkMode ? 'Escuro' : 'Claro' }}</span>
              </button>
              <span class="text-xs text-gray-500">v1.0.0</span>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="flex-1 container mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <slot></slot>
    </main>

    <!-- Footer -->
    <footer v-if="showFooter" class="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 mt-12">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
          <!-- Company Info -->
          <div class="col-span-1 md:col-span-2">
            <div class="flex items-center space-x-3 mb-4">
              <div class="p-2 bg-gradient-to-r from-primary-500 to-secondary-500 rounded-lg">
                <i class="fas fa-rocket text-white"></i>
              </div>
              <div>
                <h3 class="text-xl font-bold">DS SysTech</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Sistema de Gerenciamento Inteligente</p>
              </div>
            </div>
            <p class="text-gray-600 dark:text-gray-400 mb-4">
              Solução completa para gestão de portfólio, unidades e equipes. Transformando dados em insights.
            </p>
            <div class="flex space-x-4">
              <a href="#" class="social-icon">
                <i class="fab fa-twitter"></i>
              </a>
              <a href="#" class="social-icon">
                <i class="fab fa-linkedin-in"></i>
              </a>
              <a href="#" class="social-icon">
                <i class="fab fa-github"></i>
              </a>
              <a href="#" class="social-icon">
                <i class="fab fa-discord"></i>
              </a>
            </div>
          </div>

          <!-- Quick Links -->
          <div>
            <h4 class="font-bold text-lg mb-4">Links Rápidos</h4>
            <ul class="space-y-2">
              <li>
                <router-link to="/" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-home mr-2"></i>
                  Home
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-chart-line mr-2"></i>
                  Dashboard
                </router-link>
              </li>
              <li>
                <router-link to="/unidades" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-store mr-2"></i>
                  Unidades
                </router-link>
              </li>
              <li>
                <router-link to="/projects" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-briefcase mr-2"></i>
                  Projetos
                </router-link>
              </li>
            </ul>
          </div>

          <!-- Support -->
          <div>
            <h4 class="font-bold text-lg mb-4">Suporte</h4>
            <ul class="space-y-2">
              <li>
                <router-link to="/help" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-question-circle mr-2"></i>
                  Central de Ajuda
                </router-link>
              </li>
              <li>
                <router-link to="/docs" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-book mr-2"></i>
                  Documentação
                </router-link>
              </li>
              <li>
                <router-link to="/contact" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-envelope mr-2"></i>
                  Contato
                </router-link>
              </li>
              <li>
                <router-link to="/status" class="footer-link" @click="closeAllMenus">
                  <i class="fas fa-server mr-2"></i>
                  Status do Sistema
                </router-link>
              </li>
            </ul>
          </div>
        </div>

        <!-- Copyright -->
        <div class="border-t border-gray-200 dark:border-gray-700 mt-8 pt-6 text-center text-sm text-gray-600 dark:text-gray-400">
          <div class="flex flex-col sm:flex-row justify-between items-center">
            <p>&copy; {{ currentYear }} DS SysTech. Todos os direitos reservados.</p>
            <div class="flex space-x-6 mt-4 sm:mt-0">
              <router-link to="/terms" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
                Termos de Uso
              </router-link>
              <router-link to="/privacy" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
                Política de Privacidade
              </router-link>
              <router-link to="/cookies" class="hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
                Cookies
              </router-link>
            </div>
          </div>
          <div class="mt-4 flex items-center justify-center space-x-2 text-xs">
            <i class="fas fa-shield-alt text-green-500"></i>
            <span>Sistema seguro • SSL ativado • Atualizado em tempo real</span>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'DefaultLayout',

  props: {
    showHeader: { type: Boolean, default: true },
    showFooter: { type: Boolean, default: true }
  },

  setup() {
    const route = useRoute()
    const { isAuthenticated, userData, logout } = useAuth()

    // Theme management
    const isDarkMode = ref(false)
    const themeIcon = computed(() => 
      isDarkMode.value ? 'fa-sun' : 'fa-moon'
    )

    const loadTheme = () => {
      const savedTheme = localStorage.getItem('theme')
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
      isDarkMode.value = savedTheme ? savedTheme === 'dark' : prefersDark
    }

    const toggleTheme = () => {
      isDarkMode.value = !isDarkMode.value
      localStorage.setItem('theme', isDarkMode.value ? 'dark' : 'light')
    }

    // Menu states
    const showUserMenu = ref(false)
    const showMobileMenu = ref(false)
    const userTriggerRef = ref(null)

    // Menu items with icons and badges
    const menuItems = computed(() => {
      const items = [
        { 
          to: '/', 
          text: 'Home', 
          icon: 'fas fa-home',
          badge: null
        },
        { 
          to: '/dashboard', 
          text: 'Dashboard', 
          icon: 'fas fa-chart-line',
          badge: 'New'
        },
        { 
          to: '/unidades', 
          text: 'Unidades', 
          icon: 'fas fa-store',
          badge: null
        },
        { 
          to: '/projects', 
          text: 'Projetos', 
          icon: 'fas fa-briefcase',
          badge: '3'
        },
        { 
          to: '/reports', 
          text: 'Relatórios', 
          icon: 'fas fa-chart-bar',
          badge: null
        },
        { 
          to: '/team', 
          text: 'Equipe', 
          icon: 'fas fa-users',
          badge: null
        },
        { 
          to: '/calendar', 
          text: 'Calendário', 
          icon: 'fas fa-calendar-alt',
          badge: '5'
        }
      ]

      const requiresAuth = new Set([
        '/dashboard',
        '/unidades',
        '/projects',
        '/reports',
        '/team',
        '/calendar'
      ])

      return items.filter((item) => (requiresAuth.has(item.to) ? isAuthenticated.value : true))
    })

    // User menu items
    const userMenuItems = computed(() => [
      { 
        to: '/profile', 
        text: 'Meu Perfil', 
        icon: 'fas fa-user-circle',
        arrow: true
      },
      { 
        to: '/settings', 
        text: 'Configurações', 
        icon: 'fas fa-cog',
        arrow: true
      },
      { 
        to: '/notifications', 
        text: 'Notificações', 
        icon: 'fas fa-bell',
        arrow: true
      },
      { 
        to: '/billing', 
        text: 'Faturamento', 
        icon: 'fas fa-credit-card',
        arrow: true
      }
    ])

    // Helpers
    const isActive = (routePath) => {
      if (routePath === '/') return route.path === '/'
      return route.path.startsWith(routePath)
    }

    const userInitials = computed(() => {
      const full = userData.value?.fullName?.trim()
      if (!full) return 'U'
      const parts = full.split(/\s+/).filter(Boolean)
      if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase()
      return parts[0][0].toUpperCase()
    })

    const currentYear = computed(() => new Date().getFullYear())

    // Menu controls
    const closeUserMenu = () => {
      showUserMenu.value = false
    }

    const closeMobileMenu = () => {
      showMobileMenu.value = false
      document.body.style.overflow = ''
    }

    const closeAllMenus = () => {
      closeUserMenu()
      closeMobileMenu()
    }

    const toggleUserMenu = async () => {
      if (!showUserMenu.value) {
        closeMobileMenu()
        showUserMenu.value = true
        await nextTick()
      } else {
        showUserMenu.value = false
      }
    }

    const toggleMobileMenu = () => {
      if (!showMobileMenu.value) {
        closeUserMenu()
        showMobileMenu.value = true
        document.body.style.overflow = 'hidden'
      } else {
        closeMobileMenu()
      }
    }

    // Event handlers
    const onDocumentClick = (e) => {
      if (showUserMenu.value && !e.target.closest('.relative')) {
        closeUserMenu()
      }
    }

    const onKeyDown = (e) => {
      if (e.key === 'Escape') {
        closeAllMenus()
        if (userTriggerRef.value) userTriggerRef.value.focus?.()
      }
    }

    const onResize = () => {
      if (window.innerWidth >= 768 && showMobileMenu.value) {
        closeMobileMenu()
      }
    }

    // Logout
    const handleLogout = () => {
      logout()
      closeAllMenus()
    }

    // Watch route changes
    watch(
      () => route.fullPath,
      () => closeAllMenus()
    )

    // Lifecycle
    onMounted(() => {
      loadTheme()
      document.addEventListener('click', onDocumentClick)
      document.addEventListener('keydown', onKeyDown)
      window.addEventListener('resize', onResize)
    })

    onUnmounted(() => {
      document.removeEventListener('click', onDocumentClick)
      document.removeEventListener('keydown', onKeyDown)
      window.removeEventListener('resize', onResize)
      document.body.style.overflow = ''
    })

    return {
      route,
      isAuthenticated,
      userData,
      menuItems,
      userMenuItems,
      userInitials,
      currentYear,
      isDarkMode,
      themeIcon,
      showUserMenu,
      showMobileMenu,
      userTriggerRef,
      toggleTheme,
      toggleUserMenu,
      toggleMobileMenu,
      closeMobileMenu,
      closeAllMenus,
      isActive,
      handleLogout
    }
  }
}
</script>

<style scoped>
/* Custom styles for Tailwind components */

/* Desktop Navigation Links */
.nav-link-desktop {
  @apply flex items-center px-4 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-all duration-200 relative;
}

.nav-link-desktop-active {
  @apply text-primary-600 dark:text-primary-400 bg-primary-50 dark:bg-primary-900/30 font-semibold;
}

.nav-link-desktop-active::after {
  content: '';
  @apply absolute bottom-0 left-1/2 transform -translate-x-1/2 w-8 h-1 bg-primary-500 dark:bg-primary-400 rounded-full;
}

/* Mobile Navigation Links */
.mobile-nav-link {
  @apply flex items-center px-4 py-3 rounded-xl text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200 relative;
}

.mobile-nav-link-active {
  @apply bg-primary-50 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400 border-l-4 border-primary-500 dark:border-primary-400 font-semibold;
}

/* User Menu Items */
.user-menu-item {
  @apply flex items-center px-4 py-3 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-200;
}

/* Buttons */
.btn-primary {
  @apply inline-flex items-center px-4 py-2 bg-gradient-to-r from-primary-500 to-secondary-500 text-white font-semibold rounded-lg hover:from-primary-600 hover:to-secondary-600 transition-all duration-200 transform hover:-translate-y-0.5 active:translate-y-0 shadow-lg hover:shadow-xl;
}

.btn-outline {
  @apply inline-flex items-center px-4 py-2 border-2 border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 font-semibold rounded-lg hover:border-primary-500 hover:text-primary-600 dark:hover:text-primary-400 transition-colors duration-200;
}

/* Footer Links */
.footer-link {
  @apply flex items-center text-gray-600 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 transition-colors duration-200;
}

/* Social Icons */
.social-icon {
  @apply w-10 h-10 rounded-full bg-gray-100 dark:bg-gray-700 flex items-center justify-center text-gray-600 dark:text-gray-400 hover:bg-primary-500 hover:text-white dark:hover:bg-primary-600 transition-all duration-200;
}

/* Custom scrollbar for mobile menu */
::-webkit-scrollbar {
  width: 6px;
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
</style>