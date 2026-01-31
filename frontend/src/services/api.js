// services/api.js
import axios from 'axios';

// 1. Criamos uma instância do Axios. 
// Isso evita que tenhamos que digitar "http://localhost:8080" em todo lugar.
const api = axios.create({
    baseURL: 'http://localhost:8080', // Endereço do seu Backend
    headers: {
        'Content-Type': 'application/json',
        // Outros headers globais podem vir aqui
    }
});

// 2. Interceptor de Requisição (Request Interceptor)
// "Antes de qualquer requisição sair, faça isso:"
api.interceptors.request.use(
    (config) => {
        // Tenta pegar o token salvo no navegador
        const token = localStorage.getItem('token');
        
        // Se o token existir, injeta no cabeçalho Authorization
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        
        return config; // Solta a requisição com o token anexado
    },
    (error) => {
        // Se der erro na configuração da requisição, rejeita
        return Promise.reject(error);
    }
);

// 3. Interceptor de Resposta (Response Interceptor)
// "Quando a resposta voltar do backend, faça isso:"
api.interceptors.response.use(
    (response) => response, // Se deu sucesso (200, 201), só passa os dados
    (error) => {
        // Se o erro for 401 (Não autorizado), significa que o token expirou ou é inválido
        if (error.response && error.response.status === 401) {
            console.warn('Sessão expirada ou token inválido.');
            // Aqui poderíamos forçar um logout automático:
            // localStorage.removeItem('token');
            // window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default api;





// import axios from 'axios';

// // Detecção automática do ambiente - Versão segura para build
// const getBaseURL = () => {
//   // Verifica se estamos no browser (runtime)
//   if (typeof window !== 'undefined') {
//     // Se estamos em produção (domínio não é localhost)
//     if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
//       return `https://${window.location.hostname}`;
//     }
//     // Em desenvolvimento, usa o proxy do Vite (se configurado) ou o fallback
//     return import.meta.env.MODE === 'development' ? '/api' : (import.meta.env.VITE_API_URL || 'http://localhost:8080');
//   } else {
//     // Durante o build (Node.js), retorna uma string vazia ou o valor da env var
//     return process.env.VITE_API_URL || 'http://localhost:8080';
//   }
// };

// const api = axios.create({
//   baseURL: getBaseURL(),
//   headers: {
//     'Content-Type': 'application/json',
//     'Cache-Control': 'no-cache',
//     'Pragma': 'no-cache'
//   },
//   timeout: 15000
// });

// // DEBUG CORS - Só executa no browser
// if (typeof window !== 'undefined') {
//   console.log('🚀 API Base URL Configurada:', api.defaults.baseURL);
//   console.log('🌐 Hostname Atual:', window.location.hostname);
//   console.log('🔧 Ambiente:', import.meta.env.MODE);
//   console.log('📍 Origin:', window.location.origin);
// }

// // Interceptor de request MELHORADO
// api.interceptors.request.use(
//   (config) => {
//     // Só tenta acessar localStorage no browser
//     if (typeof window !== 'undefined') {
//       const token = localStorage.getItem('authToken');
//       if (token) {
//         config.headers.Authorization = `Bearer ${token}`;
//         console.log('🔑 Token JWT adicionado aos headers');
//       } else {
//         console.warn('⚠️ Nenhum token JWT encontrado no localStorage');
//       }
      
//       console.log(`🚀 ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`, {
//         headers: {
//           ...config.headers,
//           // Não logar o token completo por segurança
//           Authorization: config.headers.Authorization ? 'Bearer ***' : undefined
//         },
//         data: config.data
//       });
//     }
    
//     return config;
//   },
//   (error) => {
//     console.error('❌ Erro no interceptor de request:', error);
//     return Promise.reject(error);
//   }
// );

// // Interceptor de resposta MELHORADO para debug CORS
// api.interceptors.response.use(
//   (response) => {
//     if (typeof window !== 'undefined') {
//       console.log(`✅ ${response.status} ${response.config.url}`, {
//         data: response.data
//       });
//     }
//     return response;
//   },
//   (error) => {
//     // Só executa logs detalhados no browser
//     if (typeof window !== 'undefined') {
//       // Detecção específica de erro CORS
//       const isCorsError = !error.response && (
//         error.message.includes('Network Error') || 
//         error.message.includes('Failed to fetch') ||
//         error.code === 'ERR_NETWORK'
//       );
      
//       if (isCorsError) {
//         console.error('🚫 ERRO CORS DETECTADO:', {
//           message: error.message,
//           code: error.code,
//           config: {
//             url: error.config?.baseURL + error.config?.url,
//             method: error.config?.method,
//             headers: error.config?.headers
//           }
//         });
        
//         console.log('💡 SOLUÇÕES PARA CORS:');
//         console.log('1. Configure CORS no backend para permitir sua origem');
//         console.log('2. Verifique se o backend está rodando');
//         console.log('3. Use um proxy no vite.config.js');
        
//       } else if (error.response?.status === 405) {
//         console.error('🔒 ERRO 405 - MÉTODO NÃO PERMITIDO:', {
//           url: error.config?.baseURL + error.config?.url,
//           method: error.config?.method,
//           status: error.response?.status,
//           statusText: error.response?.statusText,
//           headers: error.response?.headers,
//           data: error.response?.data
//         });
        
//         console.log('💡 SOLUÇÕES PARA 405:');
//         console.log('1. Verifique se o endpoint suporta o método ' + error.config?.method);
//         console.log('2. Verifique CORS no backend (preflight OPTIONS)');
//         console.log('3. Verifique se a rota está correta no backend');
        
//       } else if (error.response?.status === 401) {
//         console.error('🔐 ERRO 401 - NÃO AUTORIZADO:', {
//           url: error.config?.url,
//           message: 'Token pode ter expirado ou ser inválido'
//         });
        
//         localStorage.removeItem('authToken');
//         delete api.defaults.headers.common['Authorization'];
        
//         // Redirecionar para login se estiver em uma SPA
//         if (window.location.pathname !== '/') {
//           window.location.href = '/';
//         }
//       } else {
//         console.error('❌ Erro na resposta:', {
//           url: error.config?.baseURL + error.config?.url,
//           method: error.config?.method,
//           status: error.response?.status,
//           statusText: error.response?.statusText,
//           data: error.response?.data,
//           message: error.message,
//           code: error.code
//         });
//       }
//     }

//     return Promise.reject(error);
//   }
// );

// // Função para testar a conexão com o backend
// export const testBackendConnection = async () => {
//   // Só executa no browser
//   if (typeof window === 'undefined') return false;
  
//   try {
//     console.log('🧪 Testando conexão com o backend...');
//     const response = await api.get('/');
//     console.log('✅ Backend respondendo:', response.status);
//     return true;
//   } catch (error) {
//     console.error('❌ Backend não respondendo:', error.message);
//     return false;
//   }
// };

// // Testar conexão automaticamente em desenvolvimento
// if (typeof window !== 'undefined' && import.meta.env.MODE === 'development') {
//   setTimeout(() => {
//     testBackendConnection();
//   }, 1000);
// }

// export default api;