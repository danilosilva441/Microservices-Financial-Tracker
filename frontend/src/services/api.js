// services/api.js - VERSÃO APRIMORADA
import axios from 'axios';

// 1. Configuração base do Axios
const api = axios.create({
  baseURL: 'http://localhost:8080', // Endereço do seu Backend
  headers: {
    'Content-Type': 'application/json',
    'Cache-Control': 'no-cache',
    'Pragma': 'no-cache'
  },
  timeout: 15000 // 15 segundos timeout
});

// DEBUG: Mostra configuração apenas no browser
if (typeof window !== 'undefined') {
  console.log('🚀 API Configurada:', {
    baseURL: api.defaults.baseURL,
    hostname: window.location.hostname,
    origin: window.location.origin
  });
}

// 2. Interceptor de Requisição (Request Interceptor)
api.interceptors.request.use(
  (config) => {
    // Tenta pegar o token salvo no navegador (APENAS no browser)
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token') || localStorage.getItem('authToken');
      
      // Se o token existir, injeta no cabeçalho Authorization
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
        console.log('🔑 Token JWT adicionado aos headers');
      } else {
        console.warn('⚠️ Nenhum token JWT encontrado no localStorage');
      }
      
      // Log da requisição (sem dados sensíveis)
      console.log(`🚀 ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`, {
        headers: {
          ...config.headers,
          // Não logar o token completo por segurança
          Authorization: config.headers.Authorization ? 'Bearer ***' : undefined
        },
        data: config.data ? '[DADOS OCULTOS]' : undefined
      });
    }
    
    return config;
  },
  (error) => {
    console.error('❌ Erro no interceptor de request:', error);
    return Promise.reject(error);
  }
);

// 3. Interceptor de Resposta (Response Interceptor)
api.interceptors.response.use(
  (response) => {
    // 🔧 CORREÇÃO CRÍTICA: Normaliza respostas .NET com $values
    if (response.data && response.data.$values && Array.isArray(response.data.$values)) {
      console.log('🔄 Normalizando resposta .NET: extraindo $values');
      response.data = response.data.$values;
    }
    
    // Log de sucesso apenas no browser
    if (typeof window !== 'undefined') {
      console.log(`✅ ${response.status} ${response.config.url}`, {
        data: Array.isArray(response.data) ? 
          `[Array com ${response.data.length} itens]` : 
          response.data
      });
    }
    
    return response;
  },
  (error) => {
    // Detecção específica de erro CORS
    const isCorsError = !error.response && (
      error.message.includes('Network Error') || 
      error.message.includes('Failed to fetch') ||
      error.code === 'ERR_NETWORK'
    );
    
    if (isCorsError) {
      console.error('🚫 ERRO CORS DETECTADO:', {
        message: error.message,
        code: error.code,
        config: {
          url: error.config?.baseURL + error.config?.url,
          method: error.config?.method
        }
      });
      
      console.log('💡 SOLUÇÕES PARA CORS:');
      console.log('1. Configure CORS no backend para permitir sua origem');
      console.log('2. Verifique se o backend está rodando');
      
    } else if (error.response?.status === 401) {
      console.warn('🔐 Sessão expirada ou token inválido.');
      
      // Remove token inválido e redireciona
      if (typeof window !== 'undefined') {
        localStorage.removeItem('token');
        localStorage.removeItem('authToken');
        
        // Se não estiver na página inicial, redireciona
        if (window.location.pathname !== '/login' && window.location.pathname !== '/') {
          window.location.href = '/login';
        }
      }
    } else {
      // Log de erro geral
      console.error('❌ Erro na resposta:', {
        url: error.config?.baseURL + error.config?.url,
        method: error.config?.method,
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
        message: error.message
      });
    }
    
    return Promise.reject(error);
  }
);

// 4. Função para testar conexão (opcional)
export const testBackendConnection = async () => {
  // Só executa no browser
  if (typeof window === 'undefined') return false;
  
  try {
    console.log('🧪 Testando conexão com o backend...');
    const response = await api.get('/');
    console.log('✅ Backend respondendo:', response.status);
    return true;
  } catch (error) {
    console.error('❌ Backend não respondendo:', error.message);
    return false;
  }
};

// Testar conexão automaticamente em desenvolvimento
if (typeof window !== 'undefined' && process.env.NODE_ENV === 'development') {
  setTimeout(() => {
    testBackendConnection();
  }, 1000);
}

export default api;