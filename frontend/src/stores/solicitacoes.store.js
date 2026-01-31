// solicitacoes.store.js
import { defineStore } from 'pinia';
import { SolicitacoesService } from './solicitacoes.service';

// Tipos de solicitação
export const TipoSolicitacaoEnum = {
  AJUSTE_VALOR: 'AJUSTE_VALOR',
  AJUSTE_METODO_PAGAMENTO: 'AJUSTE_METODO_PAGAMENTO',
  AJUSTE_ORIGEM: 'AJUSTE_ORIGEM',
  AJUSTE_HORARIO: 'AJUSTE_HORARIO',
  EXCLUSAO_LANCAMENTO: 'EXCLUSAO_LANCAMENTO',
  OUTROS: 'OUTROS',
  
  getNome: (tipo) => {
    const nomes = {
      AJUSTE_VALOR: 'Ajuste de Valor',
      AJUSTE_METODO_PAGAMENTO: 'Ajuste de Método de Pagamento',
      AJUSTE_ORIGEM: 'Ajuste de Origem',
      AJUSTE_HORARIO: 'Ajuste de Horário',
      EXCLUSAO_LANCAMENTO: 'Exclusão de Lançamento',
      OUTROS: 'Outros Ajustes',
    };
    return nomes[tipo] || 'Tipo Desconhecido';
  },
  
  getAll: () => [
    { id: 'AJUSTE_VALOR', nome: 'Ajuste de Valor' },
    { id: 'AJUSTE_METODO_PAGAMENTO', nome: 'Ajuste de Método de Pagamento' },
    { id: 'AJUSTE_ORIGEM', nome: 'Ajuste de Origem' },
    { id: 'AJUSTE_HORARIO', nome: 'Ajuste de Horário' },
    { id: 'EXCLUSAO_LANCAMENTO', nome: 'Exclusão de Lançamento' },
    { id: 'OUTROS', nome: 'Outros Ajustes' },
  ],
};

// Status de solicitação
export const StatusSolicitacaoEnum = {
  PENDENTE: 'PENDENTE',
  APROVADA: 'APROVADA',
  REJEITADA: 'REJEITADA',
  CANCELADA: 'CANCELADA',
  PROCESSANDO: 'PROCESSANDO',
  
  getNome: (status) => {
    const nomes = {
      PENDENTE: 'Pendente',
      APROVADA: 'Aprovada',
      REJEITADA: 'Rejeitada',
      CANCELADA: 'Cancelada',
      PROCESSANDO: 'Processando',
    };
    return nomes[status] || 'Status Desconhecido';
  },
  
  getCor: (status) => {
    const cores = {
      PENDENTE: '#ff9800', // Laranja
      APROVADA: '#4caf50', // Verde
      REJEITADA: '#f44336', // Vermelho
      CANCELADA: '#9e9e9e', // Cinza
      PROCESSANDO: '#2196f3', // Azul
    };
    return cores[status] || '#757575';
  },
  
  getAll: () => [
    { id: 'PENDENTE', nome: 'Pendente', cor: '#ff9800' },
    { id: 'APROVADA', nome: 'Aprovada', cor: '#4caf50' },
    { id: 'REJEITADA', nome: 'Rejeitada', cor: '#f44336' },
    { id: 'CANCELADA', nome: 'Cancelada', cor: '#9e9e9e' },
    { id: 'PROCESSANDO', nome: 'Processando', cor: '#2196f3' },
  ],
};

// Ações de revisão
export const AcaoRevisaoEnum = {
  APROVAR: 'APROVAR',
  REJEITAR: 'REJEITAR',
  CANCELAR: 'CANCELAR',
};

export const useSolicitacoesStore = defineStore('solicitacoes', {
  state: () => ({
    // Listas de solicitações
    solicitacoes: [], // Todas as solicitações
    minhasSolicitacoes: [], // Solicitações do usuário logado
    solicitacaoAtual: null, // Solicitação sendo visualizada/editada
    
    // Filtros
    filtros: {
      status: '',
      tipo: '',
      dataInicio: null,
      dataFim: null,
      busca: '',
      ordenacao: 'data_desc', // data_desc, data_asc
      apenasMinhas: false,
    },
    
    // Estatísticas
    estatisticas: {
      totalSolicitacoes: 0,
      pendentes: 0,
      aprovadas: 0,
      rejeitadas: 0,
      tempoMedioResolucao: 0,
      taxaAprovacao: 0,
    },
    
    // Dashboard
    dashboard: {
      solicitacoesPendentes: 0,
      solicitacoesHoje: 0,
      solicitacoesSemana: 0,
      solicitacoesPorTipo: {},
    },
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Tipos e status disponíveis
    tiposSolicitacao: TipoSolicitacaoEnum.getAll(),
    statusSolicitacao: StatusSolicitacaoEnum.getAll(),
    
    // Configurações
    config: {
      notificacoes: true,
      emailAlerta: false,
      limiteDiarioSolicitacoes: 5,
    },
  }),

  getters: {
    // Solicitações filtradas
    solicitacoesFiltradas: (state) => {
      let solicitacoes = state.filtros.apenasMinhas ? state.minhasSolicitacoes : state.solicitacoes;
      
      // Filtro por status
      if (state.filtros.status) {
        solicitacoes = solicitacoes.filter(s => s.status === state.filtros.status);
      }
      
      // Filtro por tipo
      if (state.filtros.tipo) {
        solicitacoes = solicitacoes.filter(s => s.tipo === state.filtros.tipo);
      }
      
      // Filtro por período
      if (state.filtros.dataInicio || state.filtros.dataFim) {
        solicitacoes = solicitacoes.filter(s => {
          const dataSolicitacao = new Date(s.dataSolicitacao || s.createdAt);
          const inicio = state.filtros.dataInicio ? new Date(state.filtros.dataInicio) : null;
          const fim = state.filtros.dataFim ? new Date(state.filtros.dataFim) : null;
          
          if (inicio && dataSolicitacao < inicio) return false;
          if (fim && dataSolicitacao > fim) return false;
          return true;
        });
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        solicitacoes = solicitacoes.filter(s => 
          s.motivo?.toLowerCase().includes(busca) ||
          s.id?.toLowerCase().includes(busca) ||
          s.faturamentoParcialId?.toLowerCase().includes(busca)
        );
      }
      
      // Ordenação
      switch (state.filtros.ordenacao) {
        case 'data_asc':
          solicitacoes.sort((a, b) => new Date(a.dataSolicitacao || a.createdAt) - new Date(b.dataSolicitacao || b.createdAt));
          break;
        case 'data_desc':
        default:
          solicitacoes.sort((a, b) => new Date(b.dataSolicitacao || b.createdAt) - new Date(a.dataSolicitacao || a.createdAt));
          break;
      }
      
      return solicitacoes;
    },
    
    // Solicitações pendentes
    solicitacoesPendentes: (state) => {
      return state.solicitacoes.filter(s => s.status === 'PENDENTE');
    },
    
    // Solicitações aprovadas
    solicitacoesAprovadas: (state) => {
      return state.solicitacoes.filter(s => s.status === 'APROVADA');
    },
    
    // Solicitações rejeitadas
    solicitacoesRejeitadas: (state) => {
      return state.solicitacoes.filter(s => s.status === 'REJEITADA');
    },
    
    // Solicitações de hoje
    solicitacoesHoje: (state) => {
      const hoje = new Date().toISOString().split('T')[0];
      return state.solicitacoes.filter(s => {
        const dataSolicitacao = new Date(s.dataSolicitacao || s.createdAt).toISOString().split('T')[0];
        return dataSolicitacao === hoje;
      });
    },
    
    // Solicitações da semana
    solicitacoesSemana: (state) => {
      const hoje = new Date();
      const inicioSemana = new Date(hoje);
      inicioSemana.setDate(hoje.getDate() - 7);
      
      return state.solicitacoes.filter(s => {
        const dataSolicitacao = new Date(s.dataSolicitacao || s.createdAt);
        return dataSolicitacao >= inicioSemana && dataSolicitacao <= hoje;
      });
    },
    
    // Solicitações por tipo
    solicitacoesPorTipo: (state) => {
      const porTipo = {};
      
      state.solicitacoes.forEach(solicitacao => {
        const tipo = TipoSolicitacaoEnum.getNome(solicitacao.tipo);
        if (!porTipo[tipo]) {
          porTipo[tipo] = { total: 0, aprovadas: 0, rejeitadas: 0, pendentes: 0 };
        }
        porTipo[tipo].total++;
        
        if (solicitacao.status === 'APROVADA') porTipo[tipo].aprovadas++;
        else if (solicitacao.status === 'REJEITADA') porTipo[tipo].rejeitadas++;
        else if (solicitacao.status === 'PENDENTE') porTipo[tipo].pendentes++;
      });
      
      return porTipo;
    },
    
    // Solicitações que requerem minha atenção (para supervisores)
    solicitacoesParaRevisao: (state, getters) => {
      return getters.solicitacoesPendentes;
    },
    
    // Verifica se o usuário atingiu o limite diário
    limiteDiarioAtingido: (state) => {
      const solicitacoesHoje = state.minhasSolicitacoes.filter(s => {
        const dataSolicitacao = new Date(s.dataSolicitacao || s.createdAt);
        const hoje = new Date();
        return dataSolicitacao.toDateString() === hoje.toDateString();
      });
      
      return solicitacoesHoje.length >= state.config.limiteDiarioSolicitacoes;
    },
    
    // Tempo médio de resolução (em horas)
    tempoMedioResolucaoCalculado: (state) => {
      const solicitacoesResolvidas = state.solicitacoes.filter(s => 
        s.status === 'APROVADA' || s.status === 'REJEITADA'
      );
      
      if (solicitacoesResolvidas.length === 0) return 0;
      
      const totalHoras = solicitacoesResolvidas.reduce((total, s) => {
        if (s.dataSolicitacao && s.dataRevisao) {
          const inicio = new Date(s.dataSolicitacao);
          const fim = new Date(s.dataRevisao);
          const horas = (fim - inicio) / (1000 * 60 * 60);
          return total + horas;
        }
        return total;
      }, 0);
      
      return totalHoras / solicitacoesResolvidas.length;
    },
  },

  actions: {
    // Carregar todas as solicitações
    async carregarSolicitacoes() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await SolicitacoesService.list();
        this.solicitacoes = response.data;
        this.calcularEstatisticas();
        this.atualizarDashboard();
        return this.solicitacoes;
      } catch (error) {
        console.error('Erro ao carregar solicitações:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar solicitações';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar minhas solicitações
    async carregarMinhasSolicitacoes() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await SolicitacoesService.minhas();
        this.minhasSolicitacoes = response.data;
        return this.minhasSolicitacoes;
      } catch (error) {
        console.error('Erro ao carregar minhas solicitações:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar minhas solicitações';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar nova solicitação
    async criarSolicitacao(solicitacaoData) {
      this.isLoading = true;
      this.error = null;
      
      // Verifica limite diário
      if (this.limiteDiarioAtingido) {
        this.error = `Limite diário de ${this.config.limiteDiarioSolicitacoes} solicitações atingido`;
        return { success: false, error: this.error };
      }
      
      try {
        const response = await SolicitacoesService.create(solicitacaoData);
        const novaSolicitacao = response.data;
        
        // Adiciona às listas locais
        this.solicitacoes.unshift(novaSolicitacao);
        this.minhasSolicitacoes.unshift(novaSolicitacao);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: novaSolicitacao };
      } catch (error) {
        console.error('Erro ao criar solicitação:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar solicitação';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Buscar solicitação por ID
    async buscarSolicitacaoPorId(id) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await SolicitacoesService.getById(id);
        this.solicitacaoAtual = response.data;
        return this.solicitacaoAtual;
      } catch (error) {
        console.error('Erro ao buscar solicitação:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar solicitação';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Revisar solicitação (aprove/rejeitar)
    async revisarSolicitacao(id, acao, justificativa) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const payload = {
          acao: acao === 'APROVAR' ? 'APROVAR' : 'REJEITAR',
          justificativa: justificativa || ''
        };
        
        const response = await SolicitacoesService.revisar(id, payload);
        const solicitacaoAtualizada = response.data;
        
        // Atualiza na lista de todas as solicitações
        const index = this.solicitacoes.findIndex(s => s.id === id);
        if (index !== -1) {
          this.solicitacoes[index] = { ...this.solicitacoes[index], ...solicitacaoAtualizada };
        }
        
        // Atualiza na lista de minhas solicitações se for do usuário atual
        const minhaIndex = this.minhasSolicitacoes.findIndex(s => s.id === id);
        if (minhaIndex !== -1) {
          this.minhasSolicitacoes[minhaIndex] = { 
            ...this.minhasSolicitacoes[minhaIndex], 
            ...solicitacaoAtualizada 
          };
        }
        
        // Atualiza solicitação atual se for a mesma
        if (this.solicitacaoAtual?.id === id) {
          this.solicitacaoAtual = { ...this.solicitacaoAtual, ...solicitacaoAtualizada };
        }
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: solicitacaoAtualizada };
      } catch (error) {
        console.error('Erro ao revisar solicitação:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao revisar solicitação';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Cancelar solicitação (usuário que criou pode cancelar se ainda pendente)
    async cancelarSolicitacao(id, motivo) {
      const solicitacao = this.minhasSolicitacoes.find(s => s.id === id);
      
      if (!solicitacao) {
        this.error = 'Solicitação não encontrada ou você não tem permissão para cancelá-la';
        return { success: false, error: this.error };
      }
      
      if (solicitacao.status !== 'PENDENTE') {
        this.error = 'Somente solicitações pendentes podem ser canceladas';
        return { success: false, error: this.error };
      }
      
      return await this.revisarSolicitacao(id, 'REJEITAR', `Cancelada pelo solicitante: ${motivo}`);
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const totalSolicitacoes = this.solicitacoes.length;
      const pendentes = this.solicitacoesPendentes.length;
      const aprovadas = this.solicitacoesAprovadas.length;
      const rejeitadas = this.solicitacoesRejeitadas.length;
      const tempoMedioResolucao = this.tempoMedioResolucaoCalculado;
      const taxaAprovacao = totalSolicitacoes > 0 ? (aprovadas / totalSolicitacoes) * 100 : 0;
      
      this.estatisticas = {
        totalSolicitacoes,
        pendentes,
        aprovadas,
        rejeitadas,
        tempoMedioResolucao,
        taxaAprovacao,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      this.dashboard = {
        solicitacoesPendentes: this.solicitacoesPendentes.length,
        solicitacoesHoje: this.solicitacoesHoje.length,
        solicitacoesSemana: this.solicitacoesSemana.length,
        solicitacoesPorTipo: this.solicitacoesPorTipo,
      };
    },

    // Métodos utilitários
    formatarData(data) {
      if (!data) return '';
      const dataObj = new Date(data);
      return dataObj.toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    },

    formatarDataRelativa(data) {
      if (!data) return '';
      
      const agora = new Date();
      const dataObj = new Date(data);
      const diffMs = agora - dataObj;
      const diffMinutos = Math.floor(diffMs / (1000 * 60));
      const diffHoras = Math.floor(diffMs / (1000 * 60 * 60));
      const diffDias = Math.floor(diffMs / (1000 * 60 * 60 * 24));
      
      if (diffMinutos < 60) {
        return `há ${diffMinutos} minuto${diffMinutos !== 1 ? 's' : ''}`;
      } else if (diffHoras < 24) {
        return `há ${diffHoras} hora${diffHoras !== 1 ? 's' : ''}`;
      } else if (diffDias < 7) {
        return `há ${diffDias} dia${diffDias !== 1 ? 's' : ''}`;
      } else {
        return this.formatarData(data);
      }
    },

    getNomeTipo(tipo) {
      return TipoSolicitacaoEnum.getNome(tipo);
    },

    getNomeStatus(status) {
      return StatusSolicitacaoEnum.getNome(status);
    },

    getCorStatus(status) {
      return StatusSolicitacaoEnum.getCor(status);
    },

    // Gerar resumo da solicitação
    gerarResumoSolicitacao(solicitacao) {
      if (!solicitacao) return '';
      
      switch (solicitacao.tipo) {
        case 'AJUSTE_VALOR':
          return `Ajuste de valor: ${solicitacao.dadosAntigos || 'N/A'} → ${solicitacao.dadosNovos || 'N/A'}`;
        case 'AJUSTE_METODO_PAGAMENTO':
          return `Ajuste de método: ${solicitacao.dadosAntigos || 'N/A'} → ${solicitacao.dadosNovos || 'N/A'}`;
        case 'AJUSTE_ORIGEM':
          return `Ajuste de origem: ${solicitacao.dadosAntigos || 'N/A'} → ${solicitacao.dadosNovos || 'N/A'}`;
        case 'AJUSTE_HORARIO':
          return `Ajuste de horário: ${solicitacao.dadosAntigos || 'N/A'} → ${solicitacao.dadosNovos || 'N/A'}`;
        case 'EXCLUSAO_LANCAMENTO':
          return `Exclusão de lançamento: ${solicitacao.dadosAntigos || 'N/A'}`;
        default:
          return solicitacao.motivo || 'Solicitação de ajuste';
      }
    },

    // Criar solicitação para ajuste de faturamento parcial
    criarSolicitacaoAjusteFaturamento(faturamentoParcialId, tipo, dadosAntigos, dadosNovos, motivo) {
      return this.criarSolicitacao({
        faturamentoParcialId,
        tipo,
        motivo,
        dadosAntigos: JSON.stringify(dadosAntigos),
        dadosNovos: JSON.stringify(dadosNovos)
      });
    },

    // Verificar se pode criar solicitação para um faturamento
    podeSolicitarAjuste(faturamentoParcial) {
      if (!faturamentoParcial) return false;
      
      // Verifica se já existe solicitação pendente para este faturamento
      const solicitacaoExistente = this.solicitacoes.find(s => 
        s.faturamentoParcialId === faturamentoParcial.id && 
        s.status === 'PENDENTE'
      );
      
      return !solicitacaoExistente && !this.limiteDiarioAtingido;
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        status: '',
        tipo: '',
        dataInicio: null,
        dataFim: null,
        busca: '',
        ordenacao: 'data_desc',
        apenasMinhas: false,
      };
    },

    // Toggle apenas minhas solicitações
    toggleApenasMinhas() {
      this.filtros.apenasMinhas = !this.filtros.apenasMinhas;
    },

    // Resetar store
    resetarStore() {
      this.solicitacoes = [];
      this.minhasSolicitacoes = [];
      this.solicitacaoAtual = null;
      this.error = null;
      this.limparFiltros();
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },

    // Notificações
    enviarNotificacao(titulo, mensagem, tipo = 'info') {
      if (!this.config.notificacoes) return;
      
      // Implementação básica de notificação
      console.log(`[${tipo.toUpperCase()}] ${titulo}: ${mensagem}`);
      
      // Em produção, integrar com um sistema de notificações
      if (Notification.permission === 'granted') {
        new Notification(titulo, { body: mensagem });
      }
    },

    // Monitorar novas solicitações (para supervisores)
    iniciarMonitoramento() {
      // Em produção, implementar WebSocket ou polling
      setInterval(async () => {
        if (this.solicitacaoAtual?.status === 'PENDENTE') {
          await this.buscarSolicitacaoPorId(this.solicitacaoAtual.id);
        }
      }, 30000); // Atualiza a cada 30 segundos
    },
  },

  // Persistência opcional
  persist: {
    key: 'solicitacoes-store',
    paths: ['filtros', 'config'],
  },
});