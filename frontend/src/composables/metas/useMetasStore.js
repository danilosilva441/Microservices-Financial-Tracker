// composables/metas/useMetasStore.js
import { defineStore } from 'pinia';
import { MetasService } from '@/services/metas.service';
import { useMetasCalculos } from './useMetasCalculos';
import { useMetasDashboard } from './useMetasDashboard';
import { useMetasAnalises } from './useMetasAnalises';
import { useMetasFilters } from './useMetasFilters';
import { useMetasUI } from './useMetasUI';
import { useMetasConfig } from './useMetasConfig';

export const useMetasStore = defineStore('metas', {
  state: () => ({
    // Dados principais
    metas: [],
    metaAtual: null,
    metaPeriodo: null,
    
    // Estatísticas de desempenho
    desempenho: {
      metaAtual: 0,
      realizadoAtual: 0,
      percentualAlcancado: 0,
      diferenca: 0,
      projecaoFinal: 0,
      tendencia: 'estavel',
    },
    
    // Histórico de desempenho
    historicoDesempenho: [],
    
    // Filtros
    filtros: {
      unidadeId: null,
      ano: new Date().getFullYear(),
      mes: null,
      busca: '',
      ordenacao: 'periodo_desc',
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
      status: 'em_andamento',
    },
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Configurações
    config: {
      alertaPercentual: 80,
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
      const { aplicarFiltros, ordenarMetas } = useMetasFilters();
      return ordenarMetas(aplicarFiltros(state.metas, state.filtros), state.filtros.ordenacao);
    },
    
    // Meta do mês atual
    metaMesAtual: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth() + 1;
      const anoAtual = hoje.getFullYear();
      
      return state.metas.find(m => m.mes === mesAtual && m.ano === anoAtual);
    },
    
    // Metas do ano atual
    metasAnoAtual: (state) => {
      const anoAtual = new Date().getFullYear();
      return state.metas.filter(m => m.ano === anoAtual);
    },
    
    // Metas com desempenho calculado
    metasComDesempenho: (state, getters) => {
      const { calcularRealizadoMock, getStatusMeta } = useMetasCalculos();
      
      return getters.metasFiltradas.map(meta => {
        const realizado = calcularRealizadoMock(meta.mes, meta.ano, state.metas) || 0;
        const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
        
        return {
          ...meta,
          realizado,
          percentualAlcancado: percentual,
          diferenca: realizado - meta.valorAlvo,
          status: getStatusMeta(percentual, state.config.alertaPercentual),
        };
      });
    },
    
    // Gráfico de desempenho anual
    graficoDesempenhoAnual: (state, getters) => {
      const { gerarGraficoDesempenhoAnual } = useMetasGraficos();
      return gerarGraficoDesempenhoAnual(state.metas, getters.metasAnoAtual);
    },
    
    // Estatísticas do ano
    estatisticasAno: (state, getters) => {
      const { calcularEstatisticasAno } = useMetasAnalises();
      return calcularEstatisticasAno(getters.metasComDesempenho, state.filtros.ano);
    },
    
    // Previsão para o próximo mês
    previsaoProximoMes: (state) => {
      const { calcularPrevisaoProximoMes } = useMetasAnalises();
      return calcularPrevisaoProximoMes(state.metas);
    },
    
    // Alertas de metas em risco
    metasEmRisco: (state, getters) => {
      const { identificarMetasEmRisco } = useMetasDashboard();
      return identificarMetasEmRisco(getters.metasComDesempenho, state.config.alertaPercentual);
    },
  },

  actions: {
    // Carregar metas de uma unidade
    async carregarMetas(unidadeId) {
      const ui = useMetasUI();
      ui.setLoading(true);
      ui.clearError();
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
        ui.setError(error.response?.data?.message || error.message || 'Erro ao carregar metas');
        throw error;
      } finally {
        ui.setLoading(false);
      }
    },

    // Carregar meta do período
    async carregarMetaPeriodo(unidadeId, mes, ano) {
      const ui = useMetasUI();
      ui.setLoading(true);
      ui.clearError();
      
      try {
        const params = { mes, ano };
        const response = await MetasService.periodo(unidadeId, params);
        this.metaPeriodo = response.data;
        return this.metaPeriodo;
      } catch (error) {
        console.error('Erro ao carregar meta do período:', error);
        ui.setError(error.response?.data?.message || error.message || 'Erro ao carregar meta do período');
        throw error;
      } finally {
        ui.setLoading(false);
      }
    },

    // Criar/atualizar meta
    async criarMeta(unidadeId, metaData) {
      const { validarMeta, verificarMetaExistente } = useMetasValidacao();
      const ui = useMetasUI();
      
      ui.setLoading(true);
      ui.clearError();
      
      try {
        // Validação
        const erros = validarMeta(metaData);
        if (erros.length > 0) {
          throw new Error(erros.join(', '));
        }
        
        // Verifica se já existe meta para este período
        const metaExistente = verificarMetaExistente(this.metas, metaData, unidadeId);
        
        let response;
        if (metaExistente) {
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
        ui.setError(error.response?.data?.message || error.message || 'Erro ao criar meta');
        return { success: false, error: this.error };
      } finally {
        ui.setLoading(false);
      }
    },

    // Calcular desempenho
    calcularDesempenho() {
      const { calcularDesempenhoAtual } = useMetasDashboard();
      this.desempenho = calcularDesempenhoAtual(this.metas, this.config);
    },

    // Atualizar dashboard
    atualizarDashboard() {
      const { atualizarDashboardMetas } = useMetasDashboard();
      this.dashboard = atualizarDashboardMetas(this.metas, this.config);
    },

    // Calcular análises
    calcularAnalises() {
      const { calcularAnalisesCompletas } = useMetasAnalises();
      this.analises = calcularAnalisesCompletas(this.metas);
    },

    // Gerar sugestão de meta baseada em histórico
    gerarSugestaoMeta(mes, ano) {
      const { calcularSugestaoMeta } = useMetasCalculos();
      return calcularSugestaoMeta(this.metas, mes, ano);
    },

    // Exportar relatório de metas
    exportarRelatorioMetas() {
      const { exportarMetasCSV } = useMetasExport();
      const metasParaExportar = this.metasComDesempenho;
      exportarMetasCSV(metasParaExportar);
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      const { aplicarFiltros: aplicar } = useMetasFilters();
      this.filtros = aplicar(this.filtros, filtros);
    },

    // Limpar filtros
    limparFiltros() {
      const { limparFiltros } = useMetasFilters();
      this.filtros = limparFiltros(this.filtros.unidadeId);
    },

    // Resetar store
    resetarStore() {
      const ui = useMetasUI();
      const { resetarFiltros } = useMetasFilters();
      
      this.metas = [];
      this.metaAtual = null;
      this.metaPeriodo = null;
      this.filtros = resetarFiltros(this.filtros.unidadeId);
      
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
      
      ui.resetarEstado();
    },

    // Limpar erro
    clearError() {
      const ui = useMetasUI();
      ui.clearError();
    },
  },

  // Persistência opcional
  persist: {
    key: 'metas-store',
    paths: ['filtros', 'config'],
  },
});