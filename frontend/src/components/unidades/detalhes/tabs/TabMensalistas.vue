<script setup>
import { useMensalistas } from '@/composables/mensalistas/useMensalistas';
import MensalistasTable from '@/components/mensalistas/MensalistasTable.vue';
import MensalistaModal from '@/components/mensalistas/MensalistaModal.vue';
import LoadingState from '@/components/common/LoadingState.vue';

const { 
  mensalistas, isLoading, isModalVisible, itemParaEditar, 
  handleOpenCreate, handleOpenEdit, handleSave, handleToggleStatus 
} = useMensalistas();
</script>

<template>
  <div class="space-y-6 animate-fade-in">
    <div class="flex justify-between items-center">
      <h3 class="text-lg font-bold text-slate-800 dark:text-white">Carteira de Mensalistas</h3>
      <button 
        @click="handleOpenCreate"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors shadow-sm flex items-center gap-2"
      >
        <span>+</span> Novo Mensalista
      </button>
    </div>

    <div v-if="isLoading && mensalistas.length === 0" class="py-8">
      <LoadingState text="Carregando mensalistas..." />
    </div>

    <MensalistasTable 
      v-else
      :mensalistas="mensalistas"
      @edit="handleOpenEdit"
      @toggle-status="handleToggleStatus"
    />

    <MensalistaModal 
      :is-visible="isModalVisible"
      :is-loading="isLoading"
      :mensalista-para-editar="itemParaEditar"
      @close="isModalVisible = false"
      @save="handleSave"
    />
  </div>
</template>