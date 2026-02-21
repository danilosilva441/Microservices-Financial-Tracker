import { ref } from 'vue';

export function useExport() {
  const isExporting = ref(false);
  const exportProgress = ref(0);
  const exportError = ref(null);

  const exportToCSV = (data, filename = 'export.csv') => {
    if (!data || !data.length) return;
    
    isExporting.value = true;
    exportProgress.value = 0;
    
    try {
      const headers = Object.keys(data[0]).join(',');
      const rows = data.map(item => 
        Object.values(item)
          .map(value => {
            if (typeof value === 'string' && value.includes(',')) {
              return `"${value}"`;
            }
            return value;
          })
          .join(',')
      );
      
      const csv = [headers, ...rows].join('\n');
      const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      
      link.href = URL.createObjectURL(blob);
      link.download = filename;
      link.click();
      URL.revokeObjectURL(link.href);
      
      exportProgress.value = 100;
    } catch (error) {
      console.error('Erro ao exportar CSV:', error);
      exportError.value = error.message;
    } finally {
      setTimeout(() => {
        isExporting.value = false;
        exportProgress.value = 0;
      }, 1000);
    }
  };

  const exportToJSON = (data, filename = 'export.json') => {
    isExporting.value = true;
    
    try {
      const json = JSON.stringify(data, null, 2);
      const blob = new Blob([json], { type: 'application/json' });
      const link = document.createElement('a');
      
      link.href = URL.createObjectURL(blob);
      link.download = filename;
      link.click();
      URL.revokeObjectURL(link.href);
    } catch (error) {
      console.error('Erro ao exportar JSON:', error);
      exportError.value = error.message;
    } finally {
      isExporting.value = false;
    }
  };

  const exportToPDF = async (data, options = {}) => {
    isExporting.value = true;
    
    try {
      // Implementar lógica de exportação PDF
      // Pode usar bibliotecas como jsPDF ou pdfmake
      console.log('Exportando para PDF:', data, options);
    } catch (error) {
      console.error('Erro ao exportar PDF:', error);
      exportError.value = error.message;
    } finally {
      isExporting.value = false;
    }
  };

  const prepareFechamentosForExport = (fechamentos, tipo = 'simples') => {
    return fechamentos.map(f => {
      const base = {
        ID: f.id,
        Data: new Date(f.data).toLocaleDateString('pt-BR'),
        'Valor Total': f.valorTotal,
        'Valor Conferido': f.valorConferido,
        Diferença: f.diferenca,
        Status: f.statusCaixa,
        Observações: f.observacoes || '',
        'Fechado Por': f.fechadoPorNome || '',
        'Fechado Em': f.fechadoEm ? new Date(f.fechadoEm).toLocaleString('pt-BR') : '',
        'Conferido Por': f.conferidoPorNome || '',
        'Conferido Em': f.conferidoEm ? new Date(f.conferidoEm).toLocaleString('pt-BR') : '',
      };

      if (tipo === 'detalhado') {
        return {
          ...base,
          'Qtd. Itens': f.itens?.length || 0,
          'Qtd. Movimentações': f.movimentacoes?.length || 0,
        };
      }

      return base;
    });
  };

  return {
    isExporting,
    exportProgress,
    exportError,
    exportToCSV,
    exportToJSON,
    exportToPDF,
    prepareFechamentosForExport,
  };
}