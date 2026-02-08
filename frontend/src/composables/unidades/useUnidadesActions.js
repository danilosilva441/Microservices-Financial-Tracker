// composables/unidades/useUnidadesActions.js
import { useUnidadesStore } from '@/stores/unidades.store';
import { useToast } from 'vue-toastification';
import "vue-toastification/dist/index.css";

export function useUnidadesActions() {
  const store = useUnidadesStore();
  const toast = useToast();

  const loadUnidades = async () => {
    console.log('🔍 [loadUnidades] Iniciando carregamento...');

    try {
      console.log('🔄 [loadUnidades] Chamando store.carregarUnidades()...');
      const unidades = await store.carregarUnidades();

      console.log('✅ [loadUnidades] Sucesso! Unidades carregadas:', unidades);
      console.log('📊 [loadUnidades] Quantidade:', unidades?.length || 0);

      toast.success(`${unidades.length} unidades carregadas`);
      return unidades;

    } catch (error) {
      console.error('❌ [loadUnidades] ERRO DETALHADO:');
      console.error('📝 Mensagem:', error.message);
      console.error('🔧 Stack:', error.stack);
      console.error('📊 Status HTTP:', error.response?.status);
      console.error('📋 Dados do erro:', error.response?.data);

      toast.error('Erro ao carregar unidades: ' + (error.response?.data?.message || error.message));
      throw error;
    }
  };

  const createUnidade = async (unidadeData) => {
    console.log('🎯 [COMPOSABLE] createUnidade chamado com dados:');
    console.log('📊 Dados recebidos:', unidadeData);
    console.log('📝 Dados formatados:', JSON.stringify(unidadeData, null, 2));

    try {
      console.log('🔄 [COMPOSABLE] Chamando store.criarUnidade()...');
      const result = await store.criarUnidade(unidadeData);

      console.log('📬 [COMPOSABLE] Resultado do store:', result);
      console.log('✅ Success:', result.success);
      console.log('📦 Data:', result.data);
      console.log('❌ Error:', result.error);

      if (result.success) {
        toast.success('Unidade criada com sucesso!');
        console.log('🎉 [COMPOSABLE] Sucesso! Unidade criada:', result.data);
        return result.data;
      } else {
        console.log('⚠️ [COMPOSABLE] Store retornou erro:', result.error);
        toast.error(result.error || 'Erro ao criar unidade');
        throw new Error(result.error);
      }
    } catch (error) {
      console.error('💥 [COMPOSABLE] Erro capturado no composable:');
      console.error('📝 Mensagem:', error.message);
      console.error('🔧 Stack:', error.stack);

      toast.error('Erro ao criar unidade: ' + error.message);
      throw error;
    }
  };

  const updateUnidade = async (id, dadosAtualizados) => {
    try {
      const result = await store.atualizarUnidade(id, dadosAtualizados);
      if (result.success) {
        toast.success('Unidade atualizada com sucesso!');
        return result.data;
      } else {
        toast.error(result.error || 'Erro ao atualizar unidade');
        throw new Error(result.error);
      }
    } catch (error) {
      toast.error('Erro ao atualizar unidade');
      throw error;
    }
  };

  const deleteUnidade = async (id) => {
    try {
      const result = await store.deletarUnidade(id);
      if (result.success) {
        toast.success('Unidade removida com sucesso!');
        return result;
      } else {
        toast.error(result.error || 'Erro ao remover unidade');
        throw new Error(result.error);
      }
    } catch (error) {
      toast.error('Erro ao remover unidade');
      throw error;
    }
  };

  const deactivateUnidade = async (id) => {
    try {
      const result = await store.desativarUnidade(id);
      if (result.success) {
        toast.success('Unidade desativada com sucesso!');
        return result;
      } else {
        toast.error(result.error || 'Erro ao desativar unidade');
        throw new Error(result.error);
      }
    } catch (error) {
      toast.error('Erro ao desativar unidade');
      throw error;
    }
  };

  const activateUnidade = (id) => {
    store.ativarUnidadeLocal(id);
    toast.success('Unidade ativada com sucesso!');
  };

  const updateProjecaoFaturamento = async (id, valorProjecao) => {
    try {
      const result = await store.atualizarProjecaoFaturamento(id, valorProjecao);
      if (result.success) {
        toast.success('Projeção de faturamento atualizada!');
        return result.data;
      } else {
        toast.error(result.error || 'Erro ao atualizar projeção');
        throw new Error(result.error);
      }
    } catch (error) {
      toast.error('Erro ao atualizar projeção');
      throw error;
    }
  };

  const getUnidade = async (id) => {
    try {
      const unidade = await store.buscarUnidadePorId(id);
      return unidade;
    } catch (error) {
      toast.error('Erro ao buscar unidade');
      throw error;
    }
  };

  const resetCurrentUnidade = () => {
    store.resetarUnidadeAtual();
  };

  const clearError = () => {
    store.clearError();
  };

  return {
    loadUnidades,
    createUnidade,
    updateUnidade,
    deleteUnidade,
    deactivateUnidade,
    activateUnidade,
    updateProjecaoFaturamento,
    getUnidade,
    resetCurrentUnidade,
    clearError,
  };
}