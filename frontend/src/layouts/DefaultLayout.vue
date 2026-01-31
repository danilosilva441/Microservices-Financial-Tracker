src/layouts/DefaultLayout.vue
<template>
  <div class="default-layout">
    <!-- Navbar -->
    <header v-if="showHeader" class="layout-header">
      <nav class="navbar">
        <!-- Logo -->
        <div class="navbar-brand">
          <router-link to="/" class="logo">
            <span>Meu App</span>
          </router-link>
        </div>

        <!-- Menu Desktop -->
        <div class="navbar-menu">
          <router-link 
            v-for="item in menuItems" 
            :key="item.to"
            :to="item.to"
            class="nav-link"
            :class="{ active: $route.path === item.to }"
          >
            <i v-if="item.icon" :class="item.icon"></i>
            {{ item.text }}
          </router-link>
        </div>

        <!-- User Menu -->
        <div class="navbar-user">
          <div class="user-dropdown" v-if="isAuthenticated">
            <button class="user-trigger" @click="toggleUserMenu">
              <div class="user-avatar">
                {{ userInitials }}
              </div>
              <span class="user-name">{{ currentUser?.name || 'Usuário' }}</span>
              <i class="fas fa-chevron-down"></i>
            </button>
            
            <div v-if="showUserMenu" class="dropdown-menu">
              <router-link to="/profile" class="dropdown-item">
                <i class="fas fa-user"></i>
                Meu Perfil
              </router-link>
              <router-link to="/settings" class="dropdown-item">
                <i class="fas fa-cog"></i>
                Configurações
              </router-link>
              <div class="dropdown-divider"></div>
              <button class="dropdown-item logout" @click="logout">
                <i class="fas fa-sign-out-alt"></i>
                Sair
              </button>
            </div>
          </div>
          
          <router-link v-else to="/login" class="btn-login">
            <i class="fas fa-sign-in-alt"></i>
            Entrar
          </router-link>
        </div>
      </nav>
    </header>

    <!-- Conteúdo Principal -->
    <main class="layout-main">
      <slot></slot>
    </main>

    <!-- Footer -->
    <footer v-if="showFooter" class="layout-footer">
      <div class="footer-content">
        <div class="footer-section">
          <h4>Meu App</h4>
          <p>Sistema de gerenciamento de portfolio</p>
        </div>
        <div class="footer-section">
          <h4>Links</h4>
          <router-link to="/">Home</router-link>
          <router-link to="/about">Sobre</router-link>
          <router-link to="/contact">Contato</router-link>
        </div>
        <div class="footer-section">
          <h4>Legal</h4>
          <router-link to="/terms">Termos</router-link>
          <router-link to="/privacy">Privacidade</router-link>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; {{ currentYear }} DS SysTech. Todos os direitos reservados.</p>
      </div>
    </footer>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'DefaultLayout',
  
  props: {
    showHeader: {
      type: Boolean,
      default: true
    },
    showFooter: {
      type: Boolean,
      default: true
    }
  },
  
  setup() {
    const route = useRoute()
    const { isAuthenticated, currentUser, logout } = useAuth()
    
    // Estado para menu do usuário
    const showUserMenu = ref(false)
    
    // Itens do menu
    const menuItems = computed(() => {
      const items = [
        { to: '/', text: 'Home', icon: 'fas fa-home' },
        { to: '/dashboard', text: 'Dashboard', icon: 'fas fa-chart-line' },
        { to: '/projects', text: 'Projetos', icon: 'fas fa-briefcase' },
        { to: '/reports', text: 'Relatórios', icon: 'fas fa-chart-bar' },
        { to: '/team', text: 'Equipe', icon: 'fas fa-users' }
      ]
      
      // Filtra itens baseado na autenticação
      return items.filter(item => {
        if (item.to === '/dashboard' || item.to === '/projects') {
          return isAuthenticated.value
        }
        return true
      })
    })
    
    // Iniciais do usuário para avatar
    const userInitials = computed(() => {
      if (!currentUser.value?.name) return 'U'
      const names = currentUser.value.name.split(' ')
      if (names.length >= 2) {
        return (names[0][0] + names[1][0]).toUpperCase()
      }
      return names[0][0].toUpperCase()
    })
    
    // Ano atual para footer
    const currentYear = computed(() => new Date().getFullYear())
    
    // Toggle menu do usuário
    const toggleUserMenu = () => {
      showUserMenu.value = !showUserMenu.value
    }
    
    // Fechar menu quando clicar fora
    const closeUserMenu = (e) => {
      if (!e.target.closest('.user-dropdown')) {
        showUserMenu.value = false
      }
    }
    
    // Logout
    const handleLogout = () => {
      logout()
      showUserMenu.value = false
    }
    
    // Adiciona listener para cliques fora do menu
    onMounted(() => {
      document.addEventListener('click', closeUserMenu)
    })
    
    onUnmounted(() => {
      document.removeEventListener('click', closeUserMenu)
    })
    
    return {
      route,
      isAuthenticated,
      currentUser,
      menuItems,
      userInitials,
      currentYear,
      showUserMenu,
      toggleUserMenu,
      logout: handleLogout
    }
  }
}
</script>

<style scoped>
.default-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.layout-header {
  background: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.navbar {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.navbar-brand .logo {
  font-size: 24px;
  font-weight: bold;
  text-decoration: none;
  color: #667eea;
  display: flex;
  align-items: center;
  gap: 10px;
}

.navbar-menu {
  display: flex;
  gap: 30px;
}

.nav-link {
  text-decoration: none;
  color: #666;
  font-weight: 500;
  padding: 8px 16px;
  border-radius: 8px;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.nav-link:hover {
  color: #667eea;
  background: rgba(102, 126, 234, 0.1);
}

.nav-link.active {
  color: #667eea;
  background: rgba(102, 126, 234, 0.1);
}

.navbar-user {
  position: relative;
}

.user-dropdown {
  position: relative;
}

.user-trigger {
  display: flex;
  align-items: center;
  gap: 10px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 8px;
  transition: all 0.3s;
}

.user-trigger:hover {
  background: rgba(0, 0, 0, 0.05);
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 14px;
}

.user-name {
  font-weight: 500;
  color: #333;
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  min-width: 200px;
  padding: 10px 0;
  z-index: 1001;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 20px;
  text-decoration: none;
  color: #333;
  transition: all 0.3s;
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

.btn-login {
  padding: 10px 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-login:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
}

.layout-main {
  flex: 1;
  padding: 30px 20px;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}

.layout-footer {
  background: #2d3748;
  color: white;
  padding: 40px 20px 20px;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 40px;
  margin-bottom: 40px;
}

.footer-section h4 {
  margin-bottom: 20px;
  font-size: 18px;
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
  transition: color 0.3s;
}

.footer-section a:hover {
  color: white;
}

.footer-bottom {
  max-width: 1200px;
  margin: 0 auto;
  padding-top: 20px;
  border-top: 1px solid #4a5568;
  text-align: center;
  color: #a0aec0;
  font-size: 14px;
}

/* Responsividade */
@media (max-width: 768px) {
  .navbar-menu {
    display: none;
  }
  
  .navbar {
    padding: 0 15px;
  }
  
  .user-name {
    display: none;
  }
  
  .layout-main {
    padding: 20px 15px;
  }
  
  .footer-content {
    grid-template-columns: 1fr;
    gap: 30px;
  }
}
</style>