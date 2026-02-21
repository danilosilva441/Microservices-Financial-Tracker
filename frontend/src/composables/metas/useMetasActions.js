// composables/metas/useMetasActions.js
import { useMetasStore } from './useMetasStore';

export function useMetasActions() {
  const store = useMetasStore();

  async function criarNovaMeta(unidadeId, metaData) {
    return await store.criarMeta(unidadeId, metaData);
  }

  async function atualizarMeta(metaId, metaData) {
    // Implementar lógica de atualização
    // Por enquanto, reutiliza criarMeta
    return await store.criarMeta(metaData.unidadeId, metaData);
  }

  async function excluirMeta(metaId) {
    // Implementar exclusão
    store.isLoading = true;
    try {
      // await MetasService.delete(metaId);
      store.metas = store.metas.filter(m => m.id !== metaId);
      store.calcularAnalises();
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    } finally {
      store.isLoading = false;
    }
  }

  async function carregarDadosIniciais(unidadeId) {
    await store.carregarMetas(unidadeId);
    await store.carregarMetaPeriodo(
      unidadeId,
      new Date().getMonth() + 1,
      new Date().getFullYear()
    );
  }

  return {
    criarNovaMeta,
    atualizarMeta,
    excluirMeta,
    carregarDadosIniciais,
  };
}