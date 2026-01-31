<script setup>
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
  kpis: {
    type: Object,
    default: () => ({})
  }
});

const getStatusColor = (status) => {
  switch (status) {
    case 'alta': return 'text-green-500';
    case 'media': return 'text-orange-500';
    case 'baixa': return 'text-red-500';
    default: return 'text-yellow-500';
  }
};

const getStatusBorderColor = (status) => {
  switch (status) {
    case 'alta': return 'border-green-500';
    case 'media': return 'border-orange-500';
    case 'baixa': return 'border-red-500';
    default: return 'border-yellow-500';
  }
};
</script>

<template>
  <!-- KPIs Principais -->
  <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
    <!-- Faturamento Total -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-500 transform hover:-translate-y-1">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Faturamento Total</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-neutral-dark number-format kpi-value">
            {{ formatCurrency(kpis.faturamentoTotal || 0) }}
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext">
            Dia {{ kpis.diaAtual || 0 }}/{{ kpis.totalDiasMes || 30 }}
          </p>
        </div>
      </div>
    </div>

    <!-- Meta Atingida -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-yellow-500 transform hover:-translate-y-1">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Meta Atingida</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
             :class="{
               'text-green-500': (kpis.percentualMeta || 0) >= 100,
               'text-yellow-500': (kpis.percentualMeta || 0) >= 70 && (kpis.percentualMeta || 0) < 100,
               'text-red-500': (kpis.percentualMeta || 0) < 70
             }">
            {{ ((kpis.percentualMeta || 0)).toFixed(1) }}%
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
            {{ formatCurrency(kpis.faturamentoTotal || 0) }} / {{ formatCurrency(kpis.metaTotal || 0) }}
          </p>
        </div>
      </div>
    </div>

    <!-- Projeção do Mês -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
         :class="getStatusBorderColor(kpis.vaiBaterMeta)">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Projeção do Mês</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
             :class="getStatusColor(kpis.vaiBaterMeta)">
            {{ ((kpis.percentualProjetado || 0)).toFixed(1) }}%
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext number-format">
            {{ formatCurrency(kpis.projecaoFinalMes || 0) }} projetado
          </p>
        </div>
      </div>
    </div>
  </div>

  <!-- KPIs Secundários -->
  <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-4 sm:mb-6 lg:mb-8">
    <!-- Média Diária -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 border-blue-400 transform hover:-translate-y-1">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Média Diária Atual</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold text-blue-500 number-format kpi-value">
            {{ formatCurrency(kpis.mediaDiariaAtual || 0) }}
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext">Por dia útil</p>
        </div>
      </div>
    </div>

    <!-- Saldo Restante -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
         :class="(kpis.saldoRestante || 0) > 0 ? 'border-red-400' : 'border-green-400'">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Saldo para Meta</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
             :class="(kpis.saldoRestante || 0) > 0 ? 'text-red-500' : 'text-green-500'">
            {{ formatCurrency(kpis.saldoRestante || 0) }}
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext">Para atingir a meta</p>
        </div>
      </div>
    </div>

    <!-- Probabilidade -->
    <div class="modern-card bg-white p-4 sm:p-5 lg:p-6 rounded-xl shadow-card hover:shadow-lg transition-all duration-300 border-l-4 transform hover:-translate-y-1" 
         :class="getStatusBorderColor(kpis.vaiBaterMeta)">
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h3 class="text-xs sm:text-sm lg:text-base text-gray-500 mb-2 font-medium">Probabilidade</h3>
          <p class="text-xl sm:text-2xl lg:text-3xl font-bold number-format kpi-value" 
             :class="getStatusColor(kpis.vaiBaterMeta)">
            {{ kpis.vaiBaterMeta === 'alta' ? 'Alta' : kpis.vaiBaterMeta === 'media' ? 'Média' : 'Baixa' }}
          </p>
          <p class="text-xs text-gray-500 mt-2 kpi-subtext">
            {{ kpis.diasRestantes || 0 }} dias restantes
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.shadow-card {
  box-shadow: 
    0 1px 3px 0 rgba(0, 0, 0, 0.1),
    0 1px 2px 0 rgba(0, 0, 0, 0.06),
    0 0 0 1px rgba(0, 0, 0, 0.02);
}

.hover\:shadow-lg:hover {
  box-shadow: 
    0 10px 15px -3px rgba(0, 0, 0, 0.1),
    0 4px 6px -2px rgba(0, 0, 0, 0.05),
    0 0 0 1px rgba(0, 0, 0, 0.02);
}

.kpi-value {
  font-feature-settings: 'tnum';
  font-variant-numeric: tabular-nums;
  line-height: 1.2;
}

.kpi-subtext {
  opacity: 0.7;
}

.number-format {
  word-break: break-word;
  overflow-wrap: break-word;
  white-space: normal;
  line-height: 1.2;
}

@media (max-width: 640px) {
  .shadow-card {
    box-shadow: 
      0 1px 2px 0 rgba(0, 0, 0, 0.05),
      0 0 0 1px rgba(0, 0, 0, 0.02);
  }
  
  .kpi-value {
    font-size: 1.25rem !important;
    line-height: 1.3;
  }
}

@media (max-width: 480px) {
  .number-format {
    font-size: 1.1rem !important;
  }
  
  .modern-card {
    padding: 1rem !important;
  }
}
</style>