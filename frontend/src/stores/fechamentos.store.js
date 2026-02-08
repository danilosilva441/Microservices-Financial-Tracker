// fechamentos.store.js
import { defineStore } from 'pinia';
import { FechamentosService } from '../services/fechamentos.service';

export const useFechamentosStore = defineStore('fechamentos', {
  state: () => ({
    fechamentos: [], // Lista de fechamentos da unidade atual
    fechamentoAtual: null, // Fechamento sendo visualizado/editado
    fechamentosPendentes: [], // Fechamentos aguardando conferência (para supervisores)
    fechamentosPorData: [], // Resultados da busca por data
    
    isLoading: false,
    error: null,
    
    filtros: {
      unidadeId: null,
      dataInicio: null,
      dataFim: null,
      status: '', // 'aberto', 'fechado', 'conferido', 'pendente'
      busca: '',
    },
    
    estatisticas: {
      totalFaturamento: 0,
      mediaDiaria: 0,
      diasUteis: 0,
      maiorFaturamento: 0,
      menorFaturamento: 0,
      totalDiferencas: 0,
    },
    
    dashboard: {
      caixasAbertosHoje: 0,
      caixasPendentesConferencia: 0,
      faturamentoHoje: 0,
      faturamentoMes: 0,
    },
  }),

  getters: {
    // Fechamentos filtrados por status e busca
    fechamentosFiltrados: (state) => {
      let fechamentos = state.fechamentos;
      
      // Filtro por status
      if (state.filtros.status) {
        fechamentos = fechamentos.filter(f => {
          if (state.filtros.status === 'aberto') return f.statusCaixa === 'Aberto';
          if (state.filtros.status === 'fechado') return f.statusCaixa === 'Fechado';
          if (state.filtros.status === 'conferido') return f.statusCaixa === 'Conferido';
          if (state.filtros.status === 'pendente') return f.status === 'Pendente';
          return true;
        });
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        fechamentos = fechamentos.filter(f => 
          f.observacoes?.toLowerCase().includes(busca) ||
          f.id?.toLowerCase().includes(busca) ||
          f.fechadoPorUserId?.toLowerCase().includes(busca)
        );
      }
      
      return fechamentos.sort((a, b) => new Date(b.data) - new Date(a.data));
    },
    
    // Fechamentos abertos (do dia atual)
    fechamentosAbertosHoje: (state) => {
      const hoje = new Date().toISOString().split('T')[0];
      return state.fechamentos.filter(f => 
        f.data === hoje && 
        (f.statusCaixa === 'Aberto' || f.status === 'Aberto')
      );
    },
    
    // Fechamentos fechados aguardando conferência
    fechamentosFechadosPendentes: (state) => {
      return state.fechamentos.filter(f => 
        f.statusCaixa === 'Fechado' && 
        (!f.conferidoPorUserId || f.status === 'Pendente')
      );
    },
    
    // Fechamentos do mês atual
    fechamentosEsteMes: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      return state.fechamentos.filter(f => {
        const dataFechamento = new Date(f.data);
        return dataFechamento.getMonth() === mesAtual && 
               dataFechamento.getFullYear() === anoAtual;
      });
    },
    
    // Faturamento total do mês
    faturamentoTotalMes: (state, getters) => {
      return getters.fechamentosEsteMes.reduce((total, f) => {
        return total + (f.valorTotal || 0);
      }, 0);
    },
    
    // Diferenças de caixa (quando valorConferido != valorTotal)
    fechamentosComDiferenca: (state) => {
      return state.fechamentos.filter(f => {
        return f.diferenca && f.diferenca !== 0;
      });
    },
    
    // Fechamentos por status para gráficos
    distribuicaoPorStatus: (state) => {
      const distribuicao = {
        abertos: 0,
        fechados: 0,
        conferidos: 0,
        pendentes: 0,
      };
      
      state.fechamentos.forEach(f => {
        if (f.statusCaixa === 'Aberto') distribuicao.abertos++;
        else if (f.statusCaixa === 'Fechado') distribuicao.fechados++;
        else if (f.statusCaixa === 'Conferido') distribuicao.conferidos++;
        else if (f.status === 'Pendente') distribuicao.pendentes++;
      });
      
      return distribuicao;
    },
    
    // Fechamento de hoje (se existir)
    fechamentoHoje: (state) => {
      const hoje = new Date().toISOString().split('T')[0];
      return state.fechamentos.find(f => f.data === hoje);
    },
  },

  actions: {
    // Abrir um novo dia (iniciar fechamento)
    async abrirDia(unidadeId, dados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.abrirDia(unidadeId, dados);
        const novoFechamento = response.data;
        
        // Adiciona à lista local
        this.fechamentos.unshift(novoFechamento);
        
        // Atualiza dashboard
        this.atualizarDashboard();
        
        return { success: true, data: novoFechamento };
      } catch (error) {
        console.error('Erro ao abrir dia:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao abrir dia';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Listar fechamentos de uma unidade
    async carregarFechamentos(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await FechamentosService.listar(unidadeId);
        this.fechamentos = response.data;
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return this.fechamentos;
      } catch (error) {
        console.error('Erro ao carregar fechamentos:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar fechamentos';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Buscar fechamento por ID
    async buscarFechamentoPorId(unidadeId, id) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.getById(unidadeId, id);
        this.fechamentoAtual = response.data;
        return this.fechamentoAtual;
      } catch (error) {
        console.error('Erro ao buscar fechamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar fechamento';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Atualizar fechamento
    async atualizarFechamento(unidadeId, id, dados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.atualizar(unidadeId, id, dados);
        const fechamentoAtualizado = response.data;
        
        // Atualiza na lista local
        const index = this.fechamentos.findIndex(f => f.id === id);
        if (index !== -1) {
          this.fechamentos[index] = { ...this.fechamentos[index], ...fechamentoAtualizado };
        }
        
        // Atualiza fechamento atual se for o mesmo
        if (this.fechamentoAtual?.id === id) {
          this.fechamentoAtual = { ...this.fechamentoAtual, ...fechamentoAtualizado };
        }
        
        this.calcularEstatisticas();
        
        return { success: true, data: fechamentoAtualizado };
      } catch (error) {
        console.error('Erro ao atualizar fechamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao atualizar fechamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Fechar caixa
    async fecharCaixa(unidadeId, id, dados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.fechar(unidadeId, id, dados);
        const fechamentoFechado = response.data;
        
        // Atualiza na lista local
        const index = this.fechamentos.findIndex(f => f.id === id);
        if (index !== -1) {
          this.fechamentos[index] = { 
            ...this.fechamentos[index], 
            ...fechamentoFechado,
            statusCaixa: 'Fechado'
          };
        }
        
        // Atualiza dashboard
        this.atualizarDashboard();
        this.calcularEstatisticas();
        
        return { success: true, data: fechamentoFechado };
      } catch (error) {
        console.error('Erro ao fechar caixa:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao fechar caixa';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Conferir fechamento (aprovado/rejeitado)
    async conferirFechamento(unidadeId, id, dados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.conferir(unidadeId, id, dados);
        const fechamentoConferido = response.data;
        
        // Atualiza na lista local
        const index = this.fechamentos.findIndex(f => f.id === id);
        if (index !== -1) {
          this.fechamentos[index] = { 
            ...this.fechamentos[index], 
            ...fechamentoConferido,
            statusCaixa: dados.aprovado ? 'Conferido' : 'Fechado'
          };
        }
        
        // Remove dos pendentes se existir
        this.fechamentosPendentes = this.fechamentosPendentes.filter(f => f.id !== id);
        
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: fechamentoConferido };
      } catch (error) {
        console.error('Erro ao conferir fechamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao conferir fechamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Reabrir fechamento
    async reabrirFechamento(unidadeId, id, motivo) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.reabrir(unidadeId, id, { motivo });
        const fechamentoReaberto = response.data;
        
        // Atualiza na lista local
        const index = this.fechamentos.findIndex(f => f.id === id);
        if (index !== -1) {
          this.fechamentos[index] = { 
            ...this.fechamentos[index], 
            ...fechamentoReaberto,
            statusCaixa: 'Aberto'
          };
        }
        
        this.atualizarDashboard();
        
        return { success: true, data: fechamentoReaberto };
      } catch (error) {
        console.error('Erro ao reabrir fechamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao reabrir fechamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar fechamentos pendentes (para supervisores)
    async carregarFechamentosPendentes() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FechamentosService.pendentes();
        this.fechamentosPendentes = response.data;
        this.atualizarDashboard();
        
        return this.fechamentosPendentes;
      } catch (error) {
        console.error('Erro ao carregar fechamentos pendentes:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar fechamentos pendentes';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Buscar fechamentos por intervalo de datas
    async buscarPorData(unidadeId, dataInicio, dataFim) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const params = {};
        if (dataInicio) params.dataInicio = dataInicio;
        if (dataFim) params.dataFim = dataFim;
        
        const response = await FechamentosService.porData(unidadeId, params);
        this.fechamentosPorData = response.data;
        
        return this.fechamentosPorData;
      } catch (error) {
        console.error('Erro ao buscar fechamentos por data:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar fechamentos por data';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const fechamentosValidos = this.fechamentos.filter(f => f.valorTotal && f.valorTotal > 0);
      
      if (fechamentosValidos.length === 0) {
        this.estatisticas = {
          totalFaturamento: 0,
          mediaDiaria: 0,
          diasUteis: 0,
          maiorFaturamento: 0,
          menorFaturamento: 0,
          totalDiferencas: 0,
        };
        return;
      }
      
      const valores = fechamentosValidos.map(f => f.valorTotal);
      const diferencas = this.fechamentosComDiferenca.reduce((total, f) => total + Math.abs(f.diferenca), 0);
      
      this.estatisticas = {
        totalFaturamento: valores.reduce((a, b) => a + b, 0),
        mediaDiaria: valores.reduce((a, b) => a + b, 0) / valores.length,
        diasUteis: fechamentosValidos.length,
        maiorFaturamento: Math.max(...valores),
        menorFaturamento: Math.min(...valores),
        totalDiferencas: diferencas,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      const hoje = new Date().toISOString().split('T')[0];
      const fechamentosHoje = this.fechamentos.filter(f => f.data === hoje);
      const fechamentosEsteMes = this.fechamentosEsteMes;
      
      this.dashboard = {
        caixasAbertosHoje: this.fechamentosAbertosHoje.length,
        caixasPendentesConferencia: this.fechamentosFechadosPendentes.length,
        faturamentoHoje: fechamentosHoje.reduce((total, f) => total + (f.valorTotal || 0), 0),
        faturamentoMes: fechamentosEsteMes.reduce((total, f) => total + (f.valorTotal || 0), 0),
      };
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        unidadeId: this.filtros.unidadeId,
        dataInicio: null,
        dataFim: null,
        status: '',
        busca: '',
      };
    },

    // Resetar store
    resetarStore() {
      this.fechamentos = [];
      this.fechamentoAtual = null;
      this.fechamentosPendentes = [];
      this.fechamentosPorData = [];
      this.error = null;
      this.filtros = {
        unidadeId: null,
        dataInicio: null,
        dataFim: null,
        status: '',
        busca: '',
      };
    },

    // Verificar se já existe fechamento para hoje
    verificarFechamentoHoje(unidadeId) {
      return new Promise(async (resolve) => {
        try {
          await this.carregarFechamentos(unidadeId);
          const hoje = new Date().toISOString().split('T')[0];
          const fechamentoHoje = this.fechamentos.find(f => f.data === hoje);
          
          resolve({
            existe: !!fechamentoHoje,
            fechamento: fechamentoHoje,
            status: fechamentoHoje?.statusCaixa
          });
        } catch (error) {
          console.error('Erro ao verificar fechamento hoje:', error);
          resolve({ existe: false, fechamento: null, status: null });
        }
      });
    },

    // Gerar relatório de caixa
    gerarRelatorioCaixa(unidadeId, dataInicio, dataFim) {
      const fechamentosPeriodo = this.fechamentos.filter(f => {
        const dataFechamento = new Date(f.data);
        const inicio = new Date(dataInicio);
        const fim = new Date(dataFim);
        
        return dataFechamento >= inicio && dataFechamento <= fim;
      });
      
      const relatorio = {
        periodo: { dataInicio, dataFim },
        totalDias: fechamentosPeriodo.length,
        totalFaturamento: fechamentosPeriodo.reduce((total, f) => total + (f.valorTotal || 0), 0),
        mediaDiaria: fechamentosPeriodo.reduce((total, f) => total + (f.valorTotal || 0), 0) / fechamentosPeriodo.length,
        totalDiferencas: fechamentosPeriodo.reduce((total, f) => total + Math.abs(f.diferenca || 0), 0),
        fechamentos: fechamentosPeriodo,
        resumoPorStatus: this.distribuicaoPorStatus,
      };
      
      return relatorio;
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'fechamentos-store',
    paths: ['filtros', 'dashboard'],
  },
});