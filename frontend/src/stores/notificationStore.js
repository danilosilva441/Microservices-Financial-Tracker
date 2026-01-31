import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useNotificationStore = defineStore('notification', () => {
  const notifications = ref([]);
  const maxNotifications = 5;

  const addNotification = (notification) => {
    const id = Date.now().toString();
    const fullNotification = {
      id,
      type: notification.type || 'info',
      message: notification.message,
      timeout: notification.timeout || 5000,
      ...notification
    };

    notifications.value.unshift(fullNotification);

    // Limita o número de notificações
    if (notifications.value.length > maxNotifications) {
      notifications.value = notifications.value.slice(0, maxNotifications);
    }

    // Remove automaticamente após timeout
    if (fullNotification.timeout > 0) {
      setTimeout(() => {
        removeNotification(id);
      }, fullNotification.timeout);
    }

    return id;
  };

  const removeNotification = (id) => {
    const index = notifications.value.findIndex(n => n.id === id);
    if (index !== -1) {
      notifications.value.splice(index, 1);
    }
  };

  const clearNotifications = () => {
    notifications.value = [];
  };

  return {
    notifications,
    addNotification,
    removeNotification,
    clearNotifications,
  };
});