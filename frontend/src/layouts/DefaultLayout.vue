<!-- src/layouts/DefaultLayout.vue -->
<template>
  <div class="default-layout">
    <!-- Navbar -->
    <header v-if="showHeader" class="layout-header">
      <nav class="navbar" aria-label="Navegação principal">
        <!-- Brand -->
        <div class="navbar-brand">
          <router-link to="/" class="logo" @click="closeAllMenus">
            <span>DS SysTech</span>
          </router-link>
        </div>

        <!-- Desktop Menu -->
        <div class="navbar-menu" aria-label="Menu desktop">
          <router-link
            v-for="item in menuItems"
            :key="item.to"
            :to="item.to"
            class="nav-link"
            :class="{ active: isActive(item.to) }"
            @click="closeAllMenus"
          >
            <i v-if="item.icon" :class="item.icon" aria-hidden="true"></i>
            {{ item.text }}
          </router-link>
        </div>

        <!-- Right Actions -->
        <div class="navbar-actions">
          <!-- Mobile toggle -->
          <button
            class="mobile-menu-toggle"
            type="button"
            @click="toggleMobileMenu"
            :aria-expanded="showMobileMenu ? 'true' : 'false'"
            aria-controls="mobileMenu"
            aria-label="Abrir menu"
          >
            <i class="fas" :class="showMobileMenu ? 'fa-times' : 'fa-bars'" aria-hidden="true"></i>
          </button>

          <!-- User Menu -->
          <div class="navbar-user">
            <div class="user-dropdown" v-if="isAuthenticated">
              <button
                ref="userTriggerRef"
                class="user-trigger"
                type="button"
                @click="toggleUserMenu"
                :aria-expanded="showUserMenu ? 'true' : 'false'"
                aria-controls="userDropdown"
              >
                <div class="user-avatar" aria-hidden="true">
                  {{ userInitials }}
                </div>
                <span class="user-name">{{ userData?.fullName || 'Usuário' }}</span>
                <i class="fas fa-chevron-down" aria-hidden="true"></i>
              </button>

              <div
                v-if="showUserMenu"
                id="userDropdown"
                class="dropdown-menu"
                role="menu"
              >
                <router-link to="/profile" class="dropdown-item" role="menuitem" @click="closeAllMenus">
                  <i class="fas fa-user" aria-hidden="true"></i>
                  Meu Perfil
                </router-link>
                <router-link to="/settings" class="dropdown-item" role="menuitem" @click="closeAllMenus">
                  <i class="fas fa-cog" aria-hidden="true"></i>
                  Configurações
                </router-link>
                <div class="dropdown-divider" role="separator"></div>
                <button class="dropdown-item logout" type="button" role="menuitem" @click="handleLogout">
                  <i class="fas fa-sign-out-alt" aria-hidden="true"></i>
                  Sair
                </button>
              </div>
            </div>

            <router-link v-else to="/login" class="btn-login" @click="closeAllMenus">
              <i class="fas fa-sign-in-alt" aria-hidden="true"></i>
              Entrar
            </router-link>
          </div>
        </div>
      </nav>

      <!-- Mobile overlay -->
      <div
        class="mobile-menu-overlay"
        :class="{ active: showMobileMenu }"
        @click="closeMobileMenu"
        aria-hidden="true"
      ></div>

      <!-- Mobile menu panel -->
      <div
        id="mobileMenu"
        class="mobile-menu"
        :class="{ active: showMobileMenu }"
        role="dialog"
        aria-modal="true"
        aria-label="Menu mobile"
      >
        <div class="mobile-menu-header">
          <div class="mobile-user" v-if="isAuthenticated">
            <div class="user-avatar big" aria-hidden="true">{{ userInitials }}</div>
            <div class="mobile-user-info">
              <div class="mobile-user-name">{{ userData?.fullName || 'Usuário' }}</div>
              <div class="mobile-user-subtitle">Conta autenticada</div>
            </div>
          </div>

          <div class="mobile-auth" v-else>
            <router-link to="/login" class="btn-login full" @click="closeAllMenus">
              <i class="fas fa-sign-in-alt" aria-hidden="true"></i>
              Entrar
            </router-link>
          </div>
        </div>

        <div class="mobile-links" aria-label="Links do menu">
          <router-link
            v-for="item in menuItems"
            :key="`m-${item.to}`"
            :to="item.to"
            class="nav-link mobile"
            :class="{ active: isActive(item.to) }"
            @click="closeAllMenus"
          >
            <i v-if="item.icon" :class="item.icon" aria-hidden="true"></i>
            {{ item.text }}
          </router-link>
        </div>

        <div class="mobile-footer" v-if="isAuthenticated">
          <router-link to="/profile" class="mobile-action" @click="closeAllMenus">
            <i class="fas fa-user" aria-hidden="true"></i>
            Meu Perfil
          </router-link>
          <router-link to="/settings" class="mobile-action" @click="closeAllMenus">
            <i class="fas fa-cog" aria-hidden="true"></i>
            Configurações
          </router-link>
          <button class="mobile-action danger" type="button" @click="handleLogout">
            <i class="fas fa-sign-out-alt" aria-hidden="true"></i>
            Sair
          </button>
        </div>
      </div>
    </header>

    <!-- Conteúdo Principal -->
    <main class="layout-main">
      <slot></slot>
    </main>

    <!-- Footer -->
    <footer v-if="showFooter" class="layout-footer">
      <div class="footer-content">
        <div class="footer-section">
          <h4>DS SysTech</h4>
          <p>Sistema de gerenciamento de portfolio</p>
        </div>
        <div class="footer-section">
          <h4>Links</h4>
          <router-link to="/" @click="closeAllMenus">Home</router-link>
          <router-link to="/about" @click="closeAllMenus">Sobre</router-link>
          <router-link to="/contact" @click="closeAllMenus">Contato</router-link>
        </div>
        <div class="footer-section">
          <h4>Legal</h4>
          <router-link to="/terms" @click="closeAllMenus">Termos</router-link>
          <router-link to="/privacy" @click="closeAllMenus">Privacidade</router-link>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; {{ currentYear }} DS SysTech. Todos os direitos reservados.</p>
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

    // states
    const showUserMenu = ref(false)
    const showMobileMenu = ref(false)
    const userTriggerRef = ref(null)

    // Itens do menu
    const menuItems = computed(() => {
      const items = [
        { to: '/', text: 'Home', icon: 'fas fa-home' },
        { to: '/dashboard', text: 'Dashboard', icon: 'fas fa-chart-line' },
        { to: '/unidades', text: 'Unidades', icon: 'fas fa-store' },
        { to: '/projects', text: 'Projetos', icon: 'fas fa-briefcase' },
        { to: '/reports', text: 'Relatórios', icon: 'fas fa-chart-bar' },
        { to: '/team', text: 'Equipe', icon: 'fas fa-users' }
      ]

      const requiresAuth = new Set([
        '/dashboard',
        '/unidades',
        '/projects',
        '/reports',
        '/team'
      ])

      return items.filter((item) => (requiresAuth.has(item.to) ? isAuthenticated.value : true))
    })

    // active route
    const isActive = (routePath) => {
      if (routePath === '/') return route.path === '/'
      return route.path.startsWith(routePath)
    }

    // user initials
    const userInitials = computed(() => {
      const full = userData.value?.fullName?.trim()
      if (!full) return 'U'
      const parts = full.split(/\s+/).filter(Boolean)
      if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase()
      return parts[0][0].toUpperCase()
    })

    const currentYear = computed(() => new Date().getFullYear())

    // helpers
    const lockBodyScroll = (lock) => {
      document.documentElement.classList.toggle('no-scroll', lock)
      document.body.classList.toggle('no-scroll', lock)
    }

    const closeUserMenu = () => {
      showUserMenu.value = false
    }

    const closeMobileMenu = () => {
      showMobileMenu.value = false
      lockBodyScroll(false)
    }

    const closeAllMenus = () => {
      closeUserMenu()
      closeMobileMenu()
    }

    // toggles
    const toggleUserMenu = async () => {
      // se abrir user menu, fecha mobile
      if (!showUserMenu.value) {
        closeMobileMenu()
        showUserMenu.value = true
        await nextTick()
      } else {
        showUserMenu.value = false
      }
    }

    const toggleMobileMenu = () => {
      // se abrir mobile, fecha user
      if (!showMobileMenu.value) closeUserMenu()

      showMobileMenu.value = !showMobileMenu.value
      lockBodyScroll(showMobileMenu.value)
    }

    // click outside
    const onDocumentClick = (e) => {
      // fecha user menu se clicar fora do dropdown
      if (showUserMenu.value && !e.target.closest('.user-dropdown')) {
        closeUserMenu()
      }
    }

    // esc close
    const onKeyDown = (e) => {
      if (e.key === 'Escape') {
        closeAllMenus()
        // devolve foco ao gatilho do user menu, se existir
        if (userTriggerRef.value) userTriggerRef.value.focus?.()
      }
    }

    // Logout
    const handleLogout = () => {
      logout()
      closeAllMenus()
    }

    // fecha menus ao trocar rota
    watch(
      () => route.fullPath,
      () => closeAllMenus()
    )

    // fecha mobile ao mudar para desktop (evita overlay preso)
    const onResize = () => {
      if (window.innerWidth >= 769 && showMobileMenu.value) {
        closeMobileMenu()
      }
    }

    onMounted(() => {
      document.addEventListener('click', onDocumentClick)
      document.addEventListener('keydown', onKeyDown)
      window.addEventListener('resize', onResize)
    })

    onUnmounted(() => {
      document.removeEventListener('click', onDocumentClick)
      document.removeEventListener('keydown', onKeyDown)
      window.removeEventListener('resize', onResize)
      lockBodyScroll(false)
    })

    return {
      route,
      isAuthenticated,
      userData,
      menuItems,
      userInitials,
      currentYear,
      showUserMenu,
      showMobileMenu,
      userTriggerRef,
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
/* Base */
.default-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f7fafc;
}

.layout-header {
  background: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
  position: sticky;
  top: 0;
  z-index: 1000;
}

/* Navbar */
.navbar {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.navbar-brand {
  min-width: 140px;
}

.navbar-brand .logo {
  font-size: 22px;
  font-weight: 800;
  text-decoration: none;
  color: #667eea;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  white-space: nowrap;
}

/* Desktop menu */
.navbar-menu {
  display: flex;
  gap: 18px;
  flex: 1;
  justify-content: center;
  min-width: 0;
}

.nav-link {
  text-decoration: none;
  color: #666;
  font-weight: 600;
  padding: 10px 14px;
  border-radius: 10px;
  transition: all 0.2s;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  position: relative;
  white-space: nowrap;
  line-height: 1;
}

.nav-link:hover {
  color: #667eea;
  background: rgba(102, 126, 234, 0.1);
}

.nav-link.active {
  color: #667eea;
  background: rgba(102, 126, 234, 0.12);
}

.nav-link.active::after {
  content: '';
  position: absolute;
  bottom: -6px;
  left: 50%;
  transform: translateX(-50%);
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: #667eea;
}

/* Right area */
.navbar-actions {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  min-width: 170px;
  justify-content: flex-end;
}

/* User */
.navbar-user {
  position: relative;
}

.user-dropdown {
  position: relative;
}

.user-trigger {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 8px 10px;
  border-radius: 12px;
  transition: all 0.2s;
}

.user-trigger:hover {
  background: rgba(0, 0, 0, 0.05);
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 999px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 14px;
}

.user-avatar.big {
  width: 42px;
  height: 42px;
  font-size: 14px;
}

.user-name {
  font-weight: 600;
  color: #333;
  max-width: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Dropdown */
.dropdown-menu {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  background: white;
  border-radius: 14px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12);
  min-width: 220px;
  padding: 10px 0;
  z-index: 1001;
  animation: dropdownFade 0.16s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

@keyframes dropdownFade {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 18px;
  text-decoration: none;
  color: #333;
  transition: all 0.2s;
  width: 100%;
  text-align: left;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

.dropdown-item:hover {
  background: rgba(102, 126, 234, 0.1);
  color: #667eea;
}

.dropdown-item.logout {
  color: #ff4d4d;
}

.dropdown-item.logout:hover {
  background: rgba(255, 77, 77, 0.1);
}

.dropdown-divider {
  height: 1px;
  background: #eee;
  margin: 10px 0;
}

/* Login button */
.btn-login {
  padding: 10px 16px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  text-decoration: none;
  font-weight: 700;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: transform 0.2s, box-shadow 0.2s;
  line-height: 1;
}

.btn-login:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.25);
}

.btn-login.full {
  width: 100%;
  justify-content: center;
}

/* Main */
.layout-main {
  flex: 1;
  padding: 24px 20px;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
  min-width: 0;
}

/* Footer */
.layout-footer {
  background: #2d3748;
  color: white;
  padding: 40px 20px 20px;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 28px;
  margin-bottom: 30px;
}

.footer-section h4 {
  margin-bottom: 14px;
  font-size: 16px;
}

.footer-section p {
  color: #a0aec0;
  line-height: 1.6;
}

.footer-section a {
  display: block;
  color: #a0aec0;
  text-decoration: none;
  margin-bottom: 10px;
  transition: color 0.2s;
}

.footer-section a:hover {
  color: white;
}

.footer-bottom {
  max-width: 1200px;
  margin: 0 auto;
  padding-top: 16px;
  border-top: 1px solid #4a5568;
  text-align: center;
  color: #a0aec0;
  font-size: 13px;
}

/* Mobile menu */
.mobile-menu-toggle {
  display: none;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  border: 1px solid rgba(0, 0, 0, 0.08);
  background: white;
  font-size: 18px;
  color: #667eea;
  cursor: pointer;
  transition: background 0.2s, transform 0.2s;
}

.mobile-menu-toggle:hover {
  background: rgba(102, 126, 234, 0.08);
  transform: translateY(-1px);
}

.mobile-menu-overlay {
  display: none;
  position: fixed;
  top: 70px; /* abaixo do header */
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.45);
  z-index: 999;
}

.mobile-menu-overlay.active {
  display: block;
}

.mobile-menu {
  display: none;
  position: fixed;
  top: 70px;
  right: 0;
  bottom: 0;
  width: min(420px, 92vw);
  background: white;
  box-shadow: -10px 0 30px rgba(0, 0, 0, 0.12);
  z-index: 1000;
  padding: 18px;
  animation: slideIn 0.2s ease;
  overflow: auto;
}

.mobile-menu.active {
  display: block;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(16px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.mobile-menu-header {
  padding-bottom: 14px;
  border-bottom: 1px solid #f0f0f0;
  margin-bottom: 14px;
}

.mobile-user {
  display: flex;
  align-items: center;
  gap: 12px;
}

.mobile-user-info {
  min-width: 0;
}

.mobile-user-name {
  font-weight: 800;
  color: #1f2937;
  line-height: 1.2;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.mobile-user-subtitle {
  color: #6b7280;
  font-size: 12px;
  margin-top: 2px;
}

.mobile-links {
  display: grid;
  gap: 8px;
}

.nav-link.mobile {
  display: flex;
  width: 100%;
  padding: 14px 14px;
  border-radius: 12px;
  font-size: 15px;
}

.mobile-footer {
  margin-top: 16px;
  padding-top: 14px;
  border-top: 1px solid #f0f0f0;
  display: grid;
  gap: 10px;
}

.mobile-action {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 12px;
  border-radius: 12px;
  text-decoration: none;
  color: #111827;
  background: #f8fafc;
  border: 1px solid rgba(0, 0, 0, 0.06);
  cursor: pointer;
  font-weight: 700;
  transition: background 0.2s, transform 0.2s;
}

.mobile-action:hover {
  background: rgba(102, 126, 234, 0.1);
  transform: translateY(-1px);
}

.mobile-action.danger {
  color: #ff4d4d;
  background: rgba(255, 77, 77, 0.08);
}

/* Responsividade */
@media (max-width: 1024px) {
  .navbar-menu {
    gap: 10px;
  }

  .nav-link {
    padding: 9px 12px;
    font-size: 14px;
  }

  .user-name {
    max-width: 140px;
  }
}

@media (max-width: 768px) {
  .navbar {
    padding: 0 14px;
  }

  .navbar-menu {
    display: none;
  }

  .mobile-menu-toggle {
    display: inline-flex;
    align-items: center;
    justify-content: center;
  }

  .user-name {
    display: none;
  }

  .layout-main {
    padding: 18px 14px;
  }

  .footer-content {
    grid-template-columns: 1fr;
    gap: 22px;
  }
}

@media (max-width: 480px) {
  .navbar-brand .logo {
    font-size: 18px;
  }

  .navbar-actions {
    min-width: 0;
  }

  .user-avatar {
    width: 34px;
    height: 34px;
    font-size: 12px;
  }

  .dropdown-menu {
    min-width: 190px;
    right: -6px;
  }
}

/* Trava scroll quando menu mobile abre (aplicada via JS) */
:global(html.no-scroll),
:global(body.no-scroll) {
  overflow: hidden;
}
</style>