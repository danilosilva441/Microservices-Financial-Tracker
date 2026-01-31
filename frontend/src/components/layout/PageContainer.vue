<!-- components/Layout/PageContainer.vue -->
<template>
  <div :class="[
    'py-8',
    fullWidth ? 'px-4' : 'container mx-auto px-4'
  ]">
    <!-- Slot para conteúdo da página -->
    <slot></slot>
  </div>
</template>

<script setup>
defineProps({
  fullWidth: {
    type: Boolean,
    default: false
  }
})
</script>