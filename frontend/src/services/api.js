import axios from 'axios';

// Cria uma instância do Axios com a URL base do nosso backend (o API Gateway)
const api = axios.create({
    baseURL: 'http://localhost:8080/api',
});

// Este é um "interceptor". Ele vai rodar antes de CADA requisição.
// A função dele é pegar o token JWT do armazenamento local e adicioná-lo
// ao cabeçalho 'Authorization' da requisição.
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('authToken');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;