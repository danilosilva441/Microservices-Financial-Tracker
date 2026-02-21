// composables/metas/useMetasAnalises.js
import { useMetasCalculos } from './useMetasCalculos';

export function useMetasAnalises() {
  const { calcularRealizadoMock, getNomeMes } = useMetasCalculos();

  function calcularEstatisticasAno(metasComDesempenho, ano) {
    const metasAno = metasComDesempenho.filter(m => m.ano === ano);
    
    if (metasAno.length === 0) {
      return {
        totalMetas: 0,
        totalRealizado: 0,
        totalMeta: 0,
        percentualGeral: 0,
        metasAtingidas: 0,
        mediaPercentual: 0,
      };
    }
    
    const totalMeta = metasAno.reduce((sum, m) => sum + m.valorAlvo, 0);
    const totalRealizado = metasAno.reduce((sum, m) => sum + (m.realizado || 0), 0);
    const percentualGeral = totalMeta > 0 ? (totalRealizado / totalMeta) * 100 : 0;
    const metasAtingidas = metasAno.filter(m => m.percentualAlcancado >= 100).length;
    const mediaPercentual = metasAno.reduce((sum, m) => sum + m.percentualAlcancado, 0) / metasAno.length;
    
    return {
      totalMetas: metasAno.length,
      totalRealizado,
      totalMeta,
      percentualGeral,
      metasAtingidas,
      mediaPercentual,
    };
  }

  function calcularPrevisaoProximoMes(metas) {
    const hoje = new Date();
    const mesAtual = hoje.getMonth() + 1;
    const anoAtual = hoje.getFullYear();
    
    const historico = [];
    for (let i = 1; i <= 3; i++) {
      let mes = mesAtual - i;
      let ano = anoAtual;
      
      if (mes <= 0) {
        mes += 12;
        ano -= 1;
      }
      
      const meta = metas.find(m => m.mes === mes && m.ano === ano);
      if (meta) {
        const realizado = calcularRealizadoMock(mes, ano, metas) || 0;
        historico.push({
          mes,
          realizado,
          percentual: meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0,
        });
      }
    }
    
    if (historico.length === 0) return 0;
    
    const mediaPercentual = historico.reduce((sum, h) => sum + h.percentual, 0) / historico.length;
    
    let proximoMes = mesAtual + 1;
    let proximoAno = anoAtual;
    if (proximoMes > 12) {
      proximoMes = 1;
      proximoAno += 1;
    }
    
    const metaProximoMes = metas.find(m => m.mes === proximoMes && m.ano === proximoAno);
    if (!metaProximoMes) return 0;
    
    return metaProximoMes.valorAlvo * (mediaPercentual / 100);
  }

  function calcularAnalisesCompletas(metas) {
    const metasComDesempenho = metas.map(meta => {
      const realizado = calcularRealizadoMock(meta.mes, meta.ano, metas) || 0;
      const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
      return { ...meta, realizado, percentual };
    });
    
    if (metasComDesempenho.length === 0) {
      return {
        melhorMes: null,
        piorMes: null,
        mediaAlcance: 0,
        totalMetasAno: 0,
        metasAtingidas: 0,
        metasNaoAtingidas: 0,
        previsaoProximoMes: 0,
      };
    }
    
    const melhorMes = [...metasComDesempenho].sort((a, b) => b.percentual - a.percentual)[0];
    const piorMes = [...metasComDesempenho].sort((a, b) => a.percentual - b.percentual)[0];
    const mediaAlcance = metasComDesempenho.reduce((sum, m) => sum + m.percentual, 0) / metasComDesempenho.length;
    
    const anoAtual = new Date().getFullYear();
    const metasAno = metasComDesempenho.filter(m => m.ano === anoAtual);
    const metasAtingidas = metasAno.filter(m => m.percentual >= 100).length;
    const metasNaoAtingidas = metasAno.filter(m => m.percentual < 100).length;
    
    const previsaoProximoMes = calcularPrevisaoProximoMes(metas);
    
    return {
      melhorMes,
      piorMes,
      mediaAlcance,
      totalMetasAno: metasAno.length,
      metasAtingidas,
      metasNaoAtingidas,
      previsaoProximoMes,
    };
  }

  return {
    calcularEstatisticasAno,
    calcularPrevisaoProximoMes,
    calcularAnalisesCompletas,
    getNomeMes,
  };
}