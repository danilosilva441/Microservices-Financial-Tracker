<!-- components/unidades/UnidadeList.vue -->
<template>
  <div class="unidade-table">
    <table class="table" aria-label="Lista de unidades">
      <thead>
        <tr>
          <th scope="col" style="width: 50px;">
            <input 
              type="checkbox" 
              v-model="selectAll"
              @change="toggleSelectAll"
              :aria-label="selectAll ? 'Desmarcar todas as unidades' : 'Marcar todas as unidades'"
            >
          </th>
          <th 
            scope="col"
            @click="sortBy('nome')" 
            @keyup.enter="sortBy('nome')"
            tabindex="0"
            class="sortable"
          >
            <div class="th-content">
              Nome
              <i v-if="sortField === 'nome'" :class="sortIcon" aria-hidden="true"></i>
              <i v-else class="fas fa-sort" aria-hidden="true"></i>
            </div>
          </th>
          <th scope="col">Status</th>
          <th 
            scope="col"
            @click="sortBy('metaMensal')" 
            @keyup.enter="sortBy('metaMensal')"
            tabindex="0"
            class="sortable"
          >
            <div class="th-content">
              Meta Mensal
              <i v-if="sortField === 'metaMensal'" :class="sortIcon" aria-hidden="true"></i>
              <i v-else class="fas fa-sort" aria-hidden="true"></i>
            </div>
          </th>
          <th scope="col">Progresso</th>
          <th scope="col">
            <div class="th-content">
              <i class="fas fa-users" aria-hidden="true"></i>
              Funcionários
            </div>
          </th>
          <th scope="col">
            <div class="th-content">
              <i class="fas fa-calendar-alt" aria-hidden="true"></i>
              Vencimento
            </div>
          </th>
          <th scope="col">Ações</th>
        </tr>
      </thead>
      <tbody>
        <tr 
          v-for="unidade in sortedUnidades" 
          :key="unidade.id"
          :class="{ 
            'table-row-selected': selectedItems.includes(unidade.id),
            'table-row-inactive': !unidade.isAtivo 
          }"
          @click="handleRowClick(unidade.id)"
          @keyup.enter="handleRowClick(unidade.id)"
          tabindex="0"
          role="button"
          :aria-label="`Unidade ${unidade.nome}. Clique para ver detalhes`"
        >
          <td @click.stop @keyup.enter.stop>
            <input 
              type="checkbox" 
              :checked="selectedItems.includes(unidade.id)"
              @change="toggleSelect(unidade.id)"
              :id="`checkbox-${unidade.id}`"
              :aria-label="`Selecionar unidade ${unidade.nome}`"
            >
          </td>
          <td>
            <div class="unidade-cell">
              <div class="unidade-avatar-sm">
                <!-- Ícone provisório - substituir depois -->
                <i class="fas fa-store" aria-hidden="true"></i>
              </div>
              <div class="unidade-info-sm">
                <div class="unidade-name">{{ unidade.nome }}</div>
                <div class="unidade-address" :title="unidade.endereco">
                  <!-- Ícone provisório para endereço -->
                  <i class="fas fa-map-marker-alt" aria-hidden="true" style="margin-right: 4px; font-size: 10px;"></i>
                  {{ unidade.endereco }}
                </div>
              </div>
            </div>
          </td>
          <td>
            <span :class="statusBadgeClass(unidade)" :title="statusText(unidade)">
              <i :class="statusIcon(unidade)" aria-hidden="true"></i>
              {{ statusText(unidade) }}
            </span>
          </td>
          <td>
            <div class="meta-cell">
              <!-- Ícone provisório para meta -->
              <i class="fas fa-bullseye" aria-hidden="true" style="margin-right: 8px; color: #667eea;"></i>
              <div class="meta-value">{{ formatCurrency(unidade.metaMensal) }}</div>
            </div>
          </td>
          <td>
            <div class="progress-cell">
              <div class="progress-info">
                <span class="progress-percentage">
                  <!-- Ícone provisório baseado no progresso -->
                  <i 
                    :class="progressIcon(unidade)" 
                    aria-hidden="true"
                    style="margin-right: 4px;"
                  ></i>
                  {{ progress(unidade) }}%
                </span>
                <span class="progress-label">da meta</span>
              </div>
              <div class="unidade-progress">
                <div 
                  class="unidade-progress-bar"
                  :style="progressBarStyle(unidade)"
                  role="progressbar"
                  :aria-valuenow="progress(unidade)"
                  aria-valuemin="0"
                  aria-valuemax="100"
                  :aria-label="`Progresso: ${progress(unidade)}% da meta`"
                ></div>
              </div>
            </div>
          </td>
          <td>
            <div class="employees-cell">
              <i class="fas fa-users" aria-hidden="true"></i>
              <span>{{ funcionariosAtivos(unidade) }}</span>
              <span class="employees-label">ativos</span>
            </div>
          </td>
          <td>
            <div class="expiration-cell">
              <div class="expiration-date">
                <i class="fas fa-calendar" aria-hidden="true"></i>
                <span v-if="unidade.dataFim" :title="formatDate(unidade.dataFim)">
                  {{ formatDate(unidade.dataFim) }}
                </span>
                <span v-else class="text-muted">
                  <i class="fas fa-ban" aria-hidden="true" style="margin-right: 4px;"></i>
                  Sem data
                </span>
              </div>
              <div 
                v-if="expirationStatus(unidade)" 
                class="expiration-badge"
                :class="expirationBadgeClass(unidade)"
              >
                <i :class="expirationIcon(unidade)" aria-hidden="true"></i>
                {{ expirationStatus(unidade).label }}
              </div>
            </div>
          </td>
          <td>
            <div class="actions-cell" @click.stop @keyup.enter.stop>
              <button 
                class="btn-action-sm"
                @click="handleEdit(unidade)"
                @keyup.enter="handleEdit(unidade)"
                :title="`Editar ${unidade.nome}`"
                :aria-label="`Editar unidade ${unidade.nome}`"
              >
                <i class="fas fa-edit" aria-hidden="true"></i>
              </button>
              
              <button 
                v-if="unidade.isAtivo"
                class="btn-action-sm btn-warning"
                @click="handleDeactivate(unidade)"
                @keyup.enter="handleDeactivate(unidade)"
                :title="`Desativar ${unidade.nome}`"
                :aria-label="`Desativar unidade ${unidade.nome}`"
              >
                <i class="fas fa-power-off" aria-hidden="true"></i>
              </button>
              
              <button 
                v-else
                class="btn-action-sm btn-success"
                @click="handleActivate(unidade)"
                @keyup.enter="handleActivate(unidade)"
                :title="`Ativar ${unidade.nome}`"
                :aria-label="`Ativar unidade ${unidade.nome}`"
              >
                <i class="fas fa-play" aria-hidden="true"></i>
              </button>
              
              <button 
                class="btn-action-sm btn-danger"
                @click="handleDelete(unidade)"
                @keyup.enter="handleDelete(unidade)"
                :title="`Excluir ${unidade.nome}`"
                :aria-label="`Excluir unidade ${unidade.nome}`"
              >
                <i class="fas fa-trash" aria-hidden="true"></i>
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    
    <div v-if="unidades.length === 0" class="table-empty" role="status" aria-live="polite">
      <i class="fas fa-store-slash" aria-hidden="true"></i>
      <p>Nenhuma unidade encontrada</p>
      <button class="btn-add-unidade" @click="$emit('add')">
        <i class="fas fa-plus" aria-hidden="true"></i>
        Adicionar primeira unidade
      </button>
    </div>
    
    <!-- Barra de seleção -->
    <div v-if="selectedItems.length > 0" class="selection-bar">
      <div class="selection-info">
        <i class="fas fa-check-circle" aria-hidden="true"></i>
        {{ selectedItems.length }} unidade(s) selecionada(s)
      </div>
      <div class="selection-actions">
        <button 
          class="btn-selection"
          @click="emitBulkAction('export')"
          title="Exportar selecionados"
        >
          <i class="fas fa-download" aria-hidden="true"></i>
          Exportar
        </button>
        <button 
          class="btn-selection btn-danger"
          @click="emitBulkAction('delete')"
          title="Excluir selecionados"
        >
          <i class="fas fa-trash" aria-hidden="true"></i>
          Excluir
        </button>
        <button 
          class="btn-selection btn-clear"
          @click="clearSelection"
          title="Limpar seleção"
        >
          <i class="fas fa-times" aria-hidden="true"></i>
          Limpar
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

const props = defineProps({
  unidades: {
    type: Array,
    required: true,
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

// Local state
const selectedItems = ref([]);
const sortField = ref('nome');
const sortDirection = ref('asc');

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

const sortIcon = computed(() => 
  sortDirection.value === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down'
);

const sortedUnidades = computed(() => {
  const sorted = [...props.unidades];
  
  sorted.sort((a, b) => {
    let aValue = a[sortField.value];
    let bValue = b[sortField.value];
    
    // Tratamento especial para alguns campos
    if (sortField.value === 'nome') {
      aValue = aValue?.toLowerCase() || '';
      bValue = bValue?.toLowerCase() || '';
    }
    
    if (sortField.value === 'metaMensal') {
      aValue = aValue || 0;
      bValue = bValue || 0;
    }
    
    if (aValue < bValue) return sortDirection.value === 'asc' ? -1 : 1;
    if (aValue > bValue) return sortDirection.value === 'asc' ? 1 : -1;
    return 0;
  });
  
  return sorted;
});

// Methods
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

const statusBadgeClass = (unidade) => {
  const badge = getStatusBadge(unidade.isAtivo);
  return `unidade-status-badge ${badge.variant === 'success' ? 'unidade-status-active' : 'unidade-status-inactive'}`;
};

const statusIcon = (unidade) => {
  const badge = getStatusBadge(unidade.isAtivo);
  return `fas fa-${badge.icon}`;
};

const statusText = (unidade) => {
  const badge = getStatusBadge(unidade.isAtivo);
  return badge.label;
};

const progress = (unidade) => {
  const faturamentoAtual = unidade.faturamentoAtual || 0;
  const meta = unidade.metaMensal || 0;
  if (meta <= 0) return 0;
  return Math.min(Math.round((faturamentoAtual / meta) * 100), 100);
};

const progressIcon = (unidade) => {
  const p = progress(unidade);
  if (p >= 75) return 'fas fa-check-circle';
  if (p >= 50) return 'fas fa-chart-line';
  if (p >= 25) return 'fas fa-exclamation-circle';
  return 'fas fa-times-circle';
};

const progressBarStyle = (unidade) => {
  const p = progress(unidade);
  let backgroundColor = '#ef4444';
  
  if (p >= 75) backgroundColor = '#10b981';
  else if (p >= 50) backgroundColor = '#f59e0b';
  else if (p >= 25) backgroundColor = '#f97316';
  
  return {
    width: `${p}%`,
    backgroundColor
  };
};

const funcionariosAtivos = (unidade) => unidade.funcionariosAtivos || 0;

const expirationStatus = (unidade) => getExpirationStatus(unidade.dataFim);

const expirationBadgeClass = (unidade) => {
  const status = expirationStatus(unidade);
  if (!status) return '';
  return `expiration-badge-${status.variant}`;
};

const expirationIcon = (unidade) => {
  const status = expirationStatus(unidade);
  if (!status) return '';
  return `fas fa-${status.icon}`;
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
</script>

<style scoped>
.unidade-table {
  width: 100%;
  overflow-x: auto;
  position: relative;
}

.table {
  width: 100%;
  border-collapse: collapse;
  min-width: 1000px;
}

.table th {
  background: #f8fafc;
  padding: 16px;
  text-align: left;
  font-weight: 600;
  color: #4a5568;
  border-bottom: 2px solid var(--unidade-border);
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  position: sticky;
  top: 0;
  z-index: 10;
}

.table td {
  padding: 16px;
  border-bottom: 1px solid var(--unidade-border);
  vertical-align: middle;
}

.table tbody tr {
  background: white;
  transition: all 0.3s;
}

.table tbody tr:hover {
  background: #f8fafc;
  cursor: pointer;
}

.table tbody tr:focus {
  outline: 2px solid var(--unidade-color-primary);
  outline-offset: -2px;
}

.table-row-selected {
  background: rgba(102, 126, 234, 0.05) !important;
}

.table-row-inactive {
  opacity: 0.7;
}

.sortable {
  cursor: pointer;
  user-select: none;
}

.sortable:hover {
  background: #edf2f7;
}

.sortable:focus {
  outline: 2px solid var(--unidade-color-primary);
}

.th-content {
  display: flex;
  align-items: center;
  gap: 6px;
}

.th-content i {
  font-size: 12px;
  color: var(--unidade-color-primary);
}

.unidade-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.unidade-avatar-sm {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 16px;
  flex-shrink: 0;
}

.unidade-info-sm {
  flex: 1;
  min-width: 0;
}

.unidade-name {
  font-weight: 600;
  color: #1a202c;
  font-size: 14px;
  margin-bottom: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.unidade-address {
  font-size: 12px;
  color: #718096;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.unidade-status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.unidade-status-active {
  background: #d1fae5;
  color: #065f46;
}

.unidade-status-inactive {
  background: #f3f4f6;
  color: #6b7280;
}

.meta-cell {
  display: flex;
  align-items: center;
}

.meta-value {
  font-weight: 700;
  color: var(--unidade-color-primary);
  font-size: 14px;
}

.progress-cell {
  min-width: 120px;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.progress-percentage {
  display: flex;
  align-items: center;
  font-weight: 700;
  color: #1a202c;
  font-size: 14px;
}

.progress-label {
  font-size: 12px;
  color: #718096;
}

.unidade-progress {
  height: 6px;
  background: #e5e7eb;
  border-radius: 3px;
  overflow: hidden;
}

.unidade-progress-bar {
  height: 100%;
  border-radius: 3px;
  transition: width 0.3s ease;
}

.employees-cell {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #1a202c;
}

.employees-cell i {
  color: var(--unidade-color-primary);
  font-size: 14px;
}

.employees-label {
  font-size: 12px;
  color: #718096;
  margin-left: 4px;
}

.expiration-cell {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.expiration-date {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  color: #1a202c;
}

.expiration-date i {
  font-size: 12px;
  color: #718096;
}

.expiration-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 10px;
  width: fit-content;
}

.expiration-badge-warning {
  background: #fef3c7;
  color: #92400e;
}

.expiration-badge-danger {
  background: #fee2e2;
  color: #dc2626;
}

.expiration-badge-success {
  background: #d1fae5;
  color: #065f46;
}

.actions-cell {
  display: flex;
  gap: 8px;
}

.btn-action-sm {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  border: 1px solid var(--unidade-border);
  background: white;
  color: #718096;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 12px;
}

.btn-action-sm:hover,
.btn-action-sm:focus {
  background: #edf2f7;
  color: #4a5568;
  transform: translateY(-1px);
  outline: none;
}

.btn-action-sm.btn-warning:hover {
  background: #fef3c7;
  color: #92400e;
  border-color: #fde68a;
}

.btn-action-sm.btn-success:hover {
  background: #d1fae5;
  color: #065f46;
  border-color: #a7f3d0;
}

.btn-action-sm.btn-danger:hover {
  background: #fee2e2;
  color: #dc2626;
  border-color: #fecaca;
}

.table-empty {
  padding: 60px 20px;
  text-align: center;
  color: #cbd5e0;
  background: #f9fafb;
  border-radius: 8px;
  margin-top: 20px;
}

.table-empty i {
  font-size: 48px;
  margin-bottom: 16px;
  color: #d1d5db;
}

.table-empty p {
  font-size: 16px;
  color: #718096;
  margin-bottom: 20px;
}

.btn-add-unidade {
  padding: 10px 20px;
  background: var(--unidade-color-primary);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s;
}

.btn-add-unidade:hover {
  background: #5a67d8;
  transform: translateY(-1px);
}

.text-muted {
  color: #a0aec0 !important;
  display: flex;
  align-items: center;
  gap: 4px;
}

.selection-bar {
  position: sticky;
  bottom: 0;
  left: 0;
  right: 0;
  background: white;
  border-top: 1px solid var(--unidade-border);
  padding: 12px 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 -2px 10px rgba(0, 0, 0, 0.1);
  z-index: 20;
}

.selection-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #4a5568;
  font-weight: 600;
}

.selection-info i {
  color: var(--unidade-color-primary);
}

.selection-actions {
  display: flex;
  gap: 8px;
}

.btn-selection {
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 13px;
  font-weight: 600;
  border: 1px solid var(--unidade-border);
  background: white;
  color: #4a5568;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: all 0.3s;
}

.btn-selection:hover {
  background: #edf2f7;
}

.btn-selection.btn-danger {
  color: #dc2626;
  border-color: #fecaca;
}

.btn-selection.btn-danger:hover {
  background: #fee2e2;
}

.btn-selection.btn-clear {
  color: #6b7280;
}

@media (max-width: 768px) {
  .table th:nth-child(5),
  .table td:nth-child(5),
  .table th:nth-child(6),
  .table td:nth-child(6) {
    display: none;
  }
  
  .selection-bar {
    flex-direction: column;
    gap: 12px;
    align-items: stretch;
  }
  
  .selection-actions {
    justify-content: flex-end;
  }
}
</style>