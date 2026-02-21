// src/router/solicitacoes.routes.js

/**
 * Rotas da API para o módulo de Solicitações
 * Base URL: /api/Solicitacoes
 */

const BASE_URL = '/api/Solicitacoes';

export const SolicitacoesRoutes = {
  // Rotas principais
  base: BASE_URL,
  
  // Endpoints específicos
  endpoints: {
    // POST /api/Solicitacoes - Criar nova solicitação de ajuste
    criar: () => BASE_URL,
    
    // GET /api/Solicitacoes - Listar todas as solicitações (com filtros)
    listar: (filtros = {}) => {
      const params = new URLSearchParams();
      
      if (filtros.status) params.append('status', filtros.status);
      if (filtros.tipo) params.append('tipo', filtros.tipo);
      if (filtros.dataInicio) params.append('dataInicio', filtros.dataInicio);
      if (filtros.dataFim) params.append('dataFim', filtros.dataFim);
      if (filtros.busca) params.append('busca', filtros.busca);
      if (filtros.pagina) params.append('pagina', filtros.pagina);
      if (filtros.limite) params.append('limite', filtros.limite);
      if (filtros.ordenacao) params.append('ordenacao', filtros.ordenacao);
      
      const queryString = params.toString();
      return queryString ? `${BASE_URL}?${queryString}` : BASE_URL;
    },
    
    // GET /api/Solicitacoes/{id} - Buscar solicitação específica
    buscarPorId: (id) => `${BASE_URL}/${id}`,
    
    // GET /api/Solicitacoes/minhas - Listar solicitações do usuário logado
    minhas: (filtros = {}) => {
      const params = new URLSearchParams();
      
      if (filtros.status) params.append('status', filtros.status);
      if (filtros.tipo) params.append('tipo', filtros.tipo);
      if (filtros.dataInicio) params.append('dataInicio', filtros.dataInicio);
      if (filtros.dataFim) params.append('dataFim', filtros.dataFim);
      
      const queryString = params.toString();
      return `${BASE_URL}/minhas${queryString ? `?${queryString}` : ''}`;
    },
    
    // PATCH /api/Solicitacoes/{id}/revisar - Aprovar ou rejeitar solicitação
    revisar: (id) => `${BASE_URL}/${id}/revisar`,
  },
  
  // Métodos utilitários para construir URLs com parâmetros
  comFiltros: (filtros) => SolicitacoesRoutes.endpoints.listar(filtros),
  comId: (id) => SolicitacoesRoutes.endpoints.buscarPorId(id),
};

/**
 * Exemplos de uso:
 * 
 * // Criar solicitação
 * POST SolicitacoesRoutes.endpoints.criar()
 * Body: {
 *   "faturamentoParcialId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
 *   "tipo": "AJUSTE_VALOR",
 *   "motivo": "Valor incorreto lançado",
 *   "dadosAntigos": "{\"valor\": 100.50}",
 *   "dadosNovos": "{\"valor\": 150.75}"
 * }
 * 
 * // Listar solicitações pendentes
 * GET SolicitacoesRoutes.endpoints.listar({ status: 'PENDENTE' })
 * 
 * // Listar minhas solicitações dos últimos 7 dias
 * GET SolicitacoesRoutes.endpoints.minhas({ 
 *   dataInicio: '2026-02-13',
 *   status: 'PENDENTE' 
 * })
 * 
 * // Aprovar solicitação
 * PATCH SolicitacoesRoutes.endpoints.revisar('3fa85f64-5717-4562-b3fc-2c963f66afa6')
 * Body: {
 *   "acao": "APROVAR",
 *   "justificativa": "Ajuste validado pelo supervisor"
 * }
 */