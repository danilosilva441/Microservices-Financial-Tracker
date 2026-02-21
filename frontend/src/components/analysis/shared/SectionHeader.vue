<!--
 * src/components/analysis/shared/SectionHeader.vue
 * SectionHeader.vue
 *
 * A reusable Vue component that serves as a section header for different parts of the financial analysis dashboard.
 * It includes a title, an optional subtitle, and an optional icon. The component also provides a slot for action buttons or links.
 * The design is responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="flex flex-col items-start justify-between gap-4 mb-6 sm:flex-row sm:items-center">
    <div class="flex items-center gap-3">
      <div v-if="icon" class="p-2 rounded-lg bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400">
        <Component :is="icon" class="w-6 h-6" />
      </div>
      <div>
        <h2 class="text-xl font-bold text-gray-900 dark:text-white">{{ title }}</h2>
        <p v-if="subtitle" class="text-sm text-gray-600 dark:text-gray-400">{{ subtitle }}</p>
      </div>
    </div>
    
    <div class="flex items-center w-full gap-2 sm:w-auto">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  subtitle: {
    type: String,
    default: ''
  },
  icon: {
    type: [Object, Function],
    default: null
  }
})
</script>