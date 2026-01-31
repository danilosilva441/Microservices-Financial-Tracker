<script setup>
import { onMounted, ref, computed } from 'vue';
import { useCaixaHoje } from '@/composables/caixa/useCaixaHoje';

const props = defineProps({
  unidadeId: { type: String, required: true }
});

const { 
  loading, 
  dataSelecionada, 
  statusCaixa, 
  caixaDiario, 
  buscarCaixaDaData, 
  iniciarDia,
  realizarLancamento
} = useCaixaHoje(props.unidadeId);

// Estado do formulário de novo lançamento
const novoLancamento = ref({
  hora: '12:00', // Padrão
  valor: 0,
  origem: '',
  pagamento: 'Dinheiro' // Default
});

const fundoCaixaInput = ref(0);

// Computed para totais
const totalDoDia = computed(() => {
  return caixaDiario.value?.valorTotalParciais || 0; // Usando campo do DTO novo
});

const handleIniciarDia = async () => {
  try {
    await iniciarDia(fundoCaixaInput.value);
  } catch (e) {
    alert("Erro ao iniciar dia: " + e.message);
  }
};

const handleLancar = async () => {
  if (novoLancamento.value.valor <= 0) return alert("Valor inválido");
  
  try {
    // Monta datas ISO completas baseadas na data selecionada + hora digitada
    const dataBase = dataSelecionada.value;
    const horaInicioIso = `${dataBase}T${novoLancamento.value.hora}:00.000Z`;
    // Hora fim +1 min simulado
    const horaFimIso = `${dataBase}T${novoLancamento.value.hora}:59.000Z`; 

    await realizarLancamento({
      valor: novoLancamento.value.valor,
      horaInicio: horaInicioIso,
      horaFim: horaFimIso,
      metodoPagamentoId: novoLancamento.value.pagamento, // Ajuste para seu Enum/ID real
      origem: novoLancamento.value.origem
    });
    
    // Limpa form
    novoLancamento.value.valor = 0;
    novoLancamento.value.origem = '';
  } catch (e) {
    alert("Erro ao lançar");
  }
};

onMounted(() => {
  buscarCaixaDaData();
});
</script>

<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center bg-gray-50 p-4 rounded-lg border">
      <div class="flex items-center gap-4">
        <label class="font-medium text-gray-700">Data do Caixa:</label>
        <input 
          type="date" 
          v-model="dataSelecionada"
          class="border rounded px-3 py-2 focus:ring-2 focus:ring-blue-500 outline-none"
        />
      </div>
      <div class="flex items-center gap-2">
        <span class="text-sm text-gray-500">Status:</span>
        <span 
          class="px-3 py-1 rounded-full text-xs font-bold uppercase"
          :class="{
            'bg-gray-200 text-gray-600': statusCaixa === 'NaoIniciado',
            'bg-green-100 text-green-700': statusCaixa === 'Aberto',
            'bg-blue-100 text-blue-700': statusCaixa === 'Fechado',
            'bg-purple-100 text-purple-700': statusCaixa === 'Conferido'
          }"
        >
          {{ statusCaixa === 'NaoIniciado' ? 'Não Iniciado' : statusCaixa }}
        </span>
      </div>
    </div>

    <div v-if="loading" class="text-center py-8 text-gray-500">
      Carregando dados do dia...
    </div>

    <div v-else-if="statusCaixa === 'NaoIniciado'" class="text-center py-10 bg-white border border-dashed border-gray-300 rounded-lg">
      <h3 class="text-lg font-medium text-gray-900 mb-2">Nenhum caixa aberto para {{ dataSelecionada }}</h3>
      <p class="text-gray-500 mb-6">Informe o fundo de caixa para iniciar as operações.</p>
      
      <div class="flex justify-center items-end gap-4">
        <div class="text-left">
          <label class="block text-xs font-medium text-gray-700 mb-1">Fundo de Caixa (R$)</label>
          <input 
            v-model="fundoCaixaInput" 
            type="number" 
            step="0.01" 
            class="border rounded px-3 py-2 w-32" 
          />
        </div>
        <button 
          @click="handleIniciarDia"
          class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
        >
          Iniciar Dia
        </button>
      </div>
    </div>

    <div v-else-if="statusCaixa === 'Aberto'" class="space-y-6">
      
      <div class="bg-white border rounded-lg p-4 shadow-sm">
        <h4 class="text-sm font-bold text-gray-700 mb-4 uppercase tracking-wide">Novo Lançamento</h4>
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4 items-end">
          <div>
            <label class="block text-xs text-gray-500 mb-1">Hora</label>
            <input type="time" v-model="novoLancamento.hora" class="w-full border rounded px-3 py-2" />
          </div>
          <div>
            <label class="block text-xs text-gray-500 mb-1">Valor (R$)</label>
            <input type="number" step="0.01" v-model="novoLancamento.valor" class="w-full border rounded px-3 py-2" />
          </div>
          <div class="md:col-span-1">
            <label class="block text-xs text-gray-500 mb-1">Origem / Descrição</label>
            <input type="text" v-model="novoLancamento.origem" placeholder="Ex: Avulso" class="w-full border rounded px-3 py-2" />
          </div>
          <div>
            <label class="block text-xs text-gray-500 mb-1">Pagamento</label>
            <select v-model="novoLancamento.pagamento" class="w-full border rounded px-3 py-2">
              <option value="Dinheiro">Dinheiro</option>
              <option value="CartaoCredito">Cartão Crédito</option>
              <option value="CartaoDebito">Cartão Débito</option>
              <option value="Pix">Pix</option>
            </select>
          </div>
          <button 
            @click="handleLancar"
            class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 font-medium"
          >
            + Lançar
          </button>
        </div>
      </div>

      <div class="bg-white border rounded-lg overflow-hidden">
        <div class="p-4 border-b flex justify-between items-center bg-gray-50">
          <h3 class="font-bold text-gray-700">Lançamentos do Dia</h3>
          <div class="text-green-700 font-bold text-lg">
            Total: {{ totalDoDia.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}
          </div>
        </div>
        
        <table class="w-full text-sm text-left">
          <thead class="text-xs text-gray-700 uppercase bg-gray-50 border-b">
            <tr>
              <th class="px-4 py-3">Hora</th>
              <th class="px-4 py-3">Origem</th>
              <th class="px-4 py-3">Pagamento</th>
              <th class="px-4 py-3 text-right">Valor</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="!caixaDiario?.faturamentosParciais?.length" class="bg-white border-b">
              <td colspan="4" class="px-4 py-8 text-center text-gray-500">
                Nenhum lançamento registrado hoje.
              </td>
            </tr>
            </tbody>
        </table>
      </div>

      <div class="flex justify-end pt-4">
        <button class="bg-red-600 text-white px-6 py-2 rounded shadow hover:bg-red-700">
          Fechar Caixa
        </button>
      </div>

    </div>

    <div v-else class="text-center py-10">
      <h3 class="text-xl font-bold text-gray-800">Este caixa já foi fechado.</h3>
      <p class="text-gray-500">Acesse a aba "Detalhes do Fechamento" para ver o relatório completo.</p>
    </div>

  </div>
</template>