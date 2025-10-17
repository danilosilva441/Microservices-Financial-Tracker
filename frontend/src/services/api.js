import axios from 'axios';

// Detecção automática do ambiente
const getBaseURL = () => {
  // Se estamos em produção (domínio não é localhost)
  if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
    // Usa o mesmo domínio do frontend para o backend (assumindo que estão no mesmo projeto)
    return `https://${window.location.hostname}`;
  }
  // Fallback para desenvolvimento
  return import.meta.env.VITE_API_URL || 'http://localhost:8080';
};

const api = axios.create({
  baseURL: getBaseURL(),
  headers: {
    'Content-Type': 'application/json',
    'Cache-Control': 'no-cache',
    'Pragma': 'no-cache'
  },
  timeout: 15000
});

// Log para debug (mostrará qual URL está sendo usada)
console.log('🚀 API Base URL Configurada:', api.defaults.baseURL);
console.log('🌐 Hostname Atual:', window.location.hostname);
console.log('🔧 Ambiente:', import.meta.env.MODE);

// ... resto do código permanece igual
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    console.log(`🚀 ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`);
    return config;
  },
  (error) => {
    console.error('❌ Erro no interceptor de request:', error);
    return Promise.reject(error);
  }
);

// Interceptor de resposta
api.interceptors.response.use(
  (response) => {
    console.log(`✅ ${response.status} ${response.config.url}`);
    return response;
  },
  (error) => {
    console.error('❌ Erro na resposta:', {
      url: error.config?.baseURL + error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      message: error.message
    });

    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      delete api.defaults.headers.common['Authorization'];
    }

    return Promise.reject(error);
  }
);

export default api;