// composables/metas/useMetasGraficos.js
import { useMetasCalculos } from './useMetasCalculos';

export function useMetasGraficos() {
  const { calcularRealizadoMock, getNomeMes } = useMetasCalculos();

  function gerarGraficoDesempenhoAnual(metas, metasAnoAtual) {
    const meses = Array.from({ length: 12 }, (_, i) => i + 1);
    
    return meses.map(mes => {
      const meta = metasAnoAtual.find(m => m.mes === mes);
      if (!meta) {
        return {
          mes,
          nomeMes: getNomeMes(mes),
          meta: 0,
          realizado: 0,
          percentual: 0,
        };
      }
      
      const realizado = calcularRealizadoMock(mes, meta.ano, metas) || 0;
      const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
      
      return {
        mes,
        nomeMes: getNomeMes(mes),
        meta: meta.valorAlvo,
        realizado,
        percentual,
      };
    });
  }

  function gerarGraficoComparativoAnual(metas, ano) {
    const metasAno = metas.filter(m => m.ano === ano);
    const meses = Array.from({ length: 12 }, (_, i) => i + 1);
    
    return {
      labels: meses.map(m => getNomeMes(m)),
      datasets: [
        {
          label: 'Meta',
          data: meses.map(m => {
            const meta = metasAno.find(meta => meta.mes === m);
            return meta?.valorAlvo || 0;
          }),
          backgroundColor: '#2196f3',
        },
        {
          label: 'Realizado',
          data: meses.map(m => {
            const meta = metasAno.find(meta => meta.mes === m);
            return meta ? (calcularRealizadoMock(m, ano, metas) || 0) : 0;
          }),
          backgroundColor: '#4caf50',
        },
      ],
    };
  }

  function gerarGraficoStatusMetas(metas) {
    const status = {
      atingidas: 0,
      emAndamento: 0,
      emRisco: 0,
      naoAtingidas: 0,
    };
    
    metas.forEach(meta => {
      const realizado = calcularRealizadoMock(meta.mes, meta.ano, metas) || 0;
      const percentual = meta.valorAlvo > 0 ? (realizado / meta.valorAlvo) * 100 : 0;
      
      if (percentual >= 100) status.atingidas++;
      else if (percentual >= 80) status.emAndamento++;
      else if (percentual >= 50) status.emRisco++;
      else status.naoAtingidas++;
    });
    
    return {
      labels: ['Atingidas', 'Em Andamento', 'Em Risco', 'Não Atingidas'],
      data: [status.atingidas, status.emAndamento, status.emRisco, status.naoAtingidas],
      colors: ['#4caf50', '#2196f3', '#ff9800', '#f44336'],
    };
  }

  return {
    gerarGraficoDesempenhoAnual,
    gerarGraficoComparativoAnual,
    gerarGraficoStatusMetas,
  };
}