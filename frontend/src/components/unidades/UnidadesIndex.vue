<!-- components/unidades/UnidadesIndex.vue -->
<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 transition-colors duration-200" :class="{ 'dark': isDarkMode }">
    <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <!-- Cabeçalho com Ações -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
        <div class="flex-1 min-w-0">
          <h1 class="text-2xl sm:text-3xl font-bold text-gray-900 dark:text-white flex items-center gap-2">
            <IconStore class="w-6 h-6 sm:w-8 sm:h-8 text-primary-500 dark:text-primary-400" />
            Unidades
          </h1>
          <p class="text-sm sm:text-base text-gray-600 dark:text-gray-400 mt-1">
            Gerencie todas as unidades da sua rede
          </p>
        </div>
        
        <div class="flex items-center gap-2 sm:gap-3">
          <button @click="navigateToCreate" class="btn-primary whitespace-nowrap">
            <IconPlus class="w-4 h-4 mr-2" />
            <span class="hidden sm:inline">Nova Unidade</span>
            <span class="sm:hidden">Nova</span>
          </button>
          <button @click="refreshData" class="btn-outline p-2 sm:px-4 sm:py-2">
            <IconRefresh class="w-4 h-4 sm:mr-2" />
            <span class="hidden sm:inline">Atualizar</span>
          </button>
        </div>
      </div>

      <!-- Estatísticas -->
      <UnidadeStats :stats="stats" :loading="isLoading" />

      <!-- Filtros -->
      <UnidadeFilters 
        v-model:search="searchTerm" 
        :filters="filters" 
        :view-mode="viewMode" 
        @toggle-filters="toggleFilters"
        @toggle-view="toggleViewMode" 
        @clear-filters="clearFilters" 
      />

      <!-- Conteúdo Principal -->
      <div class="mt-6 space-y-4">
        <!-- Ações em Lote -->
        <div v-if="selectedUnidades.length > 0" class="selection-bar">
          <div class="selection-info">
            <IconCheckCircle class="w-5 h-5 text-primary-500 dark:text-primary-400" />
            <span class="font-medium">{{ selectedUnidades.length }} unidade(s) selecionada(s)</span>
          </div>
          
          <div class="selection-actions">
            <button class="btn-selection" @click="ativarSelecionadas" title="Ativar unidades">
              <IconCheckCircle class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Ativar</span>
            </button>
            <button class="btn-selection" @click="desativarSelecionadas" title="Desativar unidades">
              <IconBan class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Desativar</span>
            </button>
            <button class="btn-selection btn-danger" @click="excluirSelecionadas" title="Excluir unidades">
              <IconTrash class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Excluir</span>
            </button>
            <button class="btn-selection" @click="exportarSelecionadas" title="Exportar unidades">
              <IconDownload class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Exportar</span>
            </button>
            <button class="btn-selection" @click="clearSelection" title="Limpar seleção">
              <IconTimes class="w-4 h-4" />
            </button>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="isLoadingData" class="bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 p-8 sm:p-12">
          <div class="flex flex-col items-center justify-center text-center">
            <IconLoader class="w-12 h-12 animate-spin text-primary-500 dark:text-primary-400 mb-4" />
            <p class="text-gray-600 dark:text-gray-400">Carregando unidades...</p>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else-if="isEmpty" class="bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 p-8 sm:p-12">
          <div class="flex flex-col items-center justify-center text-center">
            <div class="w-20 h-20 rounded-full bg-gray-100 dark:bg-gray-700 flex items-center justify-center mb-4">
              <IconStoreSlash class="w-10 h-10 text-gray-500 dark:text-gray-400" />
            </div>
            <h3 class="text-lg sm:text-xl font-bold text-gray-900 dark:text-white mb-2">
              Nenhuma unidade encontrada
            </h3>
            <p v-if="hasFilters" class="text-sm text-gray-600 dark:text-gray-400 mb-4">
              Tente limpar os filtros para ver mais resultados
            </p>
            <p v-else class="text-sm text-gray-600 dark:text-gray-400 mb-4">
              Comece criando sua primeira unidade
            </p>
            
            <div class="flex flex-col sm:flex-row gap-3">
              <button @click="openCreateModal" class="btn-primary">
                <IconPlus class="w-4 h-4 mr-2" />
                Criar primeira unidade
              </button>
              <button v-if="hasFilters" @click="clearFilters" class="btn-outline">
                <IconTimes class="w-4 h-4 mr-2" />
                Limpar filtros
              </button>
            </div>
          </div>
        </div>

        <!-- Lista de Unidades -->
        <div v-else>
          <!-- View Mode: Grid -->
          <div v-if="isGridView" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-6">
            <UnidadeCard 
              v-for="unidade in unidadesFiltradas" 
              :key="unidade.id" 
              :unidade="unidade" 
              :ui="ui"
              :selected="selectedUnidades.includes(unidade.id)"
              @click="goToDetails(unidade.id)"
              @edit="goToEdit(unidade.id)" 
              @delete="openDeleteModal(unidade)" 
              @deactivate="openDeactivateModal(unidade)"
              @select="toggleSelectUnidade(unidade.id)" 
            />
          </div>

          <!-- View Mode: Table -->
          <div v-else class="bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 overflow-hidden">
            <UnidadeList 
              :unidades="unidadesFiltradas" 
              :ui="ui" 
              :selected-items="selectedUnidades"
              @row-click="goToDetails($event)" 
              @edit="goToEdit" 
              @delete="openDeleteModal"
              @deactivate="openDeactivateModal" 
              @selection-change="selectedUnidades = $event"
            />
          </div>

          <!-- Informações da Página e Paginação -->
          <div class="mt-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div class="text-sm text-gray-600 dark:text-gray-400 order-2 sm:order-1">
              Mostrando <span class="font-medium text-gray-900 dark:text-white">{{ unidadesFiltradas.length }}</span> de 
              <span class="font-medium text-gray-900 dark:text-white">{{ totalUnidades }}</span> unidades
            </div>

            <div class="flex flex-col sm:flex-row items-center gap-4 order-1 sm:order-2">
              <!-- Itens por página -->
              <div class="flex items-center gap-2 text-sm">
                <span class="text-gray-600 dark:text-gray-400">Itens por página:</span>
                <select 
                  v-model="itemsPerPage" 
                  @change="changeItemsPerPage" 
                  class="form-select rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
                >
                  <option value="10">10</option>
                  <option value="25">25</option>
                  <option value="50">50</option>
                  <option value="100">100</option>
                </select>
              </div>

              <!-- Paginação -->
              <nav v-if="pagination.totalPaginas > 1" class="flex items-center gap-1" aria-label="Navegação de unidades">
                <button
                  class="pagination-button"
                  :class="{ 'opacity-50 cursor-not-allowed': pagination.pagina === 1 }"
                  :disabled="pagination.pagina === 1"
                  @click="changePage(pagination.pagina - 1)"
                >
                  <IconChevronLeft class="w-4 h-4" />
                </button>

                <button
                  v-for="page in visiblePages"
                  :key="page"
                  class="pagination-button"
                  :class="{ 'active': page === pagination.pagina }"
                  @click="changePage(page)"
                >
                  {{ page }}
                </button>

                <button
                  class="pagination-button"
                  :class="{ 'opacity-50 cursor-not-allowed': pagination.pagina === pagination.totalPaginas }"
                  :disabled="pagination.pagina === pagination.totalPaginas"
                  @click="changePage(pagination.pagina + 1)"
                >
                  <IconChevronRight class="w-4 h-4" />
                </button>

                <button
                  v-if="pagination.totalPaginas > 5 && pagination.pagina < pagination.totalPaginas - 2"
                  class="pagination-button"
                  @click="changePage(pagination.totalPaginas)"
                  title="Última página"
                >
                  <IconChevronsRight class="w-4 h-4" />
                </button>

                <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">
                  Página {{ pagination.pagina }} de {{ pagination.totalPaginas }}
                </span>
              </nav>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal de Criação -->
      <Teleport to="body">
        <div v-if="showCreateModal" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
          <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <div class="fixed inset-0 bg-gray-500 bg-opacity-75 dark:bg-gray-900 dark:bg-opacity-75 transition-opacity" aria-hidden="true" @click="closeCreateModal"></div>

            <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

            <div class="inline-block align-bottom bg-white dark:bg-gray-800 rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl w-full">
              <div class="bg-white dark:bg-gray-800 px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
                  <h3 class="text-lg font-semibold text-gray-900 dark:text-white flex items-center gap-2">
                    <IconPlusCircle class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                    Nova Unidade
                  </h3>
                  <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300">
                    <IconTimes class="w-5 h-5" />
                  </button>
                </div>
                <div class="mt-4">
                  <UnidadeForm 
                    ref="unidadeForm" 
                    mode="create" 
                    @success="handleCreateSuccess" 
                    @cancel="closeCreateModal" 
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </Teleport>

      <!-- Modal de Confirmação de Exclusão -->
      <ConfirmModal 
        v-if="showDeleteModal" 
        :title="`Excluir ${selectedUnidade?.nome}?`"
        message="Esta ação não pode ser desfeita. A unidade será marcada como inativa." 
        confirm-text="Excluir"
        cancel-text="Cancelar" 
        variant="danger" 
        @confirm="confirmDelete" 
        @cancel="closeDeleteModal" 
      />

      <!-- Modal de Confirmação de Desativação -->
      <ConfirmModal 
        v-if="showDeactivateModal" 
        :title="`Desativar ${selectedUnidade?.nome}?`"
        message="A unidade será desativada mas permanecerá no sistema para histórico." 
        confirm-text="Desativar"
        cancel-text="Cancelar" 
        variant="warning" 
        @confirm="confirmDeactivate" 
        @cancel="closeDeactivateModal" 
      />

      <!-- Modal de Ações em Lote -->
      <ConfirmModal 
        v-if="showBatchDeleteModal" 
        title="Excluir unidades selecionadas?"
        message="Tem certeza que deseja excluir as unidades selecionadas? Esta ação não pode ser desfeita."
        confirm-text="Excluir Todas" 
        cancel-text="Cancelar" 
        variant="danger" 
        @confirm="confirmBatchDelete"
        @cancel="closeBatchDeleteModal" 
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useTheme } from '@/composables/useTheme';
import { useUnidades } from '@/composables/unidades/useUnidades.js';

// Ícones
import IconStore from '@/components/icons/store.vue';
import IconStoreSlash from '@/components/icons/store-slash.vue';
import IconPlus from '@/components/icons/plus.vue';
import IconPlusCircle from '@/components/icons/plus-circle.vue';
import IconRefresh from '@/components/icons/refresh.vue';
import IconLoader from '@/components/icons/loader.vue';
import IconCheckCircle from '@/components/icons/check-circle.vue';
import IconBan from '@/components/icons/ban.vue';
import IconTrash from '@/components/icons/trash.vue';
import IconDownload from '@/components/icons/download.vue';
import IconTimes from '@/components/icons/times.vue';
import IconChevronLeft from '@/components/icons/chevron-left.vue';
import IconChevronRight from '@/components/icons/chevron-right.vue';
import IconChevronsRight from '@/components/icons/chevrons-right.vue';

// Componentes
import UnidadeStats from './UnidadeStats.vue';
import UnidadeFilters from './UnidadeFilters.vue';
import UnidadeCard from './UnidadeCard.vue';
import UnidadeList from './UnidadeList.vue';
import UnidadeForm from './UnidadeForm.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';

const { isDarkMode } = useTheme();
const router = useRouter();

const props = defineProps({
  showSocial: {
    type: Boolean,
    default: true
  },
  redirectTo: {
    type: String,
    default: '/unidades'
  }
});

const {
  // State
  unidadesFiltradas,
  searchTerm,
  selectedUnidades,

  // UI
  ui,
  viewMode,
  showFilters,
  showDeleteModal,
  showDeactivateModal,
  selectedForAction,

  // Filters
  filters,
  hasFilters,

  // Actions
  actions,
  loadUnidades,
  searchUnidades,
  clearSearch,

  // Status
  isLoading,
  isLoadingData,
  isEmpty,
  isGridView,
  isListView,

  // Stats
  totalUnidades,
  totalAtivas,
  totalFaturamentoProjetado,
  mediaFaturamentoProjetado,
  unidadesComVencimentoProximo,
} = useUnidades();

// Local state
const showCreateModal = ref(false);
const showBatchDeleteModal = ref(false);
const unidadeForm = ref(null);
const itemsPerPage = ref(10);
const pagination = ref({
  pagina: 1,
  limite: 10,
  total: 0,
  totalPaginas: 0,
});

// Computed
const selectedUnidade = computed(() => ui.selectedForAction);

const stats = computed(() => ({
  total: totalUnidades.value,
  ativas: totalAtivas.value,
  faturamentoProjetado: totalFaturamentoProjetado.value,
  mediaFaturamento: mediaFaturamentoProjetado.value,
  vencimentoProximo: unidadesComVencimentoProximo.value.length,
}));

const visiblePages = computed(() => {
  const pages = [];
  const current = pagination.value.pagina;
  const total = pagination.value.totalPaginas;

  let start = Math.max(1, current - 2);
  let end = Math.min(total, start + 4);

  if (end - start < 4) {
    start = Math.max(1, end - 4);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

// Watch para atualizar paginação quando os dados mudarem
watch(totalUnidades, (newTotal) => {
  pagination.value.total = newTotal;
  pagination.value.totalPaginas = Math.ceil(newTotal / pagination.value.limite);
});

// Métodos de Ação
const navigateToCreate = () => {
  router.push({ name: 'NovaUnidade' });
};

const openCreateModal = () => {
  showCreateModal.value = true;
};

const closeCreateModal = () => {
  showCreateModal.value = false;
};

const handleCreateSuccess = (unidade) => {
  closeCreateModal();
  goToDetails(unidade.id);
};

const goToDetails = (id) => {
  if (props.redirectTo && props.redirectTo.includes('{id}')) {
    const path = props.redirectTo.replace('{id}', id);
    router.push(path);
  } else {
    router.push({
      name: 'UnidadeDetalhes',
      params: { id }
    });
  }
};

const goToEdit = (id) => {
  router.push({ name: 'EditarUnidade', params: { id } });
};

const openDeleteModal = (unidade) => {
  ui.openDeleteModal(unidade);
};

const closeDeleteModal = () => {
  ui.closeDeleteModal();
};

const confirmDelete = async () => {
  if (selectedUnidade.value) {
    await actions.deleteUnidade(selectedUnidade.value.id);
    closeDeleteModal();
  }
};

const openDeactivateModal = (unidade) => {
  ui.openDeactivateModal(unidade);
};

const closeDeactivateModal = () => {
  ui.closeDeactivateModal();
};

const confirmDeactivate = async () => {
  if (selectedUnidade.value) {
    await actions.deactivateUnidade(selectedUnidade.value.id);
    closeDeactivateModal();
  }
};

const toggleViewMode = () => {
  ui.toggleViewMode();
};

const toggleFilters = () => {
  ui.toggleFilters();
};

const clearFilters = () => {
  filters.clearFilters();
  clearSearch();
};

// Métodos de seleção
const toggleSelectUnidade = (id) => {
  if (selectedUnidades.value.includes(id)) {
    selectedUnidades.value = selectedUnidades.value.filter(item => item !== id);
  } else {
    selectedUnidades.value = [...selectedUnidades.value, id];
  }
};

const clearSelection = () => {
  selectedUnidades.value = [];
};

// Ações em lote
const ativarSelecionadas = async () => {
  if (selectedUnidades.value.length === 0) return;

  try {
    await Promise.all(selectedUnidades.value.map(id => actions.activateUnidade(id)));
    selectedUnidades.value = [];
  } catch (error) {
    console.error('Erro ao ativar unidades:', error);
  }
};

const desativarSelecionadas = async () => {
  if (selectedUnidades.value.length === 0) return;

  try {
    await Promise.all(selectedUnidades.value.map(id => actions.deactivateUnidade(id)));
    selectedUnidades.value = [];
  } catch (error) {
    console.error('Erro ao desativar unidades:', error);
  }
};

const excluirSelecionadas = () => {
  if (selectedUnidades.value.length === 0) return;
  showBatchDeleteModal.value = true;
};

const confirmBatchDelete = async () => {
  try {
    await Promise.all(selectedUnidades.value.map(id => actions.deleteUnidade(id)));
    selectedUnidades.value = [];
    closeBatchDeleteModal();
  } catch (error) {
    console.error('Erro ao excluir unidades:', error);
  }
};

const closeBatchDeleteModal = () => {
  showBatchDeleteModal.value = false;
};

const exportarSelecionadas = () => {
  if (selectedUnidades.value.length === 0) return;
  alert(`Exportando ${selectedUnidades.value.length} unidades...`);
};

const refreshData = async () => {
  await loadUnidades();
};

// Métodos de paginação
const changePage = (page) => {
  if (page >= 1 && page <= pagination.value.totalPaginas) {
    pagination.value.pagina = page;
  }
};

const changeItemsPerPage = () => {
  pagination.value.limite = parseInt(itemsPerPage.value);
  pagination.value.totalPaginas = Math.ceil(totalUnidades.value / pagination.value.limite);
  pagination.value.pagina = 1;
};

// Lifecycle
onMounted(async () => {
  await loadUnidades();
  pagination.value.total = totalUnidades.value;
  pagination.value.totalPaginas = Math.ceil(totalUnidades.value / pagination.value.limite);
});
</script>

<style scoped>
@import '@/assets/default.css';
@import './CSS/UnidadesIndex.css';

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
  @apply px-3 py-1.5 text-sm font-medium rounded-lg border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500 flex items-center;
}

.btn-selection.btn-danger {
  @apply border-red-300 dark:border-red-800 text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20;
}

/* Pagination Buttons */
.pagination-button {
  @apply w-8 h-8 flex items-center justify-center rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 text-sm font-medium hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500;
}

.pagination-button.active {
  @apply bg-primary-500 border-primary-500 text-white hover:bg-primary-600;
}

/* Form Select */
.form-select {
  @apply appearance-none bg-no-repeat pr-8;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-size: 1.5em 1.5em;
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

/* Animações */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.fade-in {
  animation: fadeIn 0.3s ease-out;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .selection-bar {
    @apply mx-0 rounded-none border-x-0 border-b-0;
  }
}
</style>