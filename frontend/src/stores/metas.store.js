// metas.store.js
import { defineStore } from 'pinia';
import { MetasService } from '../services/metas.service';

export const useMetasStore = defineStore('metas', {
  state: () => ({
    // Dados principais
    metas: [], // Todas as metas da unidade
    metaAtual: null, // Meta sendo visualizada/editada
    metaPeriodo: null, // Meta do período atual (mês/ano)
    
    // Estatísticas de desempenho
    desempenho: {
      metaAtual: 0,
      realizadoAtual: 0,
      percentualAlcancado: 0,
      diferenca: 0,
      projecaoFinal: 0,
      tendencia: 'estavel', // 'crescente', 'decrescente', 'estavel'
    },
    
    // Histórico de desempenho
    historicoDesempenho: [],
    
    // Filtros
    filtros: {
      unidadeId: null,
      ano: new Date().getFullYear(),
      mes: null, // null para mostrar todas do ano
      busca: '',
      ordenacao: 'periodo_desc', // periodo_desc, periodo_asc, valor_desc, valor_asc
    },
    
    // Projeções e análises
    analises: {
      melhorMes: null,
      piorMes: null,
      mediaAlcance: 0,
      totalMetasAno: 0,
      metasAtingidas: 0,
      metasNaoAtingidas: 0,
      previsaoProximoMes: 0,
    },
    
    // Dashboard
    dashboard: {
      metaMesAtual: 0,
      realizadoMesAtual: 0,
      percentualMesAtual: 0,
      diasRestantes: 0,
      previsaoFimMes: 0,
      status: 'em_andamento', // 'atingida', 'em_andamento', 'em_risco', 'nao_atingida'
    },
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Configurações
    config: {
      alertaPercentual: 80, // Alerta quando abaixo deste percentual
      notificarAtingimento: true,
      corMetaAtingida: '#4caf50',
      corMetaEmAndamento: '#2196f3',
      corMetaEmRisco: '#ff9800',
      corMetaNaoAtingida: '#f44336',
    },
  }),

  getters: {
    // Metas filtradas
    metasFiltradas: (state) => {
      let metas = [...state.metas];
      
      // Filtro por ano
      if (state.filtros.ano) {
        metas = metas.filter(m => m.ano === state.filtros.ano);
      }
      
      // Filtro por mês
      if (state.filtros.mes) {
        metas = metas.filter(m => m.mes === state.filtros.mes);
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        metas = metas.filter(m => 
          m.descricao?.toLowerCase().includes(busca) ||
          m.valorAlvo?.toString().includes(busca)
        );
      }
      
      // Ordenação
      switch (state.filtros.ordenacao) {
        case 'periodo_asc':
          metas.sort((a, b) => {
            const periodoA = a.ano * 100 + a.mes;
            const periodoB = b.ano * 100 + b.mes;
            return periodoA - periodoB;
          });
          break;
        case 'valor_desc':
          metas.sort((a, b) => b.valorAlvo - a.valorAlvo);
          break;
        case 'valor_asc':
          metas.sort((a, b) => a.valorAlvo - b.valorAlvo);
          break;
        case 'periodo_desc':
        default:
          metas.sort((a, b) => {
            const periodoA = a.ano * 100 + a.mes;
            const periodoB = b.ano * 100 + b.mes;
            return periodoB - periodoA;
          });
          break;
      }
      
      return metas;
    },
    
    // Meta do mês atual
    metaMesAtual: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth() + 1; // JavaScript: 0-11, API: 1-12
      const anoAtual = hoje.getFullYear();
      
      return state.metas.find(m => 
        m.mes === mesAtual && m.ano === anoAtual
      );
    },
    
    // Metas do ano atual
    metasAnoAtual: (state) => {
      const anoAtual = new Date().getFullYear();
      return state.metas.filter(m => m.ano === anoAtual);
    },
    
    // Metas com desempenho calculado
    metasComDesempenho: (state, getters) => {
      return getters.metasFiltradas.map(meta => {
        // Em produção, buscar realizados da API
        const realizado = this.calcularRealizadoMock(meta.mes, meta.ano) || 0;
        const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
        
        return {
          ...meta,
          realizado,
          percentualAlcancado: percentual,
          diferenca: realizado - meta.valorAlvo,
          status: this.getStatusMeta(percentual),
        };
      });
    },
    
    // Gráfico de desempenho anual
    graficoDesempenhoAnual: (state, getters) => {
      const metasAno = getters.metasAnoAtual;
      const meses = Array.from({ length: 12 }, (_, i) => i + 1);
      
      return meses.map(mes => {
        const meta = metasAno.find(m => m.mes === mes);
        if (!meta) {
          return {
            mes,
            meta: 0,
            realizado: 0,
            percentual: 0,
          };
        }
        
        const realizado = this.calcularRealizadoMock(mes, meta.ano) || 0;
        const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
        
        return {
          mes,
          nomeMes: this.getNomeMes(mes),
          meta: meta.valorAlvo,
          realizado,
          percentual,
        };
      });
    },
    
    // Estatísticas do ano
    estatisticasAno: (state, getters) => {
      const metasCompleta = getters.metasComDesempenho.filter(m => m.ano === state.filtros.ano);
      
      if (metasCompleta.length === 0) {
        return {
          totalMetas: 0,
          totalRealizado: 0,
          totalMeta: 0,
          percentualGeral: 0,
          metasAtingidas: 0,
          mediaPercentual: 0,
        };
      }
      
      const totalMeta = metasCompleta.reduce((sum, m) => sum + m.valorAlvo, 0);
      const totalRealizado = metasCompleta.reduce((sum, m) => sum + (m.realizado || 0), 0);
      const percentualGeral = totalMeta > 0 ? (totalRealizado / totalMeta) * 100 : 0;
      const metasAtingidas = metasCompleta.filter(m => m.percentualAlcancado >= 100).length;
      const mediaPercentual = metasCompleta.reduce((sum, m) => sum + m.percentualAlcancado, 0) / metasCompleta.length;
      
      return {
        totalMetas: metasCompleta.length,
        totalRealizado,
        totalMeta,
        percentualGeral,
        metasAtingidas,
        mediaPercentual,
      };
    },
    
    // Previsão para o próximo mês
    previsaoProximoMes: (state, getters) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth() + 1;
      const anoAtual = hoje.getFullYear();
      
      // Busca histórico dos últimos 3 meses
      const historico = [];
      for (let i = 1; i <= 3; i++) {
        let mes = mesAtual - i;
        let ano = anoAtual;
        
        if (mes <= 0) {
          mes += 12;
          ano -= 1;
        }
        
        const meta = state.metas.find(m => m.mes === mes && m.ano === ano);
        if (meta) {
          const realizado = this.calcularRealizadoMock(mes, ano) || 0;
          historico.push({
            mes,
            realizado,
            percentual: meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0,
          });
        }
      }
      
      if (historico.length === 0) return 0;
      
      // Média dos últimos meses
      const mediaPercentual = historico.reduce((sum, h) => sum + h.percentual, 0) / historico.length;
      
      // Meta do próximo mês
      let proximoMes = mesAtual + 1;
      let proximoAno = anoAtual;
      if (proximoMes > 12) {
        proximoMes = 1;
        proximoAno += 1;
      }
      
      const metaProximoMes = state.metas.find(m => 
        m.mes === proximoMes && m.ano === proximoAno
      );
      
      if (!metaProximoMes) return 0;
      
      // Previsão baseada na média histórica
      return metaProximoMes.valorAlvo * (mediaPercentual / 100);
    },
    
    // Alertas de metas em risco
    metasEmRisco: (state, getters) => {
      const hoje = new Date();
      const diaAtual = hoje.getDate();
      const diasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();
      const percentualTempoDecorrido = (diaAtual / diasNoMes) * 100;
      
      return getters.metasComDesempenho.filter(meta => {
        if (meta.mes !== hoje.getMonth() + 1 || meta.ano !== hoje.getFullYear()) {
          return false;
        }
        
        // Meta está abaixo do percentual esperado para o tempo decorrido
        const percentualEsperado = Math.min(percentualTempoDecorrido * 1.1, 100); // 10% de tolerância
        return meta.percentualAlcancado < percentualEsperado && meta.percentualAlcancado < 100;
      });
    },
  },

  actions: {
    // Carregar metas de uma unidade
    async carregarMetas(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await MetasService.list(unidadeId);
        this.metas = response.data;
        this.calcularDesempenho();
        this.atualizarDashboard();
        this.calcularAnalises();
        return this.metas;
      } catch (error) {
        console.error('Erro ao carregar metas:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar metas';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar meta do período
    async carregarMetaPeriodo(unidadeId, mes, ano) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const params = { mes, ano };
        const response = await MetasService.periodo(unidadeId, params);
        this.metaPeriodo = response.data;
        return this.metaPeriodo;
      } catch (error) {
        console.error('Erro ao carregar meta do período:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar meta do período';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar/atualizar meta
    async criarMeta(unidadeId, metaData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Verifica se já existe meta para este período
        const metaExistente = this.metas.find(m => 
          m.mes === metaData.mes && 
          m.ano === metaData.ano &&
          m.unidadeId === unidadeId
        );
        
        let response;
        if (metaExistente) {
          // Em produção, teria um endpoint PUT para atualização
          // Por enquanto, simula atualização
          const index = this.metas.findIndex(m => m.id === metaExistente.id);
          this.metas[index] = { ...metaExistente, ...metaData };
          response = { data: this.metas[index] };
        } else {
          response = await MetasService.create(unidadeId, metaData);
          this.metas.push(response.data);
        }
        
        // Recalcula tudo
        this.calcularDesempenho();
        this.atualizarDashboard();
        this.calcularAnalises();
        
        return { success: true, data: response.data };
      } catch (error) {
        console.error('Erro ao criar meta:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar meta';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Calcular desempenho
    calcularDesempenho() {
      const hoje = new Date();
      const mesAtual = hoje.getMonth() + 1;
      const anoAtual = hoje.getFullYear();
      
      // Busca meta atual
      const meta = this.metas.find(m => m.mes === mesAtual && m.ano === anoAtual);
      
      if (!meta) {
        this.desempenho = {
          metaAtual: 0,
          realizadoAtual: 0,
          percentualAlcancado: 0,
          diferenca: 0,
          projecaoFinal: 0,
          tendencia: 'estavel',
        };
        return;
      }
      
      const realizado = this.calcularRealizadoMock(mesAtual, anoAtual) || 0;
      const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
      const diferenca = realizado - meta.valorAlvo;
      
      // Calcula projeção final baseada no desempenho atual
      const diasNoMes = new Date(anoAtual, mesAtual, 0).getDate();
      const diasDecorridos = hoje.getDate();
      const projecaoFinal = diasDecorridos > 0 ? (realizado / diasDecorridos) * diasNoMes : 0;
      
      // Determina tendência
      let tendencia = 'estavel';
      if (projecaoFinal > meta.valorAlvo * 1.1) {
        tendencia = 'crescente';
      } else if (projecaoFinal < meta.valorAlvo * 0.9) {
        tendencia = 'decrescente';
      }
      
      this.desempenho = {
        metaAtual: meta.valorAlvo,
        realizadoAtual: realizado,
        percentualAlcancado: percentual,
        diferenca,
        projecaoFinal,
        tendencia,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      const hoje = new Date();
      const mesAtual = hoje.getMonth() + 1;
      const anoAtual = hoje.getFullYear();
      const diasNoMes = new Date(anoAtual, mesAtual, 0).getDate();
      const diasRestantes = diasNoMes - hoje.getDate();
      
      const meta = this.metas.find(m => m.mes === mesAtual && m.ano === anoAtual);
      const realizado = this.calcularRealizadoMock(mesAtual, anoAtual) || 0;
      const percentual = meta?.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
      
      // Calcula previsão para fim do mês
      const diasDecorridos = hoje.getDate();
      const previsaoFimMes = diasDecorridos > 0 ? (realizado / diasDecorridos) * diasNoMes : 0;
      
      // Determina status
      let status = 'em_andamento';
      if (percentual >= 100) {
        status = 'atingida';
      } else if (percentual < this.config.alertaPercentual) {
        status = 'em_risco';
      } else if (previsaoFimMes < (meta?.valorAlvo || 0) * 0.8) {
        status = 'nao_atingida';
      }
      
      this.dashboard = {
        metaMesAtual: meta?.valorAlvo || 0,
        realizadoMesAtual: realizado,
        percentualMesAtual: percentual,
        diasRestantes,
        previsaoFimMes,
        status,
      };
    },

    // Calcular análises
    calcularAnalises() {
      const metasComDesempenho = this.metas.map(meta => {
        const realizado = this.calcularRealizadoMock(meta.mes, meta.ano) || 0;
        const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
        return { ...meta, realizado, percentual };
      });
      
      if (metasComDesempenho.length === 0) {
        this.analises = {
          melhorMes: null,
          piorMes: null,
          mediaAlcance: 0,
          totalMetasAno: 0,
          metasAtingidas: 0,
          metasNaoAtingidas: 0,
          previsaoProximoMes: 0,
        };
        return;
      }
      
      // Encontra melhor e pior mês
      const melhorMes = [...metasComDesempenho].sort((a, b) => b.percentual - a.percentual)[0];
      const piorMes = [...metasComDesempenho].sort((a, b) => a.percentual - b.percentual)[0];
      
      // Calcula média
      const mediaAlcance = metasComDesempenho.reduce((sum, m) => sum + m.percentual, 0) / metasComDesempenho.length;
      
      // Conta metas atingidas
      const anoAtual = new Date().getFullYear();
      const metasAno = metasComDesempenho.filter(m => m.ano === anoAtual);
      const metasAtingidas = metasAno.filter(m => m.percentual >= 100).length;
      const metasNaoAtingidas = metasAno.filter(m => m.percentual < 100).length;
      
      // Previsão para próximo mês
      const previsaoProximoMes = this.previsaoProximoMes;
      
      this.analises = {
        melhorMes,
        piorMes,
        mediaAlcance,
        totalMetasAno: metasAno.length,
        metasAtingidas,
        metasNaoAtingidas,
        previsaoProximoMes,
      };
    },

    // Métodos utilitários
    calcularRealizadoMock(mes, ano) {
      // Em produção, isso viria da API de faturamento
      // Simulação: 70-130% da meta aleatoriamente
      const meta = this.metas.find(m => m.mes === mes && m.ano === ano);
      if (!meta) return 0;
      
      // Para simulação mais realista, usa data atual
      const hoje = new Date();
      const isMesAtual = mes === hoje.getMonth() + 1 && ano === hoje.getFullYear();
      
      if (isMesAtual) {
        // Para mês atual, calcula baseado nos dias decorridos
        const diasNoMes = new Date(ano, mes, 0).getDate();
        const diasDecorridos = hoje.getDate();
        const percentualEsperado = (diasDecorridos / diasNoMes) * 100;
        
        // Variação de 80-120% do esperado
        const variacao = 0.8 + Math.random() * 0.4;
        return meta.valorAlvo * (percentualEsperado / 100) * variacao;
      } else {
        // Para meses passados, resultado final
        const alcance = 0.7 + Math.random() * 0.6; // 70-130%
        return meta.valorAlvo * alcance;
      }
    },

    getNomeMes(mes) {
      const meses = [
        'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
        'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
      ];
      return meses[mes - 1] || '';
    },

    getStatusMeta(percentual) {
      if (percentual >= 100) return 'atingida';
      if (percentual >= this.config.alertaPercentual) return 'em_andamento';
      if (percentual >= 50) return 'em_risco';
      return 'nao_atingida';
    },

    getCorStatus(status) {
      switch (status) {
        case 'atingida': return this.config.corMetaAtingida;
        case 'em_andamento': return this.config.corMetaEmAndamento;
        case 'em_risco': return this.config.corMetaEmRisco;
        case 'nao_atingida': return this.config.corMetaNaoAtingida;
        default: return '#9e9e9e';
      }
    },

    formatarValor(valor) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(valor || 0);
    },

    formatarPercentual(valor) {
      return `${valor.toFixed(1)}%`;
    },

    formatarPeriodo(mes, ano) {
      return `${this.getNomeMes(mes)}/${ano}`;
    },

    // Gerar sugestão de meta baseada em histórico
    gerarSugestaoMeta(mes, ano) {
      const hoje = new Date();
      const mesAnterior = mes === 1 ? 12 : mes - 1;
      const anoAnterior = mes === 1 ? ano - 1 : ano;
      
      // Busca meta do mês anterior
      const metaAnterior = this.metas.find(m => 
        m.mes === mesAnterior && m.ano === anoAnterior
      );
      
      if (!metaAnterior) {
        // Se não tem histórico, sugere baseado em média de meses equivalentes
        const mesesEquivalentes = this.metas.filter(m => m.mes === mes && m.ano < ano);
        if (mesesEquivalentes.length > 0) {
          const media = mesesEquivalentes.reduce((sum, m) => sum + m.valorAlvo, 0) / mesesEquivalentes.length;
          return media * 1.1; // Aumento de 10%
        }
        return 0;
      }
      
      // Calcula crescimento histórico
      const crescimento = this.calcularCrescimentoHistorico(mes, ano);
      
      // Sugere meta com crescimento aplicado
      return metaAnterior.valorAlvo * (1 + crescimento);
    },

    calcularCrescimentoHistorico(mes, ano) {
      // Calcula crescimento médio para este mês
      const historico = [];
      
      for (let i = 1; i <= 3; i++) {
        const anoHist = ano - i;
        const metaAtual = this.metas.find(m => m.mes === mes && m.ano === anoHist);
        const metaAnterior = this.metas.find(m => 
          m.mes === (mes === 1 ? 12 : mes - 1) && 
          m.ano === (mes === 1 ? anoHist - 1 : anoHist)
        );
        
        if (metaAtual && metaAnterior && metaAnterior.valorAlvo > 0) {
          const crescimento = (metaAtual.valorAlvo - metaAnterior.valorAlvo) / metaAnterior.valorAlvo;
          historico.push(crescimento);
        }
      }
      
      if (historico.length === 0) return 0.05; // Crescimento padrão de 5%
      
      // Média dos crescimentos históricos
      const media = historico.reduce((sum, c) => sum + c, 0) / historico.length;
      return Math.max(0.02, Math.min(0.15, media)); // Limita entre 2% e 15%
    },

    // Exportar relatório de metas
    exportarRelatorioMetas() {
      const metasExportar = this.metasComDesempenho;
      
      const headers = ['Período', 'Meta (R$)', 'Realizado (R$)', '% Atingido', 'Diferença (R$)', 'Status'];
      const dados = metasExportar.map(m => [
        this.formatarPeriodo(m.mes, m.ano),
        this.formatarValor(m.valorAlvo),
        this.formatarValor(m.realizado),
        this.formatarPercentual(m.percentualAlcancado),
        this.formatarValor(m.diferenca),
        this.getStatusMeta(m.percentualAlcancado).toUpperCase(),
      ]);
      
      const csvContent = [
        headers.join(';'),
        ...dados.map(row => row.join(';'))
      ].join('\n');
      
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = `metas_${new Date().toISOString().split('T')[0]}.csv`;
      link.click();
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        unidadeId: this.filtros.unidadeId,
        ano: new Date().getFullYear(),
        mes: null,
        busca: '',
        ordenacao: 'periodo_desc',
      };
    },

    // Resetar store
    resetarStore() {
      this.metas = [];
      this.metaAtual = null;
      this.metaPeriodo = null;
      this.error = null;
      this.limparFiltros();
      this.desempenho = {
        metaAtual: 0,
        realizadoAtual: 0,
        percentualAlcancado: 0,
        diferenca: 0,
        projecaoFinal: 0,
        tendencia: 'estavel',
      };
      this.dashboard = {
        metaMesAtual: 0,
        realizadoMesAtual: 0,
        percentualMesAtual: 0,
        diasRestantes: 0,
        previsaoFimMes: 0,
        status: 'em_andamento',
      };
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'metas-store',
    paths: ['filtros', 'config'],
  },
});