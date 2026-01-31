<script setup>
import { formatCurrency } from '@/utils/formatters';

const props = defineProps({
    fechamento: {
        type: Object,
        required: true
    }
});

const emit = defineEmits(['voltar']);

// Verificar se o fechamento foi feito pelo usuário atual
const foiFechadoPorMim = computed(() => {
    if (!props.fechamento || !props.fechamento.usuarioAtual) return false;

    const responsavelFechamento = props.fechamento.responsavel || '';
    const meuNome = props.fechamento.usuarioAtual.nome || '';

    return responsavelFechamento.toLowerCase().includes(meuNome.toLowerCase()) ||
        meuNome.toLowerCase().includes(responsavelFechamento.toLowerCase());
});

// Formatar data completa
const formatarDataHora = (dataString) => {
    if (!dataString) return '-';
    return new Date(dataString).toLocaleString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
};
</script>

<template>
    <div
        class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-blue-100 dark:border-blue-900 overflow-hidden">
        <!-- Cabeçalho com botão voltar -->
        <div class="p-6 border-b border-slate-100 dark:border-slate-700 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="emit('voltar')"
                    class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 flex items-center gap-2 text-sm font-medium">
                    ← Voltar para Histórico
                </button>
            </div>

            <div class="text-center">
                <h3 class="text-xl font-bold text-slate-800 dark:text-white">
                    🔍 Detalhes do Fechamento
                </h3>
                <p class="text-sm text-slate-500 dark:text-slate-400">
                    {{ formatarDataHora(fechamento.dataFechamento) }}
                </p>
            </div>

            <div class="w-24"></div> <!-- Espaçador -->
        </div>

        <!-- Informações do Fechamento -->
        <div class="p-6 border-b border-slate-100 dark:border-slate-700">
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
                <div class="flex items-center gap-2">
                    <p class="text-slate-600 dark:text-slate-400">
                        <strong>Fechado por:</strong> {{ fechamento.responsavel || 'Admin' }}
                    </p>
                    <span v-if="foiFechadoPorMim"
                        class="text-xs px-2 py-0.5 bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300 rounded">
                        Você
                    </span>
                </div>

                <div class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
                    <p class="text-xs text-slate-500 dark:text-slate-400 uppercase">Valor Total</p>
                    <p class="font-bold text-slate-800 dark:text-white text-lg">
                        {{ formatCurrency(fechamento.valorTotal || 0) }}
                    </p>
                </div>

                <div class="bg-purple-50 dark:bg-purple-900/20 p-4 rounded-lg">
                    <p class="text-xs text-slate-500 dark:text-slate-400 uppercase">Status</p>
                    <span :class="[
                        'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium',
                        fechamento.status === 'enviado'
                            ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300'
                            : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300'
                    ]">
                        {{ fechamento.status === 'enviado' ? '✅ Enviado' : '⏳ Pendente' }}
                    </span>
                </div>
            </div>

            <!-- Informações Adicionais -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                <div>
                    <p class="text-slate-600 dark:text-slate-400"><strong>Enviado por:</strong> {{ fechamento.enviadoPor
                        || fechamento.responsavel }}</p>
                    <p class="text-slate-600 dark:text-slate-400"><strong>Hora do fechamento:</strong> {{
                        formatarDataHora(fechamento.horaFechamento) }}</p>
                </div>
                <div>
                    <p class="text-slate-600 dark:text-slate-400"><strong>Hora do envio:</strong> {{
                        formatarDataHora(fechamento.horaEnvio) }}</p>
                    <p class="text-slate-600 dark:text-slate-400"><strong>Local:</strong> {{ fechamento.local || 'Caixa Central' }}</p>
                </div>
            </div>
        </div>

        <!-- Observações -->
        <div v-if="fechamento.observacoes" class="p-6 border-b border-slate-100 dark:border-slate-700">
            <h4 class="text-sm font-bold text-slate-700 dark:text-slate-300 mb-2">📝 Observações</h4>
            <div
                class="bg-slate-50 dark:bg-slate-700/30 p-4 rounded-lg whitespace-pre-line text-slate-600 dark:text-slate-400">
                {{ fechamento.observacoes }}
            </div>
        </div>

        <!-- Tabela de Lançamentos do Fechamento -->
        <div class="overflow-x-auto">
            <table class="w-full text-sm text-left">
                <thead class="bg-slate-50 dark:bg-slate-700/50 border-b dark:border-slate-700">
                    <tr>
                        <th class="px-6 py-3 text-slate-500">DATA/HORA</th>
                        <th class="px-6 py-3 text-slate-500">ORIGEM</th>
                        <th class="px-6 py-3 text-slate-500">RESPONSÁVEL (Lançamento)</th>
                        <th class="px-6 py-3 text-slate-500">FORMA PAGTO</th>
                        <th class="px-6 py-3 text-slate-500 text-right">VALOR</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 dark:divide-slate-700">
                    <tr v-for="item in (fechamento.lancamentos || [])" :key="item.id"
                        class="hover:bg-slate-50 dark:hover:bg-slate-700/30">
                        <td class="px-6 py-3 text-slate-700 dark:text-slate-300">
                            {{ formatarDataHora(item.dataHora || item.data) }}
                        </td>
                        <td class="px-6 py-3">
                            <span
                                class="px-2 py-1 bg-blue-50 text-blue-700 text-xs rounded-md border border-blue-100 dark:bg-blue-900/30 dark:text-blue-300 dark:border-blue-800">
                                {{ item.origem || 'N/A' }}
                            </span>
                        </td>
                        <td class="px-6 py-3 text-slate-600 dark:text-slate-400">
                            {{ item.responsavel || item.usuarioNome || '-' }}
                        </td>
                        <td class="px-6 py-3">
                            <span
                                class="px-2 py-1 bg-green-50 text-green-700 text-xs rounded-md border border-green-100 dark:bg-green-900/30 dark:text-green-300 dark:border-green-800">
                                {{ item.formaPagamento || item.pagamento || 'Dinheiro' }}
                            </span>
                        </td>
                        <td class="px-6 py-3 text-right font-medium text-slate-900 dark:text-white">
                            {{ formatCurrency(item.valor || 0) }}
                        </td>
                    </tr>
                    <tr v-if="!fechamento.lancamentos || fechamento.lancamentos.length === 0">
                        <td colspan="5" class="px-6 py-8 text-center text-slate-500">
                            Nenhum lançamento encontrado para este fechamento.
                        </td>
                    </tr>
                </tbody>
                <tfoot class="bg-slate-50 dark:bg-slate-700/50">
                    <tr>
                        <td colspan="4" class="px-6 py-3 text-right font-bold text-slate-700 dark:text-slate-300">
                            TOTAL DO DIA:
                        </td>
                        <td class="px-6 py-3 text-right font-bold text-lg text-green-600 dark:text-green-400">
                            {{ formatCurrency(fechamento.valorTotal || 0) }}
                        </td>
                    </tr>
                </tfoot>
            </table>
        </div>

        <!-- Registro de Assinaturas -->
        <div v-if="fechamento.assinaturas" class="p-6 border-t border-slate-100 dark:border-slate-700">
            <h4 class="text-sm font-bold text-slate-700 dark:text-slate-300 mb-4">✍️ Registro de Assinaturas</h4>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div v-for="assinatura in fechamento.assinaturas" :key="assinatura.funcao"
                    class="bg-slate-50 dark:bg-slate-700/30 p-4 rounded-lg border border-slate-200 dark:border-slate-600">
                    <p class="font-medium text-slate-700 dark:text-slate-300">{{ assinatura.funcao }}</p>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ assinatura.nome }}</p>
                    <div class="mt-2">
                        <p class="text-xs text-slate-500 dark:text-slate-500 break-all">
                            🔐 {{ assinatura.hash || 'Sem hash' }}
                        </p>
                        <p class="text-xs text-slate-500 dark:text-slate-500">
                            {{ formatarDataHora(assinatura.dataHora) }}
                        </p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Log de Ações -->
        <div v-if="fechamento.logAcoes" class="p-6 border-t border-slate-100 dark:border-slate-700">
            <h4 class="text-sm font-bold text-slate-700 dark:text-slate-300 mb-4">📋 Log de Ações do Dia</h4>
            <div class="space-y-2 max-h-60 overflow-y-auto">
                <div v-for="(log, index) in fechamento.logAcoes" :key="index"
                    class="flex items-start gap-3 p-3 bg-slate-50 dark:bg-slate-700/30 rounded-lg">
                    <div class="w-2 h-2 mt-2 rounded-full bg-blue-500"></div>
                    <div class="flex-1">
                        <div class="flex justify-between">
                            <p class="font-medium text-slate-700 dark:text-slate-300">{{ log.acao }}</p>
                            <p class="text-xs text-slate-500 dark:text-slate-500">
                                {{ formatarDataHora(log.dataHora) }}
                            </p>
                        </div>
                        <p class="text-sm text-slate-600 dark:text-slate-400">
                            <strong>Usuário:</strong> {{ log.usuario }}
                        </p>
                        <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ log.detalhes }}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>