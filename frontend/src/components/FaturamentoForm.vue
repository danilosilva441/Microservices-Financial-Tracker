<!-- components/FaturamentoForm.vue -->
<script setup>
import { ref } from 'vue';

const emit = defineEmits(['submit']);

const form = ref({
  origem: '',
  descricao: '',
  valor: 0,
  formaPagamento: 'dinheiro',
  // Campo responsavel REMOVIDO - será preenchido automaticamente
  observacoes: ''
});

const formasPagamento = [
  { value: 'dinheiro', label: 'Dinheiro' },
  { value: 'cartao_credito', label: 'Cartão de Crédito' },
  { value: 'cartao_debito', label: 'Cartão de Débito' },
  { value: 'pix', label: 'PIX' },
  { value: 'transferencia', label: 'Transferência' }
];

const handleSubmit = () => {
  if (!form.value.origem || form.value.valor <= 0) {
    alert('Preencha origem e valor corretamente');
    return;
  }

  emit('submit', {
    ...form.value,
    dataHora: new Date().toISOString(),
    valor: parseFloat(form.value.valor)
    // responsavel será adicionado automaticamente no composable
  });

  // Reset form
  form.value = {
    origem: '',
    descricao: '',
    valor: 0,
    formaPagamento: 'dinheiro',
    observacoes: ''
  };
};
</script>

<template>
  <form @submit.prevent="handleSubmit" class="space-y-4">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <!-- Origem -->
      <div>
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Origem *
        </label>
        <input
          v-model="form.origem"
          type="text"
          required
          placeholder="Ex: ROTATIVO, 755, AVULSO"
          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
        />
      </div>

      <!-- Valor -->
      <div>
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Valor (R$) *
        </label>
        <input
          v-model="form.valor"
          type="number"
          step="0.01"
          min="0"
          required
          placeholder="0,00"
          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
        />
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <!-- Forma de Pagamento -->
      <div>
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Forma de Pagamento
        </label>
        <select
          v-model="form.formaPagamento"
          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
        >
          <option v-for="fp in formasPagamento" :key="fp.value" :value="fp.value">
            {{ fp.label }}
          </option>
        </select>
      </div>

      <!-- Descrição -->
      <div>
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
          Descrição
        </label>
        <input
          v-model="form.descricao"
          type="text"
          placeholder="Breve descrição"
          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
        />
      </div>
    </div>

    <!-- Observações -->
    <div>
      <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
        Observações
      </label>
      <textarea
        v-model="form.observacoes"
        rows="2"
        placeholder="Observações adicionais..."
        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
      ></textarea>
    </div>

    <!-- Botão -->
    <div class="pt-2">
      <button
        type="submit"
        class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 px-4 rounded-lg transition-colors shadow-sm"
      >
        Adicionar Lançamento
      </button>
    </div>
  </form>
</template>