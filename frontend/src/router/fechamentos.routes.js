// src/router/fechamentos.routes.js
/**
 * Módulo: Faturamento Diário (Fechamento de Caixa)
 * Base URL: /api
 */

export const fechamentosRoutes = {
  // ==================== ROTAS PRINCIPAIS ====================
  
  /**
   * POST /api/unidades/{unidadeId}/fechamentos
   * Abrir Dia - Inicia o movimento do dia (Fundo de caixa, observações)
   * @param {string} unidadeId - ID da unidade
   * @returns {string} URL completa para abrir dia
   */
  abrirDia: (unidadeId) => `/unidades/${unidadeId}/fechamentos`,

  /**
   * GET /api/unidades/{unidadeId}/fechamentos
   * Histórico de fechamentos da unidade
   * @param {string} unidadeId - ID da unidade
   * @returns {string} URL para listar fechamentos
   */
  listar: (unidadeId) => `/unidades/${unidadeId}/fechamentos`,

  /**
   * GET /api/unidades/{unidadeId}/fechamentos/{id}
   * Detalhes de um fechamento específico
   * @param {string} unidadeId - ID da unidade
   * @param {string} id - ID do fechamento
   * @returns {string} URL para detalhes do fechamento
   */
  detalhes: (unidadeId, id) => `/unidades/${unidadeId}/fechamentos/${id}`,

  /**
   * PUT /api/unidades/{unidadeId}/fechamentos/{id}
   * Atualiza o fechamento (correção de erros)
   * @param {string} unidadeId - ID da unidade
   * @param {string} id - ID do fechamento
   * @returns {string} URL para atualizar fechamento
   */
  atualizar: (unidadeId, id) => `/unidades/${unidadeId}/fechamentos/${id}`,

  // ==================== AÇÕES DE FLUXO ====================

  /**
   * POST /api/unidades/{unidadeId}/fechamentos/{id}/fechar
   * Fechar Caixa - Líder encerra o dia informando os totais finais
   * @param {string} unidadeId - ID da unidade
   * @param {string} id - ID do fechamento
   * @returns {string} URL para fechar caixa
   */
  fechar: (unidadeId, id) => `/unidades/${unidadeId}/fechamentos/${id}/fechar`,

  /**
   * POST /api/unidades/{unidadeId}/fechamentos/{id}/conferir
   * Aprovação - Supervisor confere e aprova o caixa fechado
   * @param {string} unidadeId - ID da unidade
   * @param {string} id - ID do fechamento
   * @returns {string} URL para conferir fechamento
   */
  conferir: (unidadeId, id) => `/unidades/${unidadeId}/fechamentos/${id}/conferir`,

  /**
   * POST /api/unidades/{unidadeId}/fechamentos/{id}/reabrir
   * Correção - Reabre um caixa fechado para ajustes (requer justificativa)
   * @param {string} unidadeId - ID da unidade
   * @param {string} id - ID do fechamento
   * @returns {string} URL para reabrir fechamento
   */
  reabrir: (unidadeId, id) => `/unidades/${unidadeId}/fechamentos/${id}/reabrir`,

  // ==================== ROTAS ESPECIAIS ====================

  /**
   * GET /api/fechamentos/pendentes
   * Dashboard Supervisor - Lista todos os caixas esperando conferência
   * @returns {string} URL para fechamentos pendentes
   */
  pendentes: () => '/fechamentos/pendentes',

  /**
   * GET /api/unidades/{unidadeId}/fechamentos/por-data
   * Busca fechamentos por intervalo de datas
   * @param {string} unidadeId - ID da unidade
   * @returns {string} URL para busca por data
   */
  porData: (unidadeId) => `/unidades/${unidadeId}/fechamentos/por-data`,
};

// ==================== MODELOS DE DADOS (Schemas) ====================

/**
 * Payload para abrir dia (POST /unidades/{unidadeId}/fechamentos)
 */
export const abrirDiaPayloadSchema = {
  data: '2026-01-17',
  fundoDeCaixa: 0,
  observacoes: 'string',
};

/**
 * Payload para fechar caixa (POST /unidades/{unidadeId}/fechamentos/{id}/fechar)
 */
export const fecharCaixaPayloadSchema = {
  valorConferido: 0,
  valorTotal: 0,
  observacoes: 'string',
};

/**
 * Payload para conferir fechamento (POST /unidades/{unidadeId}/fechamentos/{id}/conferir)
 */
export const conferirFechamentoPayloadSchema = {
  aprovado: true,
  valorTotal: 0,
  observacoes: 'string',
};

/**
 * Payload para reabrir fechamento (POST /unidades/{unidadeId}/fechamentos/{id}/reabrir)
 */
export const reabrirFechamentoPayloadSchema = {
  motivo: 'string',
};

/**
 * Payload para atualizar fechamento (PUT /unidades/{unidadeId}/fechamentos/{id})
 */
export const atualizarFechamentoPayloadSchema = {
  status: 0,
  fundoDeCaixa: 0,
  observacoes: 'string',
  valorAtm: 0,
  valorBoletosMensalistas: 0,
};

/**
 * Modelo de resposta para Fechamento
 */
export const fechamentoResponseSchema = {
  id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  unidadeId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  data: '2026-01-24',
  status: 'string',
  statusCaixa: 'string',
  fundoDeCaixa: 0,
  observacoes: 'string',
  valorAtm: 0,
  valorBoletosMensalistas: 0,
  valorTotalCalculado: 0,
  valorConferido: 0,
  diferenca: 0,
  hashAssinatura: 'string',
  dataFechamento: '2026-01-24T14:01:30.430Z',
  dataConferencia: '2026-01-24T14:01:30.430Z',
  observacoesConferencia: 'string',
  valorTotalParciais: 0,
  fechadoPorUserId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  conferidoPorUserId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  valorTotal: 0,
};

// ==================== CONSTANTES ====================

/**
 * Status possíveis para o caixa
 */
export const STATUS_CAIXA = {
  ABERTO: 'Aberto',
  FECHADO: 'Fechado',
  CONFERIDO: 'Conferido',
  PENDENTE: 'Pendente',
};

/**
 * Status de fluxo do fechamento
 */
export const STATUS_FECHAMENTO = {
  EM_ANDAMENTO: 'em_andamento',
  FINALIZADO: 'finalizado',
  CONFERIDO: 'conferido',
  REABERTO: 'reaberto',
  CANCELADO: 'cancelado',
};

// ==================== HELPERS ====================

/**
 * Gera query string para busca por data
 * @param {Object} params - Parâmetros de busca
 * @param {string} params.dataInicio - Data inicial (YYYY-MM-DD)
 * @param {string} params.dataFim - Data final (YYYY-MM-DD)
 * @returns {string} Query string formatada
 */
export const getBuscaPorDataQuery = (params = {}) => {
  const queryParams = new URLSearchParams();
  
  if (params.dataInicio) queryParams.append('dataInicio', params.dataInicio);
  if (params.dataFim) queryParams.append('dataFim', params.dataFim);
  
  return queryParams.toString() ? `?${queryParams.toString()}` : '';
};

// ==================== EXEMPLOS DE USO ====================

/**
 * Exemplos de uso das rotas
 */
export const exemplosUso = {
  abrirDia: {
    url: fechamentosRoutes.abrirDia('unidade-123'),
    method: 'POST',
    payload: {
      data: '2026-02-20',
      fundoDeCaixa: 200.00,
      observacoes: 'Abertura do dia com fundo de caixa padrão'
    }
  },
  
  listar: {
    url: fechamentosRoutes.listar('unidade-123'),
    method: 'GET'
  },
  
  fechar: {
    url: fechamentosRoutes.fechar('unidade-123', 'fechamento-456'),
    method: 'POST',
    payload: {
      valorConferido: 1500.50,
      valorTotal: 1500.50,
      observacoes: 'Fechamento OK'
    }
  },
  
  conferir: {
    url: fechamentosRoutes.conferir('unidade-123', 'fechamento-456'),
    method: 'POST',
    payload: {
      aprovado: true,
      valorTotal: 1500.50,
      observacoes: 'Conferência aprovada'
    }
  },
  
  reabrir: {
    url: fechamentosRoutes.reabrir('unidade-123', 'fechamento-456'),
    method: 'POST',
    payload: {
      motivo: 'Necessário ajuste nos valores'
    }
  },
  
  pendentes: {
    url: fechamentosRoutes.pendentes(),
    method: 'GET'
  },
  
  porData: {
    url: fechamentosRoutes.porData('unidade-123') + getBuscaPorDataQuery({
      dataInicio: '2026-02-01',
      dataFim: '2026-02-20'
    }),
    method: 'GET'
  },
  
  atualizar: {
    url: fechamentosRoutes.atualizar('unidade-123', 'fechamento-456'),
    method: 'PUT',
    payload: {
      status: 1,
      fundoDeCaixa: 200.00,
      observacoes: 'Atualização de observações',
      valorAtm: 500.00,
      valorBoletosMensalistas: 300.00
    }
  },
  
  detalhes: {
    url: fechamentosRoutes.detalhes('unidade-123', 'fechamento-456'),
    method: 'GET'
  }
};