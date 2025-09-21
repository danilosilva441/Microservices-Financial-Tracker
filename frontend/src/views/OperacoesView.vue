<script setup>
import { ref, onMounted } from 'vue';
import { useOperacoesStore } from '@/stores/operacoes';
import OperacaoModal from '@/components/OperacaoModal.vue';
import { formatCurrency } from '@/utils/formatters.js';

const operacoesStore = useOperacoesStore();
const isModalVisible = ref(false);

onMounted(() => {
    operacoesStore.fetchOperacoes();
});

async function handleSave(operacaoData) {
    await operacoesStore.createOperacao(operacaoData);
    isModalVisible.value = false; // Fecha o modal após salvar
}
</script>

<template>
    <div class="p-10">
        <div class="flex justify-between items-center mb-8">
            <h1 class="text-3xl font-bold text-neutral-dark">Minhas Operações</h1>
            <button @click="isModalVisible = true"
                class="bg-primary hover:bg-primary-dark text-white font-bold py-2 px-4 rounded-lg transition-colors">
                Adicionar Nova Operação
            </button>
        </div>

        <div class="bg-white p-6 rounded-lg shadow-card">
            <table v-if="operacoesStore.operacoes.length > 0" class="w-full text-left">
                <thead>
                    <tr class="border-b">
                        <th class="py-2">Nome</th>
                        <th>Meta Mensal</th>
                        <th>Projeção</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="op in operacoesStore.operacoes" :key="op.id" class="border-b">
                        <td class="py-4 font-semibold">
                            <RouterLink :to="{ name: 'operacao-detalhes', params: { id: op.id } }"
                                class="text-primary hover:underline">
                                {{ op.nome }}
                            </RouterLink>
                        </td>
                        <td>{{ formatCurrency(op.metaMensal, op.moeda) }}</td>
                        <td>{{ formatCurrency(op.projecaoFaturamento || 0, op.moeda) }}</td>
                        <td>
                            <span :class="op.isAtiva ? 'bg-green-200 text-green-800' : 'bg-red-200 text-red-800'"
                                class="px-2 py-1 text-xs font-semibold rounded-full">
                                {{ op.isAtiva ? 'Ativa' : 'Inativa' }}
                            </span>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div v-else>Nenhuma operação cadastrada.</div>
        </div>

        <OperacaoModal :is-visible="isModalVisible" @close="isModalVisible = false" @save="handleSave" />

    </div>
</template>