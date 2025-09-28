<script setup>
import { onMounted } from 'vue';
import { useSolicitacaoStore } from '@/stores/solicitacaoStore';
import { formatCurrency } from '@/utils/formatters';

const solicitacaoStore = useSolicitacaoStore();

onMounted(() => {
  solicitacaoStore.fetchSolicitacoes();
});

async function handleRevisao(id, acao) {
  const confirmMessage = acao === 'APROVADA' 
    ? 'Tem certeza que deseja APROVAR esta solicitação?' 
    : 'Tem certeza que deseja REJEITAR esta solicitação?';

  if (window.confirm(confirmMessage)) {
    await solicitacaoStore.revisarSolicitacao(id, acao);
  }
}
</script>
<template>
  <div class="p-10">
    <h1 class="text-3xl font-bold text-neutral-dark mb-8">Gerenciar Solicitações de Ajuste</h1>

    <div v-if="solicitacaoStore.isLoading">Carregando solicitações...</div>
    <div v-else-if="solicitacaoStore.error" class="text-red-500">{{ solicitacaoStore.error }}</div>

    <div v-else-if="solicitacaoStore.solicitacoesPendentes.length > 0" class="space-y-6">
      <div v-for="s in solicitacaoStore.solicitacoesPendentes" :key="s.id" class="bg-white p-6 rounded-lg shadow-card">
        <div class="flex justify-between items-start">
          <div>
            <p class="text-sm text-gray-500">Operação: {{ s.faturamento.operacao.nome }}</p>
            <p class="text-lg font-bold">{{ s.tipo === 'remocao' ? 'Solicitação de Remoção' : 'Solicitação de Alteração' }}</p>
          </div>
          <span class="px-3 py-1 text-sm font-semibold text-yellow-800 bg-yellow-100 rounded-full">Pendente</span>
        </div>

        <hr class="my-4">

        <p class="text-sm text-gray-600 mb-2"><strong>Solicitado por:</strong> {{ s.solicitante.email }}</p>
        <p class="text-sm text-gray-600"><strong>Motivo:</strong> {{ s.motivo }}</p>

        <div class="mt-4 p-4 bg-gray-50 rounded-md grid grid-cols-2 gap-4">
          <div>
            <p class="text-xs text-gray-500">Dados Originais</p>
            <p><strong>Data:</strong> {{ new Date(JSON.parse(s.dadosAntigos).data).toLocaleDateString() }}</p>
            <p><strong>Valor:</strong> {{ formatCurrency(JSON.parse(s.dadosAntigos).valor, s.faturamento.moeda) }}</p>
          </div>
          <div v-if="s.tipo === 'alteracao'">
            <p class="text-xs text-blue-500">Dados Propostos</p>
            <p><strong>Data:</strong> {{ new Date(JSON.parse(s.dadosNovos).data).toLocaleDateString() }}</p>
            <p><strong>Valor:</strong> {{ formatCurrency(JSON.parse(s.dadosNovos).valor, s.faturamento.moeda) }}</p>
          </div>
        </div>

        <div class="flex justify-end space-x-3 mt-6">
          <button @click="handleRevisao(s.id, 'REJEITADA')" class="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700">Rejeitar</button>
          <button @click="handleRevisao(s.id, 'APROVADA')" class="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700">Aprovar</button>
        </div>
      </div>
    </div>
    <div v-else class="text-center text-gray-500 mt-10">
      Nenhuma solicitação pendente.
    </div>
  </div>
</template>