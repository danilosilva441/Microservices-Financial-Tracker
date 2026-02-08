<!-- components/unidades/UnidadesIndex.vue -->
<template>
  <div class="unidades-index">
    <!-- Cabeçalho com Ações -->
    <div class="page-header">
      <div class="header-left">
        <h1 class="page-title">Unidades</h1>
        <p class="page-subtitle">Gerencie todas as unidades da sua rede</p>
      </div>
      <div class="header-right">
        <div class="header-actions">
          <!-- Botão com roteamento para página de criação -->
           
          <button @click="navigateToCreate" class="btn btn-primary">

            <i class="fas fa-plus"></i>
            Nova Unidade
          </button>
          <button class="btn btn-outline-secondary" @click="refreshData">
            <i class="fas fa-redo"></i>
            Atualizar
          </button>
        </div>
      </div>
    </div>

    <!-- Estatísticas -->
    <UnidadeStats :stats="stats" :loading="isLoading" />

    <!-- Filtros -->
    <UnidadeFilters v-model:search="searchTerm" :filters="filters" :view-mode="viewMode" @toggle-filters="toggleFilters"
      @toggle-view="toggleViewMode" @clear-filters="clearFilters" />

    <!-- Conteúdo Principal -->
    <div class="unidades-content">
      <!-- Ações em Lote -->
      <div v-if="selectedUnidades.length > 0" class="batch-actions">
        <div class="batch-info">
          <span class="batch-count">{{ selectedUnidades.length }} unidade(s) selecionada(s)</span>
          <button class="btn-clear-batch" @click="clearSelection">
            <i class="fas fa-times"></i> Limpar seleção
          </button>
        </div>
        <div class="batch-buttons">
          <button class="btn btn-sm btn-outline-primary" @click="ativarSelecionadas" title="Ativar unidades">
            <i class="fas fa-check-circle"></i>
            Ativar
          </button>
          <button class="btn btn-sm btn-outline-secondary" @click="desativarSelecionadas" title="Desativar unidades">
            <i class="fas fa-ban"></i>
            Desativar
          </button>
          <button class="btn btn-sm btn-outline-danger" @click="excluirSelecionadas" title="Excluir unidades">
            <i class="fas fa-trash"></i>
            Excluir
          </button>
          <button class="btn btn-sm btn-outline-info" @click="exportarSelecionadas" title="Exportar unidades">
            <i class="fas fa-file-export"></i>
            Exportar
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoadingData" class="loading-state">
        <div class="spinner-border text-primary"></div>
        <p>Carregando unidades...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="isEmpty" class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-store-slash"></i>
        </div>
        <h3>Nenhuma unidade encontrada</h3>
        <p v-if="hasFilters">Tente limpar os filtros ou</p>
        <div class="empty-actions">
          <button @click="openCreateModal" class="btn-primary">
            <i class="fas fa-plus"></i>
            Criar primeira unidade
          </button>
          <button v-if="hasFilters" @click="clearFilters" class="btn-outline">
            <i class="fas fa-times"></i>
            Limpar filtros
          </button>
        </div>
      </div>

      <!-- Lista de Unidades -->
      <div v-else>
        <!-- View Mode: Grid -->
        <div v-if="isGridView" class="unidade-grid">
          <UnidadeCard v-for="unidade in unidadesFiltradas" :key="unidade.id" :unidade="unidade" :ui="ui"
            :selected="selectedUnidades.includes(unidade.id)" @click="goToDetails(unidade.id)"
            @edit="goToEdit(unidade.id)" @delete="openDeleteModal(unidade)" @deactivate="openDeactivateModal(unidade)"
            @select="toggleSelectUnidade(unidade.id)" />
        </div>

        <!-- View Mode: Table -->
        <div v-else class="unidade-table-container">
          <UnidadeList :unidades="unidadesFiltradas" :ui="ui" :selected-unidades="selectedUnidades"
            @row-click="goToDetails($event)" @edit="goToEdit" @delete="openDeleteModal"
            @deactivate="openDeactivateModal" @select="toggleSelectUnidade" @select-all="toggleSelectAll" />
        </div>

        <!-- Informações da Página -->
        <div class="page-info">
          <div class="info-left">
            <span>Mostrando {{ unidadesFiltradas.length }} de {{ totalUnidades }} unidades</span>
          </div>
          <div class="info-right">
            <!-- Opções de Exibição -->
            <div class="view-options">
              <span class="view-label">Itens por página:</span>
              <select v-model="itemsPerPage" @change="changeItemsPerPage" class="view-select">
                <option value="10">10</option>
                <option value="25">25</option>
                <option value="50">50</option>
                <option value="100">100</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Paginação (se aplicável) -->
        <div v-if="pagination.totalPaginas > 1" class="pagination-container">
          <nav aria-label="Navegação de unidades">
            <ul class="pagination">
              <li class="page-item" :class="{ disabled: pagination.pagina === 1 }">
                <button class="page-link" @click="changePage(pagination.pagina - 1)">
                  <i class="fas fa-chevron-left"></i>
                </button>
              </li>

              <li v-for="page in visiblePages" :key="page" class="page-item"
                :class="{ active: page === pagination.pagina }">
                <button class="page-link" @click="changePage(page)">
                  {{ page }}
                </button>
              </li>

              <li class="page-item" :class="{ disabled: pagination.pagina === pagination.totalPaginas }">
                <button class="page-link" @click="changePage(pagination.pagina + 1)">
                  <i class="fas fa-chevron-right"></i>
                </button>
              </li>

              <!-- Botão para última página -->
              <li v-if="pagination.totalPaginas > 5 && pagination.pagina < pagination.totalPaginas - 2"
                class="page-item">
                <button class="page-link" @click="changePage(pagination.totalPaginas)">
                  <i class="fas fa-forward"></i>
                </button>
              </li>
            </ul>
            <div class="pagination-info">
              Página {{ pagination.pagina }} de {{ pagination.totalPaginas }}
            </div>
          </nav>
        </div>
      </div>
    </div>

    <!-- Modal de Criação -->
    <div v-if="showCreateModal" class="modal-overlay" @click.self="closeCreateModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Nova Unidade</h3>
          <button @click="closeCreateModal" class="btn-close" title="Fechar">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <UnidadeForm ref="unidadeForm" mode="create" @success="handleCreateSuccess" @cancel="closeCreateModal" />
        </div>
      </div>
    </div>

    <!-- Modal de Confirmação de Exclusão -->
    <ConfirmModal v-if="showDeleteModal" :title="`Excluir ${selectedUnidade?.nome}?`"
      message="Esta ação não pode ser desfeita. A unidade será marcada como inativa." confirm-text="Excluir"
      cancel-text="Cancelar" variant="danger" @confirm="confirmDelete" @cancel="closeDeleteModal" />

    <!-- Modal de Confirmação de Desativação -->
    <ConfirmModal v-if="showDeactivateModal" :title="`Desativar ${selectedUnidade?.nome}?`"
      message="A unidade será desativada mas permanecerá no sistema para histórico." confirm-text="Desativar"
      cancel-text="Cancelar" variant="warning" @confirm="confirmDeactivate" @cancel="closeDeactivateModal" />

    <!-- Modal de Ações em Lote -->
    <ConfirmModal v-if="showBatchDeleteModal" title="Excluir unidades selecionadas?"
      message="Tem certeza que deseja excluir as unidades selecionadas? Esta ação não pode ser desfeita."
      confirm-text="Excluir Todas" cancel-text="Cancelar" variant="danger" @confirm="confirmBatchDelete"
      @cancel="closeBatchDeleteModal" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, defineProps } from 'vue';
import { useRouter } from 'vue-router';
import { useUnidades } from '@/composables/unidades/useUnidades.js';
import UnidadeStats from './UnidadeStats.vue';
import UnidadeFilters from './UnidadeFilters.vue';
import UnidadeCard from './UnidadeCard.vue';
import UnidadeList from './UnidadeList.vue';
import UnidadeForm from './UnidadeForm.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import './CSS/index.css';

// Definir props usando defineProps
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

const router = useRouter();
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

  // Mostrar no máximo 5 páginas
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

// Métodos de Ação
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
  // Verifica se tem um redirectTo específico nas props
  if (props.redirectTo && props.redirectTo.includes('{id}')) {
    const path = props.redirectTo.replace('{id}', id);
    router.push(path);
  } else {
    // Redirecionamento padrão para a página de detalhes
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

const toggleSelectAll = () => {
  if (selectedUnidades.value.length === unidadesFiltradas.value.length) {
    selectedUnidades.value = [];
  } else {
    selectedUnidades.value = unidadesFiltradas.value.map(u => u.id);
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
  // Implementar exportação das unidades selecionadas
  alert(`Exportando ${selectedUnidades.value.length} unidades...`);
};

const exportarUnidades = () => {
  // Implementar exportação de todas as unidades
  alert('Exportando todas as unidades...');
};

const refreshData = async () => {
  await loadUnidades();
};

// Métodos de paginação
const changePage = (page) => {
  if (page >= 1 && page <= pagination.value.totalPaginas) {
    pagination.value.pagina = page;
    // Aqui você pode adicionar lógica para carregar a página específica
  }
};

const changeItemsPerPage = () => {
  pagination.value.limite = parseInt(itemsPerPage.value);
  pagination.value.totalPaginas = Math.ceil(totalUnidades.value / pagination.value.limite);
  pagination.value.pagina = 1;
};

// Atualizar paginação quando os dados mudarem
onMounted(() => {
  pagination.value.total = totalUnidades.value;
  pagination.value.totalPaginas = Math.ceil(totalUnidades.value / pagination.value.limite);
});

// Lifecycle
onMounted(async () => {
  await loadUnidades();
  // Atualizar paginação após carregar os dados
  pagination.value.total = totalUnidades.value;
  pagination.value.totalPaginas = Math.ceil(totalUnidades.value / pagination.value.limite);
});
</script>

<style scoped>
@import './CSS/UnidadesIndex.css'
</style>