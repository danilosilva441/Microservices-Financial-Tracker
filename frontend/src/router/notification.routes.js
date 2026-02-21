// src/router/notification.routes.js
export const notificationRoutes = {
  // Rotas principais
  base: '/notifications',
  
  // Ações específicas
  actions: {
    list: '/notifications',
    add: '/notifications/add',
    remove: '/notifications/remove',
    clear: '/notifications/clear',
  },
  
  // Tipos de notificação (para uso em queries ou filtros)
  types: {
    success: '/notifications/type/success',
    error: '/notifications/type/error',
    warning: '/notifications/type/warning',
    info: '/notifications/type/info',
  },
  
  // Se estiver usando API (exemplo)
  api: {
    base: '/api/notifications',
    endpoints: {
      getAll: '/api/notifications',
      create: '/api/notifications',
      delete: (id) => `/api/notifications/${id}`,
      clearAll: '/api/notifications/clear',
    }
  },
  
  // Configurações
  config: {
    maxNotifications: '/notifications/config/max',
    timeout: '/notifications/config/timeout',
  }
};

// Helper function para construir URLs
export function buildNotificationRoute(path, params = {}) {
  let url = path;
  
  // Substitui parâmetros na URL (ex: /api/notifications/:id)
  Object.keys(params).forEach(key => {
    url = url.replace(`:${key}`, params[key]);
  });
  
  return url;
}

// Exemplo de uso:
// buildNotificationRoute(notificationRoutes.api.endpoints.delete(':id'), { id: '123' })
// Resultado: '/api/notifications/123'