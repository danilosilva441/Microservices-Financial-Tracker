<!-- components/unidades/UnidadeList.vue -->
<template>
  <div class="w-full" :class="{ 'dark': isDarkMode }">
    <!-- Desktop Table View -->
    <div class="hidden md:block overflow-x-auto rounded-lg border border-gray-200 dark:border-gray-700">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-800">
          <tr>
            <th scope="col" class="px-6 py-4 w-12">
              <div class="flex items-center">
                <input 
                  type="checkbox" 
                  :checked="selectAll"
                  @change="toggleSelectAll"
                  class="w-4 h-4 text-primary-600 bg-gray-100 border-gray-300 rounded focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 dark:bg-gray-700 dark:border-gray-600"
                  :aria-label="selectAll ? 'Desmarcar todas' : 'Marcar todas'"
                >
              </div>
            </th>
            <th 
              v-for="column in columns"
              :key="column.field"
              scope="col"
              @click="column.sortable ? sortBy(column.field) : null"
              @keyup.enter="column.sortable ? sortBy(column.field) : null"
              :tabindex="column.sortable ? 0 : -1"
              class="px-6 py-4 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              :class="{ 'cursor-pointer': column.sortable }"
            >
              <div class="flex items-center gap-2">
                <component :is="column.iconComponent" class="w-4 h-4 text-gray-400 dark:text-gray-500" />
                <span>{{ column.label }}</span>
                <component 
                  v-if="column.sortable" 
                  :is="getSortIcon(column.field)"
                  class="w-3 h-3 text-gray-400 dark:text-gray-500"
                />
              </div>
            </th>
            <th scope="col" class="px-6 py-4 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-900 divide-y divide-gray-200 dark:divide-gray-700">
          <tr 
            v-for="unidade in displayedUnidades" 
            :key="unidade.id"
            @click="handleRowClick(unidade.id)"
            @keyup.enter="handleRowClick(unidade.id)"
            tabindex="0"
            role="button"
            :aria-label="`Unidade ${unidade.nome}`"
            class="hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors duration-150 cursor-pointer focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-inset"
            :class="{
              'bg-primary-50 dark:bg-primary-900/20': selectedItems.includes(unidade.id),
              'opacity-60': !unidade.isAtivo
            }"
          >
            <td class="px-6 py-4 w-12" @click.stop>
              <div class="flex items-center">
                <input 
                  type="checkbox" 
                  :checked="selectedItems.includes(unidade.id)"
                  @change="toggleSelect(unidade.id)"
                  class="w-4 h-4 text-primary-600 bg-gray-100 border-gray-300 rounded focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 dark:bg-gray-700 dark:border-gray-600"
                  :aria-label="`Selecionar ${unidade.nome}`"
                >
              </div>
            </td>
            
            <!-- Nome e Endereço -->
            <td class="px-6 py-4">
              <div class="flex items-center gap-3">
                <div class="flex-shrink-0 w-10 h-10 rounded-lg bg-gradient-to-br from-primary-500 to-secondary-500 flex items-center justify-center text-white shadow-sm">
                  <IconStore class="w-5 h-5" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="text-sm font-semibold text-gray-900 dark:text-white truncate">
                    {{ unidade.nome }}
                  </div>
                  <div class="text-xs text-gray-500 dark:text-gray-400 flex items-center gap-1 mt-0.5">
                    <IconMapPin class="w-3 h-3 text-gray-400 dark:text-gray-500" />
                    <span class="truncate" :title="unidade.endereco">{{ unidade.endereco }}</span>
                  </div>
                </div>
              </div>
            </td>
            
            <!-- Status -->
            <td class="px-6 py-4">
              <span :class="statusClasses(unidade)">
                <component :is="statusIcon(unidade)" class="w-3 h-3 mr-1.5" />
                {{ statusText(unidade) }}
              </span>
            </td>
            
            <!-- Meta Mensal -->
            <td class="px-6 py-4">
              <div class="flex items-center gap-2">
                <IconTarget class="w-4 h-4 text-primary-500 dark:text-primary-400" />
                <span class="text-sm font-semibold text-gray-900 dark:text-white">
                  {{ formatCurrency(unidade.metaMensal) }}
                </span>
              </div>
            </td>
            
            <!-- Progresso -->
            <td class="px-6 py-4">
              <div class="min-w-[140px]">
                <div class="flex items-center justify-between mb-1.5">
                  <div class="flex items-center gap-1.5">
                    <component :is="progressIcon(unidade)" :style="{ color: progressColor(unidade) }" class="w-4 h-4" />
                    <span class="text-sm font-bold" :style="{ color: progressColor(unidade) }">
                      {{ progress(unidade) }}%
                    </span>
                  </div>
                  <span class="text-xs text-gray-500 dark:text-gray-400">da meta</span>
                </div>
                <div class="w-full h-2 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden">
                  <div 
                    class="h-full rounded-full transition-all duration-300"
                    :style="progressBarStyle(unidade)"
                    role="progressbar"
                    :aria-valuenow="progress(unidade)"
                    aria-valuemin="0"
                    aria-valuemax="100"
                  ></div>
                </div>
              </div>
            </td>
            
            <!-- Funcionários -->
            <td class="px-6 py-4">
              <div class="flex items-center gap-2">
                <IconUsers class="w-4 h-4 text-gray-400 dark:text-gray-500" />
                <span class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ unidade.funcionariosAtivos || 0 }}
                </span>
                <span class="text-xs text-gray-500 dark:text-gray-400">ativos</span>
              </div>
            </td>
            
            <!-- Vencimento -->
            <td class="px-6 py-4">
              <div class="flex flex-col gap-1.5">
                <div class="flex items-center gap-2 text-sm">
                  <IconCalendar class="w-4 h-4 text-gray-400 dark:text-gray-500" />
                  <span v-if="unidade.dataFim" class="text-gray-900 dark:text-white">
                    {{ formatDate(unidade.dataFim) }}
                  </span>
                  <span v-else class="text-gray-400 dark:text-gray-500 flex items-center gap-1">
                    <IconBan class="w-3 h-3" />
                    Sem data
                  </span>
                </div>
                <div v-if="expirationStatus(unidade)" :class="expirationBadgeClasses(unidade)" class="text-xs">
                  <component :is="expirationIcon(unidade)" class="w-3 h-3 mr-1" />
                  {{ expirationStatus(unidade).label }}
                </div>
              </div>
            </td>
            
            <!-- Ações -->
            <td class="px-6 py-4 text-right" @click.stop>
              <div class="flex items-center justify-end gap-2">
                <button 
                  @click="handleEdit(unidade)"
                  class="action-button text-gray-600 hover:text-primary-600 dark:text-gray-400 dark:hover:text-primary-400"
                  :title="`Editar ${unidade.nome}`"
                >
                  <IconEdit class="w-4 h-4" />
                </button>
                
                <button 
                  v-if="unidade.isAtivo"
                  @click="handleDeactivate(unidade)"
                  class="action-button text-gray-600 hover:text-yellow-600 dark:text-gray-400 dark:hover:text-yellow-400"
                  :title="`Desativar ${unidade.nome}`"
                >
                  <IconPower class="w-4 h-4" />
                </button>
                
                <button 
                  v-else
                  @click="handleActivate(unidade)"
                  class="action-button text-gray-600 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400"
                  :title="`Ativar ${unidade.nome}`"
                >
                  <IconPlay class="w-4 h-4" />
                </button>
                
                <button 
                  @click="handleDelete(unidade)"
                  class="action-button text-gray-600 hover:text-red-600 dark:text-gray-400 dark:hover:text-red-400"
                  :title="`Excluir ${unidade.nome}`"
                >
                  <IconTrash class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mobile Card View -->
    <div class="md:hidden space-y-4">
      <div 
        v-for="unidade in displayedUnidades" 
        :key="unidade.id"
        @click="handleRowClick(unidade.id)"
        class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-4 cursor-pointer hover:shadow-md transition-all duration-200"
        :class="{
          'ring-2 ring-primary-500 dark:ring-primary-400': selectedItems.includes(unidade.id),
          'opacity-60': !unidade.isAtivo
        }"
      >
        <!-- Card Header -->
        <div class="flex items-start justify-between mb-3">
          <div class="flex items-center gap-3">
            <input 
              type="checkbox" 
              :checked="selectedItems.includes(unidade.id)"
              @change.stop="toggleSelect(unidade.id)"
              class="w-5 h-5 text-primary-600 bg-gray-100 border-gray-300 rounded focus:ring-primary-500 dark:bg-gray-700 dark:border-gray-600"
            >
            <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-primary-500 to-secondary-500 flex items-center justify-center text-white shadow-sm">
              <IconStore class="w-6 h-6" />
            </div>
            <div>
              <h3 class="text-base font-bold text-gray-900 dark:text-white">{{ unidade.nome }}</h3>
              <div class="flex items-center gap-1 mt-0.5">
                <span :class="statusClasses(unidade)" class="text-xs">
                  <component :is="statusIcon(unidade)" class="w-3 h-3 mr-1" />
                  {{ statusText(unidade) }}
                </span>
              </div>
            </div>
          </div>
          <button 
            @click.stop="handleEdit(unidade)"
            class="p-2 text-gray-500 hover:text-primary-600 dark:text-gray-400 dark:hover:text-primary-400"
          >
            <IconEdit class="w-5 h-5" />
          </button>
        </div>

        <!-- Card Content -->
        <div class="space-y-3 mt-3">
          <!-- Endereço -->
          <div class="flex items-start gap-2 text-sm">
            <IconMapPin class="w-4 h-4 text-gray-400 dark:text-gray-500 mt-0.5 flex-shrink-0" />
            <span class="text-gray-600 dark:text-gray-300 flex-1">{{ unidade.endereco }}</span>
          </div>

          <!-- Meta e Progresso -->
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <IconTarget class="w-4 h-4 text-primary-500 dark:text-primary-400" />
              <span class="text-sm font-semibold text-gray-900 dark:text-white">
                {{ formatCurrency(unidade.metaMensal) }}
              </span>
            </div>
            <div class="flex items-center gap-1">
              <component :is="progressIcon(unidade)" :style="{ color: progressColor(unidade) }" class="w-4 h-4" />
              <span class="text-sm font-bold" :style="{ color: progressColor(unidade) }">
                {{ progress(unidade) }}%
              </span>
            </div>
          </div>

          <!-- Barra de Progresso -->
          <div class="w-full h-2 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden">
            <div 
              class="h-full rounded-full transition-all duration-300"
              :style="progressBarStyle(unidade)"
            ></div>
          </div>

          <!-- Funcionários e Vencimento -->
          <div class="flex items-center justify-between pt-2">
            <div class="flex items-center gap-2">
              <IconUsers class="w-4 h-4 text-gray-400 dark:text-gray-500" />
              <span class="text-sm text-gray-700 dark:text-gray-300">
                {{ unidade.funcionariosAtivos || 0 }} ativos
              </span>
            </div>
            
            <div class="flex items-center gap-2">
              <IconCalendar class="w-4 h-4 text-gray-400 dark:text-gray-500" />
              <span v-if="unidade.dataFim" class="text-sm text-gray-700 dark:text-gray-300">
                {{ formatDate(unidade.dataFim) }}
              </span>
              <span v-else class="text-sm text-gray-400 dark:text-gray-500">
                Sem data
              </span>
            </div>
          </div>

          <!-- Badge de Vencimento e Ações Adicionais -->
          <div class="flex items-center justify-between pt-2 border-t border-gray-100 dark:border-gray-700">
            <div v-if="expirationStatus(unidade)" :class="expirationBadgeClasses(unidade)" class="text-xs">
              <component :is="expirationIcon(unidade)" class="w-3 h-3 mr-1" />
              {{ expirationStatus(unidade).label }}
            </div>
            
            <div class="flex items-center gap-2">
              <button 
                v-if="!unidade.isAtivo"
                @click.stop="handleActivate(unidade)"
                class="p-2 text-green-600 hover:text-green-700 dark:text-green-400 dark:hover:text-green-300"
                :title="`Ativar ${unidade.nome}`"
              >
                <IconPlay class="w-4 h-4" />
              </button>
              <button 
                @click.stop="handleDelete(unidade)"
                class="p-2 text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300"
                :title="`Excluir ${unidade.nome}`"
              >
                <IconTrash class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty States -->
    <template v-if="unidades.length === 0">
      <!-- Mobile Empty State -->
      <div class="md:hidden text-center py-12 px-4">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gray-100 dark:bg-gray-800 mb-4">
          <IconStoreSlash class="w-10 h-10 text-gray-400 dark:text-gray-500" />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">
          Nenhuma unidade encontrada
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400 mb-6">
          Comece adicionando sua primeira unidade
        </p>
        <button @click="$emit('add')" class="btn-primary">
          <IconPlus class="w-4 h-4 mr-2" />
          Adicionar unidade
        </button>
      </div>

      <!-- Desktop Empty State -->
      <div class="hidden md:block text-center py-16 px-4">
        <div class="inline-flex items-center justify-center w-24 h-24 rounded-full bg-gray-100 dark:bg-gray-800 mb-4">
          <IconStoreSlash class="w-12 h-12 text-gray-400 dark:text-gray-500" />
        </div>
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">
          Nenhuma unidade cadastrada
        </h3>
        <p class="text-gray-500 dark:text-gray-400 mb-6">
          Clique no botão abaixo para adicionar sua primeira unidade
        </p>
        <button @click="$emit('add')" class="btn-primary">
          <IconPlus class="w-4 h-4 mr-2" />
          Adicionar unidade
        </button>
      </div>
    </template>

    <!-- Selection Bar -->
    <div v-if="selectedItems.length > 0" class="selection-bar">
      <div class="selection-info">
        <IconCheckCircle class="w-5 h-5 text-primary-500 dark:text-primary-400" />
        <span class="font-medium">{{ selectedItems.length }} unidade(s) selecionada(s)</span>
      </div>
      <div class="selection-actions">
        <button @click="emitBulkAction('export')" class="btn-selection">
          <IconDownload class="w-4 h-4 mr-2" />
          Exportar
        </button>
        <button @click="emitBulkAction('delete')" class="btn-selection btn-danger">
          <IconTrash class="w-4 h-4 mr-2" />
          Excluir
        </button>
        <button @click="clearSelection" class="btn-selection">
          <IconTimes class="w-4 h-4 mr-2" />
          Limpar
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

// Ícones
import IconStore from '@/components/icons/store.vue';
import IconStoreSlash from '@/components/icons/store-slash.vue';
import IconMapPin from '@/components/icons/map-pin.vue';
import IconTarget from '@/components/icons/target.vue';
import IconUsers from '@/components/icons/users.vue';
import IconCalendar from '@/components/icons/calendar.vue';
import IconBan from '@/components/icons/ban.vue';
import IconEdit from '@/components/icons/edit.vue';
import IconPower from '@/components/icons/power.vue';
import IconPlay from '@/components/icons/play.vue';
import IconTrash from '@/components/icons/trash.vue';
import IconCheckCircle from '@/components/icons/check-circle.vue';
import IconDownload from '@/components/icons/download.vue';
import IconTimes from '@/components/icons/times.vue';
import IconSort from '@/components/icons/sort.vue';
import IconSortUp from '@/components/icons/sort-up.vue';
import IconSortDown from '@/components/icons/sort-down.vue';

// Ícones de status
import IconCheckCircleSmall from '@/components/icons/check-circle.vue';
import IconCircle from '@/components/icons/circle.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconExclamationCircle from '@/components/icons/exclamation-circle.vue';
import IconTimesCircle from '@/components/icons/times-circle.vue';
import IconClock from '@/components/icons/clock.vue';
import IconAlertTriangle from '@/components/icons/alert-triangle.vue';

const props = defineProps({
  unidades: {
    type: Array,
    required: true,
    default: () => []
  },
  selectedItems: {
    type: Array,
    default: () => []
  },
  ui: {
    type: Object,
    default: () => ({})
  }
});

const emit = defineEmits([
  'row-click', 
  'edit', 
  'delete', 
  'deactivate', 
  'activate', 
  'selection-change',
  'bulk-action',
  'add'
]);

const { formatCurrency, formatDate, getStatusBadge, getExpirationStatus } = useUnidadesUI();

// Dark mode detection
const isDarkMode = ref(false);

const loadTheme = () => {
  const savedTheme = localStorage.getItem('theme');
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  isDarkMode.value = savedTheme ? savedTheme === 'dark' : prefersDark;
};

// Columns configuration with icon components
const columns = [
  { field: 'nome', label: 'Unidade', iconComponent: IconStore, sortable: true },
  { field: 'status', label: 'Status', iconComponent: IconCircle, sortable: true },
  { field: 'metaMensal', label: 'Meta Mensal', iconComponent: IconTarget, sortable: true },
  { field: 'progresso', label: 'Progresso', iconComponent: IconChartLine, sortable: false },
  { field: 'funcionarios', label: 'Funcionários', iconComponent: IconUsers, sortable: true },
  { field: 'vencimento', label: 'Vencimento', iconComponent: IconCalendar, sortable: true }
];

// Local state
const selectedItems = ref([...props.selectedItems]);
const sortField = ref('nome');
const sortDirection = ref('asc');

// Watch for changes in props.selectedItems
watch(() => props.selectedItems, (newVal) => {
  selectedItems.value = [...newVal];
}, { deep: true });

// Computed
const selectAll = computed({
  get: () => props.unidades.length > 0 && selectedItems.value.length === props.unidades.length,
  set: (value) => {
    if (value) {
      selectedItems.value = props.unidades.map(u => u.id);
    } else {
      selectedItems.value = [];
    }
    emitSelectionChange();
  }
});

// Apenas ordenação local, sem filtragem
const displayedUnidades = computed(() => {
  const sorted = [...props.unidades];
  
  sorted.sort((a, b) => {
    let aValue = a[sortField.value];
    let bValue = b[sortField.value];
    
    // Tratamento especial para campos específicos
    if (sortField.value === 'nome') {
      aValue = aValue?.toLowerCase() || '';
      bValue = bValue?.toLowerCase() || '';
    }
    
    if (sortField.value === 'metaMensal') {
      aValue = aValue || 0;
      bValue = bValue || 0;
    }
    
    if (sortField.value === 'funcionarios') {
      aValue = a.funcionariosAtivos || 0;
      bValue = b.funcionariosAtivos || 0;
    }
    
    if (sortField.value === 'vencimento') {
      aValue = a.dataFim || '';
      bValue = b.dataFim || '';
    }
    
    if (sortField.value === 'status') {
      aValue = a.isAtivo ? 1 : 0;
      bValue = b.isAtivo ? 1 : 0;
    }
    
    if (aValue < bValue) return sortDirection.value === 'asc' ? -1 : 1;
    if (aValue > bValue) return sortDirection.value === 'asc' ? 1 : -1;
    return 0;
  });
  
  return sorted;
});

// Methods
const getSortIcon = (field) => {
  if (sortField.value !== field) return IconSort;
  return sortDirection.value === 'asc' ? IconSortUp : IconSortDown;
};

const toggleSelectAll = () => {
  selectAll.value = !selectAll.value;
};

const toggleSelect = (id) => {
  const index = selectedItems.value.indexOf(id);
  if (index === -1) {
    selectedItems.value.push(id);
  } else {
    selectedItems.value.splice(index, 1);
  }
  emitSelectionChange();
};

const emitSelectionChange = () => {
  emit('selection-change', selectedItems.value);
};

const sortBy = (field) => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortField.value = field;
    sortDirection.value = 'asc';
  }
};

const handleRowClick = (id) => {
  emit('row-click', id);
};

const handleEdit = (unidade) => {
  emit('edit', unidade.id);
};

const handleDeactivate = (unidade) => {
  emit('deactivate', unidade);
};

const handleActivate = (unidade) => {
  emit('activate', unidade);
};

const handleDelete = (unidade) => {
  emit('delete', unidade);
};

// Status helpers
const statusClasses = (unidade) => {
  return unidade.isAtivo 
    ? 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
    : 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-400';
};

const statusIcon = (unidade) => {
  return unidade.isAtivo ? IconCheckCircleSmall : IconCircle;
};

const statusText = (unidade) => {
  return unidade.isAtivo ? 'Ativo' : 'Inativo';
};

// Progress helpers
const progress = (unidade) => {
  const faturamentoAtual = unidade.faturamentoAtual || 0;
  const meta = unidade.metaMensal || 0;
  if (meta <= 0) return 0;
  return Math.min(Math.round((faturamentoAtual / meta) * 100), 100);
};

const progressColor = (unidade) => {
  const p = progress(unidade);
  if (p >= 75) return '#10b981';
  if (p >= 50) return '#f59e0b';
  if (p >= 25) return '#f97316';
  return '#ef4444';
};

const progressIcon = (unidade) => {
  const p = progress(unidade);
  if (p >= 75) return IconCheckCircleSmall;
  if (p >= 50) return IconChartLine;
  if (p >= 25) return IconExclamationCircle;
  return IconTimesCircle;
};

const progressBarStyle = (unidade) => {
  return {
    width: `${progress(unidade)}%`,
    backgroundColor: progressColor(unidade)
  };
};

// Expiration helpers
const expirationStatus = (unidade) => getExpirationStatus(unidade.dataFim);

const expirationBadgeClasses = (unidade) => {
  const status = expirationStatus(unidade);
  if (!status) return '';
  
  const baseClasses = 'inline-flex items-center px-2 py-0.5 rounded text-xs font-medium';
  
  switch(status.variant) {
    case 'warning':
      return `${baseClasses} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400`;
    case 'danger':
      return `${baseClasses} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400`;
    case 'success':
      return `${baseClasses} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400`;
    default:
      return baseClasses;
  }
};

const expirationIcon = (unidade) => {
  const status = expirationStatus(unidade);
  if (!status) return '';
  
  switch(status.variant) {
    case 'warning': return IconClock;
    case 'danger': return IconAlertTriangle;
    case 'success': return IconCheckCircleSmall;
    default: return IconCalendar;
  }
};

const emitBulkAction = (action) => {
  emit('bulk-action', {
    action,
    items: selectedItems.value
  });
};

const clearSelection = () => {
  selectedItems.value = [];
  emitSelectionChange();
};

// Theme setup
onMounted(() => {
  loadTheme();
  
  const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
  const handleThemeChange = (e) => {
    if (!localStorage.getItem('theme')) {
      isDarkMode.value = e.matches;
    }
  };
  
  mediaQuery.addEventListener('change', handleThemeChange);
  
  onUnmounted(() => {
    mediaQuery.removeEventListener('change', handleThemeChange);
  });
});
</script>

<style scoped>
@import '@/assets/default.css';

/* Selection Bar (reutilizando do default.css) */
.selection-bar {
  @apply fixed bottom-0 left-0 right-0 md:sticky md:bottom-4 md:left-auto md:right-auto md:mx-4 bg-white dark:bg-gray-800 border-t md:border md:rounded-xl border-gray-200 dark:border-gray-700 shadow-lg p-4 flex flex-col md:flex-row justify-between items-center gap-4 md:gap-0 z-30;
}

.selection-info {
  @apply flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300;
}

.selection-actions {
  @apply flex items-center gap-2 self-end md:self-auto;
}

.btn-selection {
  @apply px-4 py-2 text-sm font-medium rounded-lg border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500;
}

.btn-selection.btn-danger {
  @apply border-red-300 dark:border-red-800 text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20;
}

/* Action Buttons */
.action-button {
  @apply p-2 rounded-lg transition-all duration-200 hover:scale-110 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 dark:focus:ring-offset-gray-900;
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
</style>