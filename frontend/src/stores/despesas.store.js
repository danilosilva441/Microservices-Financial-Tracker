// despesas.store.js
import { defineStore } from 'pinia';
import { DespesasService } from './despesas.service';

export const useDespesasStore = defineStore('despesas', {
  state: () => ({
    // Dados principais
    despesas: [],
    despesaAtual: null,
    categorias: [],
    categoriaAtual: null,
    
    // Filtros e buscas
    filtros: {
      unidadeId: null,
      dataInicio: null,
      dataFim: null,
      categoriaId: null,
      busca: '',
      ordenacao: 'data_desc', // data_desc, data_asc, valor_desc, valor_asc
    },
    
    // Upload de arquivos
    arquivoUpload: null,
    progressoUpload: 0,
    resultadoUpload: null,
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Estatísticas
    estatisticas: {
      totalMes: 0,
      totalPeriodo: 0,
      mediaDiaria: 0,
      maiorDespesa: 0,
      menorDespesa: 0,
      quantidadeDespesas: 0,
      totalPorCategoria: {},
    },
    
    // Dashboard
    dashboard: {
      despesasHoje: 0,
      despesasMes: 0,
      despesasPendentes: 0,
      proximasDespesas: [],
    },
    
    // Categorias padrão (se API não retornar)
    categoriasPadrao: [
      { id: 1, name: 'Aluguel', description: 'Pagamento de aluguel do imóvel' },
      { id: 2, name: 'Luz', description: 'Conta de energia elétrica' },
      { id: 3, name: 'Água', description: 'Conta de água' },
      { id: 4, name: 'Internet', description: 'Serviço de internet' },
      { id: 99, name: 'Outros', description: 'Outras despesas' },
    ],
  }),

  getters: {
    // Despesas filtradas e ordenadas
    despesasFiltradas: (state) => {
      let despesas = [...state.despesas];
      
      // Filtro por categoria
      if (state.filtros.categoriaId) {
        despesas = despesas.filter(d => d.categoryId === state.filtros.categoriaId);
      }
      
      // Filtro por período
      if (state.filtros.dataInicio || state.filtros.dataFim) {
        despesas = despesas.filter(d => {
          const dataDespesa = new Date(d.expenseDate || d.data);
          const inicio = state.filtros.dataInicio ? new Date(state.filtros.dataInicio) : null;
          const fim = state.filtros.dataFim ? new Date(state.filtros.dataFim) : null;
          
          if (inicio && dataDespesa < inicio) return false;
          if (fim && dataDespesa > fim) return false;
          return true;
        });
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        despesas = despesas.filter(d => 
          d.description?.toLowerCase().includes(busca) ||
          d.categoriaNome?.toLowerCase().includes(busca) ||
          d.amount?.toString().includes(busca)
        );
      }
      
      // Ordenação
      switch (state.filtros.ordenacao) {
        case 'data_asc':
          despesas.sort((a, b) => new Date(a.expenseDate || a.data) - new Date(b.expenseDate || b.data));
          break;
        case 'valor_desc':
          despesas.sort((a, b) => (b.amount || 0) - (a.amount || 0));
          break;
        case 'valor_asc':
          despesas.sort((a, b) => (a.amount || 0) - (b.amount || 0));
          break;
        case 'data_desc':
        default:
          despesas.sort((a, b) => new Date(b.expenseDate || b.data) - new Date(a.expenseDate || a.data));
          break;
      }
      
      return despesas;
    },
    
    // Despesas do mês atual
    despesasEsteMes: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      return state.despesas.filter(d => {
        const dataDespesa = new Date(d.expenseDate || d.data);
        return dataDespesa.getMonth() === mesAtual && 
               dataDespesa.getFullYear() === anoAtual;
      });
    },
    
    // Despesas de hoje
    despesasHoje: (state) => {
      const hoje = new Date().toISOString().split('T')[0];
      return state.despesas.filter(d => {
        const dataDespesa = new Date(d.expenseDate || d.data).toISOString().split('T')[0];
        return dataDespesa === hoje;
      });
    },
    
    // Total por categoria
    totalPorCategoria: (state) => {
      const totais = {};
      
      state.despesas.forEach(despesa => {
        const categoriaId = despesa.categoryId;
        const categoriaNome = state.categorias.find(c => c.id === categoriaId)?.name || 'Sem Categoria';
        
        if (!totais[categoriaNome]) {
          totais[categoriaNome] = 0;
        }
        totais[categoriaNome] += despesa.amount || 0;
      });
      
      return totais;
    },
    
    // Categorias com total
    categoriasComTotal: (state, getters) => {
      return state.categorias.map(categoria => {
        const total = state.despesas
          .filter(d => d.categoryId === categoria.id)
          .reduce((sum, d) => sum + (d.amount || 0), 0);
        
        return {
          ...categoria,
          total,
          quantidade: state.despesas.filter(d => d.categoryId === categoria.id).length
        };
      }).sort((a, b) => b.total - a.total);
    },
    
    // Top 5 categorias
    topCategorias: (state, getters) => {
      return getters.categoriasComTotal.slice(0, 5);
    },
    
    // Despesas recorrentes (mesmo valor, mesma categoria, periodicidade similar)
    despesasRecorrentes: (state) => {
      // Esta é uma implementação simples - em produção seria mais sofisticada
      const recorrentes = [];
      const grupos = {};
      
      state.despesas.forEach(despesa => {
        const chave = `${despesa.categoryId}_${despesa.amount}`;
        if (!grupos[chave]) {
          grupos[chave] = [];
        }
        grupos[chave].push(despesa);
      });
      
      // Considera recorrente se aparecer pelo menos 3 vezes
      Object.values(grupos).forEach(grupo => {
        if (grupo.length >= 3) {
          recorrentes.push({
            categoriaId: grupo[0].categoryId,
            categoriaNome: state.categorias.find(c => c.id === grupo[0].categoryId)?.name,
            valor: grupo[0].amount,
            quantidade: grupo.length,
            ultimaData: Math.max(...grupo.map(d => new Date(d.expenseDate || d.data).getTime())),
            despesas: grupo
          });
        }
      });
      
      return recorrentes.sort((a, b) => b.quantidade - a.quantidade);
    },
    
    // Despesas vencidas ou próximas do vencimento
    despesasVencimentoProximo: (state) => {
      const hoje = new Date();
      const trintaDias = new Date();
      trintaDias.setDate(hoje.getDate() + 30);
      
      // Esta é uma implementação simplificada
      // Em um sistema real, as despesas teriam data de vencimento
      return state.despesas.filter(d => {
        const dataDespesa = new Date(d.expenseDate || d.data);
        return dataDespesa > hoje && dataDespesa <= trintaDias;
      }).sort((a, b) => new Date(a.expenseDate || a.data) - new Date(b.expenseDate || b.data));
    },
  },

  actions: {
    // Carregar despesas de uma unidade
    async carregarDespesas(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await DespesasService.listByUnidade(unidadeId);
        this.despesas = response.data;
        this.calcularEstatisticas();
        this.atualizarDashboard();
        return this.despesas;
      } catch (error) {
        console.error('Erro ao carregar despesas:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar despesas';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar categorias
    async carregarCategorias() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await DespesasService.categories();
        this.categorias = response.data.length > 0 ? response.data : this.categoriasPadrao;
        return this.categorias;
      } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        // Usa categorias padrão se API falhar
        this.categorias = this.categoriasPadrao;
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar categorias';
        return this.categorias;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar nova despesa
    async criarDespesa(despesaData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await DespesasService.create(despesaData);
        const novaDespesa = response.data;
        
        // Adiciona à lista local
        this.despesas.unshift(novaDespesa);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: novaDespesa };
      } catch (error) {
        console.error('Erro ao criar despesa:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar despesa';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Criar nova categoria
    async criarCategoria(categoriaData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await DespesasService.createCategory(categoriaData);
        const novaCategoria = response.data;
        
        // Adiciona à lista local
        this.categorias.push(novaCategoria);
        
        return { success: true, data: novaCategoria };
      } catch (error) {
        console.error('Erro ao criar categoria:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar categoria';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Remover despesa
    async removerDespesa(id) {
      this.isLoading = true;
      this.error = null;
      
      try {
        await DespesasService.remove(id);
        
        // Remove da lista local
        this.despesas = this.despesas.filter(d => d.id !== id);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true };
      } catch (error) {
        console.error('Erro ao remover despesa:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao remover despesa';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Upload de arquivo
    async uploadArquivo(formData) {
      this.isLoading = true;
      this.error = null;
      this.progressoUpload = 0;
      this.resultadoUpload = null;
      
      try {
        // Configura interceptador para progresso
        const config = {
          headers: { 'Content-Type': 'multipart/form-data' },
          onUploadProgress: (progressEvent) => {
            if (progressEvent.total) {
              this.progressoUpload = Math.round((progressEvent.loaded * 100) / progressEvent.total);
            }
          }
        };
        
        const response = await DespesasService.upload(formData, config);
        this.resultadoUpload = response.data;
        
        // Recarrega despesas após upload
        if (this.filtros.unidadeId) {
          await this.carregarDespesas(this.filtros.unidadeId);
        }
        
        return { success: true, data: this.resultadoUpload };
      } catch (error) {
        console.error('Erro no upload:', error);
        this.error = error.response?.data?.message || error.message || 'Erro no upload';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
        this.progressoUpload = 0;
      }
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const despesasPeriodo = this.despesasFiltradas;
      
      if (despesasPeriodo.length === 0) {
        this.estatisticas = {
          totalMes: 0,
          totalPeriodo: 0,
          mediaDiaria: 0,
          maiorDespesa: 0,
          menorDespesa: 0,
          quantidadeDespesas: 0,
          totalPorCategoria: {},
        };
        return;
      }
      
      const valores = despesasPeriodo.map(d => d.amount || 0);
      const totaisPorCategoria = {};
      
      despesasPeriodo.forEach(despesa => {
        const categoriaId = despesa.categoryId;
        const categoriaNome = this.categorias.find(c => c.id === categoriaId)?.name || 'Sem Categoria';
        
        if (!totaisPorCategoria[categoriaNome]) {
          totaisPorCategoria[categoriaNome] = 0;
        }
        totaisPorCategoria[categoriaNome] += despesa.amount || 0;
      });
      
      const despesasMes = this.despesasEsteMes;
      const totalMes = despesasMes.reduce((total, d) => total + (d.amount || 0), 0);
      const totalPeriodo = despesasPeriodo.reduce((total, d) => total + (d.amount || 0), 0);
      
      // Calcula média diária baseada no período filtrado
      let mediaDiaria = 0;
      if (this.filtros.dataInicio && this.filtros.dataFim) {
        const inicio = new Date(this.filtros.dataInicio);
        const fim = new Date(this.filtros.dataFim);
        const dias = Math.max(1, Math.ceil((fim - inicio) / (1000 * 60 * 60 * 24)));
        mediaDiaria = totalPeriodo / dias;
      }
      
      this.estatisticas = {
        totalMes,
        totalPeriodo,
        mediaDiaria,
        maiorDespesa: Math.max(...valores),
        menorDespesa: Math.min(...valores),
        quantidadeDespesas: despesasPeriodo.length,
        totalPorCategoria: totaisPorCategoria,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      const despesasHoje = this.despesasHoje;
      const despesasMes = this.despesasEsteMes;
      const vencimentoProximo = this.despesasVencimentoProximo;
      
      this.dashboard = {
        despesasHoje: despesasHoje.reduce((total, d) => total + (d.amount || 0), 0),
        despesasMes: despesasMes.reduce((total, d) => total + (d.amount || 0), 0),
        despesasPendentes: 0, // Em um sistema real, teria lógica para despesas pendentes
        proximasDespesas: vencimentoProximo.slice(0, 5),
      };
    },

    // Métodos utilitários
    formatarValor(valor) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(valor || 0);
    },

    formatarData(data) {
      if (!data) return '';
      const dataObj = new Date(data);
      return dataObj.toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    },

    formatarDataCompleta(data) {
      if (!data) return '';
      const dataObj = new Date(data);
      return dataObj.toLocaleDateString('pt-BR', {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    },

    getNomeCategoria(categoriaId) {
      const categoria = this.categorias.find(c => c.id === categoriaId);
      return categoria ? categoria.name : 'Sem Categoria';
    },

    getDescricaoCategoria(categoriaId) {
      const categoria = this.categorias.find(c => c.id === categoriaId);
      return categoria ? categoria.description : '';
    },

    // Gerar relatório de despesas
    gerarRelatorioDespesas(dataInicio, dataFim) {
      const despesasPeriodo = this.despesas.filter(d => {
        const dataDespesa = new Date(d.expenseDate || d.data);
        const inicio = new Date(dataInicio);
        const fim = new Date(dataFim);
        
        return dataDespesa >= inicio && dataDespesa <= fim;
      });
      
      const relatorio = {
        periodo: { dataInicio, dataFim },
        totalDespesas: despesasPeriodo.length,
        valorTotal: despesasPeriodo.reduce((total, d) => total + (d.amount || 0), 0),
        despesasPorCategoria: {},
        despesasPorDia: {},
      };
      
      // Agrupa por categoria
      despesasPeriodo.forEach(despesa => {
        const categoriaNome = this.getNomeCategoria(despesa.categoryId);
        if (!relatorio.despesasPorCategoria[categoriaNome]) {
          relatorio.despesasPorCategoria[categoriaNome] = {
            valor: 0,
            quantidade: 0,
            despesas: []
          };
        }
        relatorio.despesasPorCategoria[categoriaNome].valor += despesa.amount || 0;
        relatorio.despesasPorCategoria[categoriaNome].quantidade += 1;
        relatorio.despesasPorCategoria[categoriaNome].despesas.push(despesa);
      });
      
      // Agrupa por dia
      despesasPeriodo.forEach(despesa => {
        const data = new Date(despesa.expenseDate || despesa.data).toISOString().split('T')[0];
        if (!relatorio.despesasPorDia[data]) {
          relatorio.despesasPorDia[data] = 0;
        }
        relatorio.despesasPorDia[data] += despesa.amount || 0;
      });
      
      return relatorio;
    },

    // Exportar despesas para CSV
    exportarParaCSV() {
      const headers = ['Data', 'Descrição', 'Categoria', 'Valor (R$)', 'Unidade'];
      const dados = this.despesasFiltradas.map(d => [
        this.formatarData(d.expenseDate || d.data),
        d.description || '',
        this.getNomeCategoria(d.categoryId),
        (d.amount || 0).toFixed(2).replace('.', ','),
        d.unidadeNome || ''
      ]);
      
      const csvContent = [
        headers.join(';'),
        ...dados.map(row => row.join(';'))
      ].join('\n');
      
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = `despesas_${new Date().toISOString().split('T')[0]}.csv`;
      link.click();
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
      this.calcularEstatisticas();
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        unidadeId: this.filtros.unidadeId,
        dataInicio: null,
        dataFim: null,
        categoriaId: null,
        busca: '',
        ordenacao: 'data_desc',
      };
      this.calcularEstatisticas();
    },

    // Resetar store
    resetarStore() {
      this.despesas = [];
      this.despesaAtual = null;
      this.categorias = [];
      this.categoriaAtual = null;
      this.arquivoUpload = null;
      this.progressoUpload = 0;
      this.resultadoUpload = null;
      this.error = null;
      this.filtros = {
        unidadeId: null,
        dataInicio: null,
        dataFim: null,
        categoriaId: null,
        busca: '',
        ordenacao: 'data_desc',
      };
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'despesas-store',
    paths: ['filtros', 'categorias'],
  },
});