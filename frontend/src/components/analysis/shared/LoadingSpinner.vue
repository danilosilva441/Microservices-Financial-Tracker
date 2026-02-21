<!--
 * src/components/analysis/shared/LoadingSpinner.vue
 * LoadingSpinner.vue
 *
 * A reusable Vue component that displays a loading spinner while data is being fetched or processed.
 * It can be used in various parts of the financial analysis dashboard to indicate loading states.
 * The component is designed to be flexible and theme-aware, using Tailwind CSS for styling.
 * It accepts a 'fullscreen' prop to optionally take up more space when needed.
 -->
<template>
  <div class="flex items-center justify-center p-8" :class="fullscreen ? 'min-h-[400px]' : ''">
    <div class="relative">
      <div class="w-12 h-12 border-4 border-gray-200 rounded-full dark:border-gray-700"></div>
      <div class="absolute top-0 left-0 w-12 h-12 border-4 rounded-full border-primary-500 border-t-transparent animate-spin"></div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  fullscreen: {
    type: Boolean,
    default: false
  }
})
</script>