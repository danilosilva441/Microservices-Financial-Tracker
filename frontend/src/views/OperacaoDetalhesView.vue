<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import { useOperacoesStore } from '@/store/operacoes';
import { formatCurrency } from '@/utils/formatters.js';

// Importa os componentes filhos que esta página utiliza
import FaturamentoForm from '@/components/FaturamentoForm.vue';
import SolicitacaoAjusteModal from '@/components/SolicitacaoAjusteModal.vue';

// --- SETUP BÁSICO ---
const route = useRoute(); // Para pegar o ID da operação da URL
const operacoesStore = useOperacoesStore(); // Nossa store Pinia para gerenciar dados
// Propriedade 'computada' que nos dá acesso seguro à operação atual carregada na store
const operacaoAtual = computed(() => operacoesStore.operacaoAtual);

// --- CONTROLE DO MODAL DE SOLICITAÇÃO ---
const isSolicitacaoModalVisible = ref(false); // Controla se o modal está visível ou não
const faturamentoSelecionado = ref(null);   // Guarda o faturamento que o usuário clicou

// Função chamada pelo botão "Solicitar Ajuste"
function abrirModalSolicitacao(faturamento) {
  faturamentoSelecionado.value = faturamento; // Guarda o faturamento clicado
  isSolicitacaoModalVisible.value = true;   // Abre o modal
}

// Função chamada pelo modal quando ele precisa ser fechado
function fecharModalSolicitacao() {
  isSolicitacaoModalVisible.value = false;
  faturamentoSelecionado.value = null;    // Limpa a seleção
}

// Função chamada pelo modal quando uma solicitação é enviada com sucesso
function onSolicitacaoSucesso(mensagem) {
  alert(mensagem); // Mostra o alerta de sucesso
  // Lógica para dar feedback visual instantâneo ao usuário
  if (faturamentoSelecionado.value) {
    const faturamentoNaLista = operacaoAtual.value.faturamentos.$values.find(
      f => f.id === faturamentoSelecionado.value.id
    );
    // Adiciona um status temporário no frontend para a UI reagir
    if (faturamentoNaLista) {
      faturamentoNaLista.statusAjuste = 'Pendente';
    }
  }
}

// --- LÓGICA DE DADOS ---
// Quando o componente é montado na tela, busca os detalhes da operação
onMounted(() => {
  operacoesStore.fetchOperacaoById(route.params.id);
});

// Função chamada pelo FaturamentoForm quando um novo faturamento é submetido
async function handleSaveFaturamento(faturamentoData) {
  const operacaoId = route.params.id;
  const dadosCompletos = {
    ...faturamentoData,
    moeda: operacaoAtual.value.moeda,
    origem: 'AVULSO'
  };
  // Chama a action da store para salvar o novo faturamento
  await operacoesStore.addFaturamento(operacaoId, dadosCompletos);
}
</script>

<template>
  <div class="p-10">
    <h1 class="text-3xl font-bold text-neutral-dark">Detalhes da Operação</h1>

    <div v-if="operacoesStore.isLoading" class="text-center mt-10">
      Carregando...
    </div>

    <div v-else-if="operacaoAtual" class="mt-8 grid grid-cols-1 lg:grid-cols-3 gap-8">

      <div class="col-span-1 bg-white p-6 rounded-lg shadow-card h-fit">
        <h2 class="text-xl font-bold mb-4">{{ operacaoAtual.nome }}</h2>
        <p class="text-gray-600">{{ operacaoAtual.descricao || 'Sem descrição.' }}</p>
        <hr class="my-4">
        <p><strong>Meta Mensal:</strong> {{ formatCurrency(operacaoAtual.metaMensal, operacaoAtual.moeda) }}</p>
      </div>

      <div class="col-span-1 lg:col-span-2 bg-white p-6 rounded-lg shadow-card">
        <h2 class="text-xl font-bold mb-4">Faturamentos Registrados</h2>

        <table v-if="operacaoAtual.faturamentos?.$values?.length > 0" class="w-full text-left mb-6">
          <thead class="border-b">
            <tr>
              <th class="p-2">Data</th>
              <th class="p-2">Valor</th>
              <th class="p-2">Origem</th>
              <th class="p-2">Status</th>
              <th class="p-2">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="faturamento in operacaoAtual.faturamentos.$values" :key="faturamento.id"
              :class="{ 'text-gray-400': !faturamento.isAtivo }" class="border-b">

              <td class="p-2">{{ new Date(faturamento.data).toLocaleDateString() }}</td>
              <td class="p-2">{{ formatCurrency(faturamento.valor, faturamento.moeda) }}</td>
              <td class="p-2">{{ faturamento.origem }}</td>
              <td class="p-2">
                <span v-if="faturamento.statusAjuste === 'Pendente'" class="text-yellow-600 font-semibold">
                  Pendente
                </span>
                <span v-else :class="faturamento.isAtivo ? 'text-green-600' : 'text-red-600'">
                  {{ faturamento.isAtivo ? 'Ativo' : 'Inativo' }}
                </span>
              </td>
              <td class="p-2">
                <button @click="abrirModalSolicitacao(faturamento)" class="text-blue-500 hover:text-blue-700 text-sm">
                  Solicitar Ajuste
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-else class="text-gray-500 mb-6">
          Nenhum faturamento registrado para esta operação.
        </div>

        <FaturamentoForm @submit="handleSaveFaturamento" :error-message="operacoesStore.error" />
      </div>
    </div>
  </div>

  <SolicitacaoAjusteModal :is-open="isSolicitacaoModalVisible" :faturamento="faturamentoSelecionado"
    @close="fecharModalSolicitacao" @success="onSolicitacaoSucesso" />
</template>