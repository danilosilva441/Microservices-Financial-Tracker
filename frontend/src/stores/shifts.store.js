// shifts.store.js
import { defineStore } from 'pinia';
import { ShiftsService } from '../services/shifts.service';

// Tipos de turno
export const ShiftTypeEnum = {
  MANHA: 1,
  TARDE: 2,
  NOITE: 3,
  INTEGRAL: 4,
  ESCALA_12x36: 5,
  ESCALA_24x48: 6,
  FOLGA: 7,
  FERIAS: 8,
  LICENCA: 9,
  OUTRO: 10,
  
  getNome: (tipo) => {
    const nomes = {
      1: 'Manhã',
      2: 'Tarde',
      3: 'Noite',
      4: 'Integral',
      5: 'Escala 12x36',
      6: 'Escala 24x48',
      7: 'Folga',
      8: 'Férias',
      9: 'Licença',
      10: 'Outro',
    };
    return nomes[tipo] || 'Tipo Desconhecido';
  },
  
  getCor: (tipo) => {
    const cores = {
      1: '#e3f2fd', // Azul claro
      2: '#f3e5f5', // Roxo claro
      3: '#e8f5e9', // Verde claro
      4: '#fff3e0', // Laranja claro
      5: '#fce4ec', // Rosa claro
      6: '#e0f2f1', // Ciano claro
      7: '#f5f5f5', // Cinza claro
      8: '#fff8e1', // Amarelo claro
      9: '#f1f8e9', // Verde claro 2
      10: '#f9f9f9', // Branco
    };
    return cores[tipo] || '#ffffff';
  },
  
  getCorTexto: (tipo) => {
    const cores = {
      1: '#1565c0', // Azul
      2: '#7b1fa2', // Roxo
      3: '#2e7d32', // Verde
      4: '#ef6c00', // Laranja
      5: '#c2185b', // Rosa
      6: '#00695c', // Ciano
      7: '#616161', // Cinza
      8: '#ff8f00', // Amarelo
      9: '#558b2f', // Verde 2
      10: '#424242', // Cinza escuro
    };
    return cores[tipo] || '#000000';
  },
  
  getHorarioPadrao: (tipo) => {
    const horarios = {
      1: { inicio: '08:00', fim: '14:00' },
      2: { inicio: '14:00', fim: '20:00' },
      3: { inicio: '20:00', fim: '08:00' },
      4: { inicio: '08:00', fim: '18:00' },
      5: { inicio: '07:00', fim: '19:00' },
      6: { inicio: '07:00', fim: '07:00' },
      7: { inicio: '00:00', fim: '00:00' },
      8: { inicio: '00:00', fim: '00:00' },
      9: { inicio: '00:00', fim: '00:00' },
      10: { inicio: '00:00', fim: '00:00' },
    };
    return horarios[tipo] || { inicio: '00:00', fim: '00:00' };
  },
  
  getAll: () => [
    { id: 1, nome: 'Manhã', cor: '#e3f2fd', corTexto: '#1565c0' },
    { id: 2, nome: 'Tarde', cor: '#f3e5f5', corTexto: '#7b1fa2' },
    { id: 3, nome: 'Noite', cor: '#e8f5e9', corTexto: '#2e7d32' },
    { id: 4, nome: 'Integral', cor: '#fff3e0', corTexto: '#ef6c00' },
    { id: 5, nome: 'Escala 12x36', cor: '#fce4ec', corTexto: '#c2185b' },
    { id: 6, nome: 'Escala 24x48', cor: '#e0f2f1', corTexto: '#00695c' },
    { id: 7, nome: 'Folga', cor: '#f5f5f5', corTexto: '#616161' },
    { id: 8, nome: 'Férias', cor: '#fff8e1', corTexto: '#ff8f00' },
    { id: 9, nome: 'Licença', cor: '#f1f8e9', corTexto: '#558b2f' },
    { id: 10, nome: 'Outro', cor: '#f9f9f9', corTexto: '#424242' },
  ],
};

// Tipos de intervalo
export const BreakTypeEnum = {
  ALMOCO: 1,
  CAFE: 2,
  DESCANSO: 3,
  REUNIAO: 4,
  TREINAMENTO: 5,
  OUTRO: 6,
  
  getNome: (tipo) => {
    const nomes = {
      1: 'Almoço',
      2: 'Café',
      3: 'Descanso',
      4: 'Reunião',
      5: 'Treinamento',
      6: 'Outro',
    };
    return nomes[tipo] || 'Tipo Desconhecido';
  },
  
  getAll: () => [
    { id: 1, nome: 'Almoço' },
    { id: 2, nome: 'Café' },
    { id: 3, nome: 'Descanso' },
    { id: 4, nome: 'Reunião' },
    { id: 5, nome: 'Treinamento' },
    { id: 6, nome: 'Outro' },
  ],
};

export const useShiftsStore = defineStore('shifts', {
  state: () => ({
    // Dados principais
    shifts: [], // Todas as escalas
    templates: [], // Templates de turno
    breaks: [], // Intervalos registrados
    shiftAtual: null, // Turno sendo visualizado/editado
    
    // Funcionários (simulação - em produção viria de API de usuários)
    funcionarios: [],
    
    // Filtros
    filtros: {
      unidadeId: null,
      dataInicio: null,
      dataFim: null,
      userId: null,
      tipoTurno: null,
      apenasAtivos: true,
      busca: '',
    },
    
    // Calendário
    calendario: {
      mesAtual: new Date().getMonth(),
      anoAtual: new Date().getFullYear(),
      vista: 'mes', // 'mes', 'semana', 'dia'
      dataSelecionada: new Date(),
    },
    
    // Estatísticas
    estatisticas: {
      totalTurnosMes: 0,
      totalHorasTrabalhadas: 0,
      horasExtras: 0,
      faltas: 0,
      coberturaMedia: 0,
      funcionariosAtivos: 0,
    },
    
    // Dashboard
    dashboard: {
      turnosHoje: 0,
      funcionariosTrabalhando: 0,
      escalasPendentes: 0,
      proximosFerias: [],
    },
    
    // Estado da UI
    isLoading: false,
    error: null,
    editando: false,
    
    // Configurações
    config: {
      horasTrabalhoSemanal: 44,
      intervaloObrigatorio: true,
      duracaoIntervalo: 60, // minutos
      notificarFolgas: true,
      escalaAutomatica: false,
    },
  }),

  getters: {
    // Shifts filtrados
    shiftsFiltrados: (state) => {
      let shifts = state.shifts;
      
      // Filtro por período
      if (state.filtros.dataInicio || state.filtros.dataFim) {
        shifts = shifts.filter(s => {
          const dataShift = new Date(s.data || s.startDate);
          const inicio = state.filtros.dataInicio ? new Date(state.filtros.dataInicio) : null;
          const fim = state.filtros.dataFim ? new Date(state.filtros.dataFim) : null;
          
          if (inicio && dataShift < inicio) return false;
          if (fim && dataShift > fim) return false;
          return true;
        });
      }
      
      // Filtro por funcionário
      if (state.filtros.userId) {
        shifts = shifts.filter(s => s.userId === state.filtros.userId);
      }
      
      // Filtro por tipo de turno
      if (state.filtros.tipoTurno) {
        shifts = shifts.filter(s => s.type === state.filtros.tipoTurno);
      }
      
      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        shifts = shifts.filter(s => 
          s.user?.nome?.toLowerCase().includes(busca) ||
          s.template?.name?.toLowerCase().includes(busca)
        );
      }
      
      // Filtro apenas ativos
      if (state.filtros.apenasAtivos) {
        shifts = shifts.filter(s => s.isAtivo !== false);
      }
      
      return shifts.sort((a, b) => new Date(a.data || a.startDate) - new Date(b.data || b.startDate));
    },
    
    // Shifts do dia atual
    shiftsHoje: (state) => {
      const hoje = new Date().toISOString().split('T')[0];
      return state.shifts.filter(s => {
        const dataShift = new Date(s.data || s.startDate).toISOString().split('T')[0];
        return dataShift === hoje;
      });
    },
    
    // Shifts da semana atual
    shiftsEstaSemana: (state) => {
      const hoje = new Date();
      const inicioSemana = new Date(hoje);
      inicioSemana.setDate(hoje.getDate() - hoje.getDay()); // Domingo
      const fimSemana = new Date(inicioSemana);
      fimSemana.setDate(inicioSemana.getDate() + 6); // Sábado
      
      return state.shifts.filter(s => {
        const dataShift = new Date(s.data || s.startDate);
        return dataShift >= inicioSemana && dataShift <= fimSemana;
      });
    },
    
    // Shifts do mês atual
    shiftsEsteMes: (state) => {
      const hoje = new Date();
      const mesAtual = hoje.getMonth();
      const anoAtual = hoje.getFullYear();
      
      return state.shifts.filter(s => {
        const dataShift = new Date(s.data || s.startDate);
        return dataShift.getMonth() === mesAtual && 
               dataShift.getFullYear() === anoAtual;
      });
    },
    
    // Shifts por funcionário
    shiftsPorFuncionario: (state) => {
      const porFuncionario = {};
      
      state.shifts.forEach(shift => {
        const userId = shift.userId;
        if (!porFuncionario[userId]) {
          porFuncionario[userId] = {
            funcionario: shift.user,
            shifts: [],
            totalHoras: 0,
            diasTrabalhados: 0,
            folgas: 0,
          };
        }
        porFuncionario[userId].shifts.push(shift);
        
        // Calcula horas do shift
        if (shift.startTime && shift.endTime) {
          const inicio = this.parseTimeToMinutes(shift.startTime);
          const fim = this.parseTimeToMinutes(shift.endTime);
          let horas = (fim - inicio) / 60;
          
          // Se for turno da noite que passa da meia-noite
          if (horas < 0) horas += 24;
          
          // Subtrai intervalos
          const intervalos = state.breaks.filter(b => b.shiftId === shift.id);
          intervalos.forEach(intervalo => {
            const inicioIntervalo = this.parseTimeToMinutes(intervalo.startTime);
            const fimIntervalo = this.parseTimeToMinutes(intervalo.endTime);
            horas -= (fimIntervalo - inicioIntervalo) / 60;
          });
          
          porFuncionario[userId].totalHoras += horas;
        }
        
        if (shift.type === 7) { // Folga
          porFuncionario[userId].folgas++;
        } else {
          porFuncionario[userId].diasTrabalhados++;
        }
      });
      
      return porFuncionario;
    },
    
    // Calendário para visualização
    calendarioMensal: (state) => {
      const { mesAtual, anoAtual } = state.calendario;
      const diasNoMes = new Date(anoAtual, mesAtual + 1, 0).getDate();
      const primeiroDia = new Date(anoAtual, mesAtual, 1).getDay();
      
      const semanas = [];
      let semana = Array(7).fill(null);
      let diaAtual = 1;
      
      // Preenche dias do mês anterior
      for (let i = 0; i < primeiroDia; i++) {
        const data = new Date(anoAtual, mesAtual, diaAtual - (primeiroDia - i));
        semana[i] = {
          data,
          isOutroMes: true,
          shifts: [],
        };
      }
      
      // Preenche dias do mês atual
      while (diaAtual <= diasNoMes) {
        for (let i = primeiroDia; i < 7 && diaAtual <= diasNoMes; i++) {
          const data = new Date(anoAtual, mesAtual, diaAtual);
          const dataStr = data.toISOString().split('T')[0];
          
          // Busca shifts para este dia
          const shiftsDia = state.shifts.filter(s => {
            const shiftDate = new Date(s.data || s.startDate).toISOString().split('T')[0];
            return shiftDate === dataStr;
          });
          
          semana[i] = {
            data,
            isOutroMes: false,
            isHoje: data.toDateString() === new Date().toDateString(),
            shifts: shiftsDia,
          };
          diaAtual++;
        }
        
        semanas.push([...semana]);
        semana = Array(7).fill(null);
        primeiroDia = 0; // Após primeira semana, começa no domingo
      }
      
      return semanas;
    },
    
    // Funcionários disponíveis para escala
    funcionariosDisponiveis: (state, getters) => {
      const hoje = new Date().toISOString().split('T')[0];
      
      return state.funcionarios.filter(funcionario => {
        // Verifica se já tem escala hoje
        const jaTemEscalaHoje = getters.shiftsHoje.some(s => s.userId === funcionario.id);
        return !jaTemEscalaHoje && funcionario.isAtivo;
      });
    },
    
    // Horas trabalhadas por funcionário
    horasPorFuncionario: (state, getters) => {
      const horas = {};
      
      Object.entries(getters.shiftsPorFuncionario).forEach(([userId, dados]) => {
        horas[userId] = dados.totalHoras;
      });
      
      return horas;
    },
    
    // Próximas férias
    proximasFerias: (state) => {
      const hoje = new Date();
      const trintaDias = new Date();
      trintaDias.setDate(hoje.getDate() + 30);
      
      return state.shifts.filter(s => {
        if (s.type !== 8) return false; // Apenas férias
        
        const dataInicio = new Date(s.data || s.startDate);
        return dataInicio >= hoje && dataInicio <= trintaDias;
      });
    },
    
    // Escalas pendentes (sem funcionário atribuído)
    escalasPendentes: (state) => {
      return state.shifts.filter(s => !s.userId && s.isAtivo !== false);
    },
  },

  actions: {
    // Carregar shifts de uma unidade
    async carregarShifts(unidadeId) {
      this.isLoading = true;
      this.error = null;
      this.filtros.unidadeId = unidadeId;
      
      try {
        const response = await ShiftsService.listByUnidade(unidadeId);
        this.shifts = response.data;
        this.calcularEstatisticas();
        this.atualizarDashboard();
        return this.shifts;
      } catch (error) {
        console.error('Erro ao carregar shifts:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar shifts';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Criar template de turno
    async criarTemplate(templateData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await ShiftsService.createTemplate(templateData);
        const novoTemplate = response.data;
        
        // Adiciona à lista local
        this.templates.push(novoTemplate);
        
        return { success: true, data: novoTemplate };
      } catch (error) {
        console.error('Erro ao criar template:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao criar template';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Gerar escala
    async gerarEscala(dadosEscala) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await ShiftsService.generate(dadosEscala);
        const escalaGerada = response.data;
        
        // Adiciona os novos shifts à lista
        if (Array.isArray(escalaGerada)) {
          this.shifts.push(...escalaGerada);
        } else {
          this.shifts.push(escalaGerada);
        }
        
        // Recalcula estatísticas
        this.calcularEstatisticas();
        this.atualizarDashboard();
        
        return { success: true, data: escalaGerada };
      } catch (error) {
        console.error('Erro ao gerar escala:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao gerar escala';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Registrar intervalo
    async registrarIntervalo(shiftId, intervaloData) {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await ShiftsService.breaks(shiftId, intervaloData);
        const novoIntervalo = response.data;
        
        // Adiciona à lista local
        this.breaks.push(novoIntervalo);
        
        // Atualiza estatísticas
        this.calcularEstatisticas();
        
        return { success: true, data: novoIntervalo };
      } catch (error) {
        console.error('Erro ao registrar intervalo:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao registrar intervalo';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Carregar funcionários (simulação)
    async carregarFuncionarios(unidadeId) {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Em produção, isso viria de uma API de usuários
        // Simulação de dados
        this.funcionarios = [
          { id: '1', nome: 'João Silva', email: 'joao@empresa.com', isAtivo: true, cargo: 'Operador' },
          { id: '2', nome: 'Maria Santos', email: 'maria@empresa.com', isAtivo: true, cargo: 'Operador' },
          { id: '3', nome: 'Pedro Oliveira', email: 'pedro@empresa.com', isAtivo: true, cargo: 'Líder' },
          { id: '4', nome: 'Ana Costa', email: 'ana@empresa.com', isAtivo: true, cargo: 'Supervisor' },
          { id: '5', nome: 'Carlos Lima', email: 'carlos@empresa.com', isAtivo: false, cargo: 'Operador' },
          { id: '6', nome: 'Fernanda Rocha', email: 'fernanda@empresa.com', isAtivo: true, cargo: 'Operador' },
        ];
        
        return this.funcionarios;
      } catch (error) {
        console.error('Erro ao carregar funcionários:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar funcionários';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Calcular estatísticas
    calcularEstatisticas() {
      const shiftsMes = this.shiftsEsteMes;
      const funcionariosAtivos = this.funcionarios.filter(f => f.isAtivo).length;
      
      let totalHoras = 0;
      let horasExtras = 0;
      let faltas = 0;
      
      shiftsMes.forEach(shift => {
        if (shift.startTime && shift.endTime && shift.type !== 7) { // Não contar folgas
          const inicio = this.parseTimeToMinutes(shift.startTime);
          const fim = this.parseTimeToMinutes(shift.endTime);
          let horas = (fim - inicio) / 60;
          
          // Se for turno da noite que passa da meia-noite
          if (horas < 0) horas += 24;
          
          // Subtrai intervalos
          const intervalos = this.breaks.filter(b => b.shiftId === shift.id);
          intervalos.forEach(intervalo => {
            const inicioIntervalo = this.parseTimeToMinutes(intervalo.startTime);
            const fimIntervalo = this.parseTimeToMinutes(intervalo.endTime);
            horas -= (fimIntervalo - inicioIntervalo) / 60;
          });
          
          totalHoras += horas;
          
          // Verifica horas extras (mais de 8 horas por dia)
          if (horas > 8) {
            horasExtras += horas - 8;
          }
        }
        
        // Verifica faltas (shift agendado mas sem presença registrada)
        // Esta é uma lógica simplificada
        if (shift.registrouPresenca === false) {
          faltas++;
        }
      });
      
      // Cálculo de cobertura média (funcionários por turno)
      const coberturaMedia = shiftsMes.length > 0 ? shiftsMes.length / 30 : 0; // Média por dia
      
      this.estatisticas = {
        totalTurnosMes: shiftsMes.length,
        totalHorasTrabalhadas: totalHoras,
        horasExtras,
        faltas,
        coberturaMedia,
        funcionariosAtivos,
      };
    },

    // Atualizar dashboard
    atualizarDashboard() {
      this.dashboard = {
        turnosHoje: this.shiftsHoje.length,
        funcionariosTrabalhando: this.shiftsHoje.filter(s => s.type !== 7).length,
        escalasPendentes: this.escalasPendentes.length,
        proximosFerias: this.proximasFerias.slice(0, 3),
      };
    },

    // Métodos utilitários
    parseTimeToMinutes(timeStr) {
      if (!timeStr) return 0;
      const [hours, minutes] = timeStr.split(':').map(Number);
      return hours * 60 + (minutes || 0);
    },

    formatMinutesToTime(minutes) {
      const hours = Math.floor(minutes / 60);
      const mins = minutes % 60;
      return `${hours.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}`;
    },

    formatarData(data) {
      if (!data) return '';
      const dataObj = new Date(data);
      return dataObj.toLocaleDateString('pt-BR', {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
      });
    },

    formatarDataCompleta(data) {
      if (!data) return '';
      const dataObj = new Date(data);
      return dataObj.toLocaleDateString('pt-BR', {
        weekday: 'long',
        day: '2-digit',
        month: 'long',
        year: 'numeric',
      });
    },

    formatarHorario(horario) {
      if (!horario) return '';
      return horario.substring(0, 5); // Formato HH:MM
    },

    getNomeTipo(tipo) {
      return ShiftTypeEnum.getNome(tipo);
    },

    getCorTipo(tipo) {
      return ShiftTypeEnum.getCor(tipo);
    },

    getCorTextoTipo(tipo) {
      return ShiftTypeEnum.getCorTexto(tipo);
    },

    getHorarioPadraoTipo(tipo) {
      return ShiftTypeEnum.getHorarioPadrao(tipo);
    },

    // Gerar escala automática
    async gerarEscalaAutomatica(unidadeId, mes, ano, funcionariosIds) {
      const dataInicio = new Date(ano, mes - 1, 1);
      const dataFim = new Date(ano, mes, 0); // Último dia do mês
      
      // Lógica simples de escala (em produção seria mais complexa)
      const escalas = [];
      const diasNoMes = dataFim.getDate();
      
      // Template padrão (manhã)
      const templatePadrao = this.templates.find(t => t.type === 1) || {
        id: 'template-padrao',
        name: 'Manhã',
        defaultStartTime: '08:00',
        defaultEndTime: '14:00',
        type: 1,
      };
      
      // Distribui funcionários pelos dias
      for (let dia = 1; dia <= diasNoMes; dia++) {
        const data = new Date(ano, mes - 1, dia);
        
        // Verifica se é fim de semana
        const isFimDeSemana = data.getDay() === 0 || data.getDay() === 6;
        
        // Seleciona funcionários para este dia
        const funcionariosDoDia = isFimDeSemana 
          ? funcionariosIds.slice(0, Math.ceil(funcionariosIds.length / 2)) // Menos funcionários no fim de semana
          : funcionariosIds;
        
        funcionariosDoDia.forEach((userId, index) => {
          // Alterna tipos de turno
          const tipoTurno = isFimDeSemana ? 7 : (index % 3) + 1; // Folga no fim de semana
          
          escalas.push({
            unidadeId,
            userId,
            templateId: templatePadrao.id,
            data: data.toISOString().split('T')[0],
            type: tipoTurno,
            startTime: tipoTurno !== 7 ? this.getHorarioPadraoTipo(tipoTurno).inicio : null,
            endTime: tipoTurno !== 7 ? this.getHorarioPadraoTipo(tipoTurno).fim : null,
            isAtivo: true,
          });
        });
      }
      
      // Em produção, enviaria para a API
      // Por enquanto, adiciona localmente
      this.shifts.push(...escalas);
      this.calcularEstatisticas();
      this.atualizarDashboard();
      
      return { success: true, data: escalas };
    },

    // Verificar disponibilidade de funcionário
    verificarDisponibilidadeFuncionario(userId, data) {
      const dataStr = new Date(data).toISOString().split('T')[0];
      
      // Verifica se já tem escala nesta data
      const jaTemEscala = this.shifts.some(s => 
        s.userId === userId && 
        new Date(s.data || s.startDate).toISOString().split('T')[0] === dataStr
      );
      
      if (jaTemEscala) {
        return { disponivel: false, motivo: 'Já possui escala nesta data' };
      }
      
      // Verifica se teve escala no dia anterior (descanso mínimo)
      const dataAnterior = new Date(data);
      dataAnterior.setDate(dataAnterior.getDate() - 1);
      const dataAnteriorStr = dataAnterior.toISOString().split('T')[0];
      
      const teveEscalaOntem = this.shifts.some(s => 
        s.userId === userId && 
        new Date(s.data || s.startDate).toISOString().split('T')[0] === dataAnteriorStr &&
        s.type !== 7 // Não conta folgas
      );
      
      if (teveEscalaOntem) {
        // Verifica se foi turno noturno
        const escalaOntem = this.shifts.find(s => 
          s.userId === userId && 
          new Date(s.data || s.startDate).toISOString().split('T')[0] === dataAnteriorStr
        );
        
        if (escalaOntem?.type === 3) { // Turno noturno
          return { disponivel: false, motivo: 'Trabalhou no turno noturno ontem' };
        }
      }
      
      return { disponivel: true };
    },

    // Calcular horas trabalhadas por funcionário
    calcularHorasFuncionario(userId, periodoInicio, periodoFim) {
      const shiftsFuncionario = this.shifts.filter(s => 
        s.userId === userId &&
        (!periodoInicio || new Date(s.data || s.startDate) >= new Date(periodoInicio)) &&
        (!periodoFim || new Date(s.data || s.startDate) <= new Date(periodoFim))
      );
      
      let totalHoras = 0;
      let horasExtras = 0;
      
      shiftsFuncionario.forEach(shift => {
        if (shift.startTime && shift.endTime && shift.type !== 7) {
          const inicio = this.parseTimeToMinutes(shift.startTime);
          const fim = this.parseTimeToMinutes(shift.endTime);
          let horas = (fim - inicio) / 60;
          
          if (horas < 0) horas += 24;
          
          // Subtrai intervalos
          const intervalos = this.breaks.filter(b => b.shiftId === shift.id);
          intervalos.forEach(intervalo => {
            const inicioIntervalo = this.parseTimeToMinutes(intervalo.startTime);
            const fimIntervalo = this.parseTimeToMinutes(intervalo.endTime);
            horas -= (fimIntervalo - inicioIntervalo) / 60;
          });
          
          totalHoras += horas;
          
          if (horas > 8) {
            horasExtras += horas - 8;
          }
        }
      });
      
      return { totalHoras, horasExtras, totalShifts: shiftsFuncionario.length };
    },

    // Navegar calendário
    navegarCalendario(direcao) {
      if (this.calendario.vista === 'mes') {
        if (direcao === 'anterior') {
          this.calendario.mesAtual--;
          if (this.calendario.mesAtual < 0) {
            this.calendario.mesAtual = 11;
            this.calendario.anoAtual--;
          }
        } else if (direcao === 'proximo') {
          this.calendario.mesAtual++;
          if (this.calendario.mesAtual > 11) {
            this.calendario.mesAtual = 0;
            this.calendario.anoAtual++;
          }
        }
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
        dataInicio: null,
        dataFim: null,
        userId: null,
        tipoTurno: null,
        apenasAtivos: true,
        busca: '',
      };
    },

    // Resetar store
    resetarStore() {
      this.shifts = [];
      this.templates = [];
      this.breaks = [];
      this.shiftAtual = null;
      this.funcionarios = [];
      this.error = null;
      this.calendario = {
        mesAtual: new Date().getMonth(),
        anoAtual: new Date().getFullYear(),
        vista: 'mes',
        dataSelecionada: new Date(),
      };
      this.limparFiltros();
    },

    // Limpar erro
    clearError() {
      this.error = null;
    },
  },

  // Persistência opcional
  persist: {
    key: 'shifts-store',
    paths: ['filtros', 'calendario', 'config'],
  },
});