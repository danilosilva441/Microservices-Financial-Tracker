<!--
 * src/components/common/BaseButton.vue
 * BaseButton.vue
 *
 * A reusable Vue component that serves as a base button for various actions throughout the application.
 * It supports multiple variants (primary, secondary, success, danger, warning, outline, ghost) and sizes (small, medium, large).
 * The component also includes features like loading state, disabled state, full width option, and icon support.
 * It is designed to be flexible and theme-aware, using Tailwind CSS for styling.
 -->

<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :class="[
      'inline-flex items-center justify-center font-medium rounded-lg transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2',
      sizeClasses,
      variantClasses,
      disabled || loading ? 'opacity-50 cursor-not-allowed' : '',
      fullWidth ? 'w-full' : '',
      customClass
    ]"
    @click="handleClick"
  >
    <!-- Loading State -->
    <svg v-if="loading" 
         class="w-4 h-4 mr-2 -ml-1 animate-spin" 
         xmlns="http://www.w3.org/2000/svg" 
         fill="none" 
         viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>

    <!-- Icon Left -->
    <span v-if="iconLeft" class="mr-2">{{ iconLeft }}</span>

    <!-- Content -->
    <span :class="loading ? 'opacity-75' : ''">
      <slot>{{ label }}</slot>
    </span>

    <!-- Icon Right -->
    <span v-if="iconRight" class="ml-2">{{ iconRight }}</span>
  </button>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  type: {
    type: String,
    default: 'button'
  },
  variant: {
    type: String,
    default: 'primary',
    validator: (value) => ['primary', 'secondary', 'success', 'danger', 'warning', 'outline', 'ghost'].includes(value)
  },
  size: {
    type: String,
    default: 'medium',
    validator: (value) => ['small', 'medium', 'large'].includes(value)
  },
  label: String,
  disabled: Boolean,
  loading: Boolean,
  fullWidth: Boolean,
  iconLeft: String,
  iconRight: String,
  customClass: String
});

const emit = defineEmits(['click']);

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'small': return 'px-3 py-1.5 text-xs';
    case 'large': return 'px-6 py-3 text-base';
    default: return 'px-4 py-2 text-sm';
  }
});

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'primary':
      return 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500';
    case 'secondary':
      return 'bg-gray-600 text-white hover:bg-gray-700 focus:ring-gray-500';
    case 'success':
      return 'bg-green-600 text-white hover:bg-green-700 focus:ring-green-500';
    case 'danger':
      return 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500';
    case 'warning':
      return 'bg-yellow-500 text-white hover:bg-yellow-600 focus:ring-yellow-500';
    case 'outline':
      return 'bg-transparent border border-gray-300 text-gray-700 hover:bg-gray-50 focus:ring-gray-500';
    case 'ghost':
      return 'bg-transparent text-gray-700 hover:bg-gray-100 focus:ring-gray-500';
    default:
      return 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500';
  }
});

const handleClick = (event) => {
  if (!props.disabled && !props.loading) {
    emit('click', event);
  }
};
</script>