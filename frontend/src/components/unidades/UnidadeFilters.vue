<!-- components/unidades/UnidadeFilters.vue -->
<template>
  <div class="unidade-filters">
    <!-- Barra de Pesquisa Rápida -->
    <div class="quick-filters">
      <div class="search-container">
        <i class="fas fa-search search-icon"></i>
        <input type="text" v-model="searchValue" @input="handleSearchInput" placeholder="Pesquisar unidades..."
          class="search-input" />
        <button v-if="searchValue" @click="clearSearch" class="search-clear">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div class="filter-actions">
        <button class="btn-filter" @click="toggleFilters" :class="{ active: showAdvancedFilters }">
          <i class="fas fa-filter"></i>
          Filtros
          <span v-if="hasActiveFilters" class="filter-badge"></span>
        </button>

        <button class="btn-view" @click="toggleView"
          :title="viewMode === 'grid' ? 'Visualização em lista' : 'Visualização em grade'">
          <i v-if="viewMode === 'grid'" class="fas fa-list"></i>
          <i v-else class="fas fa-th-large"></i>
        </button>

        <button v-if="hasActiveFilters" class="btn-clear" @click="clearAllFilters">
          <i class="fas fa-times"></i>
          Limpar filtros
        </button>
      </div>
    </div>

    <!-- Filtros Avançados -->
    <transition name="fade">
      <div v-if="showAdvancedFilters" class="advanced-filters">
        <div class="filter-section">
          <h4 class="filter-title">
            <i class="fas fa-toggle-on"></i>
            Status
          </h4>
          <div class="filter-options">
            <label class="filter-checkbox">
              <input type="checkbox" :checked="isStatusActive('ativa')" @change="toggleStatus('ativa')" />
              <span class="checkbox-custom"></span>
              <span class="checkbox-label">
                <span class="status-indicator active"></span>
                Ativas
              </span>
            </label>

            <label class="filter-checkbox">
              <input type="checkbox" :checked="isStatusActive('inativa')" @change="toggleStatus('inativa')" />
              <span class="checkbox-custom"></span>
              <span class="checkbox-label">
                <span class="status-indicator inactive"></span>
                Inativas
              </span>
            </label>
          </div>
        </div>

        <div class="filter-section">
          <h4 class="filter-title">
            <i class="fas fa-calendar-alt"></i>
            Vencimento
          </h4>
          <div class="filter-options">
            <label class="filter-radio">
              <input type="radio" name="expiration" value="all" :checked="expirationFilter === 'all'"
                @change="filterByExpiration('all')" />
              <span class="radio-custom"></span>
              <span class="radio-label">Todos</span>
            </label>

            <label class="filter-radio">
              <input type="radio" name="expiration" value="expiring" :checked="expirationFilter === 'expiring'"
                @change="filterByExpiration('expiring')" />
              <span class="radio-custom"></span>
              <span class="radio-label">Vence em 30 dias</span>
            </label>

            <label class="filter-radio">
              <input type="radio" name="expiration" value="expired" :checked="expirationFilter === 'expired'"
                @change="filterByExpiration('expired')" />
              <span class="radio-custom"></span>
              <span class="radio-label">Expiradas</span>
            </label>
          </div>
        </div>

        <div class="filter-section">
          <h4 class="filter-title">
            <i class="fas fa-chart-line"></i>
            Meta Mensal
          </h4>
          <div class="filter-range">
            <div class="range-labels">
              <span>{{ formatCurrency(rangeValues[0]) }}</span>
              <span>{{ formatCurrency(rangeValues[1]) }}</span>
            </div>
            <div class="range-slider-container">
              <input type="range" :min="0" :max="maxMeta" :step="1000" v-model.number="rangeValues[0]" 
                @input="updateMetaRange" class="range-slider min" />
              <input type="range" :min="0" :max="maxMeta" :step="1000" v-model.number="rangeValues[1]" 
                @input="updateMetaRange" class="range-slider max" />
              <div class="range-track">
                <div class="range-selected" :style="{
                  left: (rangeValues[0] / maxMeta * 100) + '%',
                  width: ((rangeValues[1] - rangeValues[0]) / maxMeta * 100) + '%'
                }"></div>
              </div>
            </div>
            <div class="range-inputs">
              <div class="range-input-group">
                <label>De:</label>
                <input type="number" v-model.number="rangeValues[0]" @change="updateMetaRange" 
                  :min="0" :max="rangeValues[1]" step="1000" class="range-input" />
              </div>
              <div class="range-input-group">
                <label>Até:</label>
                <input type="number" v-model.number="rangeValues[1]" @change="updateMetaRange" 
                  :min="rangeValues[0]" :max="maxMeta" step="1000" class="range-input" />
              </div>
            </div>
          </div>
        </div>

        <div class="filter-section">
          <h4 class="filter-title">
            <i class="fas fa-sort-amount-down"></i>
            Ordenar por
          </h4>
          <div class="filter-select">
            <select v-model="sortBy" @change="filterBySort" class="select-input">
              <option value="nome">Nome (A-Z)</option>
              <option value="nome_desc">Nome (Z-A)</option>
              <option value="metaMensal">Maior meta</option>
              <option value="metaMensal_desc">Menor meta</option>
              <option value="dataInicio">Mais antigas</option>
              <option value="dataInicio_desc">Mais recentes</option>
              <option value="status">Status</option>
              <option value="faturamento">Maior faturamento</option>
            </select>
            <i class="fas fa-chevron-down select-icon"></i>
          </div>
        </div>
      </div>
    </transition>

    <!-- Resumo dos Filtros Ativos -->
    <div v-if="hasActiveFilters" class="active-filters">
      <div class="filters-summary">
        <i class="fas fa-filter"></i>
        <span class="summary-text">{{ filterSummaryText }}</span>
        <button @click="clearAllFilters" class="btn-clear-sm">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  filters: {
    type: Object,
    required: true
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
const searchValue = ref(props.modelValue);
const showAdvancedFilters = ref(props.showAdvanced);
const sortBy = ref('nome');
const rangeValues = ref([0, 50000]);
const maxMeta = ref(100000);
const expirationFilter = ref('all');

// Computed
const hasActiveFilters = computed(() => {
  return props.filters?.hasFilters ||
    rangeValues.value[0] > 0 ||
    rangeValues.value[1] < maxMeta.value ||
    sortBy.value !== 'nome' ||
    expirationFilter.value !== 'all' ||
    searchValue.value.trim() !== '';
});

const filterSummaryText = computed(() => {
  const parts = [];

  // Status
  if (props.filters?.status) {
    const statusList = [];
    if (props.filters.status.includes('ativa')) statusList.push('Ativas');
    if (props.filters.status.includes('inativa')) statusList.push('Inativas');
    if (statusList.length > 0) {
      parts.push(`Status: ${statusList.join(', ')}`);
    }
  }

  // Pesquisa
  if (searchValue.value) {
    parts.push(`Pesquisa: "${searchValue.value}"`);
  }

  // Vencimento
  if (expirationFilter.value !== 'all') {
    parts.push(`Vencimento: ${expirationFilter.value === 'expiring' ? 'Próximas' : 'Expiradas'}`);
  }

  // Ordenação
  if (sortBy.value !== 'nome') {
    parts.push(`Ordenado por: ${getSortLabel(sortBy.value)}`);
  }

  // Meta
  if (rangeValues.value[0] > 0 || rangeValues.value[1] < maxMeta.value) {
    parts.push(`Meta: ${formatCurrency(rangeValues.value[0])} - ${formatCurrency(rangeValues.value[1])}`);
  }

  return parts.join(' • ') || 'Filtros ativos';
});

// Watchers
watch(() => props.modelValue, (newVal) => {
  searchValue.value = newVal;
});

watch(() => props.showAdvanced, (value) => {
  showAdvancedFilters.value = value;
});

// Methods
const handleSearchInput = (event) => {
  emit('update:modelValue', searchValue.value);
  emit('filter-change', { search: searchValue.value });
};

const clearSearch = () => {
  searchValue.value = '';
  emit('update:modelValue', '');
  emit('filter-change', { search: '' });
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

  // Emit clear events
  emit('update:modelValue', '');
  emit('clear-filters');
  emit('filter-change', {
    search: '',
    sort: 'nome',
    expiration: 'all',
    metaMin: 0,
    metaMax: 50000
  });
};

const isStatusActive = (status) => {
  return props.filters?.status?.includes(status) || false;
};

const toggleStatus = (status) => {
  const currentStatus = [...(props.filters.status || [])];
  const index = currentStatus.indexOf(status);
  
  if (index > -1) {
    currentStatus.splice(index, 1);
  } else {
    currentStatus.push(status);
  }
  
  emit('filter-change', { status: currentStatus });
};

const filterByExpiration = (type) => {
  expirationFilter.value = type;
  emit('filter-change', { expiration: type });
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
    metaMin: rangeValues.value[0],
    metaMax: rangeValues.value[1]
  });
};

const filterBySort = () => {
  emit('filter-change', { sort: sortBy.value });
};

const getSortLabel = (value) => {
  const labels = {
    'nome': 'Nome (A-Z)',
    'nome_desc': 'Nome (Z-A)',
    'metaMensal': 'Maior meta',
    'metaMensal_desc': 'Menor meta',
    'dataInicio': 'Mais antigas',
    'dataInicio_desc': 'Mais recentes',
    'status': 'Status',
    'faturamento': 'Maior faturamento'
  };
  return labels[value] || value;
};

// Initialize
onMounted(() => {
  // Se os filtros já tiverem valores, sincroniza
  if (props.filters?.metaMin !== undefined && props.filters?.metaMax !== undefined) {
    rangeValues.value = [props.filters.metaMin || 0, props.filters.metaMax || 50000];
  }
  
  if (props.filters?.sort) {
    sortBy.value = props.filters.sort;
  }
  
  if (props.filters?.expiration) {
    expirationFilter.value = props.filters.expiration;
  }
});
</script>

<style scoped>
@import './CSS/Filters.css';
</style>