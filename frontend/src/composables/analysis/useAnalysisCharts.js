// src/composables/analysis/useAnalysisCharts.js

import { storeToRefs } from 'pinia';
import { computed } from 'vue';
import { useAnalysisStore } from '@/stores/analysis.store';

export function useAnalysisCharts() {
  const store = useAnalysisStore();
  
  const {
    formattedBarChartData,
    formattedPieChartData,
    formattedTrendChartData,
    formattedSazonalidadeData,
    graficos,
  } = storeToRefs(store);
  
  // Verifica se há dados em cada gráfico
  const hasBarChartData = computed(() => {
    return formattedBarChartData.value?.datasets?.some(d => 
      d.data?.some(v => v > 0)
    ) || false;
  });
  
  const hasPieChartData = computed(() => {
    return formattedPieChartData.value?.datasets[0]?.data?.some(v => v > 0) || false;
  });
  
  const hasTrendChartData = computed(() => {
    return formattedTrendChartData.value?.datasets?.some(d => 
      d.data?.some(v => v > 0)
    ) || false;
  });
  
  const hasSazonalidadeData = computed(() => {
    return formattedSazonalidadeData.value?.datasets[0]?.data?.some(v => v > 0) || false;
  });
  
  // Opções padrão para gráficos
  const chartDefaultOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          usePointStyle: true,
          boxWidth: 8,
          padding: 20,
        },
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        callbacks: {
          label: (context) => {
            let label = context.dataset.label || '';
            if (label) label += ': ';
            if (context.parsed.y !== undefined) {
              label += new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL',
              }).format(context.parsed.y);
            } else if (context.parsed !== undefined) {
              label += new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL',
              }).format(context.parsed);
            }
            return label;
          },
        },
      },
    },
  };
  
  // Dados resumidos para cards
  const chartsSummary = computed(() => {
    return {
      bar: {
        hasData: hasBarChartData.value,
        datasets: formattedBarChartData.value?.datasets?.length || 0,
        labels: formattedBarChartData.value?.labels?.length || 0,
      },
      pie: {
        hasData: hasPieChartData.value,
        segments: formattedPieChartData.value?.labels?.length || 0,
      },
      trend: {
        hasData: hasTrendChartData.value,
        period: formattedTrendChartData.value?.labels?.length || 0,
      },
      sazonalidade: {
        hasData: hasSazonalidadeData.value,
        months: formattedSazonalidadeData.value?.labels?.length || 0,
      },
    };
  });
  
  return {
    // Dados dos gráficos
    formattedBarChartData,
    formattedPieChartData,
    formattedTrendChartData,
    formattedSazonalidadeData,
    graficos,
    
    // Status dos gráficos
    hasBarChartData,
    hasPieChartData,
    hasTrendChartData,
    hasSazonalidadeData,
    chartsSummary,
    
    // Opções
    chartDefaultOptions,
    
    // Dados brutos
    rawChartData: graficos,
  };
}