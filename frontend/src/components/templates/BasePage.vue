<!-- components/templates/BasePage.vue -->
<template>
  <div class="min-h-screen bg-gray-50 text-gray-800 flex flex-col">
    <!-- Slot para header personalizado ou uso do default -->
    <slot name="header">
      <AppHeader 
        :logo="headerLogo"
        :menu-items="headerMenuItems"
        :show-mobile-menu="showMobileMenu"
        @toggle-mobile-menu="toggleMobileMenu"
      />
    </slot>

    <!-- Conteúdo principal com ou sem sidebar -->
    <div class="flex flex-1">
      <!-- Sidebar (condicional) -->
      <slot name="sidebar" v-if="hasSidebar">
        <Sidebar :items="sidebarItems" />
      </slot>

      <!-- Conteúdo da página -->
      <main class="flex-1">
        <PageContainer :full-width="fullWidth">
          <!-- Slot para conteúdo específico da página -->
          <slot></slot>
        </PageContainer>
      </main>
    </div>

    <!-- Footer -->
    <slot name="footer">
      <AppFooter 
        :copyright="footerCopyright"
        :links="footerLinks"
        :show-year="showFooterYear"
      />
    </slot>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import AppHeader from '../Layout/AppHeader.vue'
import AppFooter from '../Layout/AppFooter.vue'
import Sidebar from '../Layout/Sidebar.vue'
import PageContainer from '../Layout/PageContainer.vue'

const props = defineProps({
  // Header props
  headerLogo: {
    type: Object,
    default: () => ({
      text: 'VueTail',
      icon: 'V'
    })
  },
  headerMenuItems: {
    type: Array,
    default: () => [
      { label: 'Home', href: '#' },
      { label: 'Sobre', href: '#' },
      { label: 'Serviços', href: '#' },
      { label: 'Portfólio', href: '#' },
      { label: 'Contato', href: '#' }
    ]
  },
  
  // Sidebar props
  hasSidebar: {
    type: Boolean,
    default: false
  },
  sidebarItems: {
    type: Array,
    default: () => []
  },
  
  // Footer props
  footerCopyright: {
    type: String,
    default: 'Modelo Responsivo Vue.js + Tailwind CSS'
  },
  footerLinks: {
    type: Array,
    default: () => [
      { label: 'GitHub', href: '#' },
      { label: 'Documentação', href: '#' },
      { label: 'Exemplos', href: '#' },
      { label: 'Contato', href: '#' }
    ]
  },
  showFooterYear: {
    type: Boolean,
    default: true
  },
  
  // Layout props
  fullWidth: {
    type: Boolean,
    default: false
  }
})

const showMobileMenu = ref(false)

const toggleMobileMenu = () => {
  showMobileMenu.value = !showMobileMenu.value
}
</script>