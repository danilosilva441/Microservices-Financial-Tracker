import { ref, onMounted, computed, watch } from 'vue';
import { useRoute } from 'vue-router';
import { useMensalistasStore } from '@/stores/mensalistas';

export function useMensalistas() {
  const route = useRoute();
  const store = useMensalistasStore();
  
  const unidadeId = ref(route.params.id);
  const isModalVisible = ref(false);
  const itemParaEditar = ref(null); // Para controlar edição

  const mensalistas = computed(() => store.mensalistas || []);
  const isLoading = computed(() => store.isLoading);
  const error = computed(() => store.error);

  const loadData = () => {
    if (unidadeId.value) {
      store.fetchMensalistas(unidadeId.value);
    }
  };

  const handleOpenCreate = () => {
    itemParaEditar.value = null; // Modo criação
    isModalVisible.value = true;
  };

  const handleOpenEdit = (mensalista) => {
    itemParaEditar.value = { ...mensalista }; // Cópia para edição
    isModalVisible.value = true;
  };

  const handleSave = async (dados) => {
    let result;
    if (itemParaEditar.value) {
      // Edição
      result = await store.updateMensalista(unidadeId.value, itemParaEditar.value.id, dados);
    } else {
      // Criação
      result = await store.createMensalista(unidadeId.value, dados);
    }

    if (result.success) {
      isModalVisible.value = false;
    } else {
      alert(result.error);
    }
  };

  const handleToggleStatus = async (mensalista) => {
    if (confirm(`Deseja ${mensalista.isAtivo ? 'desativar' : 'ativar'} este mensalista?`)) {
      await store.desativarMensalista(unidadeId.value, mensalista.id);
    }
  };

  onMounted(loadData);
  
  // Atualiza se mudar de rota
  watch(() => route.params.id, (newId) => {
    unidadeId.value = newId;
    loadData();
  });

  return {
    mensalistas,
    isLoading,
    error,
    isModalVisible,
    itemParaEditar,
    handleOpenCreate,
    handleOpenEdit,
    handleSave,
    handleToggleStatus,
    refresh: loadData
  };
}