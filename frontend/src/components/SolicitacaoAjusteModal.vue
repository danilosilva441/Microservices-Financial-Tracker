<script setup>
import { ref, watch } from 'vue';
import { useSolicitacaoStore } from '@/stores/solicitacaoStore';

const props = defineProps({
  isOpen: Boolean,
  faturamento: Object
});
const emit = defineEmits(['close', 'success']);
const solicitacaoStore = useSolicitacaoStore();
const loading = ref(false);
const form = ref({});

// Watch para resetar o formulário quando ele é aberto
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    form.value = {
      faturamentoId: props.faturamento?.id,
      tipo: 'alteracao',
      motivo: '',
      dadosAntigos: JSON.stringify({ valor: props.faturamento.valor, data: props.faturamento.data }),
      dadosNovos: JSON.stringify({ valor: props.faturamento.valor, data: new Date(props.faturamento.data).toISOString().split('T')[0] })
    };
  }
});

const enviarSolicitacao = async () => {
  loading.value = true;
  try {
    // Prepara os dados para o backend, convertendo o JSON para string
    const dadosParaEnviar = {
        ...form.value,
        dadosNovos: JSON.stringify(JSON.parse(form.value.dadosNovos)) // Re-parse para limpar
    }
    await solicitacaoStore.criarSolicitacao(dadosParaEnviar);
    emit('success', 'Solicitação enviada com sucesso!');
    emit('close');
  } catch (error) {
    alert('Erro ao enviar solicitação.');
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 bg-black bg-opacity-60 flex justify-center items-center z-50">
    <div class="bg-white rounded-lg p-8 w-full max-w-lg shadow-xl">
      <h2 class="text-2xl font-bold mb-6">Solicitar Ajuste de Faturamento</h2>
      <form @submit.prevent="enviarSolicitacao">
        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700">Tipo de Solicitação</label>
          <select v-model="form.tipo" class="mt-1 block w-full p-2 border border-gray-300 rounded-md">
            <option value="alteracao">Alteração de Valor/Data</option>
            <option value="remocao">Remoção (Desativação)</option>
          </select>
        </div>
        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700">Motivo da Solicitação *</label>
          <textarea v-model="form.motivo" required rows="3" class="mt-1 block w-full p-2 border border-gray-300 rounded-md" placeholder="Ex: Lançamento incorreto do valor..."></textarea>
        </div>
        <div v-if="form.tipo === 'alteracao'" class="grid grid-cols-2 gap-4 mb-4">
          <div>
            <label class="block text-sm font-medium text-gray-700">Novo Valor</label>
            <input type="number" step="0.01" v-model.number="JSON.parse(form.dadosNovos).valor" @input="e => form.dadosNovos = JSON.stringify({ ...JSON.parse(form.dadosNovos), valor: parseFloat(e.target.value) })" class="mt-1 block w-full p-2 border border-gray-300 rounded-md" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700">Nova Data</label>
            <input type="date" v-model="JSON.parse(form.dadosNovos).data" @input="e => form.dadosNovos = JSON.stringify({ ...JSON.parse(form.dadosNovos), data: e.target.value })" class="mt-1 block w-full p-2 border border-gray-300 rounded-md" />
          </div>
        </div>
        <div class="flex justify-end space-x-4 mt-8">
          <button type="button" @click="emit('close')" class="px-4 py-2 bg-gray-200 rounded-md hover:bg-gray-300">Cancelar</button>
          <button type="submit" :disabled="loading" class="px-4 py-2 bg-primary text-white rounded-md hover:bg-primary-dark disabled:opacity-50">
            {{ loading ? 'Enviando...' : 'Enviar Solicitação' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>