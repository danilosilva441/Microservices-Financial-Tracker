<script setup>
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useOperacoesStore } from '@/stores/operacoes';
import { formatCurrency } from '@/utils/formatters.js';

const route = useRoute();
const operacoesStore = useOperacoesStore();

// Pega o ID da operação a partir da URL
const operacaoId = route.params.id;

onMounted(() => {
    // Ação que vamos criar para buscar uma ÚNICA operação
    operacoesStore.fetchOperacaoById(operacaoId); 
});
</script>

<template>
  <div class="p-10">
    <h1 class="text-3xl font-bold text-neutral-dark">Detalhes da Operação</h1>
    
    <div v-if="operacoesStore.isLoading">Carregando...</div>
    
    <div v-else-if="operacoesStore.operacaoAtual" class="mt-8 grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div class="col-span-1 bg-white p-6 rounded-lg shadow-card h-fit">
            <h2 class="text-xl font-bold mb-4">{{ operacoesStore.operacaoAtual.nome }}</h2>
            <p class="text-gray-600">{{ operacoesStore.operacaoAtual.descricao }}</p>
            <hr class="my-4">
            <p><strong>Endereço:</strong> {{ operacoesStore.operacaoAtual.endereco || 'N/A' }}</p>
            <p><strong>Meta Mensal:</strong> {{ formatCurrency(operacoesStore.operacaoAtual.metaMensal, operacoesStore.operacaoAtual.moeda) }}</p>
            <p><strong>Projeção:</strong> {{ formatCurrency(operacoesStore.operacaoAtual.projecaoFaturamento || 0, operacoesStore.operacaoAtual.moeda) }}</p>
        </div>
        
        <div class="col-span-1 lg:col-span-2 bg-white p-6 rounded-lg shadow-card">
            <h2 class="text-xl font-bold mb-4">Faturamentos Registrados</h2>

            <table v-if="operacoesStore.operacaoAtual.faturamentos.$values.length > 0" class="w-full text-left">
              <thead>
                <tr class="border-b">
                  <th class="py-2">Data</th>
                  <th class="py-2">Valor</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="fat in operacoesStore.operacaoAtual.faturamentos.$values" :key="fat.$id" class="border-b">
                  <td class="py-4">{{ new Date(fat.data).toLocaleDateString('pt-BR') }}</td>
                  <td>{{ formatCurrency(fat.valor, fat.moeda) }}</td>
                </tr>
              </tbody>
            </table>
            
            <div v-else class="text-gray-500">
              Nenhum faturamento registrado para esta operação.
            </div>

        </div>
    </div>
  </div>
</template>