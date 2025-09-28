<script setup>
import { onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import { useOperacoesStore } from '@/stores/operacoes'; // Lembre-se de verificar se sua pasta é 'store' ou 'stores'
import { formatCurrency } from '@/utils/formatters.js';
import FaturamentoForm from '@/components/FaturamentoForm.vue';

const route = useRoute();
const operacoesStore = useOperacoesStore();

// Usamos uma propriedade computada para acessar a operação atual de forma segura
const operacaoAtual = computed(() => operacoesStore.operacaoAtual);


// Chamamos a busca de dados apenas uma vez quando o componente é montado
onMounted(() => {
  operacoesStore.fetchOperacaoById(route.params.id);
});


async function handleSaveFaturamento(faturamentoData) {
  const operacaoId = route.params.id;

  // Monta o payload completo aqui, pegando a moeda da operação atual
  // e definindo a origem como 'AVULSO'
  const dadosCompletos = {
    ...faturamentoData,
    moeda: operacaoAtual.value.moeda,
    origem: 'AVULSO'
  };

  await operacoesStore.addFaturamento(operacaoId, dadosCompletos);
}
</script>

<template>
  <div class="p-10">
    <h1 class="text-3xl font-bold text-neutral-dark">Detalhes da Operação</h1>

    <div v-if="operacoesStore.isLoading">Carregando...</div>

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
                <span :class="faturamento.isAtivo ? 'text-green-600' : 'text-red-600'">
                  {{ faturamento.isAtivo ? 'Ativo' : 'Inativo' }}
                </span>
              </td>
              <td class="p-2">
                <button v-if="faturamento.isAtivo"
                  @click="operacoesStore.deactivateFaturamento(operacaoAtual.id, faturamento.id)"
                  class="text-red-500 hover:text-red-700 text-sm">
                  Desativar
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
</template>