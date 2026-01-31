// faturamentos-parciais.store.js
import { defineStore } from 'pinia';
import { FaturamentosParciaisService } from './faturamentosParciais.service';

// Enum de métodos de pagamento (baseado no C#)
export const MetodoPagamentoEnum = {
  Dinheiro: 1,
  Pix: 2,
  CartaoCredito: 3,
  CartaoDebito: 4,
  ValeRefeicao: 5,
  Outros: 99,
  
  // Helper para obter nome do método
  getNome: (valor) => {
    const entries = Object.entries(MetodoPagamentoEnum).find(([key, value]) => value === valor);
    return entries ? entries[0] : 'Desconhecido';
  },
  
  // Helper para obter todos os métodos
  getAll: () => [
    { id: 1, nome: 'Dinheiro' },
    { id: 2, nome: 'Pix' },
    { id: 3, nome: 'Cartão de Crédito' },
    { id: 4, nome: 'Cartão de Débito' },
    { id: 5, nome: 'Vale Refeição' },
    { id: 99, nome: 'Outros' },
  ],
  
  // Métodos que são considerados "dinheiro" para cálculo de caixa
  isDinheiroOuPix: (metodoId) => [1, 2].includes(metodoId),
  
  // Métodos eletrônicos
  isEletronico: (metodoId) => [2, 3, 4, 5].includes(metodoId),
};

export const useFaturamentosParciaisStore = defineStore('faturamentosParciais', {
  state: () => ({
    // Lançamentos do dia atual
    lancamentos: [],
    lancamentoAtual: null,
    
    // Histórico e filtros
    historicoLancamentos: [],
    filtros: {
      unidadeId: null,
      dataInicio: null,
      dataFim: null,
      metodoPagamento: null,
      origem: '',
      ativos: true,
      inativos: false,
      busca: '',
    },
    
    // Métodos de pagamento disponíveis
    metodosPagamento: MetodoPagamentoEnum.getAll(),
    
    // Origens comuns
    origensComuns: [
      'Venda Balcão',
      'Delivery',
      'Telefone',
      'App',
      'Site',
      'Reserva',
      'Evento',
      'Outro'
    ],
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Estatísticas
    estatisticas: {
      totalDia: 0,
      totalDinheiroPix: 0,
      totalCartoes: 0,
      totalOutros: 0,
      quantidadeLancamentos: 0,
      valorMedio: 0,
      ultimoLancamento: null,
    },
    
    // Carrinho temporário (para múltiplos lançamentos rápidos)
    carrinho: [],
    totalCarrinho: 0,
  }),

  getters: {
    // Lançamentos filtrados
    lancamentosFiltrados: (state) => {
      let lancamentos = state.lancamentos;
      
      // Filtro por status
      if (!state.filtros.ativos && !state.filtros.inativos) {
        lancamentos = [];
      } else if (!state.filtros.ativos) {
        lancamentos = lancamentos.filter(l => !l.isAtivo);
      } else if (!state.filtros.inativos) {
        lancamentos = lancamentos.filter(l => l.isAtivo);
      }
      
      // Filtro por método de pagamento
      if (state.filtros.metodoPagamento) {
        lancamentos = lancamentos.filter(l => 
          l.metodoPagamentoId === state.filtros.metodoPagamento
        );
      }
      
      // Filtro por origem
      if (state.filtros.origem) {
        lancamentos = lancamentos.filter(l => 
          l.origem?.toLowerCase().includes(state.filtros.origem.toLowerCase())
        );
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        lancamentos = lancamentos.filter(l => 
          l.id?.toLowerCase().includes(busca) ||
          l.origem?.toLowerCase().includes(busca) ||
          l.metodoPagamentoId?.toString().includes(busca)
        );
      }
      
      return lancamentos.sort((a, b) => 
        new Date(b.horaInicio || b.createdAt) - new Date(a.horaInicio || a.createdAt)
      );
    },
    
    // Total por método de pagamento
    totalPorMetodoPagamento: (state) => {
      const totais = {};
      
      state.lancamentos.forEach(lancamento => {
        if (lancamento.isAtivo) {
          const metodo = MetodoPagamentoEnum.getNome(lancamento.metodoPagamentoId);
          if (!totais[metodo]) {
            totais[metodo] = { valor: 0, quantidade: 0 };
          }
          totais[metodo].valor += lancamento.valor || 0;
          totais[metodo].quantidade += 1;
        }
      });
      
      return totais;
    },
    
    // Total por hora do dia
    totalPorHora: (state) => {
      const totaisPorHora = {};
      
      state.lancamentos.forEach(lancamento => {
        if (lancamento.isAtivo && lancamento.horaInicio) {
          const hora = new Date(lancamento.horaInicio).getHours();
          const horaFormatada = `${hora.toString().padStart(2, '0')}:00`;
          
          if (!totaisPorHora[horaFormatada]) {
            totaisPorHora[horaFormatada] = { valor: 0, quantidade: 0 };
          }
          totaisPorHora[horaFormatada].valor += lancamento.valor || 0;
          totaisPorHora[horaFormatada].quantidade += 1;
        }
      });
      
      // Ordena por hora
      return Object.entries(totaisPorHora)
        .sort(([horaA], [horaB]) => horaA.localeCompare(horaB))
        .reduce((obj, [key, value]) => {
          obj[key] = value;
          return obj;
        }, {});
    },
    
    // Lançamentos de dinheiro e Pix (caixa físico)
    lancamentosCaixaFisico: (state) => {
      return state.lancamentos.filter(l => 
        l.isAtivo && MetodoPagamentoEnum.isDinheiroOuPix(l.metodoPagamentoId)
      );
    },
    
    // Total do caixa físico (dinheiro + Pix)
    totalCaixaFisico: (state, getters) => {
      return getters.lancamentosCaixaFisico.reduce((total, l) => total + (l.valor || 0), 0);
    },
    
    // Lançamentos de cartões
    lancamentosCartoes: (state) => {
      return state.lancamentos.filter(l => 
        l.isAtivo && (l.metodoPagamentoId === 3 || l.metodoPagamentoId === 4)
      );
    },
    
    // Total de cartões
    totalCartoes: (state, getters) => {
      return getters.lancamentosCartoes.reduce((total, l) => total + (l.valor || 0), 0);
    },
    
    // Valor médio por transação
    valorMedioTransacao: (state) => {
      const lancamentosAtivos = state.lancamentos.filter(l => l.isAtivo);
      if (lancamentosAtivos.length === 0) return 0;
      
      const total = lancamentosAtivos.reduce((sum, l) => sum + (l.valor || 0), 0);
      return total / lancamentosAtivos.length;
    },
    
    // Últimas transações (para dashboard)
    ultimasTransacoes: (state) => {
      return state.lancamentos
        .filter(l => l.isAtivo)
        .sort((a, b) => new Date(b.horaInicio || b.createdAt) - new Date(a.horaInicio || a.createdAt))
        .slice(0, 10);
    },
  },

  actions: {
    // Carregar lançamentos do dia
    async carregarLancamentosDia(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await FaturamentosParciaisService.list(unidadeId);
        this.lancamentos = response.data;
        this.calcularEstatisticas();
        return this.lancamentos;
      } catch (error) {
        console.error('Erro ao carregar lançamentos:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar lançamentos';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar novo lançamento
    async criarLancamento(unidadeId, lancamentoData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Define valores padrão se não fornecidos
        const dadosCompletos = {
          horaInicio: new Date().toISOString(),
          horaFim: new Date().toISOString(),
          origem: 'Venda Balcão',
          ...lancamentoData
        };
        
        const response = await FaturamentosParciaisService.create(unidadeId, dadosCompletos);
        const novoLancamento = response.data;
        
        // Adiciona à lista local
        this.lancamentos.unshift(novoLancamento);
        
        // Adiciona ao histórico se necessário
        this.historicoLancamentos.unshift(novoLancamento);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        
        // Limpa carrinho se estava usando
        if (this.carrinho.length > 0) {
          this.carrinho = [];
          this.totalCarrinho = 0;
        }
        
        return { success: true, data: novoLancamento };
      } catch (error) {
        console.error('Erro ao criar lançamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar lançamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Atualizar lançamento existente
    async atualizarLancamento(unidadeId, faturamentoId, dadosAtualizados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FaturamentosParciaisService.update(unidadeId, faturamentoId, dadosAtualizados);
        const lancamentoAtualizado = response.data;
        
        // Atualiza na lista local
        const index = this.lancamentos.findIndex(l => l.id === faturamentoId);
        if (index !== -1) {
          this.lancamentos[index] = { ...this.lancamentos[index], ...lancamentoAtualizado };
        }
        
        // Atualiza no histórico
        const historicoIndex = this.historicoLancamentos.findIndex(l => l.id === faturamentoId);
        if (historicoIndex !== -1) {
          this.historicoLancamentos[historicoIndex] = { 
            ...this.historicoLancamentos[historicoIndex], 
            ...lancamentoAtualizado 
          };
        }
        
        // Se for o lançamento atual, atualiza também
        if (this.lancamentoAtual?.id === faturamentoId) {
          this.lancamentoAtual = { ...this.lancamentoAtual, ...lancamentoAtualizado };
        }
        
        this.calcularEstatisticas();
        
        return { success: true, data: lancamentoAtualizado };
      } catch (error) {
        console.error('Erro ao atualizar lançamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao atualizar lançamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Remover lançamento
    async removerLancamento(unidadeId, faturamentoId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        await FaturamentosParciaisService.remove(unidadeId, faturamentoId);
        
        // Remove da lista local
        this.lancamentos = this.lancamentos.filter(l => l.id !== faturamentoId);
        
        // Remove do histórico
        this.historicoLancamentos = this.historicoLancamentos.filter(l => l.id !== faturamentoId);
        
        // Limpa lançamento atual se for o mesmo
        if (this.lancamentoAtual?.id === faturamentoId) {
          this.lancamentoAtual = null;
        }
        
        this.calcularEstatisticas();
        
        return { success: true };
      } catch (error) {
        console.error('Erro ao remover lançamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao remover lançamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Desativar lançamento (soft delete)
    async desativarLancamento(unidadeId, faturamentoId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        await FaturamentosParciaisService.desativar(unidadeId, faturamentoId);
        
        // Atualiza status na lista local
        const index = this.lancamentos.findIndex(l => l.id === faturamentoId);
        if (index !== -1) {
          this.lancamentos[index] = { ...this.lancamentos[index], isAtivo: false };
        }
        
        this.calcularEstatisticas();
        
        return { success: true };
      } catch (error) {
        console.error('Erro ao desativar lançamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao desativar lançamento';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Buscar lançamento por ID
    async buscarLancamentoPorId(unidadeId, faturamentoId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await FaturamentosParciaisService.getById(unidadeId, faturamentoId);
        this.lancamentoAtual = response.data;
        return this.lancamentoAtual;
      } catch (error) {
        console.error('Erro ao buscar lançamento:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar lançamento';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const lancamentosAtivos = this.lancamentos.filter(l => l.isAtivo);
      
      if (lancamentosAtivos.length === 0) {
        this.estatisticas = {
          totalDia: 0,
          totalDinheiroPix: 0,
          totalCartoes: 0,
          totalOutros: 0,
          quantidadeLancamentos: 0,
          valorMedio: 0,
          ultimoLancamento: null,
        };
        return;
      }
      
      const totaisPorMetodo = {
        dinheiroPix: 0,
        cartoes: 0,
        outros: 0,
      };
      
      lancamentosAtivos.forEach(lancamento => {
        if (MetodoPagamentoEnum.isDinheiroOuPix(lancamento.metodoPagamentoId)) {
          totaisPorMetodo.dinheiroPix += lancamento.valor || 0;
        } else if (lancamento.metodoPagamentoId === 3 || lancamento.metodoPagamentoId === 4) {
          totaisPorMetodo.cartoes += lancamento.valor || 0;
        } else {
          totaisPorMetodo.outros += lancamento.valor || 0;
        }
      });
      
      const totalDia = lancamentosAtivos.reduce((total, l) => total + (l.valor || 0), 0);
      const ultimoLancamento = [...lancamentosAtivos]
        .sort((a, b) => new Date(b.horaInicio || b.createdAt) - new Date(a.horaInicio || a.createdAt))[0];
      
      this.estatisticas = {
        totalDia,
        totalDinheiroPix: totaisPorMetodo.dinheiroPix,
        totalCartoes: totaisPorMetodo.cartoes,
        totalOutros: totaisPorMetodo.outros,
        quantidadeLancamentos: lancamentosAtivos.length,
        valorMedio: totalDia / lancamentosAtivos.length,
        ultimoLancamento,
      };
    },

    // Funções para carrinho de lançamentos rápidos
    adicionarAoCarrinho(lancamento) {
      this.carrinho.push({
        ...lancamento,
        idTemporario: Date.now() + Math.random(),
      });
      this.calcularTotalCarrinho();
    },

    removerDoCarrinho(idTemporario) {
      this.carrinho = this.carrinho.filter(item => item.idTemporario !== idTemporario);
      this.calcularTotalCarrinho();
    },

    calcularTotalCarrinho() {
      this.totalCarrinho = this.carrinho.reduce((total, item) => total + (item.valor || 0), 0);
    },

    limparCarrinho() {
      this.carrinho = [];
      this.totalCarrinho = 0;
    },

    async finalizarCarrinho(unidadeId) {
      if (this.carrinho.length === 0) {
        return { success: false, error: 'Carrinho vazio' };
      }
      
      this.isLoading = true;
      const resultados = [];
      
      try {
        // Processa cada item do carrinho
        for (const item of this.carrinho) {
          const { idTemporario, ...dadosLancamento } = item;
          const resultado = await this.criarLancamento(unidadeId, dadosLancamento);
          resultados.push(resultado);
        }
        
        // Verifica se todos foram sucesso
        const todosSucesso = resultados.every(r => r.success);
        
        if (todosSucesso) {
          this.limparCarrinho();
          return { 
            success: true, 
            message: `${this.carrinho.length} lançamentos realizados com sucesso` 
          };
        } else {
          return { 
            success: false, 
            error: 'Alguns lançamentos falharam',
            detalhes: resultados.filter(r => !r.success)
          };
        }
      } catch (error) {
        console.error('Erro ao finalizar carrinho:', error);
        return { success: false, error: error.message };
      } finally {
        this.isLoading = false;
      }
    },

    // Métodos utilitários
    formatarMetodoPagamento(metodoId) {
      return MetodoPagamentoEnum.getNome(metodoId);
    },

    formatarValor(valor) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(valor || 0);
    },

    formatarHora(dataHora) {
      if (!dataHora) return '';
      const data = new Date(dataHora);
      return data.toLocaleTimeString('pt-BR', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
      });
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
        metodoPagamento: null,
        origem: '',
        ativos: true,
        inativos: false,
        busca: '',
      };
    },

    // Resetar store
    resetarStore() {
      this.lancamentos = [];
      this.lancamentoAtual = null;
      this.historicoLancamentos = [];
      this.carrinho = [];
      this.totalCarrinho = 0;
      this.error = null;
      this.filtros = {
        unidadeId: null,
        dataInicio: null,
        dataFim: null,
        metodoPagamento: null,
        origem: '',
        ativos: true,
        inativos: false,
        busca: '',
      };
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'faturamentos-parciais-store',
    paths: ['filtros', 'origensComuns', 'carrinho'],
  },
});