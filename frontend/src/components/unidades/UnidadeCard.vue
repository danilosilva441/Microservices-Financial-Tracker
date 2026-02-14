<!-- components/unidades/UnidadeCard.vue -->
<template>
  <div 
    class="group bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg transition-all duration-300 overflow-hidden h-full flex flex-col"
    :class="{ 
      'cursor-pointer hover:scale-[1.02] hover:-translate-y-1': !disableClick,
      'opacity-75': !unidade.isAtivo
    }"
    @click="!disableClick ? $emit('click', unidade) : null"
  >
    <!-- Header do Card -->
    <div class="p-5 border-b border-gray-100 dark:border-gray-700 bg-gradient-to-r from-primary-50/50 to-secondary-50/50 dark:from-primary-900/10 dark:to-secondary-900/10">
      <div class="flex flex-col sm:flex-row sm:items-start justify-between gap-3">
        <div class="flex items-start gap-3 min-w-0">
          <!-- Avatar -->
          <div class="flex-shrink-0 w-12 h-12 sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500 flex items-center justify-center text-white shadow-md group-hover:scale-110 transition-transform duration-300">
            <IconStore class="w-6 h-6 sm:w-7 sm:h-7" />
          </div>

          <!-- Info -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-1">
              <h3 class="text-base sm:text-lg font-bold text-gray-900 dark:text-white truncate">
                {{ unidade.nome }}
              </h3>
              <span class="flex-shrink-0 inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium" 
                :class="statusClasses"
              >
                <span :class="statusDotClasses" class="mr-1"></span>
                {{ statusText }}
              </span>
            </div>

            <div class="space-y-1">
              <div class="flex items-center gap-1.5 text-xs sm:text-sm text-gray-600 dark:text-gray-400">
                <IconMapPin class="w-3.5 h-3.5 flex-shrink-0 text-gray-400 dark:text-gray-500" />
                <span class="truncate" :title="unidade.endereco">{{ truncatedAddress }}</span>
              </div>
              <div class="flex items-center gap-1.5 text-xs sm:text-sm text-gray-600 dark:text-gray-400">
                <IconCalendar class="w-3.5 h-3.5 flex-shrink-0 text-gray-400 dark:text-gray-500" />
                <span>{{ formatDate(unidade.dataInicio) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Body do Card -->
    <div class="p-5 flex-1">
      <!-- Meta Mensal -->
      <div class="mb-4">
        <div class="flex justify-between items-center mb-2">
          <span class="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
            Meta Mensal
          </span>
          <span class="text-base font-bold text-primary-600 dark:text-primary-400">
            {{ formatCurrency(unidade.metaMensal) }}
          </span>
        </div>

        <!-- Progress Bar -->
        <div class="space-y-1.5">
          <div class="flex justify-between text-xs">
            <span class="text-gray-500 dark:text-gray-400">0%</span>
            <span class="font-semibold" :style="{ color: progressColor }">{{ progress }}%</span>
            <span class="text-gray-500 dark:text-gray-400">100%</span>
          </div>
          <div class="h-2 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden">
            <div 
              class="h-full rounded-full transition-all duration-500 ease-out"
              :style="progressBarStyle"
            ></div>
          </div>
        </div>
      </div>

      <!-- Stats Grid -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4">
        <!-- Funcionários -->
        <div class="bg-gray-50 dark:bg-gray-900/50 rounded-lg p-3">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-lg bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center">
              <IconUsers class="w-4 h-4 text-primary-600 dark:text-primary-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 dark:text-white">{{ funcionariosAtivos || 0 }}</div>
              <div class="text-xs text-gray-500 dark:text-gray-400 truncate">Funcionários</div>
            </div>
          </div>
        </div>

        <!-- Faturamento -->
        <div class="bg-gray-50 dark:bg-gray-900/50 rounded-lg p-3">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-lg bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
              <IconChartLine class="w-4 h-4 text-green-600 dark:text-green-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 dark:text-white truncate" :title="formatCurrency(faturamentoAtual)">
                {{ formatCurrency(faturamentoAtual) }}
              </div>
              <div class="text-xs text-gray-500 dark:text-gray-400 truncate">Faturamento</div>
            </div>
          </div>
        </div>

        <!-- Dias Restantes -->
        <div class="bg-gray-50 dark:bg-gray-900/50 rounded-lg p-3">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-lg bg-yellow-100 dark:bg-yellow-900/30 flex items-center justify-center">
              <IconClock class="w-4 h-4 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 dark:text-white">{{ diasRestantes }}</div>
              <div class="text-xs text-gray-500 dark:text-gray-400 truncate">Dias restantes</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Descrição -->
      <div v-if="unidade.descricao" class="pt-3 border-t border-gray-100 dark:border-gray-700">
        <p class="text-xs sm:text-sm text-gray-600 dark:text-gray-400 leading-relaxed line-clamp-2">
          {{ truncatedDescription }}
        </p>
      </div>
    </div>

    <!-- Footer do Card -->
    <div class="p-4 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-900/50">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <!-- Actions -->
        <div class="flex items-center gap-1">
          <button 
            class="action-button text-gray-600 hover:text-primary-600 dark:text-gray-400 dark:hover:text-primary-400"
            @click.stop="$emit('edit', unidade.id)"
            :title="`Editar ${unidade.nome}`"
          >
            <IconEdit class="w-4 h-4" />
          </button>
          
          <button 
            v-if="unidade.isAtivo"
            class="action-button text-gray-600 hover:text-yellow-600 dark:text-gray-400 dark:hover:text-yellow-400"
            @click.stop="$emit('deactivate', unidade)"
            :title="`Desativar ${unidade.nome}`"
          >
            <IconPower class="w-4 h-4" />
          </button>
          
          <button 
            v-else
            class="action-button text-gray-600 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400"
            @click.stop="$emit('activate', unidade)"
            :title="`Ativar ${unidade.nome}`"
          >
            <IconPlay class="w-4 h-4" />
          </button>
          
          <button 
            class="action-button text-gray-600 hover:text-red-600 dark:text-gray-400 dark:hover:text-red-400"
            @click.stop="$emit('delete', unidade)"
            :title="`Excluir ${unidade.nome}`"
          >
            <IconTrash class="w-4 h-4" />
          </button>
        </div>

        <!-- Data de atualização -->
        <div class="text-xs text-gray-500 dark:text-gray-400 flex items-center gap-1">
          <IconClock class="w-3 h-3" />
          <span>{{ formatDate(unidade.updatedAt || unidade.createdAt) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

// Ícones
import IconStore from '@/components/icons/store.vue';
import IconMapPin from '@/components/icons/map-pin.vue';
import IconCalendar from '@/components/icons/calendar.vue';
import IconUsers from '@/components/icons/users.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconClock from '@/components/icons/clock.vue';
import IconEdit from '@/components/icons/edit.vue';
import IconPower from '@/components/icons/power.vue';
import IconPlay from '@/components/icons/play.vue';
import IconTrash from '@/components/icons/trash.vue';

const props = defineProps({
  unidade: {
    type: Object,
    required: true
  },
  ui: {
    type: Object,
    default: () => ({})
  },
  disableClick: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['click', 'edit', 'delete', 'deactivate', 'activate']);

const { formatCurrency, formatDate, getStatusBadge, getDaysToExpire } = useUnidadesUI();

// Status
const statusBadge = computed(() => getStatusBadge(props.unidade.isAtivo));
const statusClasses = computed(() => {
  const baseClasses = 'inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium';
  if (props.unidade.isAtivo) {
    return `${baseClasses} bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400`;
  }
  return `${baseClasses} bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-400`;
});
const statusDotClasses = computed(() => {
  return props.unidade.isAtivo ? 'bg-green-500' : 'bg-gray-400';
});
const statusText = computed(() => statusBadge.value.label);

// Truncate helpers
const truncatedAddress = computed(() => {
  const addr = props.unidade.endereco || '';
  return addr.length > 40 ? addr.substring(0, 40) + '...' : addr;
});

const truncatedDescription = computed(() => {
  const desc = props.unidade.descricao || '';
  return desc.length > 100 ? desc.substring(0, 100) + '...' : desc;
});

// Progress
const progress = computed(() => {
  const faturamentoAtual = props.unidade.faturamentoAtual || 0;
  const meta = props.unidade.metaMensal || 0;
  if (meta <= 0) return 0;
  return Math.min(Math.round((faturamentoAtual / meta) * 100), 100);
});

const progressColor = computed(() => {
  if (progress.value >= 75) return '#10b981';
  if (progress.value >= 50) return '#f59e0b';
  if (progress.value >= 25) return '#f97316';
  return '#ef4444';
});

const progressBarStyle = computed(() => ({
  width: `${progress.value}%`,
  backgroundColor: progressColor.value
}));

// Stats
const funcionariosAtivos = computed(() => props.unidade.funcionariosAtivos || Math.floor(Math.random() * 20) + 5);
const faturamentoAtual = computed(() => props.unidade.faturamentoAtual || 0);
const diasRestantes = computed(() => {
  const dias = getDaysToExpire(props.unidade.dataFim);
  if (dias === null) return '∞';
  if (dias < 0) return 'Vencido';
  return dias;
});
</script>

<style scoped>
@import '@/assets/default.css';

/* Action Buttons */
.action-button {
  @apply p-2 rounded-lg transition-all duration-200 hover:scale-110 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 dark:focus:ring-offset-gray-900;
}

/* Line clamp for description */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Custom scrollbar (opcional) */
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

/* Animações */
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

/* Hover effects */
.group:hover .group-hover\:scale-110 {
  transform: scale(1.1);
}
</style>