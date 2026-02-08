<!-- components/ui/ToastContainer.vue -->
<template>
  <div class="toast-container">
    <transition-group name="toast" tag="div" class="toast-list">
      <div 
        v-for="toast in toasts" 
        :key="toast.id"
        class="toast"
        :class="`toast-${toast.type}`"
        @click="removeToast(toast.id)"
      >
        <i :class="toast.icon"></i>
        <div class="toast-content">
          <div class="toast-title">{{ toast.title }}</div>
          <div class="toast-message">{{ toast.message }}</div>
        </div>
        <button class="toast-close" @click.stop="removeToast(toast.id)">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const toasts = ref([])

const addToast = (toast) => {
  const id = Date.now()
  const icons = {
    success: 'fas fa-check-circle',
    error: 'fas fa-exclamation-circle',
    warning: 'fas fa-exclamation-triangle',
    info: 'fas fa-info-circle'
  }
  
  toasts.value.push({
    id,
    type: toast.type || 'info',
    title: toast.title || '',
    message: toast.message,
    icon: icons[toast.type] || icons.info,
    duration: toast.duration || 5000
  })
  
  // Remove automático após duração
  setTimeout(() => {
    removeToast(id)
  }, toast.duration || 5000)
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(t => t.id === id)
  if (index !== -1) {
    toasts.value.splice(index, 1)
  }
}

// Expõe métodos globalmente
defineExpose({
  addToast,
  removeToast
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 10000;
}

.toast-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.toast {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  min-width: 300px;
  max-width: 400px;
  cursor: pointer;
  animation: slideInRight 0.3s ease;
  border-left: 4px solid;
}

.toast:hover {
  transform: translateX(-4px);
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.15);
}

.toast-success {
  border-left-color: #10b981;
  background: linear-gradient(135deg, #10b98110 0%, #34d39910 100%);
}

.toast-error {
  border-left-color: #ef4444;
  background: linear-gradient(135deg, #ef444410 0%, #f8717110 100%);
}

.toast-warning {
  border-left-color: #f59e0b;
  background: linear-gradient(135deg, #f59e0b10 0%, #fbbf2410 100%);
}

.toast-info {
  border-left-color: #3b82f6;
  background: linear-gradient(135deg, #3b82f610 0%, #60a5fa10 100%);
}

.toast i {
  font-size: 20px;
  margin-top: 2px;
}

.toast-success i {
  color: #10b981;
}

.toast-error i {
  color: #ef4444;
}

.toast-warning i {
  color: #f59e0b;
}

.toast-info i {
  color: #3b82f6;
}

.toast-content {
  flex: 1;
}

.toast-title {
  font-weight: 600;
  color: #1a202c;
  margin-bottom: 4px;
  font-size: 14px;
}

.toast-message {
  color: #4a5568;
  font-size: 13px;
  line-height: 1.4;
}

.toast-close {
  background: none;
  border: none;
  color: #a0aec0;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.3s;
}

.toast-close:hover {
  background: rgba(0, 0, 0, 0.05);
  color: #718096;
}

/* Animações */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

@keyframes slideInRight {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}
</style>