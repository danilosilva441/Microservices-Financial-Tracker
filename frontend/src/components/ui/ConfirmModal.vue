<!-- components/ui/ConfirmModal.vue -->
<template>
  <div class="modal-overlay" @click.self="$emit('cancel')">
    <div class="modal-container">
      <div class="modal-header" :class="variant">
        <h3>{{ title }}</h3>
        <button @click="$emit('cancel')" class="btn-close">
          <i class="fas fa-times"></i>
        </button>
      </div>
      
      <div class="modal-body">
        <div class="modal-icon" :class="variant">
          <i :class="icon"></i>
        </div>
        <p class="modal-message">{{ message }}</p>
      </div>
      
      <div class="modal-footer">
        <button @click="$emit('cancel')" class="btn btn-secondary">
          {{ cancelText }}
        </button>
        <button @click="$emit('confirm')" class="btn" :class="`btn-${variant}`">
          {{ confirmText }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  title: {
    type: String,
    required: true
  },
  message: {
    type: String,
    required: true
  },
  confirmText: {
    type: String,
    default: 'Confirmar'
  },
  cancelText: {
    type: String,
    default: 'Cancelar'
  },
  variant: {
    type: String,
    default: 'primary',
    validator: (value) => ['primary', 'danger', 'warning', 'success'].includes(value)
  }
});

const emit = defineEmits(['confirm', 'cancel']);

const icon = {
  primary: 'fas fa-info-circle',
  danger: 'fas fa-exclamation-triangle',
  warning: 'fas fa-exclamation-circle',
  success: 'fas fa-check-circle'
}[props.variant];
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
  backdrop-filter: blur(4px);
}

.modal-container {
  background: white;
  border-radius: 16px;
  width: 100%;
  max-width: 500px;
  overflow: hidden;
  animation: modalAppear 0.3s ease;
}

@keyframes modalAppear {
  from {
    opacity: 0;
    transform: scale(0.95) translateY(20px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

.modal-header {
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
}

.modal-header.primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.modal-header.danger {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
}

.modal-header.warning {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
}

.modal-header.success {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
}

.btn-close {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.btn-close:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(90deg);
}

.modal-body {
  padding: 40px 24px;
  text-align: center;
}

.modal-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  margin: 0 auto 24px;
}

.modal-icon.primary {
  background: rgba(102, 126, 234, 0.1);
  color: #667eea;
}

.modal-icon.danger {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.modal-icon.warning {
  background: rgba(245, 158, 11, 0.1);
  color: #f59e0b;
}

.modal-icon.success {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.modal-message {
  font-size: 16px;
  color: #4a5568;
  line-height: 1.6;
  margin: 0;
}

.modal-footer {
  padding: 24px;
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  border-top: 1px solid var(--unidade-border);
}

.modal-footer .btn {
  min-width: 100px;
  padding: 12px 24px;
}
</style>