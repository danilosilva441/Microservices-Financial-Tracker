/**
 * Composable para exportação de dados (XLSX, CSV, PDF).
 */
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import * as XLSX from 'xlsx';

export function useExport() {
  
  const getTimestamp = () => {
    const now = new Date();
    return `${now.getFullYear()}${(now.getMonth()+1).toString().padStart(2,'0')}${now.getDate()}_${now.getHours()}${now.getMinutes()}`;
  };

  /**
   * Exporta dados para Excel (.xlsx)
   */
  const exportToExcel = (data, filename = 'relatorio') => {
    // Achata os dados se necessário ou usa direto se for array de objetos simples
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Dados");
    XLSX.writeFile(wb, `${filename}_${getTimestamp()}.xlsx`);
  };

  /**
   * Exporta dados para CSV
   */
  const exportToCSV = (data, filename = 'relatorio') => {
    if (!data.length) return;
    const headers = Object.keys(data[0]);
    const csvContent = [
      headers.join(','), // Cabeçalho
      ...data.map(row => headers.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','))
    ].join('\r\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", `${filename}_${getTimestamp()}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };
  
  // Helper para lidar com nulls no CSV
  const replacer = (key, value) => value === null ? '' : value;

  /**
   * Exporta KPIs e Tabela para PDF
   */
  const exportToPDF = (kpis, tableData, filename = 'dashboard') => {
    const doc = new jsPDF();
    
    // Título
    doc.setFontSize(18);
    doc.text('Relatório de Performance', 14, 22);
    doc.setFontSize(11);
    doc.text(`Gerado em: ${new Date().toLocaleString()}`, 14, 30);

    // Resumo de KPIs
    doc.setFillColor(240, 240, 240);
    doc.rect(14, 35, 180, 25, 'F');
    doc.setFontSize(12);
    doc.text(`Receita: R$ ${kpis.receitaTotal}`, 20, 45);
    doc.text(`Lucro: R$ ${kpis.lucroTotal}`, 20, 55);
    doc.text(`Meta Atingida: ${kpis.percentualMetaTotal}%`, 100, 45);

    // Tabela
    if (tableData.length > 0) {
        const columns = Object.keys(tableData[0]).map(key => ({ header: key.toUpperCase(), dataKey: key }));
        autoTable(doc, {
            startY: 65,
            head: [columns.map(c => c.header)],
            body: tableData.map(row => Object.values(row)),
            theme: 'grid',
            headStyles: { fillColor: [59, 130, 246] }
        });
    }

    doc.save(`${filename}_${getTimestamp()}.pdf`);
  };

  return { exportToExcel, exportToCSV, exportToPDF };
}