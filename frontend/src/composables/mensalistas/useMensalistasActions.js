// composables/mensalistas/useMensalistasActions.js
import { computed } from 'vue';

export function useMensalistasActions(store) {
  const criarMensalista = async (unidadeId, mensalistaData) => {
    return await store.criarMensalista(unidadeId, mensalistaData);
  };

  const atualizarMensalista = async (unidadeId, mensalistaId, dadosAtualizados) => {
    return await store.atualizarMensalista(unidadeId, mensalistaId, dadosAtualizados);
  };

  const toggleAtivoMensalista = async (unidadeId, mensalistaId) => {
    return await store.toggleAtivoMensalista(unidadeId, mensalistaId);
  };

  const buscarMensalistaPorId = async (unidadeId, mensalistaId) => {
    return await store.buscarMensalistaPorId(unidadeId, mensalistaId);
  };

  const buscarPorCPF = (cpf) => {
    return store.buscarPorCPF(cpf);
  };

  const carregarEmpresas = async () => {
    return await store.carregarEmpresas();
  };

  const importarMensalistas = async (unidadeId, arquivo) => {
    return await store.importarMensalistas(unidadeId, arquivo);
  };

  const gerarCobrancasMensais = async (unidadeId, mes, ano) => {
    return await store.gerarCobrancasMensais(unidadeId, mes, ano);
  };

  return {
    criarMensalista,
    atualizarMensalista,
    toggleAtivoMensalista,
    buscarMensalistaPorId,
    buscarPorCPF,
    carregarEmpresas,
    importarMensalistas,
    gerarCobrancasMensais,
  };
}