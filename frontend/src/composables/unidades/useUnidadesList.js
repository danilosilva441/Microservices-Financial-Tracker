import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useUnidadesStore } from '@/stores/unidades'; // Assumindo que a store ainda se chama 'unidades', se mudou, ajuste aqui
import { useAuthStore } from '@/stores/authStore';

export function useUnidadesList() {
  const store = useUnidadesStore();
  const authStore = useAuthStore();
  const router = useRouter();
  
  const isModalVisible = ref(false);

  // Computed
  // Assumindo que a store retorna a propriedade 'unidades' ou 'unidades'
  const unidades = computed(() => store.unidades?.$values || []); 
  const isAdmin = computed(() => authStore.isAdmin);
  const isLoading = computed(() => store.isLoading);
  const error = computed(() => store.error);

  // Actions
  const fetchUnidades = () => store.fetchUnidades();
  
  const handleCreate = async (data) => {
    const result = await store.createUnidade(data);
    if (result.success) {
      isModalVisible.value = false;
    } else {
      alert(`Erro: ${result.error}`);
    }
  };

  const goToDetalhes = (id) => {
    // Rota atualizada para 'unidade-detalhes'
    router.push({ name: 'unidade-detalhes', params: { id } });
  };

  // Lifecycle
  onMounted(() => {
    fetchUnidades();
  });

  return {
    unidades,
    isAdmin,
    isLoading,
    error,
    isModalVisible,
    handleCreate,
    goToDetalhes,
    refresh: fetchUnidades
  };
}