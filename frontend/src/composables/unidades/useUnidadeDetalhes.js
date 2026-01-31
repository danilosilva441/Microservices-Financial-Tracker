import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useUnidadesStore } from '@/stores/unidades';
import { useAuthStore } from '@/stores/authStore';

export function useUnidadeDetalhes() {
  const route = useRoute();
  const router = useRouter();
  const store = useUnidadesStore();
  const authStore = useAuthStore();

  const activeTab = ref('visao-geral');
  const exportLoading = ref(false);

  // Actions
  const loadData = () => store.fetchUnidadeById(route.params.id);

  const deleteUnidade = async () => {
    if (confirm('Tem certeza? Isso apagará todos os dados desta unidade.')) {
      const success = await store.deleteUnidade(store.unidadeAtual.id);
      if (success) router.push({ name: 'unidades' });
    }
  };

  const addFaturamento = async (data) => {
    // ✅ Adiciona a data de hoje automaticamente
    const hoje = new Date().toISOString().split('T')[0];
    const dadosCompletos = {
      ...data,
      data: hoje,
      unidadeId: route.params.id
    };
    
    const result = await store.addFaturamento(route.params.id, dadosCompletos);
    
    if (!result.success) {
      alert(result.error || 'Erro ao adicionar faturamento');
    }
    
    return result;
  };

  const deleteFaturamento = async (id) => {
    if (confirm('Apagar este faturamento?')) {
      const result = await store.deleteFaturamento(route.params.id, id);
      
      if (!result.success) {
        alert(result.error || 'Erro ao remover faturamento');
      }
      
      return result;
    }
  };

  // Lifecycle
  onMounted(loadData);
  
  watch(() => route.params.id, (newId, oldId) => {
    if (newId && newId !== oldId) {
      loadData();
    }
  });

  return {
    unidade: computed(() => store.unidadeAtual),
    isLoading: computed(() => store.isLoading),
    isAdmin: computed(() => authStore.isAdmin),
    activeTab,
    exportLoading,
    deleteUnidade,
    addFaturamento,
    deleteFaturamento
  };
}