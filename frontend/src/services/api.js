import axios from 'axios';

// Cria a instância do axios
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:8080',
  headers: {
    'Content-Type': 'application/json',
    'Cache-Control': 'no-cache',
    'Pragma': 'no-cache'
  },
  timeout: 10000 // 10 segundos
});

// Interceptor para adicionar o token JWT em todas as requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Log para debug (remova em produção)
    console.log(`🚀 ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`, config.data || '');
    
    return config;
  },
  (error) => {
    console.error('❌ Erro no interceptor de request:', error);
    return Promise.reject(error);
  }
);

// Interceptor de resposta para tratamento global de erros
api.interceptors.response.use(
  (response) => {
    console.log(`✅ ${response.status} ${response.config.url}`, response.data);
    return response;
  },
  (error) => {
    console.error('❌ Erro na resposta:', {
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      data: error.response?.data,
      message: error.message
    });

    // Tratamento para token expirado
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      delete api.defaults.headers.common['Authorization'];
      
      // Redirecionar para login se estiver em uma rota protegida
      if (window.location.pathname !== '/') {
        window.location.href = '/';
      }
    }

    return Promise.reject(error);
  }
);

export default api;