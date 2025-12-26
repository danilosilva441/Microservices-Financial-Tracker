<script setup>
import { useUnidadesList } from '@/composables/unidades/useUnidadesList';

// Componentes
import UnidadesHeader from '@/components/unidades/lista/UnidadesHeader.vue';
import UnidadesTable from '@/components/unidades/lista/UnidadesTable.vue';
// ✅ Novo Componente Mobile
import UnidadesMobileList from '@/components/unidades/lista/UnidadesMobileList.vue'; 
import UnidadesEmptyState from '@/components/unidades/lista/UnidadesEmptyState.vue';
import OperacaoModal from '@/components/OperacaoModal.vue';
import LoadingState from '@/components/common/LoadingState.vue';
import ErrorState from '@/components/common/ErrorState.vue';

const { 
  unidades, isAdmin, isLoading, error, isModalVisible, 
  handleCreate, goToDetalhes, refresh 
} = useUnidadesList();
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors">
    
    <UnidadesHeader 
      :count="unidades.length" 
      :is-admin="isAdmin"
      @new="isModalVisible = true"
    />

    <div v-if="isLoading" class="mt-8">
      <LoadingState text="Carregando unidades..." />
    </div>

    <div v-else-if="error" class="mt-8">
      <ErrorState :message="error" @retry="refresh" />
    </div>

    <div v-else class="mt-6">
      <UnidadesTable 
        v-if="unidades.length > 0" 
        :unidades="unidades"
        @select="goToDetalhes"
      />

      <UnidadesMobileList 
        v-if="unidades.length > 0"
        :unidades="unidades"
        @select="goToDetalhes"
      />
      
      <UnidadesEmptyState 
        v-if="unidades.length === 0"
        :is-admin="isAdmin"
        @create="isModalVisible = true"
      />
    </div>

    <OperacaoModal 
      :is-visible="isModalVisible" 
      @close="isModalVisible = false" 
      @save="handleCreate"
      :is-loading="isLoading"
    />
  </div>
</template>