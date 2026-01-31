<!-- components/Layout/AppHeader.vue -->
<template>
  <header class="sticky top-0 z-50 bg-white shadow-md">
    <div class="container mx-auto px-4 py-4 flex justify-between items-center">
      <!-- Logo -->
      <div class="flex items-center space-x-2">
        <div class="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
          <span class="text-white font-bold text-xl">{{ logo.icon }}</span>
        </div>
        <h1 class="text-2xl font-bold hidden md:block">{{ logo.text }}</h1>
      </div>

      <!-- Menu Desktop -->
      <nav class="hidden md:flex space-x-8">
        <a 
          v-for="item in menuItems" 
          :key="item.label"
          :href="item.href" 
          class="font-medium hover:text-blue-600 transition-colors duration-200"
        >
          {{ item.label }}
        </a>
      </nav>

      <!-- Botões Header -->
      <div class="flex items-center space-x-4">
        <!-- Slot para botões adicionais -->
        <slot name="header-buttons">
          <button class="hidden md:inline-block bg-blue-600 text-white px-5 py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors duration-200">
            Entrar
          </button>
        </slot>
        
        <!-- Botão Menu Mobile -->
        <button 
          @click="$emit('toggle-mobile-menu')" 
          class="md:hidden text-gray-600 hover:text-gray-900"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path v-if="!showMobileMenu" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
            <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
    </div>

    <!-- Menu Mobile -->
    <div v-if="showMobileMenu" class="md:hidden bg-white border-t border-gray-200">
      <div class="container mx-auto px-4 py-3 flex flex-col space-y-3">
        <a 
          v-for="item in menuItems" 
          :key="item.label"
          :href="item.href" 
          class="py-2 font-medium hover:text-blue-600"
        >
          {{ item.label }}
        </a>
        <button class="mt-2 bg-blue-600 text-white px-5 py-2 rounded-lg font-medium hover:bg-blue-700">
          Entrar
        </button>
      </div>
    </div>
  </header>
</template>

<script setup>
defineProps({
  logo: {
    type: Object,
    required: true
  },
  menuItems: {
    type: Array,
    required: true
  },
  showMobileMenu: {
    type: Boolean,
    default: false
  }
})

defineEmits(['toggle-mobile-menu'])
</script>