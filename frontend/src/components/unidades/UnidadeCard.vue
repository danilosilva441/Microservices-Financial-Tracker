<!--
 * src/components/unidades/UnidadeCard.vue
 * UnidadeCard.vue
 *
 * A Vue component that represents a card for displaying information about a business unit (unidade).
 * It includes details such as the unit's name, address, start date, monthly goal, current revenue, and status.
 * The card also provides action buttons for editing, activating/deactivating, and deleting the unit.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 * It emits events for various actions that can be handled by the parent component.
 -->
<template>
  <div 
    class="flex flex-col h-full overflow-hidden transition-all duration-300 bg-white border border-gray-200 shadow-sm group dark:bg-gray-800 rounded-xl dark:border-gray-700 hover:shadow-lg"
    :class="{ 
      'cursor-pointer hover:scale-[1.02] hover:-translate-y-1': !disableClick,
      'opacity-75': !unidade.isAtivo
    }"
    @click="!disableClick ? $emit('click', unidade) : null"
  >
    <!-- Header do Card -->
    <div class="p-5 border-b border-gray-100 dark:border-gray-700 bg-gradient-to-r from-primary-50/50 to-secondary-50/50 dark:from-primary-900/10 dark:to-secondary-900/10">
      <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-start">
        <div class="flex items-start min-w-0 gap-3">
          <!-- Avatar -->
          <div class="flex items-center justify-center flex-shrink-0 w-12 h-12 text-white transition-transform duration-300 shadow-md sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500 group-hover:scale-110">
            <IconStore class="w-6 h-6 sm:w-7 sm:h-7" />
          </div>

          <!-- Info -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-1">
              <h3 class="text-base font-bold text-gray-900 truncate sm:text-lg dark:text-white">
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
    <div class="flex-1 p-5">
      <!-- Meta Mensal -->
      <div class="mb-4">
        <div class="flex items-center justify-between mb-2">
          <span class="text-xs font-medium tracking-wider text-gray-500 uppercase dark:text-gray-400">
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
          <div class="h-2 overflow-hidden bg-gray-200 rounded-full dark:bg-gray-700">
            <div 
              class="h-full transition-all duration-500 ease-out rounded-full"
              :style="progressBarStyle"
            ></div>
          </div>
        </div>
      </div>

      <!-- Stats Grid -->
      <div class="grid grid-cols-1 gap-3 mb-4 sm:grid-cols-3">
        <!-- Funcionários -->
        <div class="p-3 rounded-lg bg-gray-50 dark:bg-gray-900/50">
          <div class="flex items-center gap-2">
            <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-primary-100 dark:bg-primary-900/30">
              <IconUsers class="w-4 h-4 text-primary-600 dark:text-primary-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 dark:text-white">{{ funcionariosAtivos || 0 }}</div>
              <div class="text-xs text-gray-500 truncate dark:text-gray-400">Funcionários</div>
            </div>
          </div>
        </div>

        <!-- Faturamento -->
        <div class="p-3 rounded-lg bg-gray-50 dark:bg-gray-900/50">
          <div class="flex items-center gap-2">
            <div class="flex items-center justify-center w-8 h-8 bg-green-100 rounded-lg dark:bg-green-900/30">
              <IconChartLine class="w-4 h-4 text-green-600 dark:text-green-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 truncate dark:text-white" :title="formatCurrency(faturamentoAtual)">
                {{ formatCurrency(faturamentoAtual) }}
              </div>
              <div class="text-xs text-gray-500 truncate dark:text-gray-400">Faturamento</div>
            </div>
          </div>
        </div>

        <!-- Dias Restantes -->
        <div class="p-3 rounded-lg bg-gray-50 dark:bg-gray-900/50">
          <div class="flex items-center gap-2">
            <div class="flex items-center justify-center w-8 h-8 bg-yellow-100 rounded-lg dark:bg-yellow-900/30">
              <IconClock class="w-4 h-4 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="min-w-0">
              <div class="text-base font-bold text-gray-900 dark:text-white">{{ diasRestantes }}</div>
              <div class="text-xs text-gray-500 truncate dark:text-gray-400">Dias restantes</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Descrição -->
      <div v-if="unidade.descricao" class="pt-3 border-t border-gray-100 dark:border-gray-700">
        <p class="text-xs leading-relaxed text-gray-600 sm:text-sm dark:text-gray-400 line-clamp-2">
          {{ truncatedDescription }}
        </p>
      </div>
    </div>

    <!-- Footer do Card -->
    <div class="p-4 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-900/50">
      <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
        <!-- Actions -->
        <div class="flex items-center gap-1">
          <button 
            class="text-gray-600 action-button hover:text-primary-600 dark:text-gray-400 dark:hover:text-primary-400"
            @click.stop="$emit('edit', unidade.id)"
            :title="`Editar ${unidade.nome}`"
          >
            <IconEdit class="w-4 h-4" />
          </button>
          
          <button 
            v-if="unidade.isAtivo"
            class="text-gray-600 action-button hover:text-yellow-600 dark:text-gray-400 dark:hover:text-yellow-400"
            @click.stop="$emit('deactivate', unidade)"
            :title="`Desativar ${unidade.nome}`"
          >
            <IconPower class="w-4 h-4" />
          </button>
          
          <button 
            v-else
            class="text-gray-600 action-button hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400"
            @click.stop="$emit('activate', unidade)"
            :title="`Ativar ${unidade.nome}`"
          >
            <IconPlay class="w-4 h-4" />
          </button>
          
          <button 
            class="text-gray-600 action-button hover:text-red-600 dark:text-gray-400 dark:hover:text-red-400"
            @click.stop="$emit('delete', unidade)"
            :title="`Excluir ${unidade.nome}`"
          >
            <IconTrash class="w-4 h-4" />
          </button>
        </div>

        <!-- Data de atualização -->
        <div class="flex items-center gap-1 text-xs text-gray-500 dark:text-gray-400">
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