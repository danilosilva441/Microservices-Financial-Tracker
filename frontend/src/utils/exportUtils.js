/**
 * Utilitários para exportação de dados
 */

/**
 * Exporta dados para CSV
 * @param {Object} data - Dados a serem exportados
 * @param {string} filename - Nome do arquivo
 */
export const exportToCSV = (data, filename = 'export') => {
  return new Promise((resolve) => {
    // Converter objeto para CSV
    const csvContent = objectToCSV(data);
    
    // Criar blob e link de download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `${filename}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    resolve();
  });
};

/**
 * Exporta dados para PDF (simplificado - em produção use uma biblioteca como jsPDF)
 * @param {Object} data - Dados a serem exportados
 * @param {string} filename - Nome do arquivo
 */
export const exportToPDF = async (data, filename = 'export') => {
  // Em produção, implemente com jsPDF ou similar
  console.log('Exportando para PDF:', data);
  
  // Exemplo com jsPDF (descomente se instalar a biblioteca):
  /*
  const { jsPDF } = await import('jspdf');
  const doc = new jsPDF();
  
  doc.text('Dashboard KPIs', 10, 10);
  doc.text(`Período: ${data.periodo}`, 10, 20);
  doc.text(`Receita: ${data.kpis.receitaTotal}`, 10, 30);
  
  doc.save(`${filename}.pdf`);
  */
  
  // Fallback para download simples
  alert('Exportação PDF - Em desenvolvimento');
};

/**
 * Converte objeto para formato CSV
 * @param {Object} obj - Objeto a ser convertido
 * @returns {string} String CSV
 */
const objectToCSV = (obj) => {
  const flattenObject = (ob, prefix = '') => {
    return Object.keys(ob).reduce((acc, k) => {
      const pre = prefix.length ? prefix + '.' : '';
      if (typeof ob[k] === 'object' && ob[k] !== null && !Array.isArray(ob[k])) {
        Object.assign(acc, flattenObject(ob[k], pre + k));
      } else {
        acc[pre + k] = ob[k];
      }
      return acc;
    }, {});
  };

  const flatObj = flattenObject(obj);
  const headers = Object.keys(flatObj);
  const values = headers.map(header => JSON.stringify(flatObj[header], null, 0));
  
  return [headers.join(','), values.join(',')].join('\n');
};