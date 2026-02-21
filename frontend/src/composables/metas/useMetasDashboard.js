// composables/metas/useMetasDashboard.js
import { useMetasCalculos } from './useMetasCalculos';

export function useMetasDashboard() {
  const { calcularRealizadoMock } = useMetasCalculos();

  function calcularDesempenhoAtual(metas, config) {
    const hoje = new Date();
    const mesAtual = hoje.getMonth() + 1;
    const anoAtual = hoje.getFullYear();
    
    const meta = metas.find(m => m.mes === mesAtual && m.ano === anoAtual);
    
    if (!meta) {
      return {
        metaAtual: 0,
        realizadoAtual: 0,
        percentualAlcancado: 0,
        diferenca: 0,
        projecaoFinal: 0,
        tendencia: 'estavel',
      };
    }
    
    const realizado = calcularRealizadoMock(mesAtual, anoAtual, metas) || 0;
    const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
    const diferenca = realizado - meta.valorAlvo;
    
    const diasNoMes = new Date(anoAtual, mesAtual, 0).getDate();
    const diasDecorridos = hoje.getDate();
    const projecaoFinal = diasDecorridos > 0 ? (realizado / diasDecorridos) * diasNoMes : 0;
    
    let tendencia = 'estavel';
    if (projecaoFinal > meta.valorAlvo * 1.1) {
      tendencia = 'crescente';
    } else if (projecaoFinal < meta.valorAlvo * 0.9) {
      tendencia = 'decrescente';
    }
    
    return {
      metaAtual: meta.valorAlvo,
      realizadoAtual: realizado,
      percentualAlcancado: percentual,
      diferenca,
      projecaoFinal,
      tendencia,
    };
  }

  function atualizarDashboardMetas(metas, config) {
    const hoje = new Date();
    const mesAtual = hoje.getMonth() + 1;
    const anoAtual = hoje.getFullYear();
    const diasNoMes = new Date(anoAtual, mesAtual, 0).getDate();
    const diasRestantes = diasNoMes - hoje.getDate();
    
    const meta = metas.find(m => m.mes === mesAtual && m.ano === anoAtual);
    const realizado = calcularRealizadoMock(mesAtual, anoAtual, metas) || 0;
    const percentual = meta?.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
    
    const diasDecorridos = hoje.getDate();
    const previsaoFimMes = diasDecorridos > 0 ? (realizado / diasDecorridos) * diasNoMes : 0;
    
    let status = 'em_andamento';
    if (percentual >= 100) {
      status = 'atingida';
    } else if (percentual < config.alertaPercentual) {
      status = 'em_risco';
    } else if (previsaoFimMes < (meta?.valorAlvo || 0) * 0.8) {
      status = 'nao_atingida';
    }
    
    return {
      metaMesAtual: meta?.valorAlvo || 0,
      realizadoMesAtual: realizado,
      percentualMesAtual: percentual,
      diasRestantes,
      previsaoFimMes,
      status,
    };
  }

  function identificarMetasEmRisco(metasComDesempenho, alertaPercentual) {
    const hoje = new Date();
    const diaAtual = hoje.getDate();
    const diasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();
    const percentualTempoDecorrido = (diaAtual / diasNoMes) * 100;
    
    return metasComDesempenho.filter(meta => {
      if (meta.mes !== hoje.getMonth() + 1 || meta.ano !== hoje.getFullYear()) {
        return false;
      }
      
      const percentualEsperado = Math.min(percentualTempoDecorrido * 1.1, 100);
      return meta.percentualAlcancado < percentualEsperado && meta.percentualAlcancado < 100;
    });
  }

  return {
    calcularDesempenhoAtual,
    atualizarDashboardMetas,
    identificarMetasEmRisco,
  };
}