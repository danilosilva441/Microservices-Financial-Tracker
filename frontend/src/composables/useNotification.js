// composables/useNotification.js
import { useNotificationStore } from '@/stores/notificationStore';
import { storeToRefs } from 'pinia';

export function useNotification() {
  const store = useNotificationStore();
  const { notifications } = storeToRefs(store);
  const { addNotification, removeNotification, clearNotifications } = store;

  // Métodos auxiliares para tipos específicos de notificação
  const success = (message, options = {}) => {
    return addNotification({
      type: 'success',
      message,
      ...options
    });
  };

  const error = (message, options = {}) => {
    return addNotification({
      type: 'error',
      message,
      timeout: options.timeout || 7000, // Erros geralmente ficam mais tempo
      ...options
    });
  };

  const warning = (message, options = {}) => {
    return addNotification({
      type: 'warning',
      message,
      ...options
    });
  };

  const info = (message, options = {}) => {
    return addNotification({
      type: 'info',
      message,
      ...options
    });
  };

  return {
    // Estado reativo
    notifications,

    // Métodos principais
    addNotification,
    removeNotification,
    clearNotifications,

    // Métodos auxiliares
    success,
    error,
    warning,
    info,
  };
}