<!-- components/unidades/UnidadeForm.vue -->
<template>
  <form @submit.prevent="submitForm" class="w-full max-w-6xl mx-auto">
    <!-- Mensagem de Erro Geral -->
    <div v-if="serverError" 
         class="mb-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg text-red-700 dark:text-red-400 flex items-center gap-3 animate-fadeIn">
      <IconExclamationTriangle class="w-5 h-5 flex-shrink-0" />
      <span class="text-sm">{{ serverError }}</span>
    </div>

    <!-- Grid do Formulário -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 lg:gap-8">
      <!-- Informações Básicas -->
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 space-y-6">
        <h4 class="text-lg font-semibold text-gray-900 dark:text-white flex items-center gap-2 pb-3 border-b border-gray-200 dark:border-gray-700">
          <IconInfoCircle class="w-5 h-5 text-primary-500 dark:text-primary-400" />
          Informações Básicas
        </h4>

        <div class="space-y-4">
          <div>
            <label for="nome" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Nome da Unidade <span class="text-red-500">*</span>
              <span class="block text-xs text-gray-500 dark:text-gray-400 font-normal">Nome comercial ou fantasia</span>
            </label>
            <input 
              id="nome" 
              v-model="form.nome" 
              type="text" 
              :class="[
                'w-full px-4 py-2.5 rounded-lg border transition-colors focus:outline-none focus:ring-2',
                errors.nome 
                  ? 'border-red-300 dark:border-red-700 bg-red-50 dark:bg-red-900/20 focus:ring-red-500' 
                  : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 focus:border-primary-500 focus:ring-primary-500'
              ]"
              placeholder="Ex: Loja Centro, Filial Norte"
              @blur="validateField('nome')"
            />
            <p v-if="errors.nome" class="mt-1.5 text-xs text-red-600 dark:text-red-400 flex items-center gap-1">
              <IconExclamationCircle class="w-3.5 h-3.5" />
              {{ errors.nome }}
            </p>
          </div>

          <div>
            <label for="descricao" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Descrição
              <span class="block text-xs text-gray-500 dark:text-gray-400 font-normal">Informações adicionais sobre a unidade</span>
            </label>
            <textarea 
              id="descricao" 
              v-model="form.descricao" 
              rows="3"
              class="w-full px-4 py-2.5 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-500 transition-colors"
              placeholder="Descreva a unidade, atividades principais, etc."
            ></textarea>
            <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">Máximo de 500 caracteres</p>
          </div>

          <div>
            <label for="endereco" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Endereço Completo <span class="text-red-500">*</span>
              <span class="block text-xs text-gray-500 dark:text-gray-400 font-normal">Rua, número, bairro, cidade, estado</span>
            </label>
            <textarea 
              id="endereco" 
              v-model="form.endereco" 
              rows="2"
              :class="[
                'w-full px-4 py-2.5 rounded-lg border transition-colors focus:outline-none focus:ring-2',
                errors.endereco 
                  ? 'border-red-300 dark:border-red-700 bg-red-50 dark:bg-red-900/20 focus:ring-red-500' 
                  : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 focus:border-primary-500 focus:ring-primary-500'
              ]"
              placeholder="Ex: Rua Exemplo, 123 - Centro - Cidade/Estado"
              @blur="validateField('endereco')"
            ></textarea>
            <p v-if="errors.endereco" class="mt-1.5 text-xs text-red-600 dark:text-red-400 flex items-center gap-1">
              <IconExclamationCircle class="w-3.5 h-3.5" />
              {{ errors.endereco }}
            </p>
          </div>
        </div>
      </div>

      <!-- Meta Financeira -->
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 space-y-6">
        <h4 class="text-lg font-semibold text-gray-900 dark:text-white flex items-center gap-2 pb-3 border-b border-gray-200 dark:border-gray-700">
          <IconChartLine class="w-5 h-5 text-primary-500 dark:text-primary-400" />
          Meta Financeira
        </h4>

        <div class="space-y-4">
          <div>
            <label for="metaMensal" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Meta Mensal (R$) <span class="text-red-500">*</span>
              <span class="block text-xs text-gray-500 dark:text-gray-400 font-normal">Valor esperado de faturamento mensal</span>
            </label>
            <div class="relative">
              <span class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 dark:text-gray-400">R$</span>
              <input 
                id="metaMensal" 
                v-model.number="form.metaMensal" 
                type="number" 
                step="0.01" 
                min="0"
                :class="[
                  'w-full pl-10 pr-4 py-2.5 rounded-lg border transition-colors focus:outline-none focus:ring-2',
                  errors.metaMensal 
                    ? 'border-red-300 dark:border-red-700 bg-red-50 dark:bg-red-900/20 focus:ring-red-500' 
                    : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 focus:border-primary-500 focus:ring-primary-500'
                ]"
                placeholder="0,00"
                @blur="validateField('metaMensal')"
              />
            </div>
            <p v-if="errors.metaMensal" class="mt-1.5 text-xs text-red-600 dark:text-red-400 flex items-center gap-1">
              <IconExclamationCircle class="w-3.5 h-3.5" />
              {{ errors.metaMensal }}
            </p>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Período de Operação
              <span class="block text-xs text-gray-500 dark:text-gray-400 font-normal">Data de início e término do contrato</span>
            </label>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label for="dataInicio" class="block text-xs text-gray-500 dark:text-gray-400 mb-1">Data de Início *</label>
                <input 
                  id="dataInicio" 
                  v-model="form.dataInicio" 
                  type="date" 
                  :class="[
                    'w-full px-4 py-2.5 rounded-lg border transition-colors focus:outline-none focus:ring-2',
                    errors.dataInicio 
                      ? 'border-red-300 dark:border-red-700 bg-red-50 dark:bg-red-900/20 focus:ring-red-500' 
                      : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 focus:border-primary-500 focus:ring-primary-500'
                  ]"
                  @change="validateDates"
                />
              </div>
              <div>
                <label for="dataFim" class="block text-xs text-gray-500 dark:text-gray-400 mb-1">Data de Término</label>
                <input 
                  id="dataFim" 
                  v-model="form.dataFim" 
                  type="date" 
                  :min="form.dataInicio"
                  :class="[
                    'w-full px-4 py-2.5 rounded-lg border transition-colors focus:outline-none focus:ring-2',
                    errors.dataFim 
                      ? 'border-red-300 dark:border-red-700 bg-red-50 dark:bg-red-900/20 focus:ring-red-500' 
                      : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 focus:border-primary-500 focus:ring-primary-500'
                  ]"
                  @change="validateDates"
                />
              </div>
            </div>
            <p v-if="errors.dataFim" class="mt-1.5 text-xs text-red-600 dark:text-red-400 flex items-center gap-1">
              <IconExclamationCircle class="w-3.5 h-3.5" />
              {{ errors.dataFim }}
            </p>
          </div>
        </div>
      </div>

      <!-- Status e Configurações -->
      <div class="lg:col-span-2 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 space-y-6">
        <h4 class="text-lg font-semibold text-gray-900 dark:text-white flex items-center gap-2 pb-3 border-b border-gray-200 dark:border-gray-700">
          <IconCog class="w-5 h-5 text-primary-500 dark:text-primary-400" />
          Configurações
        </h4>

        <div class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">Status da Unidade</label>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <label class="cursor-pointer">
                <input type="radio" v-model="form.isAtivo" :value="true" class="hidden" />
                <div :class="[
                  'p-4 rounded-lg border-2 transition-all duration-200',
                  form.isAtivo 
                    ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20' 
                    : 'border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600'
                ]">
                  <div class="flex items-start gap-3">
                    <div class="flex-shrink-0 w-10 h-10 rounded-lg bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
                      <IconCheckCircle class="w-5 h-5 text-green-600 dark:text-green-400" />
                    </div>
                    <div>
                      <div class="font-medium text-gray-900 dark:text-white">Ativa</div>
                      <div class="text-xs text-gray-500 dark:text-gray-400 mt-1">Unidade em operação normal</div>
                    </div>
                  </div>
                </div>
              </label>

              <label class="cursor-pointer">
                <input type="radio" v-model="form.isAtivo" :value="false" class="hidden" />
                <div :class="[
                  'p-4 rounded-lg border-2 transition-all duration-200',
                  !form.isAtivo 
                    ? 'border-gray-400 dark:border-gray-500 bg-gray-50 dark:bg-gray-800' 
                    : 'border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600'
                ]">
                  <div class="flex items-start gap-3">
                    <div class="flex-shrink-0 w-10 h-10 rounded-lg bg-gray-200 dark:bg-gray-700 flex items-center justify-center">
                      <IconTimesCircle class="w-5 h-5 text-gray-600 dark:text-gray-400" />
                    </div>
                    <div>
                      <div class="font-medium text-gray-900 dark:text-white">Inativa</div>
                      <div class="text-xs text-gray-500 dark:text-gray-400 mt-1">Unidade temporariamente parada</div>
                    </div>
                  </div>
                </div>
              </label>
            </div>
          </div>

          <!-- Campos Adicionais -->
          <div>
            <button 
              type="button" 
              @click="toggleAdditionalFields"
              class="flex items-center gap-2 text-sm font-medium text-primary-600 dark:text-primary-400 hover:text-primary-700 dark:hover:text-primary-300 transition-colors"
            >
              <IconPlusCircle class="w-4 h-4" />
              {{ showAdditionalFields ? 'Ocultar' : 'Mostrar' }} campos adicionais
              <IconChevronDown :class="['w-4 h-4 transition-transform', showAdditionalFields ? 'rotate-180' : '']" />
            </button>

            <div v-if="showAdditionalFields" class="mt-4 p-4 bg-gray-50 dark:bg-gray-900/50 rounded-lg border border-gray-200 dark:border-gray-700 space-y-4 animate-slideDown">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="telefone" class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">Telefone</label>
                  <input 
                    id="telefone" 
                    v-model="form.telefone" 
                    type="tel" 
                    class="w-full px-3 py-2 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-1 focus:ring-primary-500"
                    placeholder="(00) 00000-0000"
                  />
                </div>

                <div>
                  <label for="email" class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">Email</label>
                  <input 
                    id="email" 
                    v-model="form.email" 
                    type="email" 
                    class="w-full px-3 py-2 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-primary-500 focus:ring-1 focus:ring-primary-500"
                    placeholder="unidade@empresa.com"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Resumo -->
      <div class="lg:col-span-2 bg-gradient-to-br from-primary-50 to-secondary-50 dark:from-primary-900/10 dark:to-secondary-900/10 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h4 class="text-lg font-semibold text-gray-900 dark:text-white flex items-center gap-2 pb-3 border-b border-gray-200 dark:border-gray-700 mb-4">
          <IconClipboardCheck class="w-5 h-5 text-primary-500 dark:text-primary-400" />
          Resumo da Unidade
        </h4>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
            <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Nome</div>
            <div class="font-medium text-gray-900 dark:text-white truncate" :title="form.nome">
              {{ form.nome || 'Não informado' }}
            </div>
          </div>

          <div class="p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
            <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Meta Mensal</div>
            <div class="font-medium text-gray-900 dark:text-white">
              {{ formatCurrency(form.metaMensal) }}
            </div>
          </div>

          <div class="p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
            <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Status</div>
            <div>
              <span :class="[
                'inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium',
                form.isAtivo 
                  ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' 
                  : 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-400'
              ]">
                <span :class="['w-1.5 h-1.5 rounded-full', form.isAtivo ? 'bg-green-500' : 'bg-gray-400']"></span>
                {{ form.isAtivo ? 'Ativa' : 'Inativa' }}
              </span>
            </div>
          </div>

          <div class="p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
            <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">Período</div>
            <div class="text-sm text-gray-900 dark:text-white truncate">
              {{ form.dataInicio ? formatDate(form.dataInicio) : 'Não definido' }}
              <span v-if="form.dataFim" class="text-xs text-gray-500 dark:text-gray-400">
                até {{ formatDate(form.dataFim) }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer com Ações -->
    <div class="mt-8 pt-6 border-t border-gray-200 dark:border-gray-700 flex flex-col sm:flex-row justify-between items-center gap-4">
      <div class="text-xs text-gray-500 dark:text-gray-400 flex items-center gap-2">
        <IconInfoCircle class="w-4 h-4" />
        <span>Campos marcados com <span class="text-red-500">*</span> são obrigatórios</span>
      </div>

      <div class="flex items-center gap-3 w-full sm:w-auto">
        <button 
          type="button" 
          @click="resetForm"
          class="btn-outline flex-1 sm:flex-initial justify-center"
        >
          <IconRefresh class="w-4 h-4 mr-2" />
          Limpar
        </button>
        
        <button 
          type="submit" 
          :disabled="isLoading"
          class="btn-primary flex-1 sm:flex-initial justify-center relative"
        >
          <IconSave v-if="!isLoading" class="w-4 h-4 mr-2" />
          <span v-if="!isLoading">{{ submitText }}</span>
          <div v-else class="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
        </button>
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue';
import { useUnidadeForm } from '@/composables/unidades/useUnidadeForm';

// Ícones (assumindo que você tem esses componentes SVG)
import IconInfoCircle from '@/components/icons/info-circle.vue';
import IconExclamationTriangle from '@/components/icons/exclamation-triangle.vue';
import IconExclamationCircle from '@/components/icons/exclamation-circle.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconCog from '@/components/icons/cog.vue';
import IconCheckCircle from '@/components/icons/check-circle.vue';
import IconTimesCircle from '@/components/icons/times-circle.vue';
import IconPlusCircle from '@/components/icons/plus-circle.vue';
import IconChevronDown from '@/components/icons/chevron-down.vue';
import IconClipboardCheck from '@/components/icons/clipboard-check.vue';
import IconRefresh from '@/components/icons/refresh.vue';
import IconSave from '@/components/icons/save.vue';

const props = defineProps({
  initialData: {
    type: Object,
    default: null
  },
  mode: {
    type: String,
    default: 'create' // 'create' or 'edit'
  }
});

const emit = defineEmits(['success', 'cancel', 'error']);

const {
  form,
  isLoading,
  errors,
  serverError,
  submit,
  reset,
  validate,
  validateField,
  isFormValid,
  title,
  submitText
} = useUnidadeForm(props.initialData, props.mode === 'edit');

// Local state
const showAdditionalFields = ref(false);

// Methods
const submitForm = async () => {
  const success = await submit();
  if (success) {
    emit('success', form);
  } else {
    emit('error', serverError.value);
  }
};

const resetForm = () => {
  if (confirm('Tem certeza que deseja limpar todos os campos?')) {
    reset();
  }
};

const toggleAdditionalFields = () => {
  showAdditionalFields.value = !showAdditionalFields.value;
};

const validateDates = () => {
  if (form.dataFim && form.dataInicio) {
    const inicio = new Date(form.dataInicio);
    const fim = new Date(form.dataFim);

    if (fim < inicio) {
      errors.value.dataFim = 'Data de fim não pode ser anterior à data de início';
    } else {
      delete errors.value.dataFim;
    }
  }
};

// Formatters
const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value || 0);
};

const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString('pt-BR');
};

// Auto-save draft
let saveTimeout;
watch(form, () => {
  clearTimeout(saveTimeout);
  saveTimeout = setTimeout(() => {
    console.log('Draft saved:', form);
  }, 2000);
}, { deep: true });
</script>

<style scoped>
@import '@/assets/default.css';
</style>