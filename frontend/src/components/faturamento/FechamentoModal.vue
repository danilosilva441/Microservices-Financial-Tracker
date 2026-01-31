<!-- components/faturamento/FechamentoModal.vue -->
<script setup>
import { computed } from 'vue';

const props = defineProps({
    isVisible: Boolean,
    resumo: Object,
    isLoading: Boolean,
    // Novo prop: usuário atual
    usuarioAtual: {
        type: Object,
        default: () => ({ nome: 'Operador' })
    }
});

const emit = defineEmits(['close', 'confirm']);

const observacoes = ref('');

const handleConfirm = () => {
    emit('confirm', {
        observacoes: observacoes.value
        // responsavel será preenchido automaticamente no composable
    });

    observacoes.value = '';
};
</script>

<template>
    <div v-if="isVisible" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Overlay -->
        <div class="fixed inset-0 bg-black/50" @click="$emit('close')"></div>

        <!-- Modal -->
        <div class="flex min-h-full items-center justify-center p-4">
            <div class="relative bg-white dark:bg-slate-800 rounded-xl shadow-xl w-full max-w-md">
                <!-- Header -->
                <div class="p-6 border-b border-slate-100 dark:border-slate-700">
                    <h3 class="text-xl font-bold text-slate-800 dark:text-white">
                        Fechar Caixa
                    </h3>
                    <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">
                        Confirme as informações para fechar o caixa do dia
                    </p>
                </div>

                <!-- Conteúdo -->
                <div class="p-6">
                    <!-- Resumo -->
                    <div class="mb-6 p-4 bg-slate-50 dark:bg-slate-700/50 rounded-lg">
                        <p class="text-sm text-slate-600 dark:text-slate-400">Total do dia:</p>
                        <p class="text-2xl font-bold text-green-600 dark:text-green-400">
                            {{ resumo.total?.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) || 'R$ 0,00' }}
                        </p>
                        <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                            {{ resumo.quantidade || 0 }} lançamento(s)
                        </p>
                    </div>

                    <!-- Observações -->
                    <div class="mb-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
                        <p class="text-sm text-slate-600 dark:text-slate-400 mb-1">
                            Fechamento será realizado por:
                        </p>
                        <p class="font-medium text-slate-800 dark:text-white">
                            {{ usuarioAtual.nome }}
                            <span
                                class="text-xs px-2 py-0.5 bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300 rounded ml-2">
                                Você
                            </span>
                        </p>
                    </div>
                </div>

                <!-- Footer -->
                <div class="p-6 border-t border-slate-100 dark:border-slate-700 flex justify-end gap-3">
                    <button @click="$emit('close')" :disabled="isLoading"
                        class="px-4 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors disabled:opacity-50">
                        Cancelar
                    </button>
                    <button @click="handleConfirm" :disabled="isLoading"
                        class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed">
                        <span v-if="isLoading">Processando...</span>
                        <span v-else>Confirmar Fechamento</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>