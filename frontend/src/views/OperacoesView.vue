<script setup>
import { ref, onMounted, computed } from 'vue';
import { useOperacoesStore } from '@/stores/operacoes';
import OperacaoModal from '@/components/OperacaoModal.vue';
import { formatCurrency } from '@/utils/formatters.js';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

const operacoesStore = useOperacoesStore();
const authStore = useAuthStore();
const isModalVisible = ref(false);
const router = useRouter();

// Usamos uma propriedade computada para acessar a lista de forma segura e reativa
const operacoes = computed(() => operacoesStore.operacoes?.$values || []);

onMounted(() => {
    operacoesStore.fetchOperacoes();
});

async function handleSave(operacaoData) {
    await operacoesStore.createOperacao(operacaoData);
    isModalVisible.value = false;
}

function goToDetalhes(operacaoId) {
    router.push({ name: 'operacao-detalhes', params: { id: operacaoId } });
}
</script>

<template>
    <div class="p-10">
        <div class="flex justify-between items-center mb-8">
            <h1 class="text-3xl font-bold text-neutral-dark">Minhas Operações</h1>
            <button v-if="authStore.isAdmin" @click="isModalVisible = true"
                class="bg-primary hover:bg-primary-dark text-white font-bold py-2 px-4 rounded-lg transition-colors">
                Adicionar Nova Operação
            </button>
        </div>

        <div class="bg-white p-6 rounded-lg shadow-card">
            <table v-if="operacoes.length > 0" class="w-full text-left">
                <thead class="border-b">
                    <tr>
                        <th class="py-2">Nome</th>
                        <th>Meta Mensal</th>
                        <th>Projeção</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="op in operacoes" :key="op.id" @click="goToDetalhes(op.id)"
                        class="border-b hover:bg-gray-100 cursor-pointer transition-colors">
                        <td class="py-4 font-semibold text-primary">{{ op.nome || 'Operação Sem Nome' }}</td>
                        <td>{{ formatCurrency(op.metaMensal) }}</td>
                        <td>{{ formatCurrency(op.projecaoFaturamento || 0) }}</td>
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