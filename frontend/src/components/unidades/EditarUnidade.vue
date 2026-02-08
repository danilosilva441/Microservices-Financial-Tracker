<!-- components/unidades/EditarUnidade.vue -->
<template>
  <Div>
    <div class="editar-unidade-container">
      <!-- Breadcrumb -->
      <nav class="breadcrumb-nav" aria-label="breadcrumb">
        <ol class="breadcrumb">
          <li class="breadcrumb-item">
            <router-link to="/">
              <i class="fas fa-home"></i>
              Home
            </router-link>
          </li>
          <li class="breadcrumb-item">
            <router-link to="/unidades">
              <i class="fas fa-store"></i>
              Unidades
            </router-link>
          </li>
          <li v-if="unidade" class="breadcrumb-item">
            <router-link :to="`/unidades/${unidade.id}`">
              {{ unidade.nome }}
            </router-link>
          </li>
          <li class="breadcrumb-item active" aria-current="page">
            <i class="fas fa-edit"></i>
            Editar
          </li>
        </ol>
      </nav>

      <!-- Header -->
      <div class="page-header">
        <div class="header-content">
          <div class="header-left">
            <h1 class="page-title">
              <i class="fas fa-edit"></i>
              Editar Unidade
            </h1>
            <p class="page-subtitle" v-if="unidade">
              Editando: <strong>{{ unidade.nome }}</strong>
            </p>
          </div>
          <div class="header-right">
            <router-link 
              :to="`/unidades/${unidadeId}`" 
              class="btn btn-outline-secondary"
            >
              <i class="fas fa-times"></i>
              Cancelar
            </router-link>
          </div>
        </div>
      </div>

      <!-- Conteúdo Principal -->
      <div class="page-content">
        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <div class="spinner-container">
            <div class="spinner-border text-primary"></div>
            <p>Carregando dados da unidade...</p>
          </div>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="error-state">
          <div class="error-icon">
            <i class="fas fa-exclamation-triangle"></i>
          </div>
          <h3>Erro ao carregar unidade</h3>
          <p>{{ error }}</p>
          <div class="error-actions">
            <button @click="fetchUnidade" class="btn btn-primary">
              <i class="fas fa-redo"></i>
              Tentar novamente
            </button>
            <router-link to="/unidades" class="btn btn-outline-secondary">
              <i class="fas fa-arrow-left"></i>
              Voltar para unidades
            </router-link>
          </div>
        </div>

        <!-- Formulário -->
        <div v-else-if="unidade" class="form-container">
          <UnidadeForm
            ref="unidadeForm"
            :initial-data="unidade"
            mode="edit"
            @success="handleSuccess"
            @cancel="handleCancel"
            @error="handleError"
          />
        </div>

        <!-- Not Found -->
        <div v-else class="not-found-state">
          <div class="not-found-icon">
            <i class="fas fa-store-slash"></i>
          </div>
          <h3>Unidade não encontrada</h3>
          <p>A unidade que você está tentando editar não existe ou foi removida.</p>
          <router-link to="/unidades" class="btn btn-primary">
            <i class="fas fa-arrow-left"></i>
            Voltar para unidades
          </router-link>
        </div>
      </div>

      <!-- Sidebar de Ajuda (Opcional) -->
      <div class="help-sidebar">
        <div class="help-card">
          <h4><i class="fas fa-question-circle"></i> Ajuda</h4>
          <ul class="help-list">
            <li>
              <i class="fas fa-info-circle"></i>
              <strong>Nome:</strong> Use um nome claro e descritivo
            </li>
            <li>
              <i class="fas fa-map-marker-alt"></i>
              <strong>Endereço:</strong> Inclua cidade, estado e CEP
            </li>
            <li>
              <i class="fas fa-chart-line"></i>
              <strong>Meta:</strong> Baseie-se em dados históricos
            </li>
            <li>
              <i class="fas fa-calendar"></i>
              <strong>Datas:</strong> Mantenha sempre atualizadas
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import UnidadeForm from '@/components/unidades/UnidadeForm.vue';
import { useUnidades } from '@/composables/unidades';

const route = useRoute();
const router = useRouter();
const unidadeId = route.params.id;

const {
  store,
  actions,
  loadUnidades,
  getUnidadeById
} = useUnidades();

// Estado local
const loading = ref(true);
const error = ref(null);

// Computed
const unidade = computed(() => store.unidadeAtual || getUnidadeById(unidadeId));

// Métodos
const fetchUnidade = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    // Primeiro tenta buscar localmente
    if (!unidade.value) {
      // Se não encontrar localmente, busca na API
      await actions.getUnidade(unidadeId);
    }
  } catch (err) {
    error.value = err.message || 'Erro ao carregar unidade';
    console.error('Erro ao carregar unidade:', err);
  } finally {
    loading.value = false;
  }
};

const handleSuccess = (updatedUnidade) => {
  // Redireciona para a página de detalhes
  router.push({ 
    name: 'UnidadeDetalhes', 
    params: { id: unidadeId },
    query: { updated: 'true' }
  });
};

const handleCancel = () => {
  router.push({ name: 'UnidadeDetalhes', params: { id: unidadeId } });
};

const handleError = (errorMessage) => {
  console.error('Erro no formulário:', errorMessage);
  // Você pode adicionar uma notificação aqui
};

// Lifecycle
onMounted(async () => {
  // Carrega unidades se necessário
  if (store.unidades.length === 0) {
    await loadUnidades();
  }
  
  // Busca a unidade específica
  await fetchUnidade();
});
</script>

<style scoped>
@import './CSS/EditarUnidade.css';
</style>