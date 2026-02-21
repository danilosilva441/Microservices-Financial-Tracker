<!--
 * src/components/analysis/shared/EmptyState.vue
 * EmptyState.vue
 *
 * A reusable Vue component that displays an empty state message when there is no data to show.
 * It includes an icon, a title, a message, and an optional action slot for buttons or links.
 * The component is designed to be flexible and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="flex flex-col items-center justify-center p-8 text-center">
    <div class="w-24 h-24 mb-4 text-gray-400 dark:text-gray-600">
      <!-- Chart Icon -->
      <svg v-if="type === 'chart'" xmlns="http://www.w3.org/2000/svg" class="w-full h-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
      </svg>
      
      <!-- Document Icon -->
      <svg v-else-if="type === 'table'" xmlns="http://www.w3.org/2000/svg" class="w-full h-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
      
      <!-- Filter Icon -->
      <svg v-else-if="type === 'filter'" xmlns="http://www.w3.org/2000/svg" class="w-full h-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
      </svg>
      
      <!-- Inbox Icon (default) -->
      <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-full h-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
      </svg>
    </div>
    <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">{{ title }}</h3>
    <p class="mb-4 text-gray-600 dark:text-gray-400">{{ message }}</p>
    <slot name="action"></slot>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: 'Nenhum dado encontrado'
  },
  message: {
    type: String,
    default: 'Não há dados disponíveis para exibir no momento.'
  },
  type: {
    type: String,
    default: 'empty',
    validator: (value) => ['empty', 'chart', 'table', 'filter'].includes(value)
  }
})
</script>