<!-- components/UI/Card.vue -->
<template>
  <div :class="[
    'bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow duration-300 p-6',
    highlight ? 'border-l-4 border-blue-500' : ''
  ]">
    <!-- Slot para ícone -->
    <div v-if="$slots.icon" class="mb-4">
      <slot name="icon"></slot>
    </div>
    
    <!-- Título -->
    <h4 v-if="title" class="text-xl font-bold mb-3">{{ title }}</h4>
    
    <!-- Conteúdo -->
    <div :class="['text-gray-600', compact ? 'text-sm' : '']">
      <slot></slot>
    </div>
    
    <!-- Ações (slot opcional) -->
    <div v-if="$slots.actions" class="mt-4">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: ''
  },
  highlight: {
    type: Boolean,
    default: false
  },
  compact: {
    type: Boolean,
    default: false
  }
})
</script>