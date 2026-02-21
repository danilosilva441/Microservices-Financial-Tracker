// src/router/shifts.routes.js
import { useShiftsStore } from '@/stores/shifts.store'

// Base URL para as rotas de turnos
const SHIFTS_BASE_URL = '/api/Shifts'

export const shiftsRoutes = [
  {
    path: `${SHIFTS_BASE_URL}/templates`,
    method: 'POST',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { unidadeId, name, type, defaultStartTime, defaultEndTime } = request.body
      
      try {
        const templateData = {
          unidadeId,
          name,
          type,
          defaultStartTime,
          defaultEndTime,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
        
        const result = await store.criarTemplate(templateData)
        
        if (result.success) {
          return reply.status(201).send({
            success: true,
            data: result.data,
            message: 'Template de turno criado com sucesso'
          })
        } else {
          return reply.status(400).send({
            success: false,
            error: result.error,
            message: 'Erro ao criar template de turno'
          })
        }
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message,
          message: 'Erro interno ao criar template de turno'
        })
      }
    },
    schema: {
      body: {
        type: 'object',
        required: ['unidadeId', 'name', 'type'],
        properties: {
          unidadeId: { type: 'string', format: 'uuid' },
          name: { type: 'string', minLength: 1, maxLength: 100 },
          type: { type: 'integer', minimum: 1, maximum: 10 },
          defaultStartTime: { type: 'string', pattern: '^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$' },
          defaultEndTime: { type: 'string', pattern: '^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$' }
        }
      },
      response: {
        201: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: { type: 'object' },
            message: { type: 'string' }
          }
        },
        400: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            error: { type: 'string' },
            message: { type: 'string' }
          }
        }
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/generate`,
    method: 'POST',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { unidadeId, templateId, startDate, endDate, userIds } = request.body
      
      try {
        const dadosEscala = {
          unidadeId,
          templateId,
          startDate,
          endDate,
          userIds,
          generatedAt: new Date().toISOString()
        }
        
        const result = await store.gerarEscala(dadosEscala)
        
        if (result.success) {
          return reply.status(201).send({
            success: true,
            data: result.data,
            message: 'Escala gerada com sucesso',
            meta: {
              totalShifts: Array.isArray(result.data) ? result.data.length : 1,
              period: { startDate, endDate },
              unidadeId
            }
          })
        } else {
          return reply.status(400).send({
            success: false,
            error: result.error,
            message: 'Erro ao gerar escala'
          })
        }
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message,
          message: 'Erro interno ao gerar escala'
        })
      }
    },
    schema: {
      body: {
        type: 'object',
        required: ['unidadeId', 'startDate', 'endDate', 'userIds'],
        properties: {
          unidadeId: { type: 'string', format: 'uuid' },
          templateId: { type: 'string', format: 'uuid' },
          startDate: { type: 'string', format: 'date' },
          endDate: { type: 'string', format: 'date' },
          userIds: {
            type: 'array',
            items: { type: 'string', format: 'uuid' },
            minItems: 1
          }
        }
      },
      response: {
        201: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: { 
              type: 'array',
              items: { type: 'object' }
            },
            message: { type: 'string' },
            meta: {
              type: 'object',
              properties: {
                totalShifts: { type: 'integer' },
                period: {
                  type: 'object',
                  properties: {
                    startDate: { type: 'string' },
                    endDate: { type: 'string' }
                  }
                },
                unidadeId: { type: 'string' }
              }
            }
          }
        }
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/unidade/:unidadeId`,
    method: 'GET',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { unidadeId } = request.params
      const { 
        startDate, 
        endDate, 
        userId,
        tipoTurno,
        apenasAtivos = 'true',
        page = 1,
        limit = 100 
      } = request.query
      
      try {
        await store.carregarShifts(unidadeId)
        
        // Aplicar filtros adicionais
        if (startDate || endDate) {
          store.filtros.dataInicio = startDate || null
          store.filtros.dataFim = endDate || null
        }
        
        if (userId) {
          store.filtros.userId = userId
        }
        
        if (tipoTurno) {
          store.filtros.tipoTurno = parseInt(tipoTurno)
        }
        
        if (apenasAtivos === 'false') {
          store.filtros.apenasAtivos = false
        }
        
        // Paginação
        const shiftsFiltrados = store.shiftsFiltrados
        const startIndex = (parseInt(page) - 1) * parseInt(limit)
        const endIndex = startIndex + parseInt(limit)
        const paginatedShifts = shiftsFiltrados.slice(startIndex, endIndex)
        
        // Gerar visualização de calendário
        const calendario = store.calendarioMensal
        
        return reply.status(200).send({
          success: true,
          data: {
            shifts: paginatedShifts,
            calendario,
            estatisticas: store.estatisticas,
            dashboard: store.dashboard
          },
          meta: {
            unidadeId,
            total: shiftsFiltrados.length,
            page: parseInt(page),
            limit: parseInt(limit),
            totalPages: Math.ceil(shiftsFiltrados.length / parseInt(limit)),
            filters: {
              startDate: startDate || null,
              endDate: endDate || null,
              userId: userId || null,
              tipoTurno: tipoTurno || null,
              apenasAtivos: apenasAtivos === 'true'
            }
          }
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message,
          message: 'Erro ao carregar turnos da unidade'
        })
      }
    },
    schema: {
      params: {
        type: 'object',
        required: ['unidadeId'],
        properties: {
          unidadeId: { type: 'string', format: 'uuid' }
        }
      },
      querystring: {
        type: 'object',
        properties: {
          startDate: { type: 'string', format: 'date' },
          endDate: { type: 'string', format: 'date' },
          userId: { type: 'string', format: 'uuid' },
          tipoTurno: { type: 'integer', minimum: 1, maximum: 10 },
          apenasAtivos: { type: 'string', enum: ['true', 'false'] },
          page: { type: 'integer', minimum: 1, default: 1 },
          limit: { type: 'integer', minimum: 1, maximum: 500, default: 100 }
        }
      },
      response: {
        200: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: {
              type: 'object',
              properties: {
                shifts: { type: 'array' },
                calendario: { type: 'array' },
                estatisticas: { type: 'object' },
                dashboard: { type: 'object' }
              }
            },
            meta: {
              type: 'object',
              properties: {
                unidadeId: { type: 'string' },
                total: { type: 'integer' },
                page: { type: 'integer' },
                limit: { type: 'integer' },
                totalPages: { type: 'integer' },
                filters: { type: 'object' }
              }
            }
          }
        }
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/:shiftId/breaks`,
    method: 'POST',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { shiftId } = request.params
      const { type, startTime, endTime } = request.body
      
      try {
        const intervaloData = {
          shiftId,
          type,
          startTime: new Date(startTime).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }),
          endTime: new Date(endTime).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }),
          createdAt: new Date().toISOString()
        }
        
        const result = await store.registrarIntervalo(shiftId, intervaloData)
        
        if (result.success) {
          return reply.status(201).send({
            success: true,
            data: result.data,
            message: 'Intervalo registrado com sucesso'
          })
        } else {
          return reply.status(400).send({
            success: false,
            error: result.error,
            message: 'Erro ao registrar intervalo'
          })
        }
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message,
          message: 'Erro interno ao registrar intervalo'
        })
      }
    },
    schema: {
      params: {
        type: 'object',
        required: ['shiftId'],
        properties: {
          shiftId: { type: 'string', format: 'uuid' }
        }
      },
      body: {
        type: 'object',
        required: ['type', 'startTime', 'endTime'],
        properties: {
          type: { type: 'integer', minimum: 1, maximum: 6 },
          startTime: { type: 'string', format: 'date-time' },
          endTime: { type: 'string', format: 'date-time' }
        }
      },
      response: {
        201: {
          type: 'object',
          properties: {
            success: { type: 'boolean' },
            data: { type: 'object' },
            message: { type: 'string' }
          }
        }
      }
    }
  }
]

// Rotas adicionais que podem ser úteis
export const additionalShiftsRoutes = [
  {
    path: `${SHIFTS_BASE_URL}/templates`,
    method: 'GET',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { unidadeId } = request.query
      
      try {
        let templates = store.templates
        
        if (unidadeId) {
          templates = templates.filter(t => t.unidadeId === unidadeId)
        }
        
        return reply.status(200).send({
          success: true,
          data: templates,
          meta: {
            total: templates.length,
            unidadeId: unidadeId || null
          }
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/:shiftId`,
    method: 'GET',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { shiftId } = request.params
      
      try {
        const shift = store.shifts.find(s => s.id === shiftId)
        
        if (!shift) {
          return reply.status(404).send({
            success: false,
            message: 'Turno não encontrado'
          })
        }
        
        const breaks = store.breaks.filter(b => b.shiftId === shiftId)
        
        return reply.status(200).send({
          success: true,
          data: {
            ...shift,
            breaks
          }
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/:shiftId`,
    method: 'PUT',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { shiftId } = request.params
      const updateData = request.body
      
      try {
        const shiftIndex = store.shifts.findIndex(s => s.id === shiftId)
        
        if (shiftIndex === -1) {
          return reply.status(404).send({
            success: false,
            message: 'Turno não encontrado'
          })
        }
        
        store.shifts[shiftIndex] = {
          ...store.shifts[shiftIndex],
          ...updateData,
          updatedAt: new Date().toISOString()
        }
        
        store.calcularEstatisticas()
        store.atualizarDashboard()
        
        return reply.status(200).send({
          success: true,
          data: store.shifts[shiftIndex],
          message: 'Turno atualizado com sucesso'
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/:shiftId`,
    method: 'DELETE',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { shiftId } = request.params
      
      try {
        const shiftIndex = store.shifts.findIndex(s => s.id === shiftId)
        
        if (shiftIndex === -1) {
          return reply.status(404).send({
            success: false,
            message: 'Turno não encontrado'
          })
        }
        
        // Remove o shift
        store.shifts.splice(shiftIndex, 1)
        
        // Remove os breaks associados
        store.breaks = store.breaks.filter(b => b.shiftId !== shiftId)
        
        store.calcularEstatisticas()
        store.atualizarDashboard()
        
        return reply.status(200).send({
          success: true,
          message: 'Turno excluído com sucesso'
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/:shiftId/breaks/:breakId`,
    method: 'DELETE',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { shiftId, breakId } = request.params
      
      try {
        const breakIndex = store.breaks.findIndex(b => b.id === breakId && b.shiftId === shiftId)
        
        if (breakIndex === -1) {
          return reply.status(404).send({
            success: false,
            message: 'Intervalo não encontrado'
          })
        }
        
        store.breaks.splice(breakIndex, 1)
        store.calcularEstatisticas()
        
        return reply.status(200).send({
          success: true,
          message: 'Intervalo removido com sucesso'
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  },
  
  {
    path: `${SHIFTS_BASE_URL}/stats/dashboard`,
    method: 'GET',
    handler: async (request, reply) => {
      const store = useShiftsStore()
      const { unidadeId, periodo } = request.query
      
      try {
        if (unidadeId) {
          await store.carregarShifts(unidadeId)
        }
        
        return reply.status(200).send({
          success: true,
          data: {
            dashboard: store.dashboard,
            estatisticas: store.estatisticas,
            alertas: gerarAlertas(store),
            proximosEventos: gerarProximosEventos(store)
          }
        })
      } catch (error) {
        return reply.status(500).send({
          success: false,
          error: error.message
        })
      }
    }
  }
]

// Funções auxiliares para o dashboard
function gerarAlertas(store) {
  const alertas = []
  const hoje = new Date()
  const amanha = new Date(hoje)
  amanha.setDate(hoje.getDate() + 1)
  
  // Escalas sem funcionário
  const pendentes = store.escalasPendentes
  if (pendentes.length > 0) {
    alertas.push({
      tipo: 'warning',
      titulo: 'Escalas pendentes',
      mensagem: `${pendentes.length} escala(s) sem funcionário atribuído`,
      prioridade: 'media'
    })
  }
  
  // Turnos não preenchidos para amanhã
  const turnosAmanha = store.shifts.filter(s => {
    const dataShift = new Date(s.data || s.startDate).toDateString()
    return dataShift === amanha.toDateString() && !s.userId
  })
  
  if (turnosAmanha.length > 0) {
    alertas.push({
      tipo: 'error',
      titulo: 'Turnos críticos',
      mensagem: `${turnosAmanha.length} turno(s) para amanhã sem funcionário`,
      prioridade: 'alta'
    })
  }
  
  return alertas
}

function gerarProximosEventos(store) {
  const eventos = []
  const hoje = new Date()
  
  // Próximas férias
  store.shifts
    .filter(s => s.type === 8 && new Date(s.data || s.startDate) >= hoje)
    .slice(0, 5)
    .forEach(shift => {
      eventos.push({
        tipo: 'ferias',
        data: shift.data || shift.startDate,
        funcionario: shift.user?.nome || 'Não atribuído',
        descricao: 'Férias'
      })
    })
  
  return eventos.sort((a, b) => new Date(a.data) - new Date(b.data))
}

// Exportar todas as rotas combinadas
export const shiftsRoutesCombined = [
  ...shiftsRoutes,
  ...additionalShiftsRoutes
]

export default shiftsRoutesCombined