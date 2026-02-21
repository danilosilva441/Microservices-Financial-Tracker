<!-- src/layouts/DefaultLayout.vue -->
<template>
  <div
    class="flex flex-col min-h-screen text-gray-900 transition-colors duration-200 bg-gray-50 dark:bg-gray-900 dark:text-gray-100"
    :class="{ dark: isDarkMode }">
    <!-- Navbar -->
    <header v-if="showHeader" class="sticky top-0 z-50 bg-white shadow-md dark:bg-gray-800 dark:shadow-gray-900/30">
      <nav class="container px-4 mx-auto sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <!-- Brand Logo -->
          <div class="flex items-center">
            <router-link to="/" class="flex items-center gap-2 group">
              <div class="p-2 rounded-lg bg-gradient-to-r from-primary-500 to-secondary-500">
                <LogoIcon class="w-6 h-6 text-white" />
              </div>
              <div>
                <span
                  class="text-xl font-bold text-transparent bg-gradient-to-r from-primary-600 to-secondary-600 bg-clip-text">
                  DS SysTech
                </span>
                <p class="text-xs text-gray-500 dark:text-gray-400">Management System</p>
              </div>
            </router-link>
          </div>

          <!-- Desktop Menu -->
          <div class="justify-center flex-1 hidden md:flex">
            <div class="flex space-x-1">
              <router-link v-for="item in menuItems" :key="item.to" :to="item.to" class="nav-link-desktop"
                :class="{ 'nav-link-desktop-active': isActive(item.to) }" @click="closeAllMenus">
                <component :is="item.iconComponent" class="w-4 h-4 mr-2" />
                <span class="font-medium">{{ item.text }}</span>

                <span v-if="item.badge"
                  class="ml-2 px-1.5 py-0.5 text-xs rounded-full bg-primary-100 dark:bg-primary-900 text-primary-800 dark:text-primary-200">
                  {{ item.badge }}
                </span>
              </router-link>
            </div>
          </div>

          <!-- Right Actions -->
          <div class="flex items-center space-x-4">
            <!-- Theme Toggle -->
            <button @click="toggleTheme"
              class="p-2 transition-colors duration-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
              aria-label="Alternar tema">
              <component :is="themeIconComponent" class="w-5 h-5 text-gray-600 dark:text-gray-300" />
            </button>

            <!-- User Menu -->
            <div v-if="isAuthenticated" ref="userMenuRef" class="relative">
              <button ref="userTriggerRef" @click="toggleUserMenu"
                class="flex items-center p-2 space-x-3 transition-colors duration-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
                :aria-expanded="showUserMenu ? 'true' : 'false'">
                <div class="relative">
                  <div
                    class="flex items-center justify-center w-10 h-10 font-bold text-white rounded-full shadow-lg bg-gradient-to-r from-primary-500 to-secondary-500">
                    {{ userInitials }}
                  </div>
                  <div
                    class="absolute w-4 h-4 bg-green-500 border-2 border-white rounded-full -bottom-1 -right-1 dark:border-gray-800">
                  </div>
                </div>

                <div class="hidden text-left lg:block">
                  <p class="text-sm font-semibold">
                    {{ userData?.fullName || "Usuário" }}
                  </p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Admin</p>
                </div>

                <IconChevronDown class="w-3 h-3 text-gray-400 transition-transform duration-200" :class="{ 'rotate-180': showUserMenu }" />
              </button>

              <!-- User Dropdown -->
              <div v-if="showUserMenu"
                class="absolute right-0 z-50 w-56 mt-2 overflow-hidden bg-white border border-gray-200 shadow-2xl rounded-xl dark:bg-gray-800 dark:border-gray-700"
                role="menu">
                <div class="p-4 border-b border-gray-100 dark:border-gray-700">
                  <p class="font-semibold">{{ userData?.fullName || "Usuário" }}</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">
                    {{ userData?.email || "user@example.com" }}
                  </p>
                </div>

                <div class="py-1">
                  <router-link v-for="item in userMenuItems" :key="item.to" :to="item.to" class="user-menu-item"
                    @click="closeAllMenus" role="menuitem">
                    <component :is="item.iconComponent" class="w-4 h-4 mr-3 text-gray-400" />
                    <span>{{ item.text }}</span>
                    <IconChevronRight v-if="item.arrow" class="w-3 h-3 ml-auto text-gray-400" />
                  </router-link>

                  <div class="my-1 border-t border-gray-100 dark:border-gray-700"></div>

                  <!-- ✅ CORRIGIDO: Agora ocupa a linha toda igual aos outros itens -->
                  <button @click="handleLogout"
                    class="w-full text-left text-red-600 user-menu-item dark:text-red-400 hover:text-red-700 dark:hover:text-red-300"
                    role="menuitem">
                    <IconSignOutAlt class="inline-block w-4 h-4 mr-3 align-middle" />
                    <span class="align-middle">Sair</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Login Button -->
            <router-link v-else to="/login" class="btn-primary" @click="closeAllMenus">
              <IconSignInAlt class="w-4 h-4 mr-2" />
              Entrar
            </router-link>

            <!-- Mobile Menu Toggle -->
            <button @click="toggleMobileMenu"
              class="p-2 transition-colors duration-200 rounded-lg md:hidden hover:bg-gray-100 dark:hover:bg-gray-700"
              :aria-expanded="showMobileMenu ? 'true' : 'false'" aria-label="Abrir menu">
              <component :is="mobileToggleIcon" class="w-5 h-5" />
            </button>
          </div>
        </div>
      </nav>

      <!-- Mobile Menu Overlay -->
      <div v-if="showMobileMenu" class="fixed inset-0 z-40 bg-black bg-opacity-50 md:hidden" @click="closeMobileMenu"
        aria-hidden="true"></div>

      <!-- Mobile Menu Panel -->
      <div
        class="fixed inset-y-0 right-0 z-40 w-full max-w-sm transition-transform duration-300 ease-in-out transform bg-white shadow-2xl dark:bg-gray-800 md:hidden"
        :class="{ 'translate-x-0': showMobileMenu, 'translate-x-full': !showMobileMenu }">
        <div class="flex flex-col h-full">
          <!-- Mobile Header -->
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between mb-6">
              <router-link to="/" @click="closeAllMenus" class="flex items-center space-x-2">
                <div class="p-2 rounded-lg bg-gradient-to-r from-primary-500 to-secondary-500">
                  <IconRocket class="w-5 h-5 text-white" />
                </div>
                <span class="text-xl font-bold">DS SysTech</span>
              </router-link>

              <button @click="closeMobileMenu" class="p-2">
                <IconTimes class="w-5 h-5" />
              </button>
            </div>

            <!-- User Info -->
            <div v-if="isAuthenticated" class="flex items-center p-4 space-x-4 rounded-xl bg-gray-50 dark:bg-gray-900">
              <div
                class="flex items-center justify-center text-lg font-bold text-white rounded-full shadow-lg w-14 h-14 bg-gradient-to-r from-primary-500 to-secondary-500">
                {{ userInitials }}
              </div>

              <div>
                <p class="font-bold">{{ userData?.fullName || "Usuário" }}</p>
                <p class="text-sm text-gray-500 dark:text-gray-400">Conta verificada</p>
              </div>
            </div>

            <!-- Mobile Login -->
            <div v-else class="space-y-3">
              <router-link to="/login" class="justify-center w-full btn-primary" @click="closeAllMenus">
                <IconSignInAlt class="w-4 h-4 mr-2" />
                Entrar
              </router-link>

              <router-link to="/register" class="justify-center w-full btn-outline" @click="closeAllMenus">
                <IconUserPlus class="w-4 h-4 mr-2" />
                Cadastrar
              </router-link>
            </div>
          </div>

          <!-- Mobile Menu Items -->
          <div class="flex-1 p-4 overflow-y-auto">
            <div class="space-y-1">
              <router-link v-for="item in menuItems" :key="`mobile-${item.to}`" :to="item.to" class="mobile-nav-link"
                :class="{ 'mobile-nav-link-active': isActive(item.to) }" @click="closeAllMenus">
                <div class="flex items-center justify-between w-full">
                  <div class="flex items-center">
                    <component :is="item.iconComponent" class="w-5 h-5 mr-3" />
                    <span class="font-medium">{{ item.text }}</span>
                  </div>

                  <IconChevronRight class="w-3 h-3 text-gray-400" />
                </div>

                <span v-if="item.badge"
                  class="absolute px-2 py-1 text-xs text-white rounded-full right-12 bg-primary-500">
                  {{ item.badge }}
                </span>
              </router-link>
            </div>

            <!-- User Menu Mobile -->
            <div v-if="isAuthenticated" class="mt-8">
              <p class="px-4 mb-2 text-xs font-semibold tracking-wider text-gray-500 uppercase dark:text-gray-400">
                Minha Conta
              </p>

              <div class="space-y-1">
                <router-link v-for="item in userMenuItems" :key="`mobile-user-${item.to}`" :to="item.to"
                  class="mobile-nav-link" @click="closeAllMenus">
                  <component :is="item.iconComponent" class="w-5 h-5 mr-3" />
                  <span>{{ item.text }}</span>
                </router-link>
              </div>

              <div class="pt-4 mt-4 border-t border-gray-200 dark:border-gray-700">
                <button @click="handleLogout" class="text-red-600 mobile-nav-link dark:text-red-400">
                  <IconSignOutAlt class="w-5 h-5 mr-3" />
                  <span>Sair da Conta</span>
                </button>
              </div>
            </div>
          </div>

          <!-- Mobile Footer -->
          <div class="p-4 border-t border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <button @click="toggleTheme"
                class="flex items-center p-2 transition-colors duration-200 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700">
                <component :is="themeIconComponent" class="w-5 h-5 mr-2" />
                <span class="text-sm">Tema {{ isDarkMode ? "Escuro" : "Claro" }}</span>
              </button>

              <span class="text-xs text-gray-500">v1.0.0</span>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content - MODIFICADO PARA OCUPAR TODO ESPAÇO DISPONÍVEL -->
    <main class="flex-1 w-full">
      <!-- Container condicional: com padding para páginas normais, sem padding para páginas que precisam ocupar tudo -->
      <div :class="contentContainerClass">
        <slot></slot>
      </div>
    </main>

    <!-- Footer -->
    <footer v-if="showFooter"
      class="mt-8 bg-white border-t border-gray-200 dark:bg-gray-800 dark:border-gray-700 sm:mt-12">
      <div class="container px-4 py-6 mx-auto sm:px-6 lg:px-8 sm:py-8">
        <div class="grid grid-cols-1 gap-6 md:grid-cols-4 sm:gap-8">
          <!-- Company Info -->
          <div class="col-span-1 md:col-span-2">
            <div class="flex items-center mb-4 space-x-3">
              <div class="p-2 rounded-lg bg-gradient-to-r from-primary-500 to-secondary-500">
                <IconRocket class="w-5 h-5 text-white" />
              </div>

              <div>
                <h3 class="text-xl font-bold">DS SysTech</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  Sistema de Gerenciamento Inteligente
                </p>
              </div>
            </div>

            <p class="mb-4 text-sm text-gray-600 dark:text-gray-400 sm:text-base">
              Solução completa para gestão de portfólio, unidades e equipes. Transformando dados em insights.
            </p>

            <div class="flex space-x-4">
              <a href="https://www.linkedin.com/in/danilo-ss/" class="social-icon" target="_blank"
                rel="noopener noreferrer">
                <IconLinkedin class="w-4 h-4" />
              </a>
              <a href="https://github.com/danilosilva441" class="social-icon" target="_blank" rel="noopener noreferrer">
                <IconGithub class="w-4 h-4" />
              </a>
            </div>
          </div>

          <!-- Quick Links -->
          <div>
            <h4 class="mb-4 text-lg font-bold">Links Rápidos</h4>
            <ul class="space-y-2">
              <li>
                <router-link to="/" class="footer-link" @click="closeAllMenus">
                  <IconHome class="w-4 h-4 mr-2" />
                  Home
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard" class="footer-link" @click="closeAllMenus">
                  <IconChartLine class="w-4 h-4 mr-2" />
                  Dashboard
                </router-link>
              </li>
              <li>
                <router-link to="/unidades" class="footer-link" @click="closeAllMenus">
                  <IconStore class="w-4 h-4 mr-2" />
                  Unidades
                </router-link>
              </li>
              <li>
                <router-link to="/projects" class="footer-link" @click="closeAllMenus">
                  <IconBriefcase class="w-4 h-4 mr-2" />
                  Projetos
                </router-link>
              </li>
            </ul>
          </div>

          <!-- Support -->
          <div>
            <h4 class="mb-4 text-lg font-bold">Suporte</h4>
            <ul class="space-y-2">
              <li>
                <router-link to="/help" class="footer-link" @click="closeAllMenus">
                  <IconQuestionCircle class="w-4 h-4 mr-2" />
                  Central de Ajuda
                </router-link>
              </li>
              <li>
                <router-link to="/docs" class="footer-link" @click="closeAllMenus">
                  <IconBook class="w-4 h-4 mr-2" />
                  Documentação
                </router-link>
              </li>
              <li>
                <router-link to="/contact" class="footer-link" @click="closeAllMenus">
                  <IconEnvelope class="w-4 h-4 mr-2" />
                  Contato
                </router-link>
              </li>
              <li>
                <router-link to="/status" class="footer-link" @click="closeAllMenus">
                  <component :is="IconServer" class="w-4 h-4 mr-2" />
                  Status do Sistema
                </router-link>
              </li>
            </ul>
          </div>
        </div>

        <!-- Copyright -->
        <div
          class="pt-6 mt-6 text-sm text-center text-gray-600 border-t border-gray-200 dark:border-gray-700 sm:mt-8 dark:text-gray-400">
          <div class="flex flex-col items-center justify-between sm:flex-row">
            <p>&copy; {{ currentYear }} DS SysTech. Todos os direitos reservados.</p>

            <div class="flex mt-4 space-x-6 sm:mt-0">
              <router-link to="/terms" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">
                Termos de Uso
              </router-link>
              <router-link to="/privacy" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">
                Política de Privacidade
              </router-link>
              <router-link to="/cookies" class="transition-colors hover:text-primary-600 dark:hover:text-primary-400">
                Cookies
              </router-link>
            </div>
          </div>

          <div class="flex items-center justify-center mt-4 space-x-2 text-xs">
            <IconShieldAlt class="w-4 h-4 text-green-500" />
            <span>Sistema seguro • SSL ativado • Atualizado em tempo real</span>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted, watch, nextTick, markRaw } from "vue"
import { useRoute } from "vue-router"
import { useAuth } from "@/composables/auth/useAuth"

// SVGs como componentes
import IconHome from "@/components/icons/home.vue"
import IconChartLine from "@/components/icons/chart-line.vue"
import IconStore from "@/components/icons/store.vue"
import IconBriefcase from "@/components/icons/briefcase.vue"
import IconChartBar from "@/components/icons/chart-bar.vue"
import IconUsers from "@/components/icons/users.vue"
import IconCalendarAlt from "@/components/icons/calendar-alt.vue"

import IconRocket from "@/components/icons/rocket.vue"
import IconSignInAlt from "@/components/icons/sign-in-alt.vue"
import IconUserPlus from "@/components/icons/user-plus.vue"

import IconUserCircle from "@/components/icons/user-circle.vue"
import IconCog from "@/components/icons/cog.vue"
import IconBell from "@/components/icons/bell.vue"
import IconCreditCard from "@/components/icons/credit-card.vue"
import IconSignOutAlt from "@/components/icons/sign-out-alt.vue"

import IconChevronDown from "@/components/icons/chevron-down.vue"
import IconChevronRight from "@/components/icons/chevron-right.vue"
import IconTimes from "@/components/icons/times.vue"
import IconBars from "@/components/icons/bars.vue"

import IconSun from "@/components/icons/sun.vue"
import IconMoon from "@/components/icons/moon.vue"

import IconShieldAlt from "@/components/icons/shield-alt.vue"
import IconQuestionCircle from "@/components/icons/question-circle.vue"
import IconBook from "@/components/icons/book.vue"
import IconEnvelope from "@/components/icons/envelope.vue"
import IconServer from "@/components/icons/server.vue"

import IconLinkedin from "@/components/icons/linkedin.vue"
import IconGithub from "@/components/icons/github.vue"
import LogoIcon from "@/components/icons/LogoIcon.vue"

export default {
  name: "DefaultLayout",

  // O Vue exige que componentes declarados e usados no template
  // sejam registrados aqui na Options API (quando não se usa <script setup>)
  components: {
    IconHome,
    IconChartLine,
    IconStore,
    IconBriefcase,
    IconChartBar,
    IconUsers,
    IconCalendarAlt,
    IconRocket,
    IconSignInAlt,
    IconUserPlus,
    IconUserCircle,
    IconCog,
    IconBell,
    IconCreditCard,
    IconSignOutAlt,
    IconChevronDown,
    IconChevronRight,
    IconTimes,
    IconBars,
    IconSun,
    IconMoon,
    IconShieldAlt,
    IconQuestionCircle,
    IconBook,
    IconEnvelope,
    IconServer,
    IconLinkedin,
    IconGithub,
    LogoIcon
  },

  props: {
    showHeader: { type: Boolean, default: true },
    showFooter: { type: Boolean, default: true },
    fullWidth: { type: Boolean, default: false },
    contentPadding: { type: Boolean, default: true }
  },

  setup(props) {
    const route = useRoute()
    const { isAuthenticated, userData, logout } = useAuth()

    const isDarkMode = ref(false)
    const themeIconComponent = computed(() =>
      isDarkMode.value ? markRaw(IconSun) : markRaw(IconMoon)
    )

    const loadTheme = () => {
      const savedTheme = localStorage.getItem("theme")
      const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches
      isDarkMode.value = savedTheme ? savedTheme === "dark" : prefersDark
    }

    const toggleTheme = () => {
      isDarkMode.value = !isDarkMode.value
      localStorage.setItem("theme", isDarkMode.value ? "dark" : "light")
    }

    const showUserMenu = ref(false)
    const showMobileMenu = ref(false)
    const userTriggerRef = ref(null)
    const userMenuRef = ref(null)

    const mobileToggleIcon = computed(() =>
      showMobileMenu.value ? markRaw(IconTimes) : markRaw(IconBars)
    )

    const requiresAuth = new Set([
      "/dashboard",
      "/unidades",
      "/projects",
      "/reports",
      "/team",
      "/calendar",
    ])

    const menuItems = computed(() => {
      const items = [
        { to: "/", text: "Home", iconComponent: markRaw(IconHome), badge: null },
        { to: "/dashboard", text: "Dashboard", iconComponent: markRaw(IconChartLine), badge: null },
        { to: "/unidades", text: "Unidades", iconComponent: markRaw(IconStore), badge: null },
        { to: "/projects", text: "Projetos", iconComponent: markRaw(IconBriefcase), badge: null },
        { to: "/reports", text: "Relatórios", iconComponent: markRaw(IconChartBar), badge: null },
        { to: "/team", text: "Equipe", iconComponent: markRaw(IconUsers), badge: null },
        { to: "/calendar", text: "Calendário", iconComponent: markRaw(IconCalendarAlt), badge: null },
      ]

      return items.filter((item) =>
        requiresAuth.has(item.to) ? isAuthenticated.value : true
      )
    })

    const userMenuItems = computed(() => [
      { to: "/profile", text: "Meu Perfil", iconComponent: markRaw(IconUserCircle), arrow: true },
      { to: "/settings", text: "Configurações", iconComponent: markRaw(IconCog), arrow: true },
      { to: "/notifications", text: "Notificações", iconComponent: markRaw(IconBell), arrow: true },
      { to: "/billing", text: "Faturamento", iconComponent: markRaw(IconCreditCard), arrow: true },
    ])

    const isActive = (routePath) => {
      if (routePath === "/") return route.path === "/"
      return route.path.startsWith(routePath)
    }

    const userInitials = computed(() => {
      const full = userData.value?.fullName?.trim()
      if (!full) return "U"
      const parts = full.split(/\s+/).filter(Boolean)
      if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase()
      return parts[0][0].toUpperCase()
    })

    const currentYear = computed(() => new Date().getFullYear())

    const contentContainerClass = computed(() => {
      if (props.fullWidth) return 'w-full'
      if (!props.contentPadding) return 'w-full'
      return 'container mx-auto px-2 py-2 sm:px-4 lg:px-6 sm:py-6'
    })

    const closeUserMenu = () => { showUserMenu.value = false }
    const closeMobileMenu = () => {
      showMobileMenu.value = false
      document.body.style.overflow = ""
    }
    const closeAllMenus = () => { closeUserMenu(); closeMobileMenu() }

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
        document.body.style.overflow = "hidden"
      } else {
        closeMobileMenu()
      }
    }

    const handleLogout = () => {
      logout()
      closeAllMenus()
    }

    const onDocumentClick = (e) => {
      if (!showUserMenu.value) return
      const el = userMenuRef.value
      if (el && !el.contains(e.target)) closeUserMenu()
    }

    const onKeyDown = (e) => {
      if (e.key === "Escape") {
        closeAllMenus()
        userTriggerRef.value?.focus?.()
      }
    }

    const onResize = () => {
      if (window.innerWidth >= 768 && showMobileMenu.value) closeMobileMenu()
    }

    watch(() => route.fullPath, () => closeAllMenus())

    onMounted(() => {
      loadTheme()
      document.addEventListener("click", onDocumentClick)
      document.addEventListener("keydown", onKeyDown)
      window.addEventListener("resize", onResize)
    })

    onUnmounted(() => {
      document.removeEventListener("click", onDocumentClick)
      document.removeEventListener("keydown", onKeyDown)
      window.removeEventListener("resize", onResize)
      document.body.style.overflow = ""
    })

    return {
      isDarkMode,
      contentContainerClass,
      isAuthenticated,
      userData,
      userInitials,
      showUserMenu,
      showMobileMenu,
      userTriggerRef,
      userMenuRef,
      menuItems,
      userMenuItems,
      themeIconComponent,
      mobileToggleIcon,
      currentYear,
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
@import "@/assets/default.css";
</style>