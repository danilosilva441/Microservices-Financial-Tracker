<!-- frontend/src/views/UnidadeDetalhesView.vue -->
<script setup>
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { useUnidadesStore } from '@/stores/unidades';

// Componentes
import DetalhesHeader from '@/components/unidades/detalhes/DetalhesHeader.vue';
import DetalhesTabs from '@/components/unidades/detalhes/DetalhesTabs.vue';
import TabVisaoGeral from '@/components/unidades/detalhes/tabs/TabVisaoGeral.vue';
import LoadingState from '@/components/common/LoadingState.vue';

// Importação dinâmica para evitar circular
const TabCaixaDiario = defineAsyncComponent(() => 
  import('@/components/unidades/detalhes/tabs/TabCaixaDiario.vue')
);

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const unidadeStore = useUnidadesStore();
// Estados
const isLoading = ref(true);
const unidade = ref(null);
const activeTab = ref('visao-geral');
const error = ref(null);

// Computed
const isAdmin = computed(() => authStore.isAdmin);

// Carregar unidade
const carregarUnidade = async () => {
  try {
    isLoading.value = true;
    const unidadeId = route.params.id;
    
    // Carrega a unidade
    await unidadeStore.fetchUnidade(unidadeId);
    unidade.value = unidadeStore.unidadeAtual;
    
    if (!unidade.value) {
      throw new Error('Unidade não encontrada');
    }
    
  } catch (err) {
    console.error('Erro ao carregar unidade:', err);
    error.value = err.message;
  } finally {
    isLoading.value = false;
  }
};

// Handlers
const handleDeleteUnidade = async () => {
  if (!confirm('Tem certeza que deseja excluir esta unidade?')) return;
  
  try {
    await unidadeStore.deleteUnidade(unidade.value.id);
    router.push({ name: 'unidades' });
  } catch (err) {
    alert('Erro ao excluir unidade: ' + err.message);
  }
};

// Inicialização
onMounted(() => {
  carregarUnidade();
});
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors">
    
    <div v-if="isLoading" class="mt-8">
      <LoadingState text="Carregando detalhes da unidade..." />
    </div>

    <div v-else-if="unidade && !error" class="max-w-7xl mx-auto">
      
      <DetalhesHeader 
        :unidade="unidade" 
        :is-admin="isAdmin"
        @delete="handleDeleteUnidade"
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

        <Suspense v-if="activeTab === 'faturamentos'">
          <template #default>
            <TabCaixaDiario 
              :unidade="unidade"
            />
          </template>
          <template #fallback>
            <div class="bg-white dark:bg-slate-800 rounded-xl p-8 text-center">
              <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
              <p class="mt-2 text-slate-500">Carregando caixa...</p>
            </div>
          </template>
        </Suspense>

        <div v-if="activeTab === 'configuracoes'" class="bg-white dark:bg-slate-800 rounded-xl p-8 text-center text-slate-500">
          Configurações da unidade em breve...
        </div>
      </div>

    </div>

    <div v-else class="text-center py-16">
      <div class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4 max-w-md mx-auto">
        <h3 class="text-xl font-bold text-red-700 dark:text-red-300">Unidade não encontrada</h3>
        <p class="text-red-600 dark:text-red-400 mt-2">{{ error || 'A unidade solicitada não existe.' }}</p>
        <button 
          @click="router.push({ name: 'unidades' })" 
          class="mt-4 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors"
        >
          Voltar para lista
        </button>
      </div>
    </div>
  </div>
</template>