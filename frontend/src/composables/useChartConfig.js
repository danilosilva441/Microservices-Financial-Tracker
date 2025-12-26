import { computed } from 'vue';

export function useChartConfig() {
  const getResponsiveFontSize = () => {
    const width = typeof window !== 'undefined' ? window.innerWidth : 1024;
    return width < 640 ? 9 : 11;
  };

  // Configuração Base (existente)
  const baseChartOptions = computed(() => ({
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'top', labels: { boxWidth: 10, usePointStyle: true } }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: (value) => {
            if (value >= 1000000) return (value / 1000000).toFixed(1) + 'M';
            if (value >= 1000) return (value / 1000).toFixed(0) + 'K';
            return value;
          }
        }
      }
    }
  }));

  // ✅ NOVA CONFIGURAÇÃO: Para Gráfico de Tendência (Eixo Duplo)
  const trendChartOptions = computed(() => ({
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
      mode: 'index',
      intersect: false,
    },
    plugins: {
      legend: { position: 'top' },
      tooltip: {
        callbacks: {
          label: function(context) {
            let label = context.dataset.label || '';
            if (label) label += ': ';
            if (context.parsed.y !== null) {
              label += new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(context.parsed.y);
            }
            return label;
          }
        }
      }
    },
    scales: {
      y: {
        type: 'linear',
        display: true,
        position: 'left',
        title: { display: true, text: 'Venda Diária' },
        ticks: {
            callback: (val) => val >= 1000 ? `${(val/1000).toFixed(0)}k` : val
        }
      },
      y1: {
        type: 'linear',
        display: true,
        position: 'right',
        grid: { drawOnChartArea: false }, // Remove grade do eixo secundário para não poluir
        title: { display: true, text: 'Acumulado' },
        ticks: {
            callback: (val) => val >= 1000 ? `${(val/1000).toFixed(0)}k` : val
        }
      },
      x: {
        ticks: { maxRotation: 45, minRotation: 45 }
      }
    }
  }));

  return {
    baseChartOptions,
    trendChartOptions // Exportando a nova config
  };
}