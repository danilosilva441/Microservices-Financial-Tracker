// composables/metas/useMetasCalculos.js
export function useMetasCalculos() {
  
  function calcularRealizadoMock(mes, ano, metas) {
    const meta = metas.find(m => m.mes === mes && m.ano === ano);
    if (!meta) return 0;
    
    const hoje = new Date();
    const isMesAtual = mes === hoje.getMonth() + 1 && ano === hoje.getFullYear();
    
    if (isMesAtual) {
      const diasNoMes = new Date(ano, mes, 0).getDate();
      const diasDecorridos = hoje.getDate();
      const percentualEsperado = (diasDecorridos / diasNoMes) * 100;
      
      const variacao = 0.8 + Math.random() * 0.4;
      return meta.valorAlvo * (percentualEsperado / 100) * variacao;
    } else {
      const alcance = 0.7 + Math.random() * 0.6;
      return meta.valorAlvo * alcance;
    }
  }

  function getStatusMeta(percentual, alertaPercentual = 80) {
    if (percentual >= 100) return 'atingida';
    if (percentual >= alertaPercentual) return 'em_andamento';
    if (percentual >= 50) return 'em_risco';
    return 'nao_atingida';
  }

  function getCorStatus(status, config) {
    switch (status) {
      case 'atingida': return config?.corMetaAtingida || '#4caf50';
      case 'em_andamento': return config?.corMetaEmAndamento || '#2196f3';
      case 'em_risco': return config?.corMetaEmRisco || '#ff9800';
      case 'nao_atingida': return config?.corMetaNaoAtingida || '#f44336';
      default: return '#9e9e9e';
    }
  }

  function getNomeMes(mes) {
    const meses = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
      'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    return meses[mes - 1] || '';
  }

  function formatarValor(valor) {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(valor || 0);
  }

  function formatarPercentual(valor) {
    return `${(valor || 0).toFixed(1)}%`;
  }

  function formatarPeriodo(mes, ano) {
    return `${getNomeMes(mes)}/${ano}`;
  }

  function calcularSugestaoMeta(metas, mes, ano) {
    const hoje = new Date();
    const mesAnterior = mes === 1 ? 12 : mes - 1;
    const anoAnterior = mes === 1 ? ano - 1 : ano;
    
    const metaAnterior = metas.find(m => m.mes === mesAnterior && m.ano === anoAnterior);
    
    if (!metaAnterior) {
      const mesesEquivalentes = metas.filter(m => m.mes === mes && m.ano < ano);
      if (mesesEquivalentes.length > 0) {
        const media = mesesEquivalentes.reduce((sum, m) => sum + m.valorAlvo, 0) / mesesEquivalentes.length;
        return media * 1.1;
      }
      return 0;
    }
    
    const crescimento = calcularCrescimentoHistorico(metas, mes, ano);
    return metaAnterior.valorAlvo * (1 + crescimento);
  }

  function calcularCrescimentoHistorico(metas, mes, ano) {
    const historico = [];
    
    for (let i = 1; i <= 3; i++) {
      const anoHist = ano - i;
      const metaAtual = metas.find(m => m.mes === mes && m.ano === anoHist);
      const metaAnterior = metas.find(m => 
        m.mes === (mes === 1 ? 12 : mes - 1) && 
        m.ano === (mes === 1 ? anoHist - 1 : anoHist)
      );
      
      if (metaAtual && metaAnterior && metaAnterior.valorAlvo > 0) {
        const crescimento = (metaAtual.valorAlvo - metaAnterior.valorAlvo) / metaAnterior.valorAlvo;
        historico.push(crescimento);
      }
    }
    
    if (historico.length === 0) return 0.05;
    
    const media = historico.reduce((sum, c) => sum + c, 0) / historico.length;
    return Math.max(0.02, Math.min(0.15, media));
  }

  return {
    calcularRealizadoMock,
    getStatusMeta,
    getCorStatus,
    getNomeMes,
    formatarValor,
    formatarPercentual,
    formatarPeriodo,
    calcularSugestaoMeta,
    calcularCrescimentoHistorico,
  };
}