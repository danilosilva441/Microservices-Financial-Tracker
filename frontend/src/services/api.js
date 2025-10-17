import axios from 'axios';

// Cria a instância do axios
const api = axios.create({
  // 1. Tenta usar a variável de ambiente VITE_API_URL (para produção).
  // 2. Se ela não existir, usa 'http://localhost:8080' (para desenvolvimento local).
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:8080',
});

// Interceptor para adicionar o token JWT em todas as requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;