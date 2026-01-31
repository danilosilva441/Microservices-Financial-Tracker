// mensalistas.store.js
import { defineStore } from 'pinia';
import { MensalistasService } from './mensalistas.service';

export const useMensalistasStore = defineStore('mensalistas', {
  state: () => ({
    // Dados principais
    mensalistas: [], // Todos os mensalistas
    mensalistaAtual: null, // Mensalista sendo visualizado/editado
    
    // Empresas (se aplicável)
    empresas: [],
    
    // Filtros
    filtros: {
      unidadeId: null,
      ativos: true,
      inativos: false,
      busca: '',
      ordenacao: 'nome_asc', // nome_asc, nome_desc, valor_asc, valor_desc, data_cadastro_desc
      empresaId: null,
      valorMin: null,
      valorMax: null,
    },
    
    // Estatísticas
    estatisticas: {
      totalMensalistas: 0,
      totalAtivos: 0,
      totalInativos: 0,
      valorMensalTotal: 0,
      valorMensalAtivos: 0,
      mediaValorMensal: 0,
      novosEsteMes: 0,
      canceladosEsteMes: 0,
    },
    
    // Dashboard
    dashboard: {
      mensalistasAtivos: 0,
      receitaMensalProjetada: 0,
      mensalidadesVencendo: 0,
      novosCadastrosMes: 0,
    },
    
    // Histórico de pagamentos (simulado)
    historicoPagamentos: [],
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Configurações
    config: {
      notificarVencimento: true,
      diasAntesVencimento: 7,
      gerarCobrancaAutomatica: false,
      valorMensalidadePadrao: 200.00,
    },
  }),

  getters: {
    // Mensalistas filtrados
    mensalistasFiltrados: (state) => {
      let mensalistas = [...state.mensalistas];
      
      // Filtro por status
      if (!state.filtros.ativos && !state.filtros.inativos) {
        mensalistas = [];
      } else if (!state.filtros.ativos) {
        mensalistas = mensalistas.filter(m => !m.isAtivo);
      } else if (!state.filtros.inativos) {
        mensalistas = mensalistas.filter(m => m.isAtivo);
      }
      
      // Filtro por empresa
      if (state.filtros.empresaId) {
        mensalistas = mensalistas.filter(m => m.empresaId === state.filtros.empresaId);
      }
      
      // Filtro por valor
      if (state.filtros.valorMin !== null) {
        mensalistas = mensalistas.filter(m => m.valorMensalidade >= state.filtros.valorMin);
      }
      if (state.filtros.valorMax !== null) {
        mensalistas = mensalistas.filter(m => m.valorMensalidade <= state.filtros.valorMax);
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        mensalistas = mensalistas.filter(m => 
          m.nome?.toLowerCase().includes(busca) ||
          m.cpf?.includes(busca) ||
          m.email?.toLowerCase().includes(busca) ||
          m.telefone?.includes(busca)
        );
      }
      
      // Ordenação
      switch (state.filtros.ordenacao) {
        case 'nome_desc':
          mensalistas.sort((a, b) => b.nome?.localeCompare(a.nome || '') || 0);
          break;
        case 'valor_asc':
          mensalistas.sort((a, b) => (a.valorMensalidade || 0) - (b.valorMensalidade || 0));
          break;
        case 'valor_desc':
          mensalistas.sort((a, b) => (b.valorMensalidade || 0) - (a.valorMensalidade || 0));
          break;
        case 'data_cadastro_desc':
          mensalistas.sort((a, b) => new Date(b.dataCriacao) - new Date(a.dataCriacao));
          break;
        case 'nome_asc':
        default:
          mensalistas.sort((a, b) => a.nome?.localeCompare(b.nome || '') || 0);
          break;
      }
      
      return mensalistas;
    },
    
    // Mensalistas ativos
    mensalistasAtivos: (state) => {
      return state.mensalistas.filter(m => m.isAtivo);
    },
    
    // Mensalistas inativos
    mensalistasInativos: (state) => {
      return state.mensalistas.filter(m => !m.isAtivo);
    },
    
    // Mensalistas que precisam de atenção (pagamentos atrasados, vencendo)
    mensalistasAtencao: (state) => {
      // Em produção, integraria com sistema de pagamentos
      // Por enquanto, retorna inativos e com valor alto
      return state.mensalistas.filter(m => 
        !m.isAtivo || 
        (m.valorMensalidade > 500 && !m.ultimoPagamentoConfirmado)
      );
    },
    
    // Total de receita mensal projetada
    receitaMensalProjetada: (state) => {
      return state.mensalistasAtivos.reduce((total, m) => total + (m.valorMensalidade || 0), 0);
    },
    
    // Mensalistas por empresa
    mensalistasPorEmpresa: (state) => {
      const porEmpresa = {};
      
      state.mensalistasAtivos.forEach(mensalista => {
        const empresaId = mensalista.empresaId;
        if (!porEmpresa[empresaId]) {
          porEmpresa[empresaId] = {
            empresa: state.empresas.find(e => e.id === empresaId)?.nome || 'Sem Empresa',
            quantidade: 0,
            valorTotal: 0,
            mensalistas: [],
          };
        }
        porEmpresa[empresaId].quantidade++;
        porEmpresa[empresaId].valorTotal += mensalista.valorMensalidade || 0;
        porEmpresa[empresaId].mensalistas.push(mensalista);
      });
      
      return porEmpresa;
    },
    
    // Distribuição por valor de mensalidade
    distribuicaoPorValor: (state) => {
      const distribuicao = {
        'Até R$ 100': 0,
        'R$ 101 - R$ 200': 0,
        'R$ 201 - R$ 300': 0,
        'R$ 301 - R$ 500': 0,
        'Acima de R$ 500': 0,
      };
      
      state.mensalistasAtivos.forEach(m => {
        const valor = m.valorMensalidade || 0;
        if (valor <= 100) distribuicao['Até R$ 100']++;
        else if (valor <= 200) distribuicao['R$ 101 - R$ 200']++;
        else if (valor <= 300) distribuicao['R$ 201 - R$ 300']++;
        else if (valor <= 500) distribuicao['R$ 301 - R$ 500']++;
        else distribuicao['Acima de R$ 500']++;
      });
      
      return distribuicao;
    },
    
    // Novos cadastros este mês
    novosCadastrosEsteMes: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      return state.mensalistas.filter(m => {
        const dataCriacao = new Date(m.dataCriacao);
        return dataCriacao.getMonth() === mesAtual && 
               dataCriacao.getFullYear() === anoAtual;
      });
    },
    
    // Próximos vencimentos (simulação)
    proximosVencimentos: (state) => {
      const hoje = new Date();
      const trintaDias = new Date();
      trintaDias.setDate(hoje.getDate() + 30);
      
      // Em produção, buscar do sistema de pagamentos
      // Simulação baseada em data de cadastro
      return state.mensalistasAtivos
        .filter(m => {
          const dataCadastro = new Date(m.dataCriacao);
          const diaCadastro = dataCadastro.getDate();
          const hojeDia = hoje.getDate();
          
          // Vence todo dia X do mês (baseado no dia do cadastro)
          const venceHoje = diaCadastro === hojeDia;
          const venceProximosDias = diaCadastro > hojeDia && diaCadastro <= trintaDias.getDate();
          
          return venceHoje || venceProximosDias;
        })
        .sort((a, b) => {
          const diaA = new Date(a.dataCriacao).getDate();
          const diaB = new Date(b.dataCriacao).getDate();
          return diaA - diaB;
        });
    },
    
    // Formatar CPF para exibição
    formatarCPF: () => (cpf) => {
      if (!cpf) return '';
      const cpfLimpo = cpf.replace(/\D/g, '');
      if (cpfLimpo.length !== 11) return cpf;
      return cpfLimpo.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    },
    
    // Verificar se CPF é válido
    validarCPF: () => (cpf) => {
      if (!cpf) return false;
      const cpfLimpo = cpf.replace(/\D/g, '');
      
      if (cpfLimpo.length !== 11) return false;
      if (/^(\d)\1{10}$/.test(cpfLimpo)) return false; // Todos dígitos iguais
      
      // Validação de dígitos verificadores
      let soma = 0;
      let resto;
      
      for (let i = 1; i <= 9; i++) {
        soma += parseInt(cpfLimpo.substring(i - 1, i)) * (11 - i);
      }
      
      resto = (soma * 10) % 11;
      if (resto === 10 || resto === 11) resto = 0;
      if (resto !== parseInt(cpfLimpo.substring(9, 10))) return false;
      
      soma = 0;
      for (let i = 1; i <= 10; i++) {
        soma += parseInt(cpfLimpo.substring(i - 1, i)) * (12 - i);
      }
      
      resto = (soma * 10) % 11;
      if (resto === 10 || resto === 11) resto = 0;
      if (resto !== parseInt(cpfLimpo.substring(10, 11))) return false;
      
      return true;
    },
  },

  actions: {
    // Carregar mensalistas de uma unidade
    async carregarMensalistas(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await MensalistasService.list(unidadeId);
        this.mensalistas = response.data;
        this.calcularEstatisticas();
        this.atualizarDashboard();
        return this.mensalistas;
      } catch (error) {
        console.error('Erro ao carregar mensalistas:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar mensalistas';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar novo mensalista
    async criarMensalista(unidadeId, mensalistaData) {
      this.isLoading = true;
      this.error = null;
      
      // Validação básica
      if (!mensalistaData.nome?.trim()) {
        this.error = 'Nome é obrigatório';
        return { success: false, error: this.error };
      }
      
      if (mensalistaData.cpf && !this.validarCPF(mensalistaData.cpf)) {
        this.error = 'CPF inválido';
        return { success: false, error: this.error };
      }
      
      if (!mensalistaData.valorMensalidade || mensalistaData.valorMensalidade <= 0) {
        this.error = 'Valor da mensalidade deve ser maior que zero';
        return { success: false, error: this.error };
      }
      
      try {
        const response = await MensalistasService.create(unidadeId, mensalistaData);
        const novoMensalista = response.data;
        
        // Adiciona à lista local
        this.mensalistas.unshift(novoMensalista);
        
        // Adiciona dados extras para UI
        const mensalistaCompleto = {
          ...novoMensalista,
          email: mensalistaData.email || '',
          telefone: mensalistaData.telefone || '',
          endereco: mensalistaData.endereco || '',
          observacoes: mensalistaData.observacoes || '',
        };
        
        const index = this.mensalistas.findIndex(m => m.id === novoMensalista.id);
        if (index !== -1) {
          this.mensalistas[index] = mensalistaCompleto;
        }
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: mensalistaCompleto };
      } catch (error) {
        console.error('Erro ao criar mensalista:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar mensalista';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Atualizar mensalista
    async atualizarMensalista(unidadeId, mensalistaId, dadosAtualizados) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await MensalistasService.update(unidadeId, mensalistaId, dadosAtualizados);
        const mensalistaAtualizado = response.data;
        
        // Atualiza na lista local
        const index = this.mensalistas.findIndex(m => m.id === mensalistaId);
        if (index !== -1) {
          this.mensalistas[index] = { 
            ...this.mensalistas[index], 
            ...mensalistaAtualizado 
          };
        }
        
        // Se for o mensalista atual, atualiza também
        if (this.mensalistaAtual?.id === mensalistaId) {
          this.mensalistaAtual = { ...this.mensalistaAtual, ...mensalistaAtualizado };
        }
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: mensalistaAtualizado };
      } catch (error) {
        console.error('Erro ao atualizar mensalista:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao atualizar mensalista';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Buscar mensalista por ID
    async buscarMensalistaPorId(unidadeId, mensalistaId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await MensalistasService.getById(unidadeId, mensalistaId);
        this.mensalistaAtual = response.data;
        return this.mensalistaAtual;
      } catch (error) {
        console.error('Erro ao buscar mensalista:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar mensalista';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Ativar/desativar mensalista
    async toggleAtivoMensalista(unidadeId, mensalistaId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const mensalista = this.mensalistas.find(m => m.id === mensalistaId);
        if (!mensalista) {
          this.error = 'Mensalista não encontrado';
          return { success: false, error: this.error };
        }
        
        const novoStatus = !mensalista.isAtivo;
        
        // Atualiza localmente primeiro para resposta mais rápida
        const index = this.mensalistas.findIndex(m => m.id === mensalistaId);
        if (index !== -1) {
          this.mensalistas[index] = { ...this.mensalistas[index], isAtivo: novoStatus };
        }
        
        // Chama a API
        await MensalistasService.desativar(unidadeId, mensalistaId);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { 
          success: true, 
          data: { 
            id: mensalistaId, 
            isAtivo: novoStatus,
            mensagem: novoStatus ? 'Mensalista ativado com sucesso' : 'Mensalista desativado com sucesso'
          } 
        };
      } catch (error) {
        console.error('Erro ao alterar status do mensalista:', error);
        
        // Reverte alteração local em caso de erro
        const mensalista = this.mensalistas.find(m => m.id === mensalistaId);
        if (mensalista) {
          const index = this.mensalistas.findIndex(m => m.id === mensalistaId);
          if (index !== -1) {
            this.mensalistas[index] = { ...this.mensalistas[index], isAtivo: !mensalista.isAtivo };
          }
        }
        
        this.error = error.response?.data?.message || error.message || 'Erro ao alterar status do mensalista';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar empresas (simulação)
    async carregarEmpresas() {
      // Em produção, viria de uma API de empresas
      // Simulação de dados
      this.empresas = [
        { id: '1', nome: 'Empresa A', cnpj: '12.345.678/0001-90' },
        { id: '2', nome: 'Empresa B', cnpj: '98.765.432/0001-10' },
        { id: '3', nome: 'Empresa C', cnpj: '11.223.344/0001-55' },
        { id: '4', nome: 'Particular', cnpj: null },
      ];
      return this.empresas;
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      const ativos = this.mensalistasAtivos;
      const inativos = this.mensalistasInativos;
      const novosEsteMes = this.novosCadastrosEsteMes.length;
      
      // Calcula cancelamentos deste mês (mensalistas que foram desativados este mês)
      const canceladosEsteMes = this.mensalistas.filter(m => {
        if (!m.dataAtualizacao || m.isAtivo) return false;
        const dataAtualizacao = new Date(m.dataAtualizacao);
        return dataAtualizacao.getMonth() === mesAtual && 
               dataAtualizacao.getFullYear() === anoAtual;
      }).length;
      
      const valorMensalAtivos = ativos.reduce((total, m) => total + (m.valorMensalidade || 0), 0);
      const valorMensalTotal = this.mensalistas.reduce((total, m) => total + (m.valorMensalidade || 0), 0);
      const mediaValorMensal = ativos.length > 0 ? valorMensalAtivos / ativos.length : 0;
      
      this.estatisticas = {
        totalMensalistas: this.mensalistas.length,
        totalAtivos: ativos.length,
        totalInativos: inativos.length,
        valorMensalTotal,
        valorMensalAtivos,
        mediaValorMensal,
        novosEsteMes,
        canceladosEsteMes,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      const vencendo = this.proximosVencimentos.length;
      const novosCadastros = this.novosCadastrosEsteMes.length;
      
      this.dashboard = {
        mensalistasAtivos: this.estatisticas.totalAtivos,
        receitaMensalProjetada: this.receitaMensalProjetada,
        mensalidadesVencendo: vencendo,
        novosCadastrosMes: novosCadastros,
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

    formatarDataComHora(data) {
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

    // Gerar relatório de mensalistas
    gerarRelatorioMensalistas() {
      const headers = ['Nome', 'CPF', 'Valor Mensalidade', 'Status', 'Empresa', 'Data Cadastro', 'Última Atualização'];
      const dados = this.mensalistasFiltrados.map(m => [
        m.nome || '',
        this.formatarCPF(m.cpf) || '',
        this.formatarValor(m.valorMensalidade),
        m.isAtivo ? 'Ativo' : 'Inativo',
        this.empresas.find(e => e.id === m.empresaId)?.nome || 'Particular',
        this.formatarData(m.dataCriacao),
        this.formatarData(m.dataAtualizacao),
      ]);
      
      const csvContent = [
        headers.join(';'),
        ...dados.map(row => row.join(';'))
      ].join('\n');
      
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = `mensalistas_${new Date().toISOString().split('T')[0]}.csv`;
      link.click();
    },

    // Buscar mensalista por CPF
    buscarPorCPF(cpf) {
      const cpfLimpo = cpf.replace(/\D/g, '');
      return this.mensalistas.find(m => {
        const mensalistaCPF = m.cpf?.replace(/\D/g, '');
        return mensalistaCPF === cpfLimpo;
      });
    },

    // Verificar duplicidade de CPF
    verificarDuplicidadeCPF(cpf, mensalistaId = null) {
      const cpfLimpo = cpf.replace(/\D/g, '');
      if (!cpfLimpo) return false;
      
      return this.mensalistas.some(m => {
        if (mensalistaId && m.id === mensalistaId) return false; // Ignora o próprio mensalista
        const mensalistaCPF = m.cpf?.replace(/\D/g, '');
        return mensalistaCPF === cpfLimpo;
      });
    },

    // Gerar cobranças (simulação)
    async gerarCobrancasMensais(unidadeId, mes, ano) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, integraria com sistema de pagamentos
        // Simulação: gera cobranças para todos os ativos
        const mensalistasAtivos = this.mensalistasAtivos;
        const cobrancas = mensalistasAtivos.map(mensalista => {
          return {
            mensalistaId: mensalista.id,
            nome: mensalista.nome,
            valor: mensalista.valorMensalidade,
            mes,
            ano,
            dataVencimento: new Date(ano, mes - 1, 10), // Vence dia 10
            status: 'pendente',
            geradoEm: new Date().toISOString(),
          };
        });
        
        // Atualiza histórico local
        this.historicoPagamentos.push(...cobrancas);
        
        return { 
          success: true, 
          data: { 
            quantidade: cobrancas.length,
            valorTotal: cobrancas.reduce((total, c) => total + c.valor, 0),
            cobrancas 
          } 
        };
      } catch (error) {
        console.error('Erro ao gerar cobranças:', error);
        this.error = error.message || 'Erro ao gerar cobranças';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Importar mensalistas de arquivo (simulação)
    async importarMensalistas(unidadeId, arquivo) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, processaria o arquivo CSV/Excel
        // Simulação de importação
        const mensalistasImportados = [
          {
            nome: 'Cliente Importado 1',
            cpf: '111.222.333-44',
            valorMensalidade: 250.00,
            empresaId: '1',
            email: 'cliente1@email.com',
            telefone: '(11) 99999-9999',
          },
          {
            nome: 'Cliente Importado 2',
            cpf: '222.333.444-55',
            valorMensalidade: 300.00,
            empresaId: '2',
            email: 'cliente2@email.com',
            telefone: '(11) 88888-8888',
          },
        ];
        
        // Processa cada mensalista
        const resultados = [];
        for (const dados of mensalistasImportados) {
          const resultado = await this.criarMensalista(unidadeId, dados);
          resultados.push(resultado);
        }
        
        const sucessos = resultados.filter(r => r.success).length;
        const falhas = resultados.filter(r => !r.success).length;
        
        return { 
          success: true, 
          data: { 
            total: mensalistasImportados.length,
            sucessos,
            falhas,
            resultados 
          } 
        };
      } catch (error) {
        console.error('Erro ao importar mensalistas:', error);
        this.error = error.message || 'Erro ao importar mensalistas';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        unidadeId: this.filtros.unidadeId,
        ativos: true,
        inativos: false,
        busca: '',
        ordenacao: 'nome_asc',
        empresaId: null,
        valorMin: null,
        valorMax: null,
      };
    },

    // Resetar store
    resetarStore() {
      this.mensalistas = [];
      this.mensalistaAtual = null;
      this.empresas = [];
      this.historicoPagamentos = [];
      this.error = null;
      this.limparFiltros();
      this.estatisticas = {
        totalMensalistas: 0,
        totalAtivos: 0,
        totalInativos: 0,
        valorMensalTotal: 0,
        valorMensalAtivos: 0,
        mediaValorMensal: 0,
        novosEsteMes: 0,
        canceladosEsteMes: 0,
      };
      this.dashboard = {
        mensalistasAtivos: 0,
        receitaMensalProjetada: 0,
        mensalidadesVencendo: 0,
        novosCadastrosMes: 0,
      };
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'mensalistas-store',
    paths: ['filtros', 'config'],
  },
});