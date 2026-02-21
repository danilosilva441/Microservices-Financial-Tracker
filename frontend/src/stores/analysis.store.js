import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import analysisService from '@/services/analysis.service';
import { useNotificationStore } from '@/stores/notificationStore';

export const useAnalysisStore = defineStore('analysis', () => {
  // Estado
  const dashboardData = ref(null);
  const loading = ref(false);
  const error = ref(null);
  const filters = ref({
    startDate: null,
    endDate: null,
    unidadeId: null,
    periodo: 'mes_atual', // mes_atual, mes_anterior, trimestre, ano
    tipoGrafico: 'barra', // barra, pizza, linha
  });

  const cache = ref(new Map());
  const lastUpdated = ref(null);
  const autoRefreshInterval = ref(null);
  const refreshInterval = ref(300000); // 5 minutos em ms
  const isAutoRefreshEnabled = ref(false);

  // Importar store de notificações se disponível
  let notificationStore;
  try {
    notificationStore = useNotificationStore();
  } catch {
    // Store de notificações não disponível
  }

  // Getters (valores computados)
  const kpis = computed(() => dashboardData.value?.data?.kpis || {});
  const graficos = computed(() => dashboardData.value?.data?.graficos || {});
  const desempenho = computed(() => dashboardData.value?.data?.desempenho || {});
  const period = computed(() => dashboardData.value?.data?.period || {});
  const metadata = computed(() => dashboardData.value?.metadata || {});

  // Getters para métricas específicas
  const receitaTotal = computed(() => kpis.value.receitaTotal || 0);
  const lucroTotal = computed(() => kpis.value.lucroTotal || 0);
  const despesaTotal = computed(() => kpis.value.despesaTotal || 0);
  const metaTotal = computed(() => kpis.value.metaTotal || 0);
  const percentualMeta = computed(() => kpis.value.percentualMetaTotal || 0);
  const unidadesAtivas = computed(() => kpis.value.unidadesAtivas || 0);
  const melhorUnidade = computed(() => kpis.value.melhorUnidade || {});
  const piorUnidade = computed(() => kpis.value.piorUnidade || {});
  const projecao = computed(() => kpis.value.projecao || {});
  const benchmark = computed(() => kpis.value.benchmark || {});
  const analise = computed(() => kpis.value.analise || {});

  // Getters para gráficos formatados
  const formattedBarChartData = computed(() => {
    const rawData = graficos.value.barChartData;
    if (!rawData) return null;
    
    return {
      labels: rawData.labels,
      datasets: rawData.datasets.map(dataset => ({
        ...dataset,
        data: dataset.data.map(value => Number(value) || 0),
        // Adiciona configurações visuais
        borderWidth: 1,
        borderColor: dataset.backgroundColor + 'DD',
        hoverBackgroundColor: dataset.backgroundColor + 'CC',
      }))
    };
  });

  const formattedPieChartData = computed(() => {
    const rawData = graficos.value.pieChartData;
    if (!rawData) return null;
    
    const hasData = rawData.datasets[0].data.some(val => val > 0);
    if (!hasData) {
      return {
        labels: ['Sem dados disponíveis'],
        datasets: [{
          data: [1],
          backgroundColor: ['#e5e7eb'],
          borderWidth: 0
        }]
      };
    }
    
    return {
      labels: rawData.labels,
      datasets: [{
        ...rawData.datasets[0],
        data: rawData.datasets[0].data.map(value => Number(value) || 0),
        borderColor: '#ffffff',
        borderWidth: 2,
        hoverOffset: 15,
      }]
    };
  });

  const formattedTrendChartData = computed(() => {
    const rawData = graficos.value.trendChartData;
    if (!rawData) return null;
    
    // Se não houver dados, cria um gráfico vazio com labels de exemplo
    if (!rawData.labels || rawData.labels.length === 0) {
      const days = Array.from({ length: 30 }, (_, i) => `Dia ${i + 1}`);
      return {
        labels: days,
        datasets: [
          {
            label: 'Vendas Diárias',
            data: Array(30).fill(0),
            borderColor: '#3b82f6',
            backgroundColor: 'rgba(59, 130, 246, 0.1)',
            type: 'bar',
            order: 2,
          },
          {
            label: 'Acumulado',
            data: Array(30).fill(0),
            borderColor: '#10b981',
            borderWidth: 2,
            type: 'line',
            order: 1,
            tension: 0.4,
          }
        ]
      };
    }
    
    return {
      labels: rawData.labels,
      datasets: rawData.datasets.map(dataset => ({
        ...dataset,
        data: dataset.data.map(value => Number(value) || 0),
        fill: dataset.type === 'line',
      }))
    };
  });

  const formattedSazonalidadeData = computed(() => {
    const rawData = graficos.value.sazonalidadeChartData;
    if (!rawData) return null;
    
    return {
      labels: rawData.labels,
      datasets: [{
        ...rawData.datasets[0],
        data: rawData.datasets[0].data.map(value => Number(value) || 0),
        borderColor: '#6366f1',
        borderWidth: 2,
        backgroundColor: rawData.datasets[0].backgroundColor + '80',
      }]
    };
  });

  // Getters para análise de desempenho
  const sortedDesempenho = computed(() => {
    const allUnits = desempenho.value?.todas || [];
    return [...allUnits].sort((a, b) => {
      // Ordena por lucro (descendente)
      return (b.lucro || 0) - (a.lucro || 0);
    });
  });

  const unidadesComLucro = computed(() => 
    sortedDesempenho.value.filter(u => u.lucro > 0).length
  );

  const unidadesComPrejuizo = computed(() => 
    sortedDesempenho.value.filter(u => u.lucro < 0).length
  );

  const unidadesAcimaDaMeta = computed(() => 
    sortedDesempenho.value.filter(u => u.percentualMeta >= 100).length
  );

  const unidadesAbaixoDaMeta = computed(() => 
    sortedDesempenho.value.filter(u => u.percentualMeta < 100 && u.percentualMeta > 0).length
  );

  // Métricas de performance
  const ticketMedioGeral = computed(() => {
    const unidades = sortedDesempenho.value.filter(u => u.fechamentosCount > 0);
    if (unidades.length === 0) return 0;
    
    const totalTicket = unidades.reduce((sum, u) => sum + (u.ticketMedio || 0), 0);
    return totalTicket / unidades.length;
  });

  const crescimentoPeriodo = computed(() => analise.value.crescimentoPeriodo || 0);

  // Status da meta
  const metaStatus = computed(() => {
    const percentual = percentualMeta.value;
    if (percentual >= 100) return 'success';
    if (percentual >= 70) return 'warning';
    if (percentual > 0) return 'danger';
    return 'neutro';
  });

  const metaStatusText = computed(() => {
    const status = metaStatus.value;
    const textos = {
      success: 'Meta Atingida',
      warning: 'Meta Parcial',
      danger: 'Meta em Risco',
      neutro: 'Sem Dados'
    };
    return textos[status];
  });

  // Projeção
  const projecaoStatus = computed(() => projecao.value.status || 'neutro');
  const probabilidadeMeta = computed(() => projecao.value.probabilidade || 'Indefinida');

  // Cache key generator
  const getCacheKey = (filters) => {
    return JSON.stringify({
      startDate: filters.startDate,
      endDate: filters.endDate,
      unidadeId: filters.unidadeId,
      periodo: filters.periodo,
    });
  };

  // Ações
  const fetchDashboardData = async (customFilters = {}, forceRefresh = false) => {
    loading.value = true;
    error.value = null;
    
    try {
      // Combina filtros padrão com customizados
      const mergedFilters = { ...filters.value, ...customFilters };
      const cacheKey = getCacheKey(mergedFilters);

      // Verifica cache se não for força refresh
      if (!forceRefresh && cache.value.has(cacheKey)) {
        const cachedData = cache.value.get(cacheKey);
        // Verifica se o cache ainda é válido (menos de 5 minutos)
        const now = new Date();
        const cacheTime = new Date(cachedData.timestamp);
        const diffMinutes = (now - cacheTime) / (1000 * 60);
        
        if (diffMinutes < 5) {
          dashboardData.value = cachedData.data;
          lastUpdated.value = cachedData.timestamp;
          loading.value = false;
          
          // Notifica sucesso (cache)
          if (notificationStore) {
            notificationStore.addNotification({
              type: 'info',
              message: 'Dados carregados do cache',
              timeout: 3000,
            });
          }
          
          return cachedData.data;
        }
      }

      const response = await analysisService.getDashboardData(mergedFilters);
      
      if (response.success) {
        dashboardData.value = response;
        lastUpdated.value = new Date().toISOString();
        
        // Armazena no cache
        cache.value.set(cacheKey, {
          data: response,
          timestamp: lastUpdated.value,
        });
        
        // Limpa cache antigo (mantém apenas os 10 mais recentes)
        if (cache.value.size > 10) {
          const keys = Array.from(cache.value.keys()).slice(0, -10);
          keys.forEach(key => cache.value.delete(key));
        }

        // Notifica sucesso
        if (notificationStore) {
          notificationStore.addNotification({
            type: 'success',
            message: 'Dashboard atualizado com sucesso',
            timeout: 3000,
          });
        }

        return response;
      } else {
        throw new Error('Falha ao carregar dados do dashboard');
      }
    } catch (err) {
      error.value = err.message || 'Erro ao buscar dados do dashboard';
      
      // Notifica erro
      if (notificationStore) {
        notificationStore.addNotification({
          type: 'error',
          message: `Erro ao carregar dados: ${err.message}`,
          timeout: 5000,
        });
      }
      
      console.error('Erro no analysisStore:', err);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const updateFilters = (newFilters) => {
    const oldFilters = JSON.stringify(filters.value);
    filters.value = { ...filters.value, ...newFilters };
    
    // Se os filtros mudaram, limpa o cache específico
    if (oldFilters !== JSON.stringify(filters.value)) {
      clearSpecificCache();
    }
  };

  const resetFilters = () => {
    filters.value = {
      startDate: null,
      endDate: null,
      unidadeId: null,
      periodo: 'mes_atual',
      tipoGrafico: 'barra',
    };
    clearCache();
  };

  const clearCache = () => {
    cache.value.clear();
  };

  const clearSpecificCache = () => {
    // Remove apenas entradas que não correspondem aos filtros atuais
    const currentKey = getCacheKey(filters.value);
    const keysToDelete = [];
    
    for (const key of cache.value.keys()) {
      if (key !== currentKey) {
        keysToDelete.push(key);
      }
    }
    
    keysToDelete.forEach(key => cache.value.delete(key));
  };

  const refreshData = async () => {
    if (loading.value) return;
    return await fetchDashboardData({}, true);
  };

  const setAutoRefresh = (enabled, interval = null) => {
    if (autoRefreshInterval.value) {
      clearInterval(autoRefreshInterval.value);
      autoRefreshInterval.value = null;
    }

    isAutoRefreshEnabled.value = enabled;

    if (enabled) {
      const refreshTime = interval || refreshInterval.value;
      autoRefreshInterval.value = setInterval(() => {
        refreshData();
      }, refreshTime);
    }
  };

  const exportToCSV = () => {
    if (!dashboardData.value) {
      throw new Error('Nenhum dado disponível para exportação');
    }

    const data = dashboardData.value.data;
    let csvContent = "data:text/csv;charset=utf-8,";

    // Cabeçalhos
    csvContent += "Unidade,Meta,Receita,Despesa,Lucro,% Meta,Ticket Médio,Status\n";

    // Dados das unidades
    data.desempenho.todas.forEach(unidade => {
      const row = [
        `"${unidade.nome}"`,
        unidade.metaMensal,
        unidade.receita,
        unidade.despesa,
        unidade.lucro,
        unidade.percentualMeta,
        unidade.ticketMedio,
        unidade.status
      ].join(',');
      
      csvContent += row + "\n";
    });

    // Adiciona KPIs
    csvContent += "\nKPIs\n";
    csvContent += `Receita Total,${data.kpis.receitaTotal}\n`;
    csvContent += `Despesa Total,${data.kpis.despesaTotal}\n`;
    csvContent += `Lucro Total,${data.kpis.lucroTotal}\n`;
    csvContent += `Meta Total,${data.kpis.metaTotal}\n`;
    csvContent += `% Meta Total,${data.kpis.percentualMetaTotal}\n`;

    // Cria link de download
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", `dashboard_${new Date().toISOString().split('T')[0]}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const exportToJSON = () => {
    if (!dashboardData.value) {
      throw new Error('Nenhum dado disponível para exportação');
    }

    const dataStr = JSON.stringify(dashboardData.value.data, null, 2);
    const dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr);
    
    const link = document.createElement("a");
    link.setAttribute("href", dataUri);
    link.setAttribute("download", `dashboard_${new Date().toISOString().split('T')[0]}.json`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const resetData = () => {
    dashboardData.value = null;
    loading.value = false;
    error.value = null;
    clearCache();
  };

  // Watchers
watch(() => filters.value.periodo, (newPeriodo, oldPeriodo) => {
  // Só executa se o período realmente mudou (não executa na inicialização)
  if (newPeriodo && newPeriodo !== oldPeriodo) {
    adjustDatesForPeriod(newPeriodo);
  }
});

  // Funções auxiliares
  const adjustDatesForPeriod = (periodo) => {
    const today = new Date();
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    switch (periodo) {
      case 'mes_atual':
        filters.value.startDate = formatDate(firstDayOfMonth);
        filters.value.endDate = formatDate(lastDayOfMonth);
        break;
      case 'mes_anterior':
        const firstDayLastMonth = new Date(today.getFullYear(), today.getMonth() - 1, 1);
        const lastDayLastMonth = new Date(today.getFullYear(), today.getMonth(), 0);
        filters.value.startDate = formatDate(firstDayLastMonth);
        filters.value.endDate = formatDate(lastDayLastMonth);
        break;
      case 'trimestre':
        const quarter = Math.floor(today.getMonth() / 3);
        const firstDayOfQuarter = new Date(today.getFullYear(), quarter * 3, 1);
        filters.value.startDate = formatDate(firstDayOfQuarter);
        filters.value.endDate = formatDate(lastDayOfMonth);
        break;
      case 'ano':
        const firstDayOfYear = new Date(today.getFullYear(), 0, 1);
        filters.value.startDate = formatDate(firstDayOfYear);
        filters.value.endDate = formatDate(lastDayOfMonth);
        break;
    }
  };

  const formatDate = (date) => {
    return date.toISOString().split('T')[0];
  };

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value || 0);
  };

  const getStatusColor = (status) => {
    const colors = {
      success: '#10b981',
      warning: '#f59e0b',
      danger: '#ef4444',
      neutro: '#6b7280'
    };
    return colors[status] || colors.neutro;
  };

  // Inicialização
  const init = () => {
    console.log('Analysis Store inicializado');
    // Configura datas iniciais para o mês atual
    adjustDatesForPeriod('mes_atual');
  };

  // Limpeza ao destruir
  const cleanup = () => {
    if (autoRefreshInterval.value) {
      clearInterval(autoRefreshInterval.value);
    }
  };

  // Inicializa automaticamente
  init();

  return {
    // Estado
    dashboardData,
    loading,
    error,
    filters,
    lastUpdated,
    isAutoRefreshEnabled,
    refreshInterval,
    
    // Getters básicos
    kpis,
    graficos,
    desempenho,
    period,
    metadata,
    
    // Getters de métricas
    receitaTotal,
    lucroTotal,
    despesaTotal,
    metaTotal,
    percentualMeta,
    unidadesAtivas,
    melhorUnidade,
    piorUnidade,
    projecao,
    benchmark,
    analise,
    
    // Getters de gráficos
    formattedBarChartData,
    formattedPieChartData,
    formattedTrendChartData,
    formattedSazonalidadeData,
    
    // Getters de análise
    sortedDesempenho,
    unidadesComLucro,
    unidadesComPrejuizo,
    unidadesAcimaDaMeta,
    unidadesAbaixoDaMeta,
    ticketMedioGeral,
    crescimentoPeriodo,
    metaStatus,
    metaStatusText,
    projecaoStatus,
    probabilidadeMeta,
    
    // Ações principais
    fetchDashboardData,
    updateFilters,
    resetFilters,
    refreshData,
    
    // Cache management
    clearCache,
    clearSpecificCache,
    
    // Auto-refresh
    setAutoRefresh,
    
    // Exportação
    exportToCSV,
    exportToJSON,
    
    // Utilidades
    formatCurrency,
    getStatusColor,
    
    // Limpeza
    cleanup,
  };
});