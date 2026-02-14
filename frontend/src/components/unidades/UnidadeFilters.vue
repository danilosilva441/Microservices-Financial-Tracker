<!-- components/unidades/UnidadeFilters.vue -->
<template>
  <div class="w-full space-y-4">
    <!-- Barra de Pesquisa Rápida -->
    <div class="flex flex-col sm:flex-row items-stretch sm:items-center gap-3">
      <div class="relative flex-1">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <IconSearch class="w-4 h-4 text-gray-400 dark:text-gray-500" />
        </div>
        
        <input 
          type="text" 
          :value="searchValue" 
          @input="handleSearchInput" 
          placeholder="Pesquisar unidades por nome, endereço..."
          class="w-full pl-9 pr-9 py-2.5 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-500 transition-colors"
        />
        
        <button 
          v-if="searchValue" 
          @click="clearSearch" 
          class="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300 transition-colors"
        >
          <IconTimes class="w-4 h-4" />
        </button>
      </div>

      <div class="flex items-center gap-2">
        <!-- Botão Filtros Avançados -->
        <button 
          @click="toggleFilters" 
          :class="[
            'btn-filter relative px-3 py-2.5 text-sm font-medium rounded-lg border transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500',
            showAdvancedFilters 
              ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300' 
              : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600'
          ]"
        >
          <IconFilter class="w-4 h-4 mr-2" />
          <span class="hidden sm:inline">Filtros</span>
          <span v-if="hasActiveFilters" class="absolute -top-1 -right-1 w-2 h-2 bg-primary-500 rounded-full"></span>
        </button>

        <!-- Botão Alternar Visualização -->
        <button 
          @click="toggleView"
          class="p-2.5 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500"
          :title="viewMode === 'grid' ? 'Visualização em lista' : 'Visualização em grade'"
        >
          <IconList v-if="viewMode === 'grid'" class="w-4 h-4" />
          <IconGrid v-else class="w-4 h-4" />
        </button>

        <!-- Botão Limpar Filtros -->
        <button 
          v-if="hasActiveFilters" 
          @click="clearAllFilters"
          class="btn-clear px-3 py-2.5 text-sm font-medium rounded-lg border border-red-300 dark:border-red-800 text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-red-500"
        >
          <IconTimes class="w-4 h-4 sm:mr-2" />
          <span class="hidden sm:inline">Limpar</span>
        </button>
      </div>
    </div>

    <!-- Filtros Avançados -->
    <transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="opacity-0 -translate-y-2"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition-all duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 -translate-y-2"
    >
      <div v-if="showAdvancedFilters" class="bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-lg p-4 sm:p-5 space-y-5">
        <!-- Grid de Filtros -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
          <!-- Status -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider flex items-center gap-1.5">
              <IconToggleOn class="w-4 h-4 text-primary-500 dark:text-primary-400" />
              Status
            </h4>
            <div class="space-y-2">
              <label class="flex items-center gap-2 cursor-pointer group">
                <input 
                  type="checkbox" 
                  :checked="isStatusActive('ativa')" 
                  @change="toggleStatus('ativa')"
                  class="sr-only"
                />
                <div :class="[
                  'w-4 h-4 rounded border transition-colors flex items-center justify-center',
                  isStatusActive('ativa') 
                    ? 'bg-primary-500 border-primary-500' 
                    : 'border-gray-300 dark:border-gray-600 group-hover:border-primary-500'
                ]">
                  <IconCheck v-if="isStatusActive('ativa')" class="w-3 h-3 text-white" />
                </div>
                <span class="text-sm text-gray-700 dark:text-gray-300 flex items-center gap-1.5">
                  <span class="w-2 h-2 rounded-full bg-green-500"></span>
                  Ativas
                </span>
              </label>

              <label class="flex items-center gap-2 cursor-pointer group">
                <input 
                  type="checkbox" 
                  :checked="isStatusActive('inativa')" 
                  @change="toggleStatus('inativa')"
                  class="sr-only"
                />
                <div :class="[
                  'w-4 h-4 rounded border transition-colors flex items-center justify-center',
                  isStatusActive('inativa') 
                    ? 'bg-primary-500 border-primary-500' 
                    : 'border-gray-300 dark:border-gray-600 group-hover:border-primary-500'
                ]">
                  <IconCheck v-if="isStatusActive('inativa')" class="w-3 h-3 text-white" />
                </div>
                <span class="text-sm text-gray-700 dark:text-gray-300 flex items-center gap-1.5">
                  <span class="w-2 h-2 rounded-full bg-gray-400"></span>
                  Inativas
                </span>
              </label>
            </div>
          </div>

          <!-- Vencimento -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider flex items-center gap-1.5">
              <IconCalendar class="w-4 h-4 text-primary-500 dark:text-primary-400" />
              Vencimento
            </h4>
            <div class="space-y-2">
              <label class="flex items-center gap-2 cursor-pointer group">
                <input 
                  type="radio" 
                  name="expiration" 
                  value="all" 
                  :checked="expirationFilter === 'all'"
                  @change="filterByExpiration('all')"
                  class="sr-only"
                />
                <div :class="[
                  'w-4 h-4 rounded-full border transition-colors flex items-center justify-center',
                  expirationFilter === 'all'
                    ? 'border-primary-500' 
                    : 'border-gray-300 dark:border-gray-600 group-hover:border-primary-500'
                ]">
                  <div v-if="expirationFilter === 'all'" class="w-2 h-2 rounded-full bg-primary-500"></div>
                </div>
                <span class="text-sm text-gray-700 dark:text-gray-300">Todos</span>
              </label>

              <label class="flex items-center gap-2 cursor-pointer group">
                <input 
                  type="radio" 
                  name="expiration" 
                  value="expiring" 
                  :checked="expirationFilter === 'expiring'"
                  @change="filterByExpiration('expiring')"
                  class="sr-only"
                />
                <div :class="[
                  'w-4 h-4 rounded-full border transition-colors flex items-center justify-center',
                  expirationFilter === 'expiring'
                    ? 'border-primary-500' 
                    : 'border-gray-300 dark:border-gray-600 group-hover:border-primary-500'
                ]">
                  <div v-if="expirationFilter === 'expiring'" class="w-2 h-2 rounded-full bg-primary-500"></div>
                </div>
                <span class="text-sm text-gray-700 dark:text-gray-300 flex items-center gap-1.5">
                  <IconClock class="w-3.5 h-3.5 text-yellow-500" />
                  Vence em 30 dias
                </span>
              </label>

              <label class="flex items-center gap-2 cursor-pointer group">
                <input 
                  type="radio" 
                  name="expiration" 
                  value="expired" 
                  :checked="expirationFilter === 'expired'"
                  @change="filterByExpiration('expired')"
                  class="sr-only"
                />
                <div :class="[
                  'w-4 h-4 rounded-full border transition-colors flex items-center justify-center',
                  expirationFilter === 'expired'
                    ? 'border-primary-500' 
                    : 'border-gray-300 dark:border-gray-600 group-hover:border-primary-500'
                ]">
                  <div v-if="expirationFilter === 'expired'" class="w-2 h-2 rounded-full bg-primary-500"></div>
                </div>
                <span class="text-sm text-gray-700 dark:text-gray-300 flex items-center gap-1.5">
                  <IconAlertCircle class="w-3.5 h-3.5 text-red-500" />
                  Expiradas
                </span>
              </label>
            </div>
          </div>

          <!-- Meta Mensal -->
          <div class="space-y-3 md:col-span-2">
            <h4 class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider flex items-center gap-1.5">
              <IconChartLine class="w-4 h-4 text-primary-500 dark:text-primary-400" />
              Meta Mensal (R$)
            </h4>
            
            <div class="space-y-4">
              <!-- Range Slider -->
              <div class="relative pt-6">
                <div class="flex justify-between text-xs text-gray-600 dark:text-gray-400 mb-2">
                  <span>{{ formatCurrency(rangeValues[0]) }}</span>
                  <span>{{ formatCurrency(rangeValues[1]) }}</span>
                </div>
                
                <div class="relative h-2">
                  <div class="absolute w-full h-2 bg-gray-200 dark:bg-gray-700 rounded-full"></div>
                  
                  <div 
                    class="absolute h-2 bg-primary-500 dark:bg-primary-400 rounded-full"
                    :style="{
                      left: (rangeValues[0] / maxMeta * 100) + '%',
                      width: ((rangeValues[1] - rangeValues[0]) / maxMeta * 100) + '%'
                    }"
                  ></div>
                  
                  <input 
                    type="range" 
                    :min="0" 
                    :max="maxMeta" 
                    :step="1000" 
                    v-model.number="rangeValues[0]" 
                    @input="updateMetaRange"
                    class="absolute top-0 w-full h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:bg-white [&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-primary-500 [&::-webkit-slider-thumb]:shadow-lg [&::-webkit-slider-thumb]:cursor-pointer"
                  />
                  
                  <input 
                    type="range" 
                    :min="0" 
                    :max="maxMeta" 
                    :step="1000" 
                    v-model.number="rangeValues[1]" 
                    @input="updateMetaRange"
                    class="absolute top-0 w-full h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:bg-white [&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-primary-500 [&::-webkit-slider-thumb]:shadow-lg [&::-webkit-slider-thumb]:cursor-pointer"
                  />
                </div>
              </div>

              <!-- Inputs Numéricos -->
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs text-gray-500 dark:text-gray-400 mb-1">De</label>
                  <div class="relative">
                    <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 dark:text-gray-400">R$</span>
                    <input 
                      type="number" 
                      v-model.number="rangeValues[0]" 
                      @change="updateMetaRange"
                      :min="0" 
                      :max="rangeValues[1]" 
                      step="1000"
                      class="w-full pl-9 pr-3 py-1.5 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-1 focus:ring-primary-500"
                    />
                  </div>
                </div>
                
                <div>
                  <label class="block text-xs text-gray-500 dark:text-gray-400 mb-1">Até</label>
                  <div class="relative">
                    <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 dark:text-gray-400">R$</span>
                    <input 
                      type="number" 
                      v-model.number="rangeValues[1]" 
                      @change="updateMetaRange"
                      :min="rangeValues[0]" 
                      :max="maxMeta" 
                      step="1000"
                      class="w-full pl-9 pr-3 py-1.5 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-1 focus:ring-primary-500"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Ordenação -->
        <div class="pt-3 border-t border-gray-200 dark:border-gray-700">
          <div class="flex flex-col sm:flex-row sm:items-center gap-3">
            <label class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider flex items-center gap-1.5">
              <IconSort class="w-4 h-4 text-primary-500 dark:text-primary-400" />
              Ordenar por
            </label>
            
            <div class="relative flex-1 max-w-xs">
              <select 
                v-model="sortBy" 
                @change="filterBySort"
                class="w-full pl-3 pr-9 py-2 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-500 appearance-none"
              >
                <option value="nome">Nome (A-Z)</option>
                <option value="nome_desc">Nome (Z-A)</option>
                <option value="metaMensal">Maior meta</option>
                <option value="metaMensal_desc">Menor meta</option>
                <option value="dataInicio">Mais antigas</option>
                <option value="dataInicio_desc">Mais recentes</option>
                <option value="status">Status</option>
                <option value="faturamento">Maior faturamento</option>
              </select>
              <IconChevronDown class="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" />
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- Resumo dos Filtros Ativos -->
    <div v-if="hasActiveFilters" class="flex flex-wrap items-center gap-2 p-3 bg-primary-50 dark:bg-primary-900/20 rounded-lg border border-primary-200 dark:border-primary-800 animate-fadeIn">
      <IconFilter class="w-4 h-4 text-primary-600 dark:text-primary-400" />
      
      <span class="text-sm text-gray-700 dark:text-gray-300 flex-1">
        {{ filterSummaryText }}
      </span>
      
      <button 
        @click="clearAllFilters"
        class="p-1 hover:bg-primary-100 dark:hover:bg-primary-800 rounded-full transition-colors"
        title="Limpar todos os filtros"
      >
        <IconTimes class="w-4 h-4 text-gray-500 dark:text-gray-400" />
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

// Ícones
import IconSearch from '@/components/icons/search.vue';
import IconTimes from '@/components/icons/times.vue';
import IconFilter from '@/components/icons/filter.vue';
import IconList from '@/components/icons/list.vue';
import IconGrid from '@/components/icons/grid.vue';
import IconToggleOn from '@/components/icons/toggle-on.vue';
import IconCheck from '@/components/icons/check.vue';
import IconCalendar from '@/components/icons/calendar.vue';
import IconClock from '@/components/icons/clock.vue';
import IconAlertCircle from '@/components/icons/alert-circle.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconSort from '@/components/icons/sort.vue';
import IconChevronDown from '@/components/icons/chevron-down.vue';

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  filters: {
    type: Object,
    required: true,
    default: () => ({
      status: [],
      expiration: 'all',
      sort: 'nome',
      metaMin: 0,
      metaMax: 50000,
      hasFilters: false
    })
  },
  viewMode: {
    type: String,
    default: 'grid'
  },
  showAdvanced: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits([
  'update:modelValue',
  'toggle-filters',
  'toggle-view',
  'clear-filters',
  'filter-change'
]);

const { formatCurrency } = useUnidadesUI();

// Local state
const searchValue = ref(props.modelValue || '');
const showAdvancedFilters = ref(props.showAdvanced || false);
const sortBy = ref(props.filters?.sort || 'nome');
const rangeValues = ref([
  props.filters?.metaMin || 0, 
  props.filters?.metaMax || 50000
]);
const maxMeta = ref(100000);
const expirationFilter = ref(props.filters?.expiration || 'all');

// Computed
const hasActiveFilters = computed(() => {
  return (
    searchValue.value !== '' ||
    (props.filters?.status && props.filters.status.length > 0) ||
    rangeValues.value[0] > 0 ||
    rangeValues.value[1] < maxMeta.value ||
    sortBy.value !== 'nome' ||
    expirationFilter.value !== 'all'
  );
});

const filterSummaryText = computed(() => {
  const parts = [];

  // Pesquisa (agora verifica searchValue local)
  if (searchValue.value) {
    parts.push(`"${searchValue.value}"`);
  }

  // Status
  if (props.filters?.status && props.filters.status.length > 0) {
    const statusList = [];
    if (props.filters.status.includes('ativa')) statusList.push('Ativas');
    if (props.filters.status.includes('inativa')) statusList.push('Inativas');
    if (statusList.length > 0) {
      parts.push(`Status: ${statusList.join(', ')}`);
    }
  }

  // Vencimento
  if (expirationFilter.value !== 'all') {
    parts.push(expirationFilter.value === 'expiring' ? 'Vencem em 30 dias' : 'Expiradas');
  }

  // Ordenação
  if (sortBy.value !== 'nome') {
    const sortLabels = {
      'nome': 'Nome (A-Z)',
      'nome_desc': 'Nome (Z-A)',
      'metaMensal': 'Maior meta',
      'metaMensal_desc': 'Menor meta',
      'dataInicio': 'Mais antigas',
      'dataInicio_desc': 'Mais recentes',
      'status': 'Status',
      'faturamento': 'Maior faturamento'
    };
    parts.push(`Ordenado: ${sortLabels[sortBy.value]}`);
  }

  // Meta
  if (rangeValues.value[0] > 0 || rangeValues.value[1] < maxMeta.value) {
    parts.push(`Meta: ${formatCurrency(rangeValues.value[0])} - ${formatCurrency(rangeValues.value[1])}`);
  }

  return parts.length > 0 ? parts.join(' • ') : 'Nenhum filtro aplicado';
});

// Watchers
watch(() => props.modelValue, (newVal) => {
  searchValue.value = newVal || '';
});

watch(() => props.showAdvanced, (value) => {
  showAdvancedFilters.value = value;
});

watch(() => props.filters, (newFilters) => {
  if (newFilters) {
    // Atualiza sortBy
    if (newFilters.sort && newFilters.sort !== sortBy.value) {
      sortBy.value = newFilters.sort;
    }
    
    // Atualiza expirationFilter
    if (newFilters.expiration && newFilters.expiration !== expirationFilter.value) {
      expirationFilter.value = newFilters.expiration;
    }
    
    // Atualiza rangeValues
    if (newFilters.metaMin !== undefined && newFilters.metaMax !== undefined) {
      rangeValues.value = [
        newFilters.metaMin || 0,
        newFilters.metaMax || 50000
      ];
    }
  }
}, { deep: true, immediate: true });

// Methods
const handleSearchInput = (event) => {
  searchValue.value = event.target.value;
  // Emite update:modelValue para o v-model
  emit('update:modelValue', searchValue.value);
  // Emite filter-change com todos os filtros atuais
  emit('filter-change', { 
    ...props.filters,
    search: searchValue.value 
  });
};

const clearSearch = () => {
  searchValue.value = '';
  emit('update:modelValue', '');
  emit('filter-change', { 
    ...props.filters,
    search: '' 
  });
};

const toggleFilters = () => {
  showAdvancedFilters.value = !showAdvancedFilters.value;
  emit('toggle-filters', showAdvancedFilters.value);
};

const toggleView = () => {
  const newViewMode = props.viewMode === 'grid' ? 'list' : 'grid';
  emit('toggle-view', newViewMode);
};

const clearAllFilters = () => {
  // Reset local state
  searchValue.value = '';
  sortBy.value = 'nome';
  rangeValues.value = [0, 50000];
  expirationFilter.value = 'all';

  // Emit events
  emit('update:modelValue', '');
  emit('clear-filters');
  emit('filter-change', {
    status: [],
    expiration: 'all',
    sort: 'nome',
    metaMin: 0,
    metaMax: 50000,
    search: '',
    hasFilters: false
  });
};

const isStatusActive = (status) => {
  return props.filters?.status?.includes(status) || false;
};

const toggleStatus = (status) => {
  const currentStatus = props.filters?.status ? [...props.filters.status] : [];
  const index = currentStatus.indexOf(status);
  
  if (index > -1) {
    currentStatus.splice(index, 1);
  } else {
    currentStatus.push(status);
  }
  
  emit('filter-change', { 
    ...props.filters,
    status: currentStatus,
    search: searchValue.value,
    hasFilters: currentStatus.length > 0 || hasActiveFilters.value
  });
};

const filterByExpiration = (type) => {
  expirationFilter.value = type;
  emit('filter-change', { 
    ...props.filters,
    expiration: type,
    search: searchValue.value,
    hasFilters: true
  });
};

const updateMetaRange = () => {
  // Garante que os valores estejam na ordem correta
  if (rangeValues.value[0] > rangeValues.value[1]) {
    rangeValues.value = [rangeValues.value[1], rangeValues.value[0]];
  }
  
  // Garante limites
  rangeValues.value[0] = Math.max(0, rangeValues.value[0]);
  rangeValues.value[1] = Math.min(maxMeta.value, rangeValues.value[1]);
  
  emit('filter-change', {
    ...props.filters,
    metaMin: rangeValues.value[0],
    metaMax: rangeValues.value[1],
    search: searchValue.value,
    hasFilters: true
  });
};

const filterBySort = () => {
  emit('filter-change', { 
    ...props.filters,
    sort: sortBy.value,
    search: searchValue.value,
    hasFilters: true
  });
};

// Initialize
onMounted(() => {
  // Garantir que os valores iniciais estejam sincronizados
  if (props.filters) {
    if (props.filters.metaMin !== undefined && props.filters.metaMax !== undefined) {
      rangeValues.value = [props.filters.metaMin || 0, props.filters.metaMax || 50000];
    }
    
    if (props.filters.sort) {
      sortBy.value = props.filters.sort;
    }
    
    if (props.filters.expiration) {
      expirationFilter.value = props.filters.expiration;
    }
  }
  
  if (props.modelValue) {
    searchValue.value = props.modelValue;
  }
  
  if (props.showAdvanced !== undefined) {
    showAdvancedFilters.value = props.showAdvanced;
  }
});
</script>

<style scoped>
@import '@/assets/default.css';

/* Animações */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fadeIn {
  animation: fadeIn 0.3s ease-out;
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

/* Range slider custom styles */
input[type=range]::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: white;
  border: 2px solid theme('colors.primary.500');
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  margin-top: -6px;
}

input[type=range]::-moz-range-thumb {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: white;
  border: 2px solid theme('colors.primary.500');
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  cursor: pointer;
}

input[type=range]::-webkit-slider-runnable-track {
  width: 100%;
  height: 4px;
  background: transparent;
}

input[type=range]::-moz-range-track {
  width: 100%;
  height: 4px;
  background: transparent;
}
</style>