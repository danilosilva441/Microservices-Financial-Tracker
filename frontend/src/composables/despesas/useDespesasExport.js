// composables/despesas/useDespesasExport.js
import { useDespesasStore } from '@/stores/despesas.store'

export const useDespesasExport = () => {
  const store = useDespesasStore()

  const gerarRelatorioDespesas = (dataInicio, dataFim) => {
    return store.gerarRelatorioDespesas(dataInicio, dataFim)
  }

  const exportarParaCSV = () => {
    store.exportarParaCSV()
  }

  const exportarParaPDF = async () => {
    // Implementação para PDF (requer biblioteca adicional)
    console.warn('Exportação para PDF não implementada')
  }

  const exportarParaExcel = () => {
    // Implementação para Excel (requer biblioteca adicional)
    console.warn('Exportação para Excel não implementada')
  }

  const imprimirRelatorio = (despesas, titulo = 'Relatório de Despesas') => {
    const janelaImpressao = window.open('', '_blank')
    
    const html = `
      <html>
        <head>
          <title>${titulo}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            h1 { color: #333; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th { background: #f4f4f4; text-align: left; padding: 8px; }
            td { padding: 8px; border-bottom: 1px solid #ddd; }
            .total { font-weight: bold; margin-top: 20px; }
          </style>
        </head>
        <body>
          <h1>${titulo}</h1>
          <p>Gerado em: ${new Date().toLocaleDateString('pt-BR')}</p>
          
          <table>
            <thead>
              <tr>
                <th>Data</th>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Valor</th>
              </tr>
            </thead>
            <tbody>
              ${despesas.map(d => `
                <tr>
                  <td>${store.formatarData(d.expenseDate || d.data)}</td>
                  <td>${d.description || '-'}</td>
                  <td>${store.getNomeCategoria(d.categoryId)}</td>
                  <td>${store.formatarValor(d.amount)}</td>
                </tr>
              `).join('')}
            </tbody>
          </table>
          
          <div class="total">
            Total: ${store.formatarValor(despesas.reduce((sum, d) => sum + (d.amount || 0), 0))}
          </div>
        </body>
      </html>
    `
    
    janelaImpressao.document.write(html)
    janelaImpressao.document.close()
    janelaImpressao.print()
  }

  return {
    gerarRelatorioDespesas,
    exportarParaCSV,
    exportarParaPDF,
    exportarParaExcel,
    imprimirRelatorio
  }
}