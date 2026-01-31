import { ref, computed, watch } from 'vue';
import faturamentoService from '@/services/faturamentoService';

export function useCaixaHoje(unidadeId) {
  const loading = ref(false);
  const erro = ref(null);
  
  // Data selecionada no input (Padrão: Hoje YYYY-MM-DD)
  const dataSelecionada = ref(new Date().toISOString().split('T')[0]);
  
  // Objeto do caixa retornado pela API
  const caixaDiario = ref(null);
  const statusCaixa = ref(null); // 'NaoIniciado', 'Aberto', 'Fechado', 'Conferido'

  // --- Buscas ---
  const buscarCaixaDaData = async () => {
    if (!unidadeId) return;
    
    loading.value = true;
    erro.value = null;
    caixaDiario.value = null;
    statusCaixa.value = 'NaoIniciado';

    try {
      // Busca lista filtrada pela data. A API retorna array.
      const response = await faturamentoService.listarFechamentos(unidadeId, {
        dataInicio: dataSelecionada.value,
        dataFim: dataSelecionada.value
      });

      if (response && response.length > 0) {
        caixaDiario.value = response[0];
        statusCaixa.value = caixaDiario.value.status; // 'Aberto', 'Fechado', etc.
      } else {
        statusCaixa.value = 'NaoIniciado';
      }
    } catch (e) {
      console.error("Erro ao buscar caixa:", e);
      erro.value = "Erro ao carregar dados do caixa.";
    } finally {
      loading.value = false;
    }
  };

  // --- Ações ---
  const iniciarDia = async (fundoDeCaixa) => {
    loading.value = true;
    try {
      const novoCaixa = await faturamentoService.iniciarDia(unidadeId, {
        data: dataSelecionada.value,
        fundoDeCaixa: Number(fundoDeCaixa),
        observacoes: "Iniciado via Sistema"
      });
      caixaDiario.value = novoCaixa;
      statusCaixa.value = 'Aberto';
    } catch (e) {
      erro.value = e.response?.data?.message || "Erro ao iniciar o dia.";
      throw e;
    } finally {
      loading.value = false;
    }
  };

  const realizarLancamento = async (payloadLancamento) => {
    if (!caixaDiario.value?.id) return;
    
    // A API de lançamentos parciais (que você já tinha) deve ser chamada aqui.
    // Assumindo que você tem um endpoint para adicionar parcial no faturamentoService ou api
    // Vou usar a estrutura genérica, ajuste conforme seu endpoint de parciais
    try {
      // payloadLancamento deve conter: { valor, origem, metodoPagamentoId, ... }
      // Nota: Precisamos garantir que a data do lançamento bata com a data do caixa
      // ou a API trata isso.
      
      // Aqui estou chamando o endpoint de criar parcial (que você testou nos logs anteriores)
      await faturamentoService.adicionarLancamento(unidadeId, caixaDiario.value.id, payloadLancamento);
      
      // Recarrega para atualizar totais
      await buscarCaixaDaData();
    } catch (e) {
      console.error(e);
      throw new Error("Erro ao realizar lançamento.");
    }
  };

  // Observa mudança de data para recarregar
  watch(dataSelecionada, () => {
    buscarCaixaDaData();
  });

  return {
    loading,
    erro,
    dataSelecionada,
    caixaDiario,
    statusCaixa,
    buscarCaixaDaData,
    iniciarDia,
    realizarLancamento
  };
}