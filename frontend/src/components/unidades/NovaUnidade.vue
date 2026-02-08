<!-- components/unidades/NovaUnidade.vue -->
<template>
  <div>
    <div class="nova-unidade-container">
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
          <li class="breadcrumb-item active" aria-current="page">
            <i class="fas fa-plus-circle"></i>
            Nova Unidade
          </li>
        </ol>
      </nav>

      <!-- Header -->
      <div class="page-header">
        <div class="header-content">
          <div class="header-left">
            <h1 class="page-title">
              <i class="fas fa-plus-circle"></i>
              Nova Unidade
            </h1>
            <p class="page-subtitle">
              Cadastre uma nova unidade no sistema
            </p>
          </div>
          <div class="header-right">
            <router-link to="/unidades" class="btn btn-outline-secondary">
              <i class="fas fa-times"></i>
              Cancelar
            </router-link>
          </div>
        </div>
      </div>

      <!-- Conteúdo Principal -->
      <div class="page-content">
        <!-- Formulário -->
        <div class="form-container">
          <UnidadeForm
            ref="unidadeForm"
            mode="create"
            @success="handleSuccess"
            @cancel="handleCancel"
            @error="handleError"
          />
        </div>

        <!-- Sidebar de Ajuda -->
        <div class="help-sidebar">
          <div class="help-card">
            <h4><i class="fas fa-lightbulb"></i> Dicas para Cadastro</h4>
            <div class="tip-section">
              <h5><i class="fas fa-check-circle"></i> Boas Práticas</h5>
              <ul class="tip-list">
                <li>Use nomes curtos e descritivos</li>
                <li>Preencha todos os campos obrigatórios (*)</li>
                <li>Mantenha dados de contato atualizados</li>
                <li>Defina metas realistas</li>
              </ul>
            </div>
            
            <div class="tip-section">
              <h5><i class="fas fa-exclamation-triangle"></i> O que evitar</h5>
              <ul class="tip-list warning">
                <li>Nomes muito longos ou genéricos</li>
                <li>Metas irreais ou não mensuráveis</li>
                <li>Datas inconsistentes</li>
                <li>Endereços incompletos</li>
              </ul>
            </div>
            
            <div class="info-section">
              <h5><i class="fas fa-info-circle"></i> Informações</h5>
              <p>
                Após o cadastro, você poderá:
              </p>
              <ul class="info-list">
                <li>Adicionar funcionários</li>
                <li>Registrar faturamentos</li>
                <li>Acompanhar metas</li>
                <li>Gerar relatórios</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import UnidadeForm from '@/components/unidades/UnidadeForm.vue';

const router = useRouter();
const unidadeForm = ref(null);

// Métodos
const handleSuccess = (novaUnidade) => {
  // Redireciona para a página de detalhes da nova unidade
  router.push({ 
    name: 'UnidadeDetalhes', 
    params: { id: novaUnidade.id },
    query: { created: 'true' }
  });
};

const handleCancel = () => {
  router.push({ name: 'Unidades' });
};

const handleError = (errorMessage) => {
  console.error('Erro ao criar unidade:', errorMessage);
  // Você pode adicionar uma notificação aqui
};

// Método para limpar formulário (opcional)
const clearForm = () => {
  if (unidadeForm.value) {
    unidadeForm.value.reset();
  }
};
</script>

<style scoped>
@import './CSS/NovaUnidade.css';
</style>