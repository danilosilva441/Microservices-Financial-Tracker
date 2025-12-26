<script setup>
import { useUnidadeDetalhes } from '@/composables/unidades/useUnidadeDetalhes';

// Componentes
import DetalhesHeader from '@/components/unidades/detalhes/DetalhesHeader.vue';
import DetalhesTabs from '@/components/unidades/detalhes/DetalhesTabs.vue';
import TabVisaoGeral from '@/components/unidades/detalhes/tabs/TabVisaoGeral.vue';
import TabCaixaDiario from '@/components/unidades/detalhes/tabs/TabCaixaDiario.vue';
import LoadingState from '@/components/common/LoadingState.vue';
//import TabMensalistas from '@/components/unidades/detalhes/tabs/TabMensalistas.vue';

const { 
  unidade, isLoading, activeTab, isAdmin,
  addFaturamento, deleteFaturamento, deleteUnidade 
} = useUnidadeDetalhes();
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors">
    
    <div v-if="isLoading" class="mt-8">
      <LoadingState text="Carregando detalhes da unidade..." />
    </div>

    <div v-else-if="unidade" class="max-w-7xl mx-auto">
      
      <DetalhesHeader 
        :unidade="unidade" 
        :is-admin="isAdmin"
        @delete="deleteUnidade"
      />

      <DetalhesTabs 
        :active-tab="activeTab" 
        @change="tab => activeTab = tab" 
      />

      <div class="mt-6">
        <TabVisaoGeral 
          v-if="activeTab === 'visao-geral'" 
          :unidade="unidade" 
        />

      <TabCaixaDiario 
        v-if="activeTab === 'faturamentos'" 
        :unidade="unidade"
        @add="addFaturamento"
        @delete="deleteFaturamento"
/>

        <!-- <TabMensalistas v-if="activeTab === 'mensalistas'" /> -->
        
        <div v-if="activeTab === 'configuracoes'" class="p-8 text-center text-slate-500">
          Configurações da unidade em breve...
        </div>
      </div>

    </div>

    <div v-else class="text-center py-16">
      <h3 class="text-xl font-bold text-slate-700 dark:text-white">Unidade não encontrada</h3>
      <button @click="$router.push({ name: 'unidades' })" class="text-blue-600 mt-2 hover:underline">
        Voltar para lista
      </button>
    </div>
  </div>
</template>