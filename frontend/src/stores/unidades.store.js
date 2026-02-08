// unidades.store.js
import { defineStore } from 'pinia';
import UnidadeService from '../services/unidades.service';
import { formatarDataParaAPI } from '../utils/dateFormatter';

export const useUnidadesStore = defineStore('unidades', {
  state: () => ({
    unidades: [],
    unidadeAtual: null,
    isLoading: false,
    error: null,
    filtros: {
      ativas: true,
      inativas: false,
      busca: '',
    },
    paginacao: {
      pagina: 1,
      limite: 10,
      total: 0,
      totalPaginas: 0,
    },
  }),

  getters: {
    // Unidades filtradas
    unidadesFiltradas: (state) => {
      let unidades = state.unidades;

      // Filtro por status
      if (!state.filtros.ativas && !state.filtros.inativas) {
        unidades = [];
      } else if (!state.filtros.ativas) {
        unidades = unidades.filter(u => !u.isAtivo);
      } else if (!state.filtros.inativas) {
        unidades = unidades.filter(u => u.isAtivo);
      }

      // Filtro por busca
      if (state.filtros.busca) {
        const busca = state.filtros.busca.toLowerCase();
        unidades = unidades.filter(u =>
          u.nome?.toLowerCase().includes(busca) ||
          u.descricao?.toLowerCase().includes(busca) ||
          u.endereco?.toLowerCase().includes(busca)
        );
      }

      return unidades;
    },

    // Unidades ativas
    unidadesAtivas: (state) => state.unidades.filter(u => u.isAtivo),

    // Unidades inativas
    unidadesInativas: (state) => state.unidades.filter(u => !u.isAtivo),

    // Total de unidades
    totalUnidades: (state) => state.unidades.length,

    // Total de unidades ativas
    totalAtivas: (state) => state.unidades.filter(u => u.isAtivo).length,

    // Total de faturamento projetado
    totalFaturamentoProjetado: (state) => {
      return state.unidades.reduce((total, unidade) => {
        return total + (unidade.metaMensal || 0);
      }, 0);
    },

    // Média de faturamento projetado
    mediaFaturamentoProjetado: (state) => {
      const ativas = state.unidades.filter(u => u.isAtivo);
      if (ativas.length === 0) return 0;

      return ativas.reduce((total, unidade) => {
        return total + (unidade.metaMensal || 0);
      }, 0) / ativas.length;
    },

    // Unidades com vencimento próximo (30 dias)
    unidadesComVencimentoProximo: (state) => {
      const hoje = new Date();
      const trintaDias = new Date();
      trintaDias.setDate(hoje.getDate() + 30);

      return state.unidades.filter(u => {
        if (!u.dataFim || !u.isAtivo) return false;

        const dataFim = new Date(u.dataFim);
        return dataFim >= hoje && dataFim <= trintaDias;
      });
    },
  },

  actions: {
    // Carrega todas as unidades
    async carregarUnidades() {
      this.isLoading = true;
      this.error = null;

      try {
        console.log('📞 [STORE] Chamando UnidadeService.getAll()...');
        const response = await UnidadeService.getAll();

        console.log('📥 [STORE] Resposta bruta da API:', response);

        // 🔧 CORREÇÃO: Extrai o array de $values se existir
        let unidades = response;

        // Se a resposta tiver formato {$id, $values: [...]}
        if (response && response.$values && Array.isArray(response.$values)) {
          console.log('🔄 [STORE] Extraindo unidades de $values');
          unidades = response.$values;
        }
        // Ou se for apenas um array
        else if (Array.isArray(response)) {
          unidades = response;
        }
        // Se for um objeto com outra estrutura
        else if (response && typeof response === 'object') {
          console.log('🔍 [STORE] Analisando estrutura do objeto:', Object.keys(response));

          // Tenta encontrar array em outras propriedades
          for (const key in response) {
            if (Array.isArray(response[key])) {
              console.log(`✅ [STORE] Encontrado array em propriedade: ${key}`);
              unidades = response[key];
              break;
            }
          }
        }

        console.log('✅ [STORE] Unidades processadas:', unidades);
        console.log('📊 [STORE] Tipo final:', typeof unidades);
        console.log('🔢 [STORE] Quantidade final:', unidades?.length || 0);

        this.unidades = unidades || [];
        return this.unidades;

      } catch (error) {
        console.error('❌ [STORE] ERRO em carregarUnidades:', error);
        this.error = error.response?.data?.message || error.message || 'Erro ao carregar unidades';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Cria uma nova unidade
    async criarUnidade(unidadeData) {
      this.isLoading = true;
      this.error = null;

      try {
        console.log('🛒 Dados originais:', unidadeData);

        // 🔧 CORREÇÃO: Formatar datas para ISO 8601
        const dadosFormatados = {
          ...unidadeData,
          // Converte dataInicio para ISO string com hora
          dataInicio: unidadeData.dataInicio ?
            new Date(unidadeData.dataInicio + 'T00:00:00').toISOString() :
            null,
          // Se dataFim for string vazia, envia null
          dataFim: unidadeData.dataFim && unidadeData.dataFim.trim() !== '' ?
            new Date(unidadeData.dataFim + 'T00:00:00').toISOString() :
            null
        };

        console.log('📤 Dados formatados para envio:', dadosFormatados);

        // REMOVA O CAMPO dataFim SE FOR NULL
        if (dadosFormatados.dataFim === null) {
          delete dadosFormatados.dataFim;
        }

        console.log('🎯 Dados após remover dataFim null:', dadosFormatados);

        // Agora envia os dados formatados
        console.log('📞 Chamando UnidadeService.create()...');
        const novaUnidade = await UnidadeService.create(dadosFormatados);

        console.log('✅ Resposta da API:', novaUnidade);

        // Adiciona à lista local
        this.unidades.push(novaUnidade);

        console.log('🎯 Unidade adicionada localmente. Total:', this.unidades.length);

        // 🔥 RETORNO OBRIGATÓRIO!
        return { success: true, data: novaUnidade };

      } catch (error) {
        console.error('❌ ERRO ao criar unidade:', error);
        console.error('📊 Status HTTP:', error.response?.status);
        console.error('📝 Mensagem do backend:', error.response?.data);

        // Mensagem detalhada para o usuário
        const errorMessage = error.response?.data?.message ||
          error.response?.data?.title ||
          error.message ||
          'Erro ao criar unidade';

        console.error('📢 Mensagem de erro final:', errorMessage);
        this.error = errorMessage;

        // 🔥 RETORNO OBRIGATÓRIO MESMO NO ERRO!
        return { success: false, error: errorMessage };

      } finally {
        this.isLoading = false;
        console.log('🏁 Função criarUnidade finalizada');
      }
    },

    // Busca detalhes de uma unidade específica
    async buscarUnidadePorId(id) {
      this.isLoading = true;
      this.error = null;

      try {
        const unidade = await UnidadeService.getById(id);
        this.unidadeAtual = unidade;
        return unidade;
      } catch (error) {
        console.error(`Erro ao buscar unidade ${id}:`, error);
        this.error = error.response?.data?.message || error.message || 'Erro ao buscar unidade';
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    // Atualiza uma unidade
    async atualizarUnidade(id, dadosAtualizados) {
      this.isLoading = true;
      this.error = null;

      try {
        const unidadeAtualizada = await UnidadeService.atualizarUnidade(id, dadosAtualizados);

        // Atualiza na lista local
        const index = this.unidades.findIndex(u => u.id === id);
        if (index !== -1) {
          this.unidades[index] = { ...this.unidades[index], ...unidadeAtualizada };
        }

        // Se for a unidade atual, atualiza também
        if (this.unidadeAtual?.id === id) {
          this.unidadeAtual = { ...this.unidadeAtual, ...unidadeAtualizada };
        }

        return { success: true, data: unidadeAtualizada };
      } catch (error) {
        console.error(`Erro ao atualizar unidade ${id}:`, error);
        this.error = error.response?.data?.message || error.message || 'Erro ao atualizar unidade';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Atualiza projeção de faturamento
    async atualizarProjecaoFaturamento(id, valorProjecao) {
      this.isLoading = true;
      this.error = null;

      try {
        const resultado = await UnidadeService.updateProjecao(id, valorProjecao);

        // Atualiza na lista local
        const index = this.unidades.findIndex(u => u.id === id);
        if (index !== -1) {
          this.unidades[index] = {
            ...this.unidades[index],
            projecaoFaturamento: valorProjecao
          };
        }

        return { success: true, data: resultado };
      } catch (error) {
        console.error(`Erro ao atualizar projeção da unidade ${id}:`, error);
        this.error = error.response?.data?.message || error.message || 'Erro ao atualizar projeção';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Desativa uma unidade
    async desativarUnidade(id) {
      this.isLoading = true;
      this.error = null;

      try {
        await UnidadeService.desativarUnidade(id);

        // Atualiza status na lista local
        const index = this.unidades.findIndex(u => u.id === id);
        if (index !== -1) {
          this.unidades[index] = { ...this.unidades[index], isAtivo: false };
        }

        return { success: true };
      } catch (error) {
        console.error(`Erro ao desativar unidade ${id}:`, error);
        this.error = error.response?.data?.message || error.message || 'Erro ao desativar unidade';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Ativa uma unidade (método local - se a API não tiver endpoint específico)
    ativarUnidadeLocal(id) {
      const index = this.unidades.findIndex(u => u.id === id);
      if (index !== -1) {
        this.unidades[index] = { ...this.unidades[index], isAtivo: true };
      }
    },

    // Deleta uma unidade (soft delete - API faz o trabalho)
    async deletarUnidade(id) {
      this.isLoading = true;
      this.error = null;

      try {
        // Nota: A API tem DELETE /api/unidades/{id}
        // Se você tiver um service method, use-o aqui
        // Por enquanto, apenas remove localmente
        await this.desativarUnidade(id); // Usa desativação como soft delete

        // Remove da lista local se realmente for deletar
        // this.unidades = this.unidades.filter(u => u.id !== id);

        return { success: true };
      } catch (error) {
        console.error(`Erro ao deletar unidade ${id}:`, error);
        this.error = error.response?.data?.message || error.message || 'Erro ao deletar unidade';
        return { success: false, error: this.error };
      } finally {
        this.isLoading = false;
      }
    },

    // Filtra unidades
    aplicarFiltros(filtros) {
      this.filtros = { ...this.filtros, ...filtros };
    },

    // Limpa filtros
    limparFiltros() {
      this.filtros = {
        ativas: true,
        inativas: false,
        busca: '',
      };
    },

    // Busca unidades por nome ou descrição
    buscarUnidades(termo) {
      this.filtros.busca = termo;
    },

    // Reseta a unidade atual
    resetarUnidadeAtual() {
      this.unidadeAtual = null;
    },

    // Limpa mensagens de erro
    clearError() {
      this.error = null;
    },

    // Busca unidade localmente (sem chamar API)
    getUnidadeLocal(id) {
      return this.unidades.find(u => u.id === id);
    },

    // Atualiza unidade localmente (otimista)
    atualizarUnidadeLocal(id, dados) {
      const index = this.unidades.findIndex(u => u.id === id);
      if (index !== -1) {
        this.unidades[index] = { ...this.unidades[index], ...dados };
      }

      if (this.unidadeAtual?.id === id) {
        this.unidadeAtual = { ...this.unidadeAtual, ...dados };
      }
    },

    // Adiciona unidade localmente (otimista)
    adicionarUnidadeLocal(unidade) {
      this.unidades.unshift(unidade); // Adiciona no início
    },
  },

  // Persistência com localStorage (opcional)
  persist: {
    key: 'unidades-store',
    paths: ['unidades', 'filtros', 'paginacao'],
  },
});