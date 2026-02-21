import { useFechamentosStore } from '../../stores/fechamentos.store';

export function useFechamentosActions() {
  const fechamentosStore = useFechamentosStore();

  const fecharCaixa = async (unidadeId, id, dados) => {
    return await fechamentosStore.fecharCaixa(unidadeId, id, dados);
  };

  const reabrirFechamento = async (unidadeId, id, motivo) => {
    return await fechamentosStore.reabrirFechamento(unidadeId, id, motivo);
  };

  const gerarRelatorioCaixa = (unidadeId, dataInicio, dataFim) => {
    return fechamentosStore.gerarRelatorioCaixa(unidadeId, dataInicio, dataFim);
  };

  const calcularEstatisticas = () => {
    fechamentosStore.calcularEstatisticas();
  };

  const atualizarDashboard = () => {
    fechamentosStore.atualizarDashboard();
  };

  return {
    fecharCaixa,
    reabrirFechamento,
    gerarRelatorioCaixa,
    calcularEstatisticas,
    atualizarDashboard,
  };
}