import { ref, computed, onMounted } from 'vue';
import { useFaturamentoStore } from '@/stores/faturamento';
import { useUnidadesStore } from '@/stores/unidades';

export function useFechamentoCaixa(unidadeId) {
  const faturamentoStore = useFaturamentoStore();
  const unidadesStore = useUnidadesStore();

  const isModalOpen = ref(false);
  
  // Computed
  const fechamentos = computed(() => faturamentoStore.fechamentos || []);
  const isLoading = computed(() => faturamentoStore.isLoading);
  
  // Dados do dia atual
  const faturamentosHoje = computed(() => {
    const hoje = new Date().toISOString().split('T')[0]; // YYYY-MM-DD
    const lista = unidadesStore.unidadeAtual?.faturamentos?.$values || [];
    
    return lista.filter(f => {
      // ✅ CORREÇÃO: Tenta ler 'data' (fechamentos) OU 'horaInicio' (parciais novos)
      const dataItem = f.data || f.horaInicio;
      
      // Se não tiver data nenhuma (undefined/null), ignora para não quebrar
      if (!dataItem) return false;

      return dataItem.startsWith(hoje);
    });
  });

  // Cálculos em tempo real
  const resumoHoje = computed(() => {
    return faturamentosHoje.value.reduce((acc, item) => {
      // Garante que é número
      const valor = Number(item.valor) || 0;
      acc.total += valor;
      return acc;
    }, { total: 0 });
  });

  const fetchHistorico = async () => {
    if (unidadeId) {
      await faturamentoStore.fetchFechamentos(unidadeId);
    }
  };

  const realizarFechamento = async (dados) => {
    const payload = {
      ...dados,
      data: new Date().toISOString().split('T')[0]
    };

    const result = await faturamentoStore.criarFechamento(unidadeId, payload);
    
    if (result.success) {
      isModalOpen.value = false;
      await fetchHistorico();
    } else {
      alert(result.error);
    }
  };

  onMounted(() => {
    fetchHistorico();
  });

  return {
    fechamentos,
    faturamentosHoje,
    resumoHoje,
    isLoading,
    isModalOpen,
    fetchHistorico,
    realizarFechamento
  };
}