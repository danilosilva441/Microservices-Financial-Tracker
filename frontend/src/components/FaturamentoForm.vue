<script setup>
import { ref } from 'vue';

// Define que o componente pode receber uma propriedade chamada 'errorMessage'
defineProps({
  errorMessage: String
});

const emit = defineEmits(['submit']);

const valor = ref(0);
const data = ref(new Date().toISOString().split('T')[0]);

function handleSubmit() {
  const dataUtc = new Date(data.value + 'T12:00:00Z');
  emit('submit', {
    valor: parseFloat(valor.value),
    data: dataUtc,
  });
  valor.value = 0;
}
</script>

<template>
  <form @submit.prevent="handleSubmit" class="mt-6 border-t pt-6">
    <h3 class="text-lg font-semibold mb-4">Adicionar Novo Faturamento</h3>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
      <div>
        <label for="valor" class="block text-sm font-medium text-gray-700">Valor</label>
        <input v-model="valor" id="valor" type="number" step="0.01" required class="mt-1 w-full p-2 border rounded-md" />
      </div>
      <div>
        <label for="data" class="block text-sm font-medium text-gray-700">Data</label>
        <input v-model="data" id="data" type="date" required class="mt-1 w-full p-2 border rounded-md" />
      </div>
      <div>
        <button type="submit" class="w-full bg-primary hover:bg-primary-dark text-white font-bold py-2 px-4 rounded-lg transition-colors">
          Registrar
        </button>
      </div>
    </div>

    <div v-if="errorMessage" class="mt-4 p-3 bg-red-100 text-red-700 rounded-md text-sm">
      <strong>Erro:</strong> {{ errorMessage }}
    </div>
  </form>
</template>