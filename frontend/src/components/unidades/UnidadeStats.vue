<!-- components/unidades/UnidadeStats.vue -->
<template>
  <div class="w-full">
    <!-- Loading State -->
    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
      <div v-for="i in 4" :key="i" class="bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 p-6 animate-pulse">
        <div class="flex justify-between items-start mb-4">
          <div class="w-14 h-14 rounded-xl bg-gray-200 dark:bg-gray-700"></div>
          <div class="w-8 h-8 rounded-full bg-gray-200 dark:bg-gray-700"></div>
        </div>
        <div class="h-8 w-24 bg-gray-200 dark:bg-gray-700 rounded mb-2"></div>
        <div class="h-4 w-32 bg-gray-200 dark:bg-gray-700 rounded mb-4"></div>
        <div class="h-px bg-gray-200 dark:bg-gray-700 my-4"></div>
        <div class="flex justify-between">
          <div class="h-4 w-16 bg-gray-200 dark:bg-gray-700 rounded"></div>
          <div class="h-4 w-12 bg-gray-200 dark:bg-gray-700 rounded"></div>
        </div>
      </div>
    </div>

    <!-- Stats Grid -->
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
      <!-- Total de Unidades -->
      <div class="group relative bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 overflow-hidden">
        <!-- Barra Superior Colorida -->
        <div class="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-primary-500 to-secondary-500"></div>
        
        <div class="p-5 sm:p-6">
          <div class="flex items-start justify-between mb-4">
            <div class="w-12 h-12 sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500 flex items-center justify-center text-white shadow-lg group-hover:scale-110 transition-transform duration-300">
              <IconStore class="w-6 h-6 sm:w-7 sm:h-7" />
            </div>
            
            <div :class="[
              'flex items-center justify-center w-8 h-8 rounded-full text-xs',
              trendClass(stats.total)
            ]">
              <IconArrowUp v-if="stats.total > 0" class="w-4 h-4" />
              <IconMinus v-else class="w-4 h-4" />
            </div>
          </div>

          <div class="space-y-1">
            <div class="text-2xl sm:text-3xl font-bold text-gray-900 dark:text-white">
              {{ stats.total }}
            </div>
            <div class="text-xs sm:text-sm text-gray-600 dark:text-gray-400">
              Total de Unidades
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
            <div class="flex items-center justify-between text-xs sm:text-sm">
              <span class="text-gray-500 dark:text-gray-400">Ativas:</span>
              <span class="font-semibold text-gray-900 dark:text-white flex items-center gap-1">
                <span class="w-2 h-2 rounded-full bg-green-500"></span>
                {{ stats.ativas }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Faturamento Projetado -->
      <div class="group relative bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 overflow-hidden">
        <div class="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-green-500 to-emerald-500"></div>
        
        <div class="p-5 sm:p-6">
          <div class="flex items-start justify-between mb-4">
            <div class="w-12 h-12 sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-green-500 to-emerald-500 flex items-center justify-center text-white shadow-lg group-hover:scale-110 transition-transform duration-300">
              <IconChartLine class="w-6 h-6 sm:w-7 sm:h-7" />
            </div>
            
            <div :class="[
              'flex items-center justify-center w-8 h-8 rounded-full text-xs',
              trendClass(stats.faturamentoProjetado)
            ]">
              <IconArrowUp v-if="stats.faturamentoProjetado > 0" class="w-4 h-4" />
              <IconArrowDown v-else-if="stats.faturamentoProjetado < 0" class="w-4 h-4" />
              <IconMinus v-else class="w-4 h-4" />
            </div>
          </div>

          <div class="space-y-1">
            <div class="text-xl sm:text-2xl font-bold text-gray-900 dark:text-white truncate">
              {{ formatCurrency(stats.faturamentoProjetado) }}
            </div>
            <div class="text-xs sm:text-sm text-gray-600 dark:text-gray-400">
              Faturamento Projetado
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
            <div class="flex items-center justify-between text-xs sm:text-sm">
              <span class="text-gray-500 dark:text-gray-400">Média:</span>
              <span class="font-semibold text-gray-900 dark:text-white">
                {{ formatCurrency(stats.mediaFaturamento) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Vencimento Próximo -->
      <div class="group relative bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 overflow-hidden">
        <div class="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-yellow-500 to-amber-500"></div>
        
        <div class="p-5 sm:p-6">
          <div class="flex items-start justify-between mb-4">
            <div class="w-12 h-12 sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-yellow-500 to-amber-500 flex items-center justify-center text-white shadow-lg group-hover:scale-110 transition-transform duration-300">
              <IconClock class="w-6 h-6 sm:w-7 sm:h-7" />
            </div>
            
            <div :class="[
              'flex items-center justify-center w-8 h-8 rounded-full text-xs',
              trendClass(stats.vencimentoProximo)
            ]">
              <IconArrowUp v-if="stats.vencimentoProximo > 5" class="w-4 h-4" />
              <IconAlertCircle v-else-if="stats.vencimentoProximo > 0" class="w-4 h-4 text-yellow-600 dark:text-yellow-400" />
              <IconCheckCircle v-else class="w-4 h-4 text-green-600 dark:text-green-400" />
            </div>
          </div>

          <div class="space-y-1">
            <div class="text-2xl sm:text-3xl font-bold text-gray-900 dark:text-white">
              {{ stats.vencimentoProximo }}
            </div>
            <div class="text-xs sm:text-sm text-gray-600 dark:text-gray-400">
              Vencem em 30 dias
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
            <div class="flex items-center justify-between text-xs sm:text-sm">
              <span class="text-gray-500 dark:text-gray-400">Status:</span>
              <span :class="[
                'inline-flex items-center gap-1 font-semibold',
                stats.vencimentoProximo > 5 ? 'text-green-600 dark:text-green-400' :
                stats.vencimentoProximo > 0 ? 'text-yellow-600 dark:text-yellow-400' :
                'text-gray-600 dark:text-gray-400'
              ]">
                <IconAlertCircle v-if="stats.vencimentoProximo > 0 && stats.vencimentoProximo <= 5" class="w-3 h-3" />
                <IconCheckCircle v-else-if="stats.vencimentoProximo === 0" class="w-3 h-3" />
                {{ stats.vencimentoProximo > 0 ? 'Atenção' : 'OK' }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Taxa de Crescimento -->
      <div class="group relative bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg transition-all duration-300 hover:-translate-y-1 overflow-hidden">
        <div class="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-purple-500 to-violet-500"></div>
        
        <div class="p-5 sm:p-6">
          <div class="flex items-start justify-between mb-4">
            <div class="w-12 h-12 sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-purple-500 to-violet-500 flex items-center justify-center text-white shadow-lg group-hover:scale-110 transition-transform duration-300">
              <IconTrophy class="w-6 h-6 sm:w-7 sm:h-7" />
            </div>
            
            <div :class="[
              'flex items-center justify-center w-8 h-8 rounded-full text-xs',
              trendClass(growthRate - 20)
            ]">
              <IconArrowUp v-if="growthRate >= 20" class="w-4 h-4" />
              <IconArrowDown v-else class="w-4 h-4" />
            </div>
          </div>

          <div class="space-y-1">
            <div class="flex items-baseline gap-1">
              <span class="text-2xl sm:text-3xl font-bold text-gray-900 dark:text-white">{{ growthRate }}</span>
              <span class="text-lg sm:text-xl text-gray-500 dark:text-gray-400">%</span>
            </div>
            <div class="text-xs sm:text-sm text-gray-600 dark:text-gray-400">
              Taxa de Crescimento
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
            <div class="flex items-center justify-between text-xs sm:text-sm">
              <span class="text-gray-500 dark:text-gray-400">Meta 20%:</span>
              <span :class="[
                'font-semibold',
                growthRate >= 20 ? 'text-green-600 dark:text-green-400' : 'text-yellow-600 dark:text-yellow-400'
              ]">
                {{ growthRate >= 20 ? '✅ Atingida' : '⚠️ Em andamento' }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Insights (Opcional) -->
    <div v-if="!loading && stats.total > 0" class="mt-6 grid grid-cols-2 sm:grid-cols-4 gap-3 sm:gap-4">
      <div class="bg-gray-50 dark:bg-gray-800/50 rounded-lg p-3 text-center">
        <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Taxa de Ativação</div>
        <div class="text-base sm:text-lg font-bold text-gray-900 dark:text-white">
          {{ Math.round((stats.ativas / stats.total) * 100) }}%
        </div>
      </div>
      
      <div class="bg-gray-50 dark:bg-gray-800/50 rounded-lg p-3 text-center">
        <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Média por Unidade</div>
        <div class="text-base sm:text-lg font-bold text-gray-900 dark:text-white truncate" :title="formatCurrency(stats.mediaFaturamento)">
          {{ formatCurrency(stats.mediaFaturamento) }}
        </div>
      </div>
      
      <div class="bg-gray-50 dark:bg-gray-800/50 rounded-lg p-3 text-center">
        <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Unidades Inativas</div>
        <div class="text-base sm:text-lg font-bold text-gray-900 dark:text-white">
          {{ stats.total - stats.ativas }}
        </div>
      </div>
      
      <div class="bg-gray-50 dark:bg-gray-800/50 rounded-lg p-3 text-center">
        <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Projeção Anual</div>
        <div class="text-base sm:text-lg font-bold text-gray-900 dark:text-white truncate" :title="formatCurrency(stats.faturamentoProjetado * 12)">
          {{ formatCurrency(stats.faturamentoProjetado * 12) }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

// Ícones (assumindo que você tem esses componentes SVG)
import IconStore from '@/components/icons/store.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconClock from '@/components/icons/clock.vue';
import IconTrophy from '@/components/icons/trophy.vue';
import IconArrowUp from '@/components/icons/arrow-up.vue';
import IconArrowDown from '@/components/icons/arrow-down.vue';
import IconMinus from '@/components/icons/minus.vue';
import IconAlertCircle from '@/components/icons/alert-circle.vue';
import IconCheckCircle from '@/components/icons/check-circle.vue';

const props = defineProps({
  stats: {
    type: Object,
    required: true,
    default: () => ({
      total: 0,
      ativas: 0,
      faturamentoProjetado: 0,
      mediaFaturamento: 0,
      vencimentoProximo: 0
    })
  },
  loading: {
    type: Boolean,
    default: false
  }
});

const { formatCurrency } = useUnidadesUI();

// Computed
const growthRate = computed(() => {
  if (props.stats.total === 0) return 0;
  return Math.min(Math.round((props.stats.ativas / props.stats.total) * 100), 100);
});

// Trend helpers
const trendClass = (value) => {
  if (value > 0) return 'trend-up bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400';
  if (value < 0) return 'trend-down bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400';
  return 'trend-neutral bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-gray-400';
};
</script>

<style scoped>
@import '@/assets/default.css';
/* Animações */
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100 dark:bg-gray-800;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-300 dark:bg-gray-600 rounded-full;
}

::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400 dark:bg-gray-500;
}

/* Hover effects */
.group:hover .group-hover\:scale-110 {
  transform: scale(1.1);
}

/* Transitions */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Responsive text truncation */
@media (max-width: 640px) {
  .truncate {
    max-width: 120px;
  }
}
</style>