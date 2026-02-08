<!-- components/unidades/UnidadeDetalhes.vue (Atualizado) -->
<!-- components/unidades/UnidadeDetalhes.vue (CORRIGIDO) -->
<template>
  <div class="unidade-detalhes-container">
    <!-- Breadcrumb -->
    <nav class="breadcrumb">
      <ol>
        <li><router-link to="/unidades">Unidades</router-link></li>
        <li><i class="fas fa-chevron-right"></i></li>
        <li v-if="unidade">{{ unidade.nome }}</li>
        <li v-else>Carregando...</li>
      </ol>
    </nav>

    <!-- Header com Informações da Unidade -->
    <div v-if="unidade" class="unidade-header">
      <div class="header-left">
        <div class="unidade-title">
          <h1>{{ unidade.nome }}</h1>
          <span class="status-badge" :class="getStatusClass(unidade.status)">
            {{ unidade.status }}
          </span>
        </div>
        <div class="unidade-info">
          <div class="info-item">
            <i class="fas fa-map-marker-alt"></i>
            <span>{{ unidade.endereco }}, {{ unidade.cidade }} - {{ unidade.estado }}</span>
          </div>
          <div class="info-item">
            <i class="fas fa-phone"></i>
            <span>{{ unidade.telefone || 'Não informado' }}</span>
          </div>
          <div class="info-item">
            <i class="fas fa-envelope"></i>
            <span>{{ unidade.email || 'Não informado' }}</span>
          </div>
        </div>
      </div>
      <div class="header-right">
        <div class="header-actions">
          <button class="btn btn-primary" @click="goToEdit(unidade.id)">
            <i class="fas fa-edit"></i>
            Editar
          </button>
          <button class="btn btn-outline-danger" @click="openDeleteModal(unidade)">
            <i class="fas fa-trash"></i>
            Excluir
          </button>
        </div>
      </div>
    </div>

    <!-- Tabs de Navegação -->
    <div class="tabs-container">
      <div class="tabs">
        <button 
          v-for="tab in tabs" 
          :key="tab.id"
          class="tab"
          :class="{ 'active': activeTab === tab.id }"
          @click="changeTab(tab.id)"
        >
          <i :class="tab.icon"></i>
          {{ tab.label }}
        </button>
      </div>
    </div>

    <!-- Conteúdo da Tab Ativa -->
    <div class="tab-content">
      <!-- Dashboard -->
      <div v-if="activeTab === 'dashboard'" class="dashboard-tab">
        <div class="dashboard-grid">
          <!-- Progresso da Meta -->
          <div class="dashboard-card large">
            <div class="card-header">
              <h3><i class="fas fa-chart-line"></i> Progresso da Meta</h3>
              <div class="card-actions">
                <button class="btn-action" @click="refreshDashboardData">
                  <i class="fas fa-redo"></i>
                </button>
              </div>
            </div>
            <div class="card-body">
              <div v-if="dashboardLoading" class="loading-state">
                <div class="spinner-border spinner-border-sm"></div>
                <span>Carregando dados do dashboard...</span>
              </div>

              <div v-else-if="dashboardError" class="error-state">
                <i class="fas fa-exclamation-circle"></i>
                <span>Erro ao carregar dados: {{ dashboardError }}</span>
                <button @click="refreshDashboardData" class="btn btn-sm btn-outline-primary">
                  Tentar novamente
                </button>
              </div>

              <div v-else-if="dashboardData" class="dashboard-content">
                <!-- Progresso da Meta -->
                <div class="progress-container">
                  <div class="progress-header">
                    <div class="progress-label">
                      <span>Meta: {{ formatCurrency(dashboardData.metaMensal || 0) }}</span>
                      <span>Atual: {{ formatCurrency(dashboardData.faturamentoAtual || 0) }}</span>
                    </div>
                    <div class="progress-percentage">
                      {{ dashboardData.progressoMeta || 0 }}%
                    </div>
                  </div>
                  <div class="progress-bar-large">
                    <div class="progress-fill" :style="{
                      width: `${dashboardData.progressoMeta || 0}%`,
                      backgroundColor: getProgressColor(dashboardData.progressoMeta || 0)
                    }"></div>
                  </div>
                  <div class="progress-footer">
                    <span>0%</span>
                    <span>50%</span>
                    <span>100%</span>
                  </div>
                </div>

                <!-- Estatísticas do Dashboard -->
                <div class="progress-stats">
                  <div class="stat-item">
                    <div class="stat-label">Meta Restante:</div>
                    <div class="stat-value">
                      {{ formatCurrency(dashboardData.metaRestante || 0) }}
                    </div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-label">Média Diária Necessária:</div>
                    <div class="stat-value">
                      {{ formatCurrency(dashboardData.mediaDiariaNecessaria || 0) }}
                    </div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-label">Projeção Mês:</div>
                    <div class="stat-value">
                      {{ formatCurrency(dashboardData.projecaoFaturamento || 0) }}
                    </div>
                  </div>
                </div>

                <!-- Gráficos/Outros dados do dashboard -->
                <div class="dashboard-charts">
                  <div class="chart-placeholder">
                    <i class="fas fa-chart-bar"></i>
                    <p>Dashboard da Unidade</p>
                    <small>Dados de performance e métricas</small>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Resumo Financeiro -->
          <div class="dashboard-card">
            <div class="card-header">
              <h3><i class="fas fa-chart-pie"></i> Resumo Financeiro</h3>
            </div>
            <div class="card-body">
              <div v-if="dashboardData" class="finance-summary">
                <div class="finance-item">
                  <span class="finance-label">Faturamento Total:</span>
                  <span class="finance-value success">
                    {{ formatCurrency(dashboardData.faturamentoTotal || 0) }}
                  </span>
                </div>
                <div class="finance-item">
                  <span class="finance-label">Despesas Totais:</span>
                  <span class="finance-value danger">
                    {{ formatCurrency(dashboardData.despesasTotais || 0) }}
                  </span>
                </div>
                <div class="finance-item">
                  <span class="finance-label">Lucro Líquido:</span>
                  <span class="finance-value" :class="(dashboardData.lucroLiquido || 0) >= 0 ? 'success' : 'danger'">
                    {{ formatCurrency(dashboardData.lucroLiquido || 0) }}
                  </span>
                </div>
                <div class="finance-item">
                  <span class="finance-label">Margem:</span>
                  <span class="finance-value">
                    {{ dashboardData.margem || 0 }}%
                  </span>
                </div>
              </div>
              <div v-else class="no-data">
                <i class="fas fa-chart-line"></i>
                <p>Nenhum dado financeiro disponível</p>
              </div>
            </div>
          </div>

          <!-- Informações Gerais -->
          <div class="dashboard-card">
            <div class="card-header">
              <h3><i class="fas fa-info-circle"></i> Informações Gerais</h3>
            </div>
            <div class="card-body">
              <div class="info-grid">
                <div class="info-item">
                  <span class="info-label">CNPJ:</span>
                  <span class="info-value">{{ unidade?.cnpj || 'Não informado' }}</span>
                </div>
                <div class="info-item">
                  <span class="info-label">Data de Abertura:</span>
                  <span class="info-value">{{ formatDate(unidade?.dataAbertura) || 'Não informada' }}</span>
                </div>
                <div class="info-item">
                  <span class="info-label">Responsável:</span>
                  <span class="info-value">{{ unidade?.responsavel || 'Não informado' }}</span>
                </div>
                <div class="info-item">
                  <span class="info-label">Funcionários:</span>
                  <span class="info-value">{{ dashboardData?.funcionariosAtivos || 0 }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Informações Gerais -->
      <div v-if="activeTab === 'info'" class="info-tab">
        <div class="info-grid">
          <div class="info-card">
            <h3><i class="fas fa-building"></i> Dados da Empresa</h3>
            <div class="info-content">
              <div class="info-row">
                <span class="info-label">Razão Social:</span>
                <span class="info-value">{{ unidade?.razaoSocial || unidade?.nome }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">CNPJ:</span>
                <span class="info-value">{{ unidade?.cnpj || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Inscrição Estadual:</span>
                <span class="info-value">{{ unidade?.inscricaoEstadual || 'Não informada' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Segmento:</span>
                <span class="info-value">{{ unidade?.segmento || 'Não informado' }}</span>
              </div>
            </div>
          </div>

          <div class="info-card">
            <h3><i class="fas fa-map-marked-alt"></i> Endereço</h3>
            <div class="info-content">
              <div class="info-row">
                <span class="info-label">Endereço:</span>
                <span class="info-value">{{ unidade?.endereco || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Complemento:</span>
                <span class="info-value">{{ unidade?.complemento || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Bairro:</span>
                <span class="info-value">{{ unidade?.bairro || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Cidade/UF:</span>
                <span class="info-value">{{ unidade?.cidade || '' }} - {{ unidade?.estado || '' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">CEP:</span>
                <span class="info-value">{{ unidade?.cep || 'Não informado' }}</span>
              </div>
            </div>
          </div>

          <div class="info-card">
            <h3><i class="fas fa-user-tie"></i> Contato</h3>
            <div class="info-content">
              <div class="info-row">
                <span class="info-label">Responsável:</span>
                <span class="info-value">{{ unidade?.responsavel || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Telefone:</span>
                <span class="info-value">{{ unidade?.telefone || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Email:</span>
                <span class="info-value">{{ unidade?.email || 'Não informado' }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Celular:</span>
                <span class="info-value">{{ unidade?.celular || 'Não informado' }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Documentos -->
      <div v-if="activeTab === 'docs'" class="docs-tab">
        <div class="docs-header">
          <h3>Documentos da Unidade</h3>
          <button class="btn btn-primary">
            <i class="fas fa-plus"></i>
            Adicionar Documento
          </button>
        </div>
        <div class="docs-list">
          <div class="doc-item">
            <i class="fas fa-file-contract doc-icon"></i>
            <div class="doc-info">
              <h4>Contrato Social</h4>
              <p>Última atualização: 15/01/2024</p>
            </div>
            <div class="doc-actions">
              <button class="btn-action">
                <i class="fas fa-download"></i>
              </button>
              <button class="btn-action">
                <i class="fas fa-eye"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

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
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import { useUnidades } from '@/composables/unidades';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

const route = useRoute();
const router = useRouter();
const unidadeId = route.params.id;

// Use composables
const {
  store,
  actions,
  loadUnidades,
  getUnidadeById
} = useUnidades();

const { formatCurrency, formatDate, getStatusBadge, getDaysToExpire } = useUnidadesUI();

// Estado do componente
const activeTab = ref('dashboard');
const showDeleteModal = ref(false);
const selectedUnidade = ref(null);
const isLoading = ref(false);

// Dados do dashboard
const dashboardLoading = ref(false);
const dashboardError = ref(null);
const dashboardData = ref(null);

// Tabs disponíveis
const tabs = [
  { id: 'dashboard', label: 'Dashboard', icon: 'fas fa-tachometer-alt' },
  { id: 'info', label: 'Informações', icon: 'fas fa-info-circle' },
  { id: 'docs', label: 'Documentos', icon: 'fas fa-file-contract' },
  { id: 'financeiro', label: 'Financeiro', icon: 'fas fa-chart-line' },
  { id: 'funcionarios', label: 'Funcionários', icon: 'fas fa-users' }
];

// Computed
const unidade = computed(() => {
  if (store.unidadeAtual && store.unidadeAtual.id === unidadeId) {
    return store.unidadeAtual;
  }
  return getUnidadeById(unidadeId);
});

// Métodos
const changeTab = (tabId) => {
  activeTab.value = tabId;
  if (tabId === 'dashboard' && !dashboardData.value) {
    loadDashboardData();
  }
};

const goToEdit = (id) => {
  router.push({ name: 'EditarUnidade', params: { id } });
};

const openDeleteModal = (unidade) => {
  selectedUnidade.value = unidade;
  showDeleteModal.value = true;
};

const closeDeleteModal = () => {
  showDeleteModal.value = false;
  selectedUnidade.value = null;
};

const confirmDelete = async () => {
  if (selectedUnidade.value) {
    try {
      await actions.deleteUnidade(selectedUnidade.value.id);
      closeDeleteModal();
      router.push('/unidades');
    } catch (error) {
      console.error('Erro ao excluir unidade:', error);
    }
  }
};

const loadDashboardData = async () => {
  if (!unidadeId) return;

  dashboardLoading.value = true;
  dashboardError.value = null;

  try {
    // Mock de dados do dashboard - substitua por chamada real à API
    dashboardData.value = await mockDashboardData();
  } catch (error) {
    console.error('Erro ao carregar dados do dashboard:', error);
    dashboardError.value = error.message || 'Erro ao carregar dados do dashboard';
  } finally {
    dashboardLoading.value = false;
  }
};

const refreshDashboardData = () => {
  loadDashboardData();
};

const getProgressColor = (progress) => {
  if (progress >= 75) return '#10b981';
  if (progress >= 50) return '#f59e0b';
  if (progress >= 25) return '#f97316';
  return '#ef4444';
};

const getStatusClass = (status) => {
  switch (status?.toLowerCase()) {
    case 'ativa':
    case 'active':
      return 'active';
    case 'inativa':
    case 'inactive':
      return 'inactive';
    case 'pendente':
    case 'pending':
      return 'pending';
    default:
      return 'unknown';
  }
};

// Mock de dados do dashboard (REMOVA QUANDO IMPLEMENTAR API REAL)
const mockDashboardData = async () => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve({
        metaMensal: 50000,
        faturamentoAtual: 32500,
        progressoMeta: 65,
        metaRestante: 17500,
        mediaDiariaNecessaria: 1166.67,
        projecaoFaturamento: 48750,
        faturamentoTotal: 32500,
        despesasTotais: 15000,
        lucroLiquido: 17500,
        margem: 35,
        funcionariosAtivos: 12,
        ticketMedio: 125.50,
        clientesAtendidos: 259,
        melhorDia: '2024-01-15',
        piorDia: '2024-01-08'
      });
    }, 500);
  });
};

// Função para buscar a unidade específica
const fetchUnidade = async () => {
  if (!unidadeId) return;
  
  isLoading.value = true;
  try {
    await actions.getUnidadeById(unidadeId);
  } catch (error) {
    console.error('Erro ao carregar unidade:', error);
  } finally {
    isLoading.value = false;
  }
};

// Watchers
watch(
  () => route.params.id,
  (newId) => {
    if (newId) {
      fetchUnidade();
      if (activeTab.value === 'dashboard') {
        loadDashboardData();
      }
    }
  }
);

// Lifecycle
onMounted(async () => {
  await fetchUnidade();
  
  // Carrega dados iniciais se necessário
  if (store.unidades.length === 0) {
    await loadUnidades();
  }
  
  // Carrega dashboard se estiver na tab correta
  if (activeTab.value === 'dashboard') {
    await loadDashboardData();
  }
});
</script>

<style scoped>
@import './CSS/UnidadeDetalhes.css';
</style>