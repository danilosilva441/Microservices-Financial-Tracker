// composables/mensalistas/useMensalistasRelatorio.js
import { computed } from 'vue';

export function useMensalistasRelatorio(store) {
  const gerarRelatorioMensalistas = () => {
    store.gerarRelatorioMensalistas();
  };

  const exportarParaCSV = (dados, nomeArquivo) => {
    const headers = ['Nome', 'CPF', 'Valor', 'Status', 'Empresa', 'Data Cadastro'];
    const linhas = dados.map(item => [
      item.nome,
      store.formatarCPF(item.cpf),
      store.formatarValor(item.valorMensalidade),
      item.isAtivo ? 'Ativo' : 'Inativo',
      store.empresas.find(e => e.id === item.empresaId)?.nome || 'Particular',
      store.formatarData(item.dataCriacao),
    ]);

    const csvContent = [
      headers.join(';'),
      ...linhas.map(linha => linha.join(';'))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${nomeArquivo}_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
  };

  const dadosRelatorio = computed(() => {
    return store.mensalistasFiltrados.map(m => ({
      ...m,
      valorFormatado: store.formatarValor(m.valorMensalidade),
      dataCadastroFormatada: store.formatarData(m.dataCriacao),
      empresaNome: store.empresas.find(e => e.id === m.empresaId)?.nome || 'Particular',
    }));
  });

  return {
    gerarRelatorioMensalistas,
    exportarParaCSV,
    dadosRelatorio,
  };
}