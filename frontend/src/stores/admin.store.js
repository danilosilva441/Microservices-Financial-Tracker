// admin.store.js
import { defineStore } from 'pinia';
import { AdminService } from './admin.service';

// Definição de roles/perfis
export const RolesEnum = {
  // Roles existentes
  USER: 'User',
  ADMIN: 'Admin',
  DEV: 'Dev',
  
  // Novos perfis hierárquicos
  GERENTE: 'Gerente',
  SUPERVISOR: 'Supervisor',
  LIDER: 'Lider',
  OPERADOR: 'Operador',
  
  // IDs correspondentes (baseado no C#)
  IDs: {
    User: 'a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1',
    Admin: 'b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2',
    Dev: 'c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3',
    Gerente: 'd4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4',
    Supervisor: 'e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5',
    Lider: 'f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6',
    Operador: 'a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7',
  },
  
  // Hierarquia de permissões
  HIERARCHY: {
    Dev: 7,      // Máximo acesso
    Admin: 6,
    Gerente: 5,
    Supervisor: 4,
    Lider: 3,
    Operador: 2,
    User: 1,     // Mínimo acesso
  },
  
  // Funções auxiliares
  getNome: (role) => {
    const nomes = {
      'User': 'Usuário',
      'Admin': 'Administrador',
      'Dev': 'Desenvolvedor',
      'Gerente': 'Gerente',
      'Supervisor': 'Supervisor',
      'Lider': 'Líder',
      'Operador': 'Operador',
    };
    return nomes[role] || role;
  },
  
  getDescricao: (role) => {
    const descricoes = {
      'User': 'Usuário básico do sistema',
      'Admin': 'Administrador com acesso total',
      'Dev': 'Desenvolvedor - acesso técnico completo',
      'Gerente': 'Gerente de unidade/empresa',
      'Supervisor': 'Supervisor de equipe',
      'Lider': 'Líder de turno/equipe',
      'Operador': 'Operador de caixa',
    };
    return descricoes[role] || 'Perfil não definido';
  },
  
  getCor: (role) => {
    const cores = {
      'User': '#9e9e9e',
      'Admin': '#d32f2f',
      'Dev': '#7b1fa2',
      'Gerente': '#1976d2',
      'Supervisor': '#388e3c',
      'Lider': '#f57c00',
      'Operador': '#0288d1',
    };
    return cores[role] || '#757575';
  },
  
  getNivel: (role) => {
    return RolesEnum.HIERARCHY[role] || 0;
  },
  
  podeGerenciar: (roleUsuario, roleAlvo) => {
    const nivelUsuario = RolesEnum.getNivel(roleUsuario);
    const nivelAlvo = RolesEnum.getNivel(roleAlvo);
    return nivelUsuario > nivelAlvo;
  },
  
  getAll: () => [
    { id: 'User', nome: 'Usuário', descricao: 'Usuário básico do sistema', cor: '#9e9e9e', nivel: 1 },
    { id: 'Operador', nome: 'Operador', descricao: 'Operador de caixa', cor: '#0288d1', nivel: 2 },
    { id: 'Lider', nome: 'Líder', descricao: 'Líder de turno/equipe', cor: '#f57c00', nivel: 3 },
    { id: 'Supervisor', nome: 'Supervisor', descricao: 'Supervisor de equipe', cor: '#388e3c', nivel: 4 },
    { id: 'Gerente', nome: 'Gerente', descricao: 'Gerente de unidade/empresa', cor: '#1976d2', nivel: 5 },
    { id: 'Admin', nome: 'Administrador', descricao: 'Administrador com acesso total', cor: '#d32f2f', nivel: 6 },
    { id: 'Dev', nome: 'Desenvolvedor', descricao: 'Desenvolvedor - acesso técnico completo', cor: '#7b1fa2', nivel: 7 },
  ],
  
  // Verifica se usuário tem permissão para acessar funcionalidade
  temPermissao: (roleUsuario, funcionalidade) => {
    const permissoes = {
      // Permissões por role
      'User': ['view_dashboard', 'view_profile'],
      'Operador': ['view_dashboard', 'view_profile', 'create_faturamento', 'view_unidade'],
      'Lider': ['view_dashboard', 'view_profile', 'create_faturamento', 'fechar_caixa', 'view_equipe'],
      'Supervisor': ['view_dashboard', 'view_profile', 'create_faturamento', 'fechar_caixa', 'conferir_caixa', 'gerenciar_equipe', 'view_relatorios'],
      'Gerente': ['view_dashboard', 'view_profile', 'create_faturamento', 'fechar_caixa', 'conferir_caixa', 'gerenciar_equipe', 'gerenciar_unidades', 'view_relatorios', 'config_unidade'],
      'Admin': ['*'], // Acesso total
      'Dev': ['*'], // Acesso total
    };
    
    const permissoesUsuario = permissoes[roleUsuario] || [];
    return permissoesUsuario.includes('*') || permissoesUsuario.includes(funcionalidade);
  },
};

export const useAdminStore = defineStore('admin', {
  state: () => ({
    // Usuários e vínculos
    usuarios: [],
    vinculos: [], // Vínculos usuário-unidade
    usuarioSelecionado: null,
    
    // Unidades disponíveis
    unidadesDisponiveis: [],
    
    // Filtros
    filtros: {
      buscaUsuario: '',
      role: '',
      unidadeId: null,
      apenasAtivos: true,
    },
    
    // Estatísticas administrativas
    estatisticas: {
      totalUsuarios: 0,
      usuariosPorRole: {},
      unidadesComUsuarios: 0,
      mediaUsuariosPorUnidade: 0,
      novosUsuariosMes: 0,
    },
    
    // Dashboard administrativo
    dashboard: {
      usuariosAtivos: 0,
      unidadesAtivas: 0,
      solicitacoesPendentes: 0,
      alertasSistema: 0,
    },
    
    // Estado da UI
    isLoading: false,
    error: null,
    
    // Logs de atividades administrativas
    logsAtividades: [],
    
    // Configurações administrativas
    config: {
      notificarNovosUsuarios: true,
      logAtividades: true,
      limiteVinculosPorUsuario: 5,
      autoAtribuirRoles: false,
    },
  }),

  getters: {
    // Usuários filtrados
    usuariosFiltrados: (state) => {
      let usuarios = [...state.usuarios];
      
      // Filtro por role
      if (state.filtros.role) {
        usuarios = usuarios.filter(u => u.role === state.filtros.role);
      }
      
      // Filtro por busca
      if (state.filtros.buscaUsuario) {
        const busca = state.filtros.buscaUsuario.toLowerCase();
        usuarios = usuarios.filter(u => 
          u.fullName?.toLowerCase().includes(busca) ||
          u.email?.toLowerCase().includes(busca) ||
          u.phoneNumber?.includes(busca)
        );
      }
      
      // Filtro por status
      if (state.filtros.apenasAtivos) {
        usuarios = usuarios.filter(u => u.isActive !== false);
      }
      
      return usuarios.sort((a, b) => a.fullName?.localeCompare(b.fullName || ''));
    },
    
    // Usuários com vínculos
    usuariosComVinculos: (state) => {
      return state.usuarios.map(usuario => {
        const vinculosUsuario = state.vinculos.filter(v => v.userId === usuario.id);
        return {
          ...usuario,
          vinculos: vinculosUsuario,
          unidades: vinculosUsuario.map(v => 
            state.unidadesDisponiveis.find(u => u.id === v.unidadeId)
          ).filter(Boolean),
        };
      });
    },
    
    // Usuários por unidade
    usuariosPorUnidade: (state) => {
      const porUnidade = {};
      
      state.vinculos.forEach(vinculo => {
        const unidadeId = vinculo.unidadeId;
        if (!porUnidade[unidadeId]) {
          porUnidade[unidadeId] = {
            unidade: state.unidadesDisponiveis.find(u => u.id === unidadeId),
            usuarios: [],
            roles: {},
          };
        }
        
        const usuario = state.usuarios.find(u => u.id === vinculo.userId);
        if (usuario) {
          porUnidade[unidadeId].usuarios.push(usuario);
          
          // Conta roles
          if (!porUnidade[unidadeId].roles[usuario.role]) {
            porUnidade[unidadeId].roles[usuario.role] = 0;
          }
          porUnidade[unidadeId].roles[usuario.role]++;
        }
      });
      
      return porUnidade;
    },
    
    // Unidades disponíveis para vínculo
    unidadesParaVinculo: (state, getters) => {
      const usuario = state.usuarioSelecionado;
      if (!usuario) return state.unidadesDisponiveis;
      
      // Filtra unidades onde o usuário já tem vínculo
      const vinculosUsuario = state.vinculos.filter(v => v.userId === usuario.id);
      const unidadesComVinculo = new Set(vinculosUsuario.map(v => v.unidadeId));
      
      return state.unidadesDisponiveis.filter(u => !unidadesComVinculo.has(u.id));
    },
    
    // Distribuição de roles
    distribuicaoRoles: (state) => {
      const distribuicao = {};
      
      state.usuarios.forEach(usuario => {
        if (usuario.isActive !== false) {
          const role = usuario.role || 'User';
          if (!distribuicao[role]) {
            distribuicao[role] = 0;
          }
          distribuicao[role]++;
        }
      });
      
      return distribuicao;
    },
    
    // Verifica se usuário pode gerenciar outro
    podeGerenciarUsuario: (state) => (usuarioAlvo) => {
      const usuarioAtual = state.usuarioSelecionado; // Em produção, viria do auth store
      if (!usuarioAtual || !usuarioAlvo) return false;
      
      return RolesEnum.podeGerenciar(usuarioAtual.role, usuarioAlvo.role);
    },
    
    // Verifica se usuário pode vincular a unidade
    podeVincularUnidade: (state, getters) => (unidadeId) => {
      const usuario = state.usuarioSelecionado;
      if (!usuario) return false;
      
      // Verifica limite de vínculos
      const vinculosUsuario = state.vinculos.filter(v => v.userId === usuario.id);
      if (vinculosUsuario.length >= state.config.limiteVinculosPorUsuario) {
        return false;
      }
      
      // Verifica se já tem vínculo com esta unidade
      const jaTemVinculo = vinculosUsuario.some(v => v.unidadeId === unidadeId);
      return !jaTemVinculo;
    },
  },

  actions: {
    // Carregar todos os usuários (em produção, viria da API)
    async carregarUsuarios() {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, chamaria API de usuários
        // Simulação de dados
        this.usuarios = [
          {
            id: '1',
            fullName: 'Administrador Sistema',
            email: 'admin@sistema.com',
            phoneNumber: '(11) 99999-9999',
            role: 'Admin',
            isActive: true,
            createdAt: '2024-01-01T00:00:00Z',
          },
          {
            id: '2',
            fullName: 'João Silva - Gerente',
            email: 'joao.gerente@empresa.com',
            phoneNumber: '(11) 98888-8888',
            role: 'Gerente',
            isActive: true,
            createdAt: '2024-01-15T00:00:00Z',
          },
          {
            id: '3',
            fullName: 'Maria Santos - Supervisor',
            email: 'maria.supervisor@empresa.com',
            phoneNumber: '(11) 97777-7777',
            role: 'Supervisor',
            isActive: true,
            createdAt: '2024-02-01T00:00:00Z',
          },
          {
            id: '4',
            fullName: 'Pedro Oliveira - Líder',
            email: 'pedro.lider@empresa.com',
            phoneNumber: '(11) 96666-6666',
            role: 'Lider',
            isActive: true,
            createdAt: '2024-02-15T00:00:00Z',
          },
          {
            id: '5',
            fullName: 'Ana Costa - Operador',
            email: 'ana.operador@empresa.com',
            phoneNumber: '(11) 95555-5555',
            role: 'Operador',
            isActive: true,
            createdAt: '2024-03-01T00:00:00Z',
          },
          {
            id: '6',
            fullName: 'Carlos Lima - Operador',
            email: 'carlos.operador@empresa.com',
            phoneNumber: '(11) 94444-4444',
            role: 'Operador',
            isActive: false,
            createdAt: '2024-03-15T00:00:00Z',
          },
        ];
        
        this.calcularEstatisticas();
        this.atualizarDashboard();
        return this.usuarios;
      } catch (error) {
        console.error('Erro ao carregar usuários:', error);
        this.error = error.message || 'Erro ao carregar usuários';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar unidades disponíveis
    async carregarUnidades() {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, viria da API de unidades
        // Simulação de dados
        this.unidadesDisponiveis = [
          { id: 'unidade-1', nome: 'Unidade Centro', endereco: 'Rua Centro, 100' },
          { id: 'unidade-2', nome: 'Unidade Zona Sul', endereco: 'Av. Sul, 200' },
          { id: 'unidade-3', nome: 'Unidade Zona Norte', endereco: 'Rua Norte, 300' },
          { id: 'unidade-4', nome: 'Unidade Shopping', endereco: 'Shopping Center, Loja 50' },
        ];
        
        return this.unidadesDisponiveis;
      } catch (error) {
        console.error('Erro ao carregar unidades:', error);
        this.error = error.message || 'Erro ao carregar unidades';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar vínculos de um usuário
    async carregarVinculosUsuario(userId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, usaria a função importada
        // Simulação de dados
        const vinculosUsuario = this.vinculos.filter(v => v.userId === userId);
        
        // Se não tem vínculos, busca da API (simulação)
        if (vinculosUsuario.length === 0) {
          // Simula chamada à API
          const respostaSimulada = [
            {
              userId: userId,
              unidadeId: 'unidade-1',
              dataVinculo: '2024-01-15T00:00:00Z',
              roleNoVinculo: 'Gerente',
            },
            {
              userId: userId,
              unidadeId: 'unidade-2',
              dataVinculo: '2024-02-01T00:00:00Z',
              roleNoVinculo: 'Supervisor',
            },
          ];
          
          // Adiciona aos vínculos locais
          respostaSimulada.forEach(vinculo => {
            if (!this.vinculos.some(v => v.userId === vinculo.userId && v.unidadeId === vinculo.unidadeId)) {
              this.vinculos.push(vinculo);
            }
          });
        }
        
        return this.vinculos.filter(v => v.userId === userId);
      } catch (error) {
        console.error('Erro ao carregar vínculos:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar vínculos';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Vincular usuário a unidade
    async vincularUsuarioUnidade(vinculoData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Validação
        if (!vinculoData.userId || !vinculoData.unidadeId) {
          this.error = 'ID do usuário e unidade são obrigatórios';
          return { success: false, error: this.error };
        }
        
        // Verifica se já existe vínculo
        const vinculoExistente = this.vinculos.find(v => 
          v.userId === vinculoData.userId && 
          v.unidadeId === vinculoData.unidadeId
        );
        
        if (vinculoExistente) {
          this.error = 'Usuário já está vinculado a esta unidade';
          return { success: false, error: this.error };
        }
        
        // Chama a API
        const response = await AdminService.vincularUsuarioUnidade(vinculoData);
        const novoVinculo = response.data;
        
        // Adiciona ao estado local
        this.vinculos.push({
          ...novoVinculo,
          dataVinculo: new Date().toISOString(),
        });
        
        // Registra log de atividade
        this.registrarLogAtividade({
          tipo: 'VINCULO_USUARIO',
          usuarioId: 'admin', // Em produção, seria o ID do admin logado
          descricao: `Vinculou usuário ${vinculoData.userId} à unidade ${vinculoData.unidadeId}`,
          dados: vinculoData,
          data: new Date().toISOString(),
        });
        
        // Recalcula estatísticas
        this.calcularEstatisticas();
        
        return { success: true, data: novoVinculo };
      } catch (error) {
        console.error('Erro ao vincular usuário:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao vincular usuário';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Remover vínculo
    async removerVinculo(userId, unidadeId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, chamaria API DELETE
        // Remove localmente
        const index = this.vinculos.findIndex(v => 
          v.userId === userId && v.unidadeId === unidadeId
        );
        
        if (index === -1) {
          this.error = 'Vínculo não encontrado';
          return { success: false, error: this.error };
        }
        
        const vinculoRemovido = this.vinculos[index];
        this.vinculos.splice(index, 1);
        
        // Registra log de atividade
        this.registrarLogAtividade({
          tipo: 'REMOCAO_VINCULO',
          usuarioId: 'admin',
          descricao: `Removeu vínculo do usuário ${userId} da unidade ${unidadeId}`,
          dados: vinculoRemovido,
          data: new Date().toISOString(),
        });
        
        // Recalcula estatísticas
        this.calcularEstatisticas();
        
        return { success: true, data: vinculoRemovido };
      } catch (error) {
        console.error('Erro ao remover vínculo:', error);
        this.error = error.message || 'Erro ao remover vínculo';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Atualizar role de usuário
    async atualizarRoleUsuario(userId, novaRole) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, chamaria API de atualização de role
        const usuario = this.usuarios.find(u => u.id === userId);
        if (!usuario) {
          this.error = 'Usuário não encontrado';
          return { success: false, error: this.error };
        }
        
        // Verifica se a nova role é válida
        if (!RolesEnum.getAll().some(r => r.id === novaRole)) {
          this.error = 'Role inválida';
          return { success: false, error: this.error };
        }
        
        // Atualiza localmente
        const roleAnterior = usuario.role;
        usuario.role = novaRole;
        
        // Registra log de atividade
        this.registrarLogAtividade({
          tipo: 'ATUALIZACAO_ROLE',
          usuarioId: 'admin',
          descricao: `Alterou role do usuário ${usuario.fullName} de ${roleAnterior} para ${novaRole}`,
          dados: { userId, roleAnterior, novaRole },
          data: new Date().toISOString(),
        });
        
        // Recalcula estatísticas
        this.calcularEstatisticas();
        
        return { success: true, data: usuario };
      } catch (error) {
        console.error('Erro ao atualizar role:', error);
        this.error = error.message || 'Erro ao atualizar role';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      // Conta usuários por role
      const usuariosPorRole = {};
      const roles = RolesEnum.getAll();
      roles.forEach(role => {
        usuariosPorRole[role.id] = 0;
      });
      
      this.usuarios.forEach(usuario => {
        if (usuario.isActive !== false) {
          const role = usuario.role || 'User';
          usuariosPorRole[role] = (usuariosPorRole[role] || 0) + 1;
        }
      });
      
      // Calcula novos usuários este mês
      const novosUsuariosMes = this.usuarios.filter(u => {
        if (!u.createdAt) return false;
        const dataCriacao = new Date(u.createdAt);
        return dataCriacao.getMonth() === mesAtual && 
               dataCriacao.getFullYear() === anoAtual;
      }).length;
      
      // Calcula unidades com usuários
      const unidadesComUsuarios = new Set(this.vinculos.map(v => v.unidadeId)).size;
      const mediaUsuariosPorUnidade = unidadesComUsuarios > 0 
        ? this.vinculos.length / unidadesComUsuarios 
        : 0;
      
      this.estatisticas = {
        totalUsuarios: this.usuarios.filter(u => u.isActive !== false).length,
        usuariosPorRole,
        unidadesComUsuarios,
        mediaUsuariosPorUnidade,
        novosUsuariosMes,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      this.dashboard = {
        usuariosAtivos: this.usuarios.filter(u => u.isActive !== false).length,
        unidadesAtivas: this.unidadesDisponiveis.length,
        solicitacoesPendentes: 0, // Em produção, viria de outro módulo
        alertasSistema: 0, // Em produção, viria de monitoramento
      };
    },

    // Registrar log de atividade
    registrarLogAtividade(logData) {
      if (!this.config.logAtividades) return;
      
      const logCompleto = {
        id: Date.now().toString(),
        ...logData,
        data: new Date().toISOString(),
      };
      
      this.logsAtividades.unshift(logData);
      
      // Mantém apenas os últimos 100 logs
      if (this.logsAtividades.length > 100) {
        this.logsAtividades = this.logsAtividades.slice(0, 100);
      }
      
      // Em produção, enviaria para servidor
      console.log('[LOG ADMIN]', logData.descricao);
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
        minute: '2-digit',
      });
    },

    formatarRole(role) {
      return RolesEnum.getNome(role);
    },

    getCorRole(role) {
      return RolesEnum.getCor(role);
    },

    getDescricaoRole(role) {
      return RolesEnum.getDescricao(role);
    },

    // Verificar permissões
    temPermissao(usuarioRole, funcionalidade) {
      return RolesEnum.temPermissao(usuarioRole, funcionalidade);
    },

    // Verificar se pode gerenciar
    podeGerenciarRole(roleUsuario, roleAlvo) {
      return RolesEnum.podeGerenciar(roleUsuario, roleAlvo);
    },

    // Sugerir role baseada em hierarquia
    sugerirRoleParaUsuario(dadosUsuario) {
      // Lógica de sugestão baseada em experiência, unidade, etc.
      // Por enquanto, retorna role padrão
      return 'Operador';
    },

    // Gerar relatório administrativo
    gerarRelatorioAdmin() {
      const headers = ['Nome', 'E-mail', 'Role', 'Status', 'Unidades Vinculadas', 'Data Cadastro'];
      const dados = this.usuariosComVinculos.map(u => {
        const unidades = u.unidades.map(unidade => unidade?.nome).filter(Boolean).join(', ');
        
        return [
          u.fullName || '',
          u.email || '',
          this.formatarRole(u.role),
          u.isActive ? 'Ativo' : 'Inativo',
          unidades || 'Nenhuma',
          this.formatarData(u.createdAt),
        ];
      });
      
      const csvContent = [
        headers.join(';'),
        ...dados.map(row => row.join(';'))
      ].join('\n');
      
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = `relatorio_admin_${new Date().toISOString().split('T')[0]}.csv`;
      link.click();
    },

    // Buscar usuário por e-mail ou nome
    buscarUsuario(termo) {
      const termoLower = termo.toLowerCase();
      return this.usuarios.filter(u => 
        u.email?.toLowerCase().includes(termoLower) ||
        u.fullName?.toLowerCase().includes(termoLower)
      );
    },

    // Aplicar filtros
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpar filtros
    limparFiltros() {
      this.filtros = {
        buscaUsuario: '',
        role: '',
        unidadeId: null,
        apenasAtivos: true,
      };
    },

    // Selecionar usuário
    selecionarUsuario(usuario) {
      this.usuarioSelecionado = usuario;
    },

    // Limpar seleção
    limparSelecao() {
      this.usuarioSelecionado = null;
    },

    // Resetar store
    resetarStore() {
      this.usuarios = [];
      this.vinculos = [];
      this.usuarioSelecionado = null;
      this.unidadesDisponiveis = [];
      this.logsAtividades = [];
      this.error = null;
      this.limparFiltros();
      this.estatisticas = {
        totalUsuarios: 0,
        usuariosPorRole: {},
        unidadesComUsuarios: 0,
        mediaUsuariosPorUnidade: 0,
        novosUsuariosMes: 0,
      };
      this.dashboard = {
        usuariosAtivos: 0,
        unidadesAtivas: 0,
        solicitacoesPendentes: 0,
        alertasSistema: 0,
      };
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'admin-store',
    paths: ['filtros', 'config'],
  },
});