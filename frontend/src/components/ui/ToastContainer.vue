<script setup>
import { useToast } from '@/composables/useToast';

const { toasts, removeToast } = useToast();
</script>

<template>
  <div class="fixed top-4 right-4 z-50 w-full max-w-sm space-y-4">
    <transition-group name="toast">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="[
          'transform transition-all duration-300 ease-in-out',
          'rounded-lg shadow-lg p-4 flex items-start',
          toast.type === 'success' ? 'bg-green-50 border border-green-200' :
          toast.type === 'error' ? 'bg-red-50 border border-red-200' :
          toast.type === 'warning' ? 'bg-yellow-50 border border-yellow-200' :
          'bg-blue-50 border border-blue-200',
          toast.show ? 'translate-x-0 opacity-100' : 'translate-x-full opacity-0'
        ]"
        role="alert"
      >
        <!-- Icon -->
        <div :class="[
          'flex-shrink-0 mr-3',
          toast.type === 'success' ? 'text-green-400' :
          toast.type === 'error' ? 'text-red-400' :
          toast.type === 'warning' ? 'text-yellow-400' :
          'text-blue-400'
        ]">
          <span class="text-xl">{{ toast.icon }}</span>
        </div>

        <!-- Content -->
        <div class="flex-1 min-w-0">
          <h4 :class="[
            'text-sm font-semibold',
            toast.type === 'success' ? 'text-green-800' :
            toast.type === 'error' ? 'text-red-800' :
            toast.type === 'warning' ? 'text-yellow-800' :
            'text-blue-800'
          ]">
            {{ toast.title }}
          </h4>
          <p :class="[
            'text-sm mt-1',
            toast.type === 'success' ? 'text-green-600' :
            toast.type === 'error' ? 'text-red-600' :
            toast.type === 'warning' ? 'text-yellow-600' :
            'text-blue-600'
          ]">
            {{ toast.message }}
          </p>
        </div>

        <!-- Close Button -->
        <button
          @click="removeToast(toast.id)"
          class="ml-4 flex-shrink-0 text-gray-400 hover:text-gray-600 transition-colors"
        >
          <span class="sr-only">Fechar</span>
          <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}
</style>