import { ref } from 'vue';

const toasts = ref([]);
const toastId = ref(0);

export function useToast() {
  const showToast = (options) => {
    const id = ++toastId.value;
    const toast = {
      id,
      title: options.title || '',
      message: options.message || '',
      type: options.type || 'info',
      duration: options.duration || 5000,
      icon: options.icon || getDefaultIcon(options.type),
      show: true
    };

    toasts.value.push(toast);

    // Remove automaticamente após a duração
    if (toast.duration > 0) {
      setTimeout(() => {
        removeToast(id);
      }, toast.duration);
    }

    return id;
  };

  const getDefaultIcon = (type) => {
    switch (type) {
      case 'success': return '✅';
      case 'error': return '❌';
      case 'warning': return '⚠️';
      case 'info': return 'ℹ️';
      default: return '💬';
    }
  };

  const removeToast = (id) => {
    const index = toasts.value.findIndex(toast => toast.id === id);
    if (index !== -1) {
      // Anima a saída
      toasts.value[index].show = false;
      setTimeout(() => {
        toasts.value.splice(index, 1);
      }, 300);
    }
  };

  const success = (message, title = 'Sucesso') => {
    return showToast({ type: 'success', title, message });
  };

  const error = (message, title = 'Erro') => {
    return showToast({ type: 'error', title, message });
  };

  const warning = (message, title = 'Aviso') => {
    return showToast({ type: 'warning', title, message });
  };

  const info = (message, title = 'Informação') => {
    return showToast({ type: 'info', title, message });
  };

  return {
    toasts,
    showToast,
    removeToast,
    success,
    error,
    warning,
    info
  };
}