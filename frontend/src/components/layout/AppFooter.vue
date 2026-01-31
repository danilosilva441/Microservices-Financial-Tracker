<!-- components/Layout/AppFooter.vue -->
<template>
  <footer class="bg-gray-800 text-white py-8">
    <div class="container mx-auto px-4">
      <div class="flex flex-col md:flex-row justify-between items-center">
        <div class="mb-6 md:mb-0">
          <div class="flex items-center space-x-2 mb-3">
            <div class="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
              <span class="text-white font-bold">V</span>
            </div>
            <h3 class="text-xl font-bold">VueTail Model</h3>
          </div>
          <p class="text-gray-400">{{ copyright }}</p>
        </div>
        
        <div class="flex space-x-6">
          <a 
            v-for="link in links" 
            :key="link.label"
            :href="link.href" 
            class="text-gray-300 hover:text-white transition-colors"
          >
            {{ link.label }}
          </a>
        </div>
      </div>
      
      <div class="border-t border-gray-700 mt-8 pt-6 text-center text-gray-400 text-sm">
        <p>© {{ currentYear }} - {{ copyright }}. Todos os direitos reservados.</p>
        <p class="mt-1">Desenvolvido para funcionar perfeitamente em dispositivos móveis e desktop.</p>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  copyright: {
    type: String,
    default: 'Modelo Responsivo Vue.js + Tailwind CSS'
  },
  links: {
    type: Array,
    default: () => []
  },
  showYear: {
    type: Boolean,
    default: true
  }
})

const currentYear = computed(() => {
  return props.showYear ? new Date().getFullYear() : ''
})
</script>