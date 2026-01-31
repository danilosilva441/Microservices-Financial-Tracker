<template>
  <div 
    :class="[
      'rounded-xl border transition-all duration-200',
      hoverable ? 'hover:shadow-lg hover:-translate-y-0.5' : '',
      shadow ? 'shadow-sm' : '',
      bgColor,
      borderColor,
      padding,
      width
    ]"
  >
    <!-- Header -->
    <div v-if="$slots.header || title" 
         :class="['px-4 py-3 border-b', headerBorderColor]">
      <slot name="header">
        <h3 class="text-sm font-semibold text-gray-900">{{ title }}</h3>
        <p v-if="subtitle" class="text-xs text-gray-500 mt-1">{{ subtitle }}</p>
      </slot>
    </div>

    <!-- Content -->
    <div :class="['p-4', contentClass]">
      <slot></slot>
    </div>

    <!-- Footer -->
    <div v-if="$slots.footer" 
         :class="['px-4 py-3 border-t', footerBorderColor]">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  title: String,
  subtitle: String,
  hoverable: {
    type: Boolean,
    default: false
  },
  shadow: {
    type: Boolean,
    default: true
  },
  bgColor: {
    type: String,
    default: 'bg-white'
  },
  borderColor: {
    type: String,
    default: 'border-gray-200'
  },
  headerBorderColor: {
    type: String,
    default: 'border-gray-200'
  },
  footerBorderColor: {
    type: String,
    default: 'border-gray-200'
  },
  padding: {
    type: String,
    default: 'p-4'
  },
  width: {
    type: String,
    default: 'w-full'
  },
  contentClass: {
    type: String,
    default: ''
  }
});
</script>