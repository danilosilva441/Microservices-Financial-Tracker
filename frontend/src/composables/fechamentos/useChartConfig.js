import { computed } from 'vue';

export function useChartConfig() {
  const chartColors = {
    primary: '#3B82F6',
    secondary: '#F97316',
    success: '#22C55E',
    danger: '#EF4444',
    warning: '#F59E0B',
    info: '#06B6D4',
    purple: '#A855F7',
    pink: '#EC4899',
  };

  const chartDefaults = {
    responsive: true,
    maintainAspectRatio: false,
  };

  const getLineChartOptions = (title = '') => ({
    ...chartDefaults,
    plugins: {
      legend: {
        position: 'top',
      },
      title: {
        display: !!title,
        text: title,
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function(value) {
            return 'R$ ' + value.toLocaleString('pt-BR');
          },
        },
      },
    },
  });

  const getBarChartOptions = (title = '') => ({
    ...chartDefaults,
    plugins: {
      legend: {
        position: 'top',
      },
      title: {
        display: !!title,
        text: title,
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function(value) {
            return 'R$ ' + value.toLocaleString('pt-BR');
          },
        },
      },
    },
  });

  const getPieChartOptions = (title = '') => ({
    ...chartDefaults,
    plugins: {
      legend: {
        position: 'right',
      },
      title: {
        display: !!title,
        text: title,
      },
    },
  });

  const prepareTimeSeriesData = (fechamentos, campo = 'valorTotal') => {
    const dados = fechamentos
      .sort((a, b) => new Date(a.data) - new Date(b.data))
      .map(f => ({
        x: new Date(f.data),
        y: f[campo] || 0,
      }));

    return {
      datasets: [
        {
          label: 'Faturamento Diário',
          data: dados,
          borderColor: chartColors.primary,
          backgroundColor: chartColors.primary + '20',
          tension: 0.4,
        },
      ],
    };
  };

  const prepareStatusDistribution = (distribuicao) => ({
    labels: ['Abertos', 'Fechados', 'Conferidos', 'Pendentes'],
    datasets: [
      {
        data: [
          distribuicao.abertos,
          distribuicao.fechados,
          distribuicao.conferidos,
          distribuicao.pendentes,
        ],
        backgroundColor: [
          chartColors.primary,
          chartColors.warning,
          chartColors.success,
          chartColors.danger,
        ],
      },
    ],
  });

  const prepareComparativoMensal = (fechamentos, meses = 6) => {
    const hoje = new Date();
    const dados = [];
    
    for (let i = meses - 1; i >= 0; i--) {
      const mes = new Date(hoje.getFullYear(), hoje.getMonth() - i, 1);
      const mesStr = mes.toLocaleDateString('pt-BR', { month: 'short', year: '2-digit' });
      
      const fechamentosMes = fechamentos.filter(f => {
        const dataF = new Date(f.data);
        return dataF.getMonth() === mes.getMonth() && 
               dataF.getFullYear() === mes.getFullYear();
      });
      
      const total = fechamentosMes.reduce((sum, f) => sum + (f.valorTotal || 0), 0);
      
      dados.push({
        mes: mesStr,
        faturamento: total,
        diferencas: fechamentosMes.reduce((sum, f) => sum + Math.abs(f.diferenca || 0), 0),
      });
    }
    
    return {
      labels: dados.map(d => d.mes),
      datasets: [
        {
          label: 'Faturamento',
          data: dados.map(d => d.faturamento),
          backgroundColor: chartColors.primary,
        },
        {
          label: 'Diferenças',
          data: dados.map(d => d.diferencas),
          backgroundColor: chartColors.danger,
        },
      ],
    };
  };

  return {
    chartColors,
    getLineChartOptions,
    getBarChartOptions,
    getPieChartOptions,
    prepareTimeSeriesData,
    prepareStatusDistribution,
    prepareComparativoMensal,
  };
}