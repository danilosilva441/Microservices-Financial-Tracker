<!--
 * src/components/analysis/shared/ErrorState.vue
 * ErrorState.vue
 *
 * A reusable Vue component that displays an error state message when there is an issue loading data.
 * It includes an icon, a title, a message, and an optional retry button if the error is retryable.
 * The component is designed to be flexible and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="flex flex-col items-center justify-center p-8 text-center">
    <div class="w-24 h-24 mb-4 text-red-500">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-full h-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
      </svg>
    </div>
    <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">{{ title }}</h3>
    <p class="mb-4 text-gray-600 dark:text-gray-400">{{ message }}</p>
    <button 
      v-if="retryable"
      @click="$emit('retry')"
      class="btn-primary"
    >
      <IconArrowPath class="w-5 h-5 mr-2" />
      Tentar novamente
    </button>
  </div>
</template>

<script setup>
import IconArrowPath from '@/components/icons/ArrowPathIcon.vue'

defineProps({
  title: {
    type: String,
    default: 'Erro ao carregar dados'
  },
  message: {
    type: String,
    default: 'Ocorreu um erro ao carregar os dados. Tente novamente mais tarde.'
  },
  retryable: {
    type: Boolean,
    default: true
  }
})

defineEmits(['retry'])
</script>