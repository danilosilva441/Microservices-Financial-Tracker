// services/api.js - VERSÃO PARA LOCAL E PRODUÇÃO
import axios from 'axios';

// 1. Configuração dinâmica baseada no ambiente
const getBaseURL = () => {
  // Se VITE_API_URL estiver definida (via variável de ambiente no .env), use-a
  if (import.meta.env.VITE_API_URL) {
    return import.meta.env.VITE_API_URL;
  }
  
  // Se estiver no navegador
  if (typeof window !== 'undefined') {
    const hostname = window.location.hostname;
    
    // Verifica se é localhost (Desenvolvimento)
    // 👇 AQUI ESTÁ A MÁGICA: Retornamos apenas '/api' em vez de 'http://localhost:8080'
    if (hostname === 'localhost' || hostname === '127.0.0.1') {
      return ''; 
    }
    
    // Se estiver no domínio de produção
    if (hostname.includes('dssystech.com')) {
      // Use HTTPS para produção
      return 'https://dssystech.com'; // Geralmente APIs de prod têm um subdomínio
    }
  }
  
  // Fallback para localhost
  return '/api';
};

// 2. Configuração base do Axios
const api = axios.create({
  baseURL: getBaseURL(),
  headers: {
    'Content-Type': 'application/json',
    'Cache-Control': 'no-cache',
    'Pragma': 'no-cache'
  },
  timeout: 30000, // 30 segundos para produção
  withCredentials: false, // Desative se não usar cookies
});

// DEBUG: Mostra configuração apenas no browser
if (typeof window !== 'undefined') {
  console.log('🚀 API Configurada:', {
    baseURL: api.defaults.baseURL,
    env: import.meta.env.MODE,
    hostname: window.location.hostname,
    origin: window.location.origin,
    viteApiUrl: import.meta.env.VITE_API_URL || 'não definido'
  });
}

// 3. Função para obter token de forma segura
const getToken = () => {
  try {
    if (typeof window === 'undefined') return null;
    
    // Tenta várias chaves possíveis para o token
    const token = 
      localStorage.getItem('token') ||
      localStorage.getItem('authToken') ||
      localStorage.getItem('access_token') ||
      localStorage.getItem('jwt');
    
    return token;
  } catch (error) {
    console.warn('⚠️ Erro ao acessar localStorage:', error.message);
    return null;
  }
};

// 4. Interceptor de Requisição (Request Interceptor)
api.interceptors.request.use(
  (config) => {
    // Obtém o token
    const token = getToken();
    
    // Se o token existir, injeta no cabeçalho Authorization
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
      
      if (typeof window !== 'undefined' && import.meta.env.DEV) {
        console.log('🔑 Token JWT adicionado aos headers');
      }
    } else if (typeof window !== 'undefined' && import.meta.env.DEV) {
      console.warn('⚠️ Nenhum token JWT encontrado no localStorage');
    }
    
    // Log da requisição apenas em desenvolvimento
    if (typeof window !== 'undefined' && import.meta.env.DEV) {
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

// 5. Interceptor de Resposta (Response Interceptor)
api.interceptors.response.use(
  (response) => {
    // 🔧 CORREÇÃO CRÍTICA: Normaliza respostas .NET com $values
    if (response.data && response.data.$values && Array.isArray(response.data.$values)) {
      if (import.meta.env.DEV) {
        console.log('🔄 Normalizando resposta .NET: extraindo $values');
      }
      response.data = response.data.$values;
    }
    
    // Log de sucesso apenas em desenvolvimento
    if (typeof window !== 'undefined' && import.meta.env.DEV) {
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
      error.code === 'ERR_NETWORK' ||
      error.message.includes('CORS')
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
      
      if (import.meta.env.DEV) {
        console.log('💡 SOLUÇÕES PARA CORS:');
        console.log('1. Configure CORS no backend para permitir sua origem');
        console.log('2. Verifique se o backend está rodando');
      }
      
    } else if (error.response?.status === 401) {
      console.warn('🔐 Sessão expirada ou token inválido.');
      
      // Remove token inválido e redireciona
      if (typeof window !== 'undefined') {
        localStorage.removeItem('token');
        localStorage.removeItem('authToken');
        localStorage.removeItem('access_token');
        localStorage.removeItem('jwt');
        
        // Se não estiver na página inicial, redireciona
        if (window.location.pathname !== '/login' && window.location.pathname !== '/') {
          // Usa timeout para evitar loops
          setTimeout(() => {
            window.location.href = '/login';
          }, 100);
        }
      }
    } else if (error.response?.status === 404) {
      console.warn('🔍 Endpoint não encontrado:', error.config?.url);
    } else if (error.response?.status === 500) {
      console.error('💥 Erro interno do servidor:', {
        url: error.config?.url,
        data: error.response?.data
      });
    } else {
      // Log de erro geral
      console.error('❌ Erro na resposta:', {
        url: error.config?.url,
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

// 6. Função para testar conexão (útil para debug)
export const testBackendConnection = async () => {
  // Só executa no browser
  if (typeof window === 'undefined') return false;
  
  try {
    console.log('🧪 Testando conexão com o backend...');
    // Se o seu health check for na raiz da API
    const response = await api.get('/health'); 
    console.log('✅ Backend respondendo:', response.status);
    return true;
  } catch (error) {
    // Tenta endpoint alternativo (talvez a raiz da API responda algo)
    try {
      const response = await api.get('/');
      console.log('✅ Backend respondendo (endpoint raiz):', response.status);
      return true;
    } catch (secondError) {
      console.error('❌ Backend não respondendo:', {
        url: api.defaults.baseURL,
        error: secondError.message
      });
      return false;
    }
  }
};

// 7. Função para verificar se está em produção
export const isProduction = () => {
  return import.meta.env.PROD || window.location.hostname.includes('dssystech.com');
};

// 8. Função para obter URL base (útil para outros serviços)
export const getApiBaseURL = () => api.defaults.baseURL;

// Testar conexão automaticamente em desenvolvimento
if (typeof window !== 'undefined' && import.meta.env.DEV) {
  setTimeout(() => {
    testBackendConnection();
  }, 2000);
}

export default api;