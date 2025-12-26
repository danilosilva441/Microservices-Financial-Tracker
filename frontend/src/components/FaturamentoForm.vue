<script setup>
import { ref, onMounted } from 'vue';

const emit = defineEmits(['submit']);

const form = ref({
  valor: '',
  origem: '',
  metodoPagamentoId: 'Dinheiro',
  dataHora: '' // Novo campo para data/hora manual
});

// Opções do Enum
const metodosPagamento = [
  { label: 'Dinheiro', value: 'Dinheiro' },
  { label: 'Pix', value: 'Pix' },
  { label: 'Cartão de Crédito', value: 'CartaoCredito' },
  { label: 'Cartão de Débito', value: 'CartaoDebito' },
  { label: 'Vale Refeição', value: 'ValeRefeicao' },
  { label: 'Outros', value: 'Outros' }
];

// Inicializa o campo de data com "Agora" (formato local para o input datetime-local)
onMounted(() => {
  resetForm();
});

const resetForm = () => {
  const agora = new Date();
  // Ajuste para o fuso horário local no input datetime-local
  agora.setMinutes(agora.getMinutes() - agora.getTimezoneOffset());
  
  form.value = {
    valor: '',
    origem: '',
    metodoPagamentoId: 'Dinheiro',
    dataHora: agora.toISOString().slice(0, 16) // Formato YYYY-MM-DDTHH:mm
  };
};

const handleSubmit = () => {
  if (!form.value.valor || !form.value.origem || !form.value.dataHora) {
    alert('Preencha todos os campos.');
    return;
  }

  // 1. Pega a hora de início escolhida pelo operador
  const dataInicio = new Date(form.value.dataHora);
  
  // 2. Adiciona 1 minuto para a hora fim (Para evitar o erro do backend)
  const dataFim = new Date(dataInicio.getTime() + 60000); // + 60.000ms (1 min)

  const payload = {
    valor: parseFloat(form.value.valor),
    horaInicio: dataInicio.toISOString(),
    horaFim: dataFim.toISOString(),
    metodoPagamentoId: form.value.metodoPagamentoId,
    origem: form.value.origem.toUpperCase()
  };

  console.log('📤 Payload Ajustado:', payload);

  emit('submit', payload);
  resetForm(); // Limpa e reseta a hora para "agora"
};
</script>

<template>
  <form @submit.prevent="handleSubmit" class="bg-white dark:bg-slate-800 p-4 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
    
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
      
      <div>
        <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Data/Hora</label>
        <input 
          v-model="form.dataHora" 
          type="datetime-local" 
          class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none text-sm"
          required
        />
      </div>

      <div>
        <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Valor (R$)</label>
        <div class="relative">
          <span class="absolute left-3 top-2 text-slate-400">R$</span>
          <input 
            v-model="form.valor" 
            type="number" step="0.01" min="0" 
            class="w-full pl-9 pr-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none font-bold text-slate-700 dark:text-white"
            placeholder="0.00"
            required
          />
        </div>
      </div>

      <div>
        <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Origem / Descrição</label>
        <input 
          v-model="form.origem" 
          type="text" 
          class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none uppercase"
          placeholder="EX: AVULSO"
          required
        />
      </div>

      <div>
        <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Pagamento</label>
        <select 
          v-model="form.metodoPagamentoId" 
          class="w-full px-3 py-2 border rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
        >
          <option v-for="metodo in metodosPagamento" :key="metodo.value" :value="metodo.value">
            {{ metodo.label }}
          </option>
        </select>
      </div>

    </div>

    <div class="mt-4 flex justify-end">
      <button 
        type="submit" 
        class="w-full md:w-auto bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-8 rounded-lg transition-colors shadow-md active:transform active:scale-95 flex items-center justify-center gap-2"
      >
        <span>+</span> Lançar Entrada
      </button>
    </div>

  </form>
</template>