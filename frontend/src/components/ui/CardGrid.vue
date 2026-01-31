<!-- components/UI/CardGrid.vue -->
<template>
  <div :class="[
    'grid gap-6',
    columnsMobile ? `grid-cols-${columnsMobile}` : 'grid-cols-1',
    columnsTablet ? `md:grid-cols-${columnsTablet}` : 'md:grid-cols-2',
    columnsDesktop ? `lg:grid-cols-${columnsDesktop}` : 'lg:grid-cols-3'
  ]">
    <slot></slot>
  </div>
</template>

<script setup>
defineProps({
  columnsMobile: {
    type: [String, Number],
    default: '1'
  },
  columnsTablet: {
    type: [String, Number],
    default: '2'
  },
  columnsDesktop: {
    type: [String, Number],
    default: '3'
  }
})
</script>