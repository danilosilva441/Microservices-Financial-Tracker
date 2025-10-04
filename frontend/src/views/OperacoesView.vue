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

// Função para calcular o percentual de progresso
const calcularProgresso = (operacao) => {
    if (!operacao.metaMensal || operacao.metaMensal <= 0) return 0;
    return (operacao.projecaoFaturamento || 0) / operacao.metaMensal * 100;
};

// Função para determinar a cor do progresso baseado no percentual
const getCorProgresso = (percentual) => {
    if (percentual >= 150) return 'bg-purple-600'; // Excelente desempenho
    if (percentual >= 120) return 'bg-indigo-600'; // Ótimo desempenho
    if (percentual >= 100) return 'bg-green-600';  // Meta batida
    if (percentual >= 80) return 'bg-green-500';   // Quase lá
    if (percentual >= 60) return 'bg-yellow-500';  // Em andamento
    if (percentual >= 40) return 'bg-orange-500';  // Atenção
    if (percentual >= 20) return 'bg-red-500';     // Crítico
    return 'bg-red-600';                           // Muito crítico
};

// Função para determinar a cor do texto do percentual
const getCorTextoPercentual = (percentual) => {
    if (percentual >= 150) return 'text-purple-700';
    if (percentual >= 120) return 'text-indigo-700';
    if (percentual >= 100) return 'text-green-700';
    if (percentual >= 80) return 'text-green-600';
    if (percentual >= 60) return 'text-yellow-600';
    if (percentual >= 40) return 'text-orange-600';
    if (percentual >= 20) return 'text-red-600';
    return 'text-red-700';
};

// Função para formatar o texto do percentual
const getTextoPercentual = (percentual) => {
    if (percentual > 100) {
        return `${Math.round(percentual)}% (Ultrapassou)`;
    }
    return `${Math.round(percentual)}%`;
};

// Função para determinar a cor da badge de status
const getCorStatus = (operacao) => {
    const percentual = calcularProgresso(operacao);
    if (percentual >= 100) return 'bg-green-100 text-green-800';
    if (percentual >= 80) return 'bg-blue-100 text-blue-800';
    if (percentual >= 60) return 'bg-yellow-100 text-yellow-800';
    if (percentual >= 40) return 'bg-orange-100 text-orange-800';
    return 'bg-red-100 text-red-800';
};

// Função para determinar a cor do ponto da badge
const getCorPontoStatus = (operacao) => {
    const percentual = calcularProgresso(operacao);
    if (percentual >= 100) return 'bg-green-400';
    if (percentual >= 80) return 'bg-blue-400';
    if (percentual >= 60) return 'bg-yellow-400';
    if (percentual >= 40) return 'bg-orange-400';
    return 'bg-red-400';
};

// Função para determinar o texto do status
const getTextoStatus = (operacao) => {
    const percentual = calcularProgresso(operacao);
    if (!operacao.isAtivo) return 'Inativa';
    if (percentual >= 150) return 'Excelente';
    if (percentual >= 120) return 'Ótimo';
    if (percentual >= 100) return 'Meta Batida';
    if (percentual >= 80) return 'Quase Lá';
    if (percentual >= 60) return 'Em Andamento';
    if (percentual >= 40) return 'Atenção';
    if (percentual >= 20) return 'Crítico';
    return 'Muito Crítico';
};
</script>

<template>
    <div class="p-4 sm:p-6 lg:p-8">
        <!-- Cabeçalho -->
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 sm:mb-8 gap-4">
            <div>
                <h1 class="text-2xl sm:text-3xl font-bold text-neutral-dark">Minhas Operações</h1>
                <p class="text-gray-600 text-sm sm:text-base mt-1">
                    {{ operacoes.length }} operação{{ operacoes.length !== 1 ? 'es' : '' }} cadastrada{{ operacoes.length !== 1 ? 's' : '' }}
                </p>
            </div>
            <button 
                v-if="authStore.isAdmin" 
                @click="isModalVisible = true"
                class="flex items-center justify-center bg-primary hover:bg-primary-dark text-white font-medium py-3 px-4 sm:py-2 sm:px-4 rounded-lg transition-colors duration-200 w-full sm:w-auto"
            >
                <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
                </svg>
                Nova Operação
            </button>
        </div>

        <!-- Loading State -->
        <div v-if="operacoesStore.isLoading" class="bg-white rounded-lg shadow-card p-8 text-center">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary mb-4"></div>
            <p class="text-gray-600">Carregando operações...</p>
        </div>

        <!-- Lista de Operações -->
        <div v-else class="bg-white rounded-lg shadow-card overflow-hidden">
            <!-- Desktop Table -->
            <div class="hidden lg:block">
                <table v-if="operacoes.length > 0" class="w-full">
                    <thead class="bg-gray-50 border-b">
                        <tr>
                            <th class="py-4 px-6 text-left text-sm font-medium text-gray-500 uppercase tracking-wider">
                                Operação
                            </th>
                            <th class="py-4 px-6 text-left text-sm font-medium text-gray-500 uppercase tracking-wider">
                                Meta Mensal
                            </th>
                            <th class="py-4 px-6 text-left text-sm font-medium text-gray-500 uppercase tracking-wider">
                                Projeção
                            </th>
                            <th class="py-4 px-6 text-left text-sm font-medium text-gray-500 uppercase tracking-wider">
                                Status
                            </th>
                            <th class="py-4 px-6 text-left text-sm font-medium text-gray-500 uppercase tracking-wider">
                                Progresso
                            </th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-200">
                        <tr 
                            v-for="op in operacoes" 
                            :key="op.id" 
                            @click="goToDetalhes(op.id)"
                            class="hover:bg-gray-50 cursor-pointer transition-colors duration-150"
                        >
                            <td class="py-4 px-6">
                                <div class="flex items-center">
                                    <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-4 flex-shrink-0">
                                        <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                                        </svg>
                                    </div>
                                    <div>
                                        <div class="font-semibold text-primary text-sm sm:text-base">{{ op.nome || 'Operação Sem Nome' }}</div>
                                        <div class="text-gray-500 text-xs sm:text-sm truncate max-w-xs">
                                            {{ op.descricao || 'Sem descrição' }}
                                        </div>
                                    </div>
                                </div>
                            </td>
                            <td class="py-4 px-6 text-sm font-medium text-gray-900">
                                {{ formatCurrency(op.metaMensal) }}
                            </td>
                            <td class="py-4 px-6 text-sm font-medium text-blue-600">
                                {{ formatCurrency(op.projecaoFaturamento || 0) }}
                            </td>
                            <td class="py-4 px-6">
                                <span 
                                    :class="getCorStatus(op)"
                                    class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium"
                                >
                                    <span 
                                        :class="getCorPontoStatus(op)"
                                        class="w-2 h-2 rounded-full mr-2"
                                    ></span>
                                    {{ getTextoStatus(op) }}
                                </span>
                            </td>
                            <td class="py-4 px-6">
                                <div class="flex items-center">
                                    <div class="w-20 bg-gray-200 rounded-full h-2 mr-3">
                                        <div 
                                            :class="getCorProgresso(calcularProgresso(op))"
                                            class="h-2 rounded-full transition-all duration-300"
                                            :style="{ 
                                                width: `${Math.min(calcularProgresso(op), 100)}%` 
                                            }"
                                        ></div>
                                    </div>
                                    <span :class="getCorTextoPercentual(calcularProgresso(op)) + ' text-xs font-medium'">
                                        {{ getTextoPercentual(calcularProgresso(op)) }}
                                    </span>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Mobile Cards -->
            <div v-if="operacoes.length > 0" class="lg:hidden">
                <div 
                    v-for="op in operacoes" 
                    :key="op.id"
                    @click="goToDetalhes(op.id)"
                    class="border-b border-gray-200 last:border-b-0 p-4 hover:bg-gray-50 cursor-pointer transition-colors"
                >
                    <div class="flex items-start justify-between mb-3">
                        <div class="flex items-center">
                            <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center mr-3 flex-shrink-0">
                                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                                </svg>
                            </div>
                            <div>
                                <h3 class="font-semibold text-primary text-base">{{ op.nome || 'Operação Sem Nome' }}</h3>
                                <p class="text-gray-500 text-sm line-clamp-1">{{ op.descricao || 'Sem descrição' }}</p>
                            </div>
                        </div>
                        <span 
                            :class="getCorStatus(op)"
                            class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium flex-shrink-0 ml-2"
                        >
                            <span 
                                :class="getCorPontoStatus(op)"
                                class="w-1.5 h-1.5 rounded-full mr-1"
                            ></span>
                            {{ getTextoStatus(op) }}
                        </span>
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-3">
                        <div>
                            <p class="text-xs text-gray-500 mb-1">Meta Mensal</p>
                            <p class="text-sm font-medium">{{ formatCurrency(op.metaMensal) }}</p>
                        </div>
                        <div>
                            <p class="text-xs text-gray-500 mb-1">Projeção</p>
                            <p class="text-sm font-medium text-blue-600">{{ formatCurrency(op.projecaoFaturamento || 0) }}</p>
                        </div>
                    </div>

                    <div class="mb-2">
                        <p class="text-xs text-gray-500 mb-1">Progresso</p>
                        <div class="flex items-center">
                            <div class="flex-1 bg-gray-200 rounded-full h-2 mr-3">
                                <div 
                                    :class="getCorProgresso(calcularProgresso(op))"
                                    class="h-2 rounded-full transition-all duration-300"
                                    :style="{ 
                                        width: `${Math.min(calcularProgresso(op), 100)}%` 
                                    }"
                                ></div>
                            </div>
                            <span :class="getCorTextoPercentual(calcularProgresso(op)) + ' text-xs font-medium whitespace-nowrap'">
                                {{ getTextoPercentual(calcularProgresso(op)) }}
                            </span>
                        </div>
                    </div>

                    <div class="flex justify-end">
                        <span class="text-xs text-gray-400 flex items-center">
                            Clique para ver detalhes
                            <svg class="w-3 h-3 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
                            </svg>
                        </span>
                    </div>
                </div>
            </div>

            <!-- Estado Vazio -->
            <div v-if="!operacoesStore.isLoading && operacoes.length === 0" class="text-center py-12 px-4">
                <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                </svg>
                <h3 class="text-lg font-medium text-gray-900 mb-2">Nenhuma operação cadastrada</h3>
                <p class="text-gray-500 mb-6 max-w-sm mx-auto">
                    Comece criando sua primeira operação para acompanhar seus faturamentos e metas.
                </p>
                <button 
                    v-if="authStore.isAdmin"
                    @click="isModalVisible = true"
                    class="bg-primary hover:bg-primary-dark text-white font-medium py-2 px-6 rounded-lg transition-colors inline-flex items-center"
                >
                    <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
                    </svg>
                    Criar Primeira Operação
                </button>
            </div>
        </div>

        <OperacaoModal :is-visible="isModalVisible" @close="isModalVisible = false" @save="handleSave" />
    </div>
</template>

<style scoped>
.line-clamp-1 {
    display: -webkit-box;
    -webkit-line-clamp: 1;
    -webkit-box-orient: vertical;
    overflow: hidden;
}

/* Melhorias para dark mode */
@media (prefers-color-scheme: dark) {
    .bg-white {
        background-color: #374151;
    }
    
    .bg-gray-50 {
        background-color: #4b5563;
    }
    
    .text-gray-500 {
        color: #9ca3af;
    }
    
    .text-gray-900 {
        color: #f9fafb;
    }
    
    .border-gray-200 {
        border-color: #4b5563;
    }
    
    .divide-gray-200 > * + * {
        border-color: #4b5563;
    }
    
    .hover\:bg-gray-50:hover {
        background-color: #4b5563;
    }
}
</style>