<script setup>
import { ref, watch } from 'vue';

const props = defineProps({ isVisible: Boolean });
const emit = defineEmits(['close', 'save']);

// --- ESTADO DO FORMULÁRIO SIMPLIFICADO ---
const nome = ref('');
const descricao = ref('');
const endereco = ref('');
const metaMensal = ref(0);
const dataInicio = ref('');

// --- FUNÇÕES ---
function resetForm() {
  nome.value = '';
  descricao.value = '';
  endereco.value = '';
  metaMensal.value = 0;
  dataInicio.value = new Date().toISOString().split('T')[0];
}

function handleSubmit() {
  // Envia os dados para o componente pai, sem o campo 'moeda'
  emit('save', {
    nome: nome.value,
    descricao: descricao.value,
    endereco: endereco.value,
    metaMensal: parseFloat(metaMensal.value),
    dataInicio: new Date(dataInicio.value),
  });
}

// Observa a propriedade 'isVisible' para resetar o formulário quando o modal abre
watch(() => props.isVisible, (newValue) => {
  if (newValue) {
    resetForm();
  }
});
</script>

<template>
  <div v-if="isVisible" @click.self="emit('close')"
    class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
    <div class="bg-white p-8 rounded-lg shadow-card w-full max-w-md">
      <h2 class="text-2xl font-bold mb-6">Nova Operação</h2>
      <form @submit.prevent="handleSubmit">
        <div class="space-y-4">
          <div>
            <label for="nome" class="block">Nome da Operação</label>
            <input v-model="nome" id="nome" type="text" required class="w-full mt-1 p-2 border rounded-md" />
          </div>
          <div>
            <label for="descricao" class="block">Descrição (Opcional)</label>
            <textarea v-model="descricao" id="descricao" rows="3" class="w-full mt-1 p-2 border rounded-md"></textarea>
          </div>
          <div>
            <label for="endereco" class="block">Endereço (Opcional)</label>
            <input v-model="endereco" id="endereco" type="text" class="w-full mt-1 p-2 border rounded-md" />
          </div>
          
          <div>
            <label for="metaMensal" class="block">Meta Mensal (R$)</label>
            <input v-model="metaMensal" id="metaMensal" type="number" step="0.01" required
              class="w-full mt-1 p-2 border rounded-md" />
          </div>
          
          <div>
            <label for="dataInicio" class="block">Data de Início</label>
            <input v-model="dataInicio" id="dataInicio" type="date" required
              class="w-full mt-1 p-2 border rounded-md" />
          </div>
        </div>
        <div class="mt-8 flex justify-end space-x-4">
          <button type="button" @click="emit('close')" class="px-4 py-2 bg-gray-200 rounded-md">Cancelar</button>
          <button type="submit" class="px-4 py-2 bg-primary text-white rounded-md">Salvar</button>
        </div>
      </form>
    </div>
  </div>
</template>