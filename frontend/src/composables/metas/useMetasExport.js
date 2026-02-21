// composables/metas/useMetasExport.js
import { useMetasCalculos } from './useMetasCalculos';

export function useMetasExport() {
  const { formatarValor, formatarPercentual, formatarPeriodo, getStatusMeta } = useMetasCalculos();

  function exportarMetasCSV(metas) {
    const headers = ['Período', 'Meta (R$)', 'Realizado (R$)', '% Atingido', 'Diferença (R$)', 'Status'];
    const dados = metas.map(m => [
      formatarPeriodo(m.mes, m.ano),
      formatarValor(m.valorAlvo),
      formatarValor(m.realizado),
      formatarPercentual(m.percentualAlcancado),
      formatarValor(m.diferenca),
      getStatusMeta(m.percentualAlcancado).toUpperCase(),
    ]);
    
    const csvContent = [
      headers.join(';'),
      ...dados.map(row => row.join(';'))
    ].join('\n');
    
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `metas_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
  }

  function exportarMetasPDF(metas) {
    // Implementar exportação PDF
    console.log('Exportar PDF:', metas);
  }

  function exportarMetasExcel(metas) {
    // Implementar exportação Excel
    console.log('Exportar Excel:', metas);
  }

  return {
    exportarMetasCSV,
    exportarMetasPDF,
    exportarMetasExcel,
  };
}