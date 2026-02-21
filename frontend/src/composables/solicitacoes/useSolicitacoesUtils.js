import { useSolicitacoesStore } from '@/stores/solicitacoes.store';

export function useSolicitacoesUtils() {
  const store = useSolicitacoesStore();

  const formatarData = (data) => {
    return store.formatarData(data);
  };

  const formatarDataRelativa = (data) => {
    return store.formatarDataRelativa(data);
  };

  const gerarResumoSolicitacao = (solicitacao) => {
    return store.gerarResumoSolicitacao(solicitacao);
  };

  const formatarValorMonetario = (valor) => {
    if (valor === null || valor === undefined) return '';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  const formatarCPFCNPJ = (valor) => {
    if (!valor) return '';
    const str = valor.toString().replace(/\D/g, '');
    
    if (str.length === 11) {
      return str.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    } else if (str.length === 14) {
      return str.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }
    return valor;
  };

  const formatarTelefone = (valor) => {
    if (!valor) return '';
    const str = valor.toString().replace(/\D/g, '');
    
    if (str.length === 11) {
      return str.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    } else if (str.length === 10) {
      return str.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    }
    return valor;
  };

  const compararDados = (dadosAntigos, dadosNovos) => {
    try {
      const antigos = typeof dadosAntigos === 'string' ? JSON.parse(dadosAntigos) : dadosAntigos;
      const novos = typeof dadosNovos === 'string' ? JSON.parse(dadosNovos) : dadosNovos;
      
      const diferencas = [];
      
      for (const key in novos) {
        if (antigos[key] !== novos[key]) {
          diferencas.push({
            campo: key,
            antigo: antigos[key],
            novo: novos[key],
          });
        }
      }
      
      return diferencas;
    } catch (error) {
      console.error('Erro ao comparar dados:', error);
      return [];
    }
  };

  const exportarParaCSV = (solicitacoes) => {
    const cabecalhos = [
      'ID',
      'Tipo',
      'Status',
      'Motivo',
      'Data Solicitação',
      'Faturamento',
      'Solicitante',
    ];

    const linhas = solicitacoes.map(s => [
      s.id,
      store.getNomeTipo(s.tipo),
      store.getNomeStatus(s.status),
      s.motivo,
      store.formatarData(s.dataSolicitacao || s.createdAt),
      s.faturamentoParcialId,
      s.usuarioId,
    ]);

    const csv = [
      cabecalhos.join(','),
      ...linhas.map(linha => linha.map(celula => 
        celula ? `"${celula.toString().replace(/"/g, '""')}"` : ''
      ).join(','))
    ].join('\n');

    const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `solicitacoes_${new Date().toISOString().split('T')[0]}.csv`);
    link.click();
    
    URL.revokeObjectURL(url);
  };

  return {
    formatarData,
    formatarDataRelativa,
    gerarResumoSolicitacao,
    formatarValorMonetario,
    formatarCPFCNPJ,
    formatarTelefone,
    compararDados,
    exportarParaCSV,
  };
}