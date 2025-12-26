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
    // ✅ CORREÇÃO: Removemos o override da origem. 
    // Passamos o 'data' puro, pois o formulário já manda tudo correto.
    await store.addFaturamento(route.params.id, data);
  };

  const deleteFaturamento = async (id) => {
    if (confirm('Apagar este faturamento?')) {
      await store.deleteFaturamento(store.unidadeAtual.id, id);
    }
  };

  // Lifecycle
  onMounted(loadData);
  watch(() => route.params.id, loadData);

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