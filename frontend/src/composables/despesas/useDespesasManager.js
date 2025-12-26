import { ref, onMounted, computed, watch } from 'vue';
import { useRoute } from 'vue-router';
import { useDespesasStore } from '@/stores/despesas';
import { useUnidadesStore } from '@/stores/unidades'; // Para obter nome da unidade, se necessário

/**
 * Composable para gerenciamento da tela de Despesas.
 * Centraliza lógica de busca, criação, exclusão e upload.
 * * @returns {Object} Estados e métodos para a View.
 */
export function useDespesas() {
  const route = useRoute();
  const despesasStore = useDespesasStore();
  const unidadesStore = useUnidadesStore(); // Opcional, para contexto

  // Estados Locais
  const isModalVisible = ref(false);
  const unidadeId = ref(route.params.id); // Pega o ID da URL

  // Computed Properties (Reativas)
  const despesas = computed(() => despesasStore.despesas?.$values || despesasStore.despesas || []);
  const categorias = computed(() => despesasStore.categorias?.$values || despesasStore.categorias || []);
  const isLoading = computed(() => despesasStore.isLoading);
  const error = computed(() => despesasStore.error);

  /**
   * Calcula o total de despesas listadas na tela.
   */
  const totalDespesas = computed(() => {
    return despesas.value.reduce((acc, d) => acc + d.amount, 0);
  });

  /**
   * Carrega os dados iniciais (Despesas da unidade e Categorias globais).
   */
  const loadData = async () => {
    if (!unidadeId.value) return;
    
    // Busca paralela para performance
    await Promise.all([
      despesasStore.fetchDespesasPorUnidade(unidadeId.value),
      despesasStore.fetchCategorias()
    ]);
  };

  /**
   * Processa a criação de uma nova despesa.
   * Lida com o upload do arquivo (se houver) antes de salvar o registro.
   * * @param {Object} formData - Dados do formulário { amount, description, date, categoryId, file? }
   */
  const handleCreate = async (formData) => {
    try {
      // 1. Upload do comprovante (se existir)
      // Nota: O backend atual espera upload separado ou multipart? 
      // Baseado no seu store, o upload é um endpoint separado que retorna o caminho.
      let comprovantePath = null;
      
      if (formData.file) {
        const uploadResult = await despesasStore.uploadComprovante(formData.file);
        if (uploadResult.success) {
          comprovantePath = uploadResult.path; // Ajustar conforme retorno real do backend
        } else {
          throw new Error('Falha no upload do comprovante');
        }
      }

      // 2. Monta o DTO (Data Transfer Object)
      const payload = {
        amount: parseFloat(formData.amount),
        description: formData.description,
        expenseDate: formData.date, // ISO String
        categoryId: formData.categoryId,
        unidadeId: unidadeId.value,
        receiptPath: comprovantePath // Se o backend suportar salvar o caminho
      };

      // 3. Envia para API
      const result = await despesasStore.createDespesa(payload);
      
      if (result.success) {
        isModalVisible.value = false;
        await despesasStore.fetchDespesasPorUnidade(unidadeId.value); // Recarrega lista
      } else {
        alert(result.error);
      }
    } catch (err) {
      console.error(err);
      alert('Erro ao salvar despesa: ' + err.message);
    }
  };

  /**
   * Exclui uma despesa após confirmação.
   * @param {string} id - UUID da despesa.
   */
  const handleDelete = async (id) => {
    if (confirm('Tem certeza que deseja excluir esta despesa?')) {
      const success = await despesasStore.deleteDespesa(id);
      if (success) {
        await despesasStore.fetchDespesasPorUnidade(unidadeId.value);
      }
    }
  };

  /**
   * Cria uma nova categoria "on-the-fly" dentro do modal.
   */
  const handleCreateCategory = async (name) => {
    await despesasStore.createCategoria(name, 'Criada via web');
  };

  // Lifecycle
  onMounted(loadData);
  
  // Recarrega se mudar de rota (ex: navegar entre unidades)
  watch(() => route.params.id, (newId) => {
    unidadeId.value = newId;
    loadData();
  });

  return {
    despesas,
    categorias,
    totalDespesas,
    isLoading,
    error,
    isModalVisible,
    handleCreate,
    handleDelete,
    handleCreateCategory,
    refresh: loadData
  };
}