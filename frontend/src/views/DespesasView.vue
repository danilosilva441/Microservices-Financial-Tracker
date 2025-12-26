<script setup>
import { useDespesas } from '@/composables/despesas/useDespesasManager';
import { useRouter } from 'vue-router';

// Componentes
import DespesasHeader from '@/components/despesas/DespesasHeader.vue';
import DespesasTable from '@/components/despesas/DespesasTable.vue';
import DespesaModal from '@/components/despesas/DespesaModal.vue';
import LoadingState from '@/components/common/LoadingState.vue';
import ErrorState from '@/components/common/ErrorState.vue';

const router = useRouter();

// 1. ✅ Adicionamos 'handleCreateCategory' na desestruturação
const { 
  despesas, categorias, totalDespesas, isLoading, error, isModalVisible, 
  handleCreate, handleDelete, refresh, handleCreateCategory 
} = useDespesas();

// Navegação de volta para detalhes da unidade
const goBack = () => router.back();
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors">
    
    <DespesasHeader 
      :total="totalDespesas" 
      @new="isModalVisible = true"
      @back="goBack"
    />

    <div v-if="isLoading" class="mt-12">
      <LoadingState text="Carregando despesas..." />
    </div>

    <div v-else-if="error" class="mt-12">
      <ErrorState :message="error" @retry="refresh" />
    </div>

    <div v-else class="mt-6 animate-fade-in">
      <DespesasTable 
        :despesas="despesas" 
        @delete="handleDelete"
        @view-receipt="(path) => console.log('Abrir comprovante:', path)"
      />
    </div>

    <DespesaModal 
      :is-visible="isModalVisible"
      :is-loading="isLoading"
      :categorias="categorias"
      @close="isModalVisible = false"
      @save="handleCreate"
      @create-category="handleCreateCategory"
    />

  </div>
</template>