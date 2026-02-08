<!-- components/unidades/UnidadeForm.vue -->
<template>
  <form @submit.prevent="submitForm" class="unidade-form">
    <!-- Header do Formulário -->
    <div class="form-header">
      <h3>{{ title }}</h3>
      <div class="form-actions">
        <button type="button" class="btn btn-secondary" @click="$emit('cancel')" :disabled="isLoading">
          Cancelar
        </button>
        <button type="submit" class="btn btn-primary" :disabled="isLoading || !isFormValid">
          <span v-if="isLoading" class="spinner-border spinner-border-sm"></span>
          {{ submitText }}
        </button>
      </div>
    </div>

    <!-- Mensagem de Erro Geral -->
    <div v-if="serverError" class="alert alert-danger">
      <i class="fas fa-exclamation-circle"></i>
      {{ serverError }}
    </div>

    <!-- Grid do Formulário -->
    <div class="form-grid">
      <!-- Informações Básicas -->
      <div class="form-section">
        <h4 class="section-title">
          <i class="fas fa-info-circle"></i>
          Informações Básicas
        </h4>

        <div class="form-group">
          <label for="nome" class="form-label">
            Nome da Unidade *
            <span class="label-helper">Nome comercial ou fantasia</span>
          </label>
          <input id="nome" v-model="form.nome" type="text" :class="{ 'is-invalid': errors.nome }" class="form-control"
            placeholder="Ex: Loja Centro, Filial Norte" @blur="validateField('nome')" />
          <div v-if="errors.nome" class="invalid-feedback">
            <i class="fas fa-exclamation-circle"></i>
            {{ errors.nome }}
          </div>
        </div>

        <div class="form-group">
          <label for="descricao" class="form-label">
            Descrição
            <span class="label-helper">Informações adicionais sobre a unidade</span>
          </label>
          <textarea id="descricao" v-model="form.descricao" class="form-control" rows="3"
            placeholder="Descreva a unidade, atividades principais, etc."></textarea>
          <div class="form-help">Máximo de 500 caracteres</div>
        </div>

        <div class="form-group">
          <label for="endereco" class="form-label">
            Endereço Completo *
            <span class="label-helper">Rua, número, bairro, cidade, estado</span>
          </label>
          <textarea id="endereco" v-model="form.endereco" :class="{ 'is-invalid': errors.endereco }"
            class="form-control" rows="2" placeholder="Ex: Rua Exemplo, 123 - Centro - Cidade/Estado"
            @blur="validateField('endereco')"></textarea>
          <div v-if="errors.endereco" class="invalid-feedback">
            <i class="fas fa-exclamation-circle"></i>
            {{ errors.endereco }}
          </div>
        </div>
      </div>

      <!-- Meta Financeira -->
      <div class="form-section">
        <h4 class="section-title">
          <i class="fas fa-chart-line"></i>
          Meta Financeira
        </h4>

        <div class="form-group">
          <label for="metaMensal" class="form-label">
            Meta Mensal (R$) *
            <span class="label-helper">Valor esperado de faturamento mensal</span>
          </label>
          <div class="input-group">
            <span class="input-group-text">R$</span>
            <input id="metaMensal" v-model.number="form.metaMensal" type="number" step="0.01" min="0"
              :class="{ 'is-invalid': errors.metaMensal }" class="form-control" placeholder="0.00"
              @blur="validateField('metaMensal')" />
          </div>
          <div v-if="errors.metaMensal" class="invalid-feedback">
            <i class="fas fa-exclamation-circle"></i>
            {{ errors.metaMensal }}
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">
            Período de Operação
            <span class="label-helper">Data de início e término do contrato</span>
          </label>
          <div class="date-grid">
            <div class="date-group">
              <label for="dataInicio" class="date-label">Data de Início *</label>
              <input id="dataInicio" v-model="form.dataInicio" type="date" :class="{ 'is-invalid': errors.dataInicio }"
                class="form-control" @change="validateDates" />
            </div>
            <div class="date-group">
              <label for="dataFim" class="date-label">Data de Término</label>
              <input id="dataFim" v-model="form.dataFim" type="date" :class="{ 'is-invalid': errors.dataFim }"
                class="form-control" :min="form.dataInicio" @change="validateDates" />
            </div>
          </div>
          <div v-if="errors.dataFim" class="invalid-feedback">
            <i class="fas fa-exclamation-circle"></i>
            {{ errors.dataFim }}
          </div>
        </div>
      </div>

      <!-- Status e Configurações -->
      <div class="form-section">
        <h4 class="section-title">
          <i class="fas fa-cog"></i>
          Configurações
        </h4>

        <div class="form-group">
          <label class="form-label">Status da Unidade</label>
          <div class="status-options">
            <label class="status-option">
              <input type="radio" v-model="form.isAtivo" :value="true" class="status-radio" />
              <div class="status-card" :class="{ active: form.isAtivo }">
                <div class="status-icon active">
                  <i class="fas fa-check-circle"></i>
                </div>
                <div class="status-info">
                  <div class="status-title">Ativa</div>
                  <div class="status-description">Unidade em operação normal</div>
                </div>
              </div>
            </label>

            <label class="status-option">
              <input type="radio" v-model="form.isAtivo" :value="false" class="status-radio" />
              <div class="status-card" :class="{ inactive: !form.isAtivo }">
                <div class="status-icon inactive">
                  <i class="fas fa-times-circle"></i>
                </div>
                <div class="status-info">
                  <div class="status-title">Inativa</div>
                  <div class="status-description">Unidade temporariamente parada</div>
                </div>
              </div>
            </label>
          </div>
        </div>

        <!-- Campos Adicionais (pode expandir) -->
        <div class="form-group">
          <label class="form-label">
            <i class="fas fa-plus-circle"></i>
            Campos Adicionais
            <button type="button" class="btn-toggle-fields" @click="toggleAdditionalFields">
              {{ showAdditionalFields ? 'Ocultar' : 'Mostrar' }}
              <i :class="showAdditionalFields ? 'fas fa-chevron-up' : 'fas fa-chevron-down'"></i>
            </button>
          </label>

          <div v-if="showAdditionalFields" class="additional-fields">
            <!-- Adicione mais campos aqui conforme necessário -->
            <div class="form-group-sm">
              <label for="telefone" class="form-label-sm">Telefone</label>
              <input id="telefone" v-model="form.telefone" type="tel" class="form-control-sm"
                placeholder="(00) 00000-0000" />
            </div>

            <div class="form-group-sm">
              <label for="email" class="form-label-sm">Email</label>
              <input id="email" v-model="form.email" type="email" class="form-control-sm"
                placeholder="unidade@empresa.com" />
            </div>
          </div>
        </div>
      </div>

      <!-- Resumo -->
      <div class="form-section summary-section">
        <h4 class="section-title">
          <i class="fas fa-clipboard-check"></i>
          Resumo
        </h4>

        <div class="summary-card">
          <div class="summary-item">
            <span class="summary-label">Nome:</span>
            <span class="summary-value">{{ form.nome || 'Não informado' }}</span>
          </div>

          <div class="summary-item">
            <span class="summary-label">Meta Mensal:</span>
            <span class="summary-value">{{ formatCurrency(form.metaMensal) }}</span>
          </div>

          <div class="summary-item">
            <span class="summary-label">Status:</span>
            <span class="summary-value">
              <span :class="form.isAtivo ? 'badge-success' : 'badge-danger'">
                {{ form.isAtivo ? 'Ativa' : 'Inativa' }}
              </span>
            </span>
          </div>

          <div class="summary-item">
            <span class="summary-label">Período:</span>
            <span class="summary-value">
              {{ form.dataInicio ? formatDate(form.dataInicio) : 'Não definido' }}
              {{ form.dataFim ? `até ${formatDate(form.dataFim)}` : '' }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer do Formulário -->
    <div class="form-footer">
      <div class="form-notes">
        <p><i class="fas fa-info-circle"></i> Campos marcados com * são obrigatórios</p>
        <p><i class="fas fa-save"></i> Todas as alterações são salvas automaticamente no histórico</p>
      </div>

      <div class="form-actions">
        <button type="button" class="btn btn-outline-secondary" @click="resetForm" :disabled="isLoading">
          <i class="fas fa-redo"></i>
          Limpar
        </button>
        <button type="submit" class="btn btn-primary" :disabled="isLoading || !isFormValid">
          <i class="fas fa-save"></i>
          {{ submitText }}
        </button>
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue';
import { useUnidadeForm } from '@/composables/unidades/useUnidadeForm';

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

// Formatters (simplificados para o exemplo)
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

// Watch for form changes to auto-save draft
let saveTimeout;
watch(form, () => {
  clearTimeout(saveTimeout);
  saveTimeout = setTimeout(() => {
    // Auto-save logic here
    console.log('Draft saved:', form);
  }, 2000);
}, { deep: true });

// No final do seu UnidadeForm.vue, dentro da tag <script setup>
// Adicione temporariamente:
import { useUnidadesActions } from '@/composables/unidades/useUnidadesActions';

const unidadesActions = useUnidadesActions();

// Teste direto - execute no console do navegador
const testarCriacaoDireta = async () => {
  console.log('🧪 TESTE DIRETO: Iniciando...');

  const dadosTeste = {
    nome: 'Loja Teste ' + new Date().getTime(),
    metaMensal: 10000,
    dataInicio: '2024-02-07',
    isAtivo: true,
    descricao: 'Loja de teste criada via console',
    endereco: 'Rua Teste, 123 - Centro'
  };

  console.log('📤 Dados de teste:', dadosTeste);

  try {
    const resultado = await unidadesActions.createUnidade(dadosTeste);
    console.log('✅ TESTE BEM SUCEDIDO:', resultado);
  } catch (error) {
    console.error('❌ TESTE FALHOU:', error);
  }
};

// Exporte a função para testar no console
window.testarCriacaoDireta = testarCriacaoDireta;
console.log('🧪 Para testar diretamente, execute: testarCriacaoDireta()');

</script>

<style scoped>
.unidade-form {
  background: white;
  border-radius: 12px;
  overflow: hidden;
}

.form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px;
  border-bottom: 1px solid var(--unidade-border);
  background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
}

.form-header h3 {
  margin: 0;
  font-size: 20px;
  color: #1a202c;
  font-weight: 700;
}

.form-actions {
  display: flex;
  gap: 12px;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 32px;
  padding: 32px;
}

@media (min-width: 1024px) {
  .form-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .summary-section {
    grid-column: 1 / -1;
  }
}

.form-section {
  animation: slideIn 0.3s ease;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #2d3748;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 2px solid var(--unidade-border);
  display: flex;
  align-items: center;
  gap: 10px;
}

.section-title i {
  color: var(--unidade-color-primary);
  font-size: 14px;
}

.form-group {
  margin-bottom: 24px;
}

.form-label {
  display: block;
  font-weight: 600;
  color: #4a5568;
  margin-bottom: 8px;
  font-size: 14px;
}

.label-helper {
  display: block;
  font-weight: 400;
  color: #718096;
  font-size: 12px;
  margin-top: 2px;
}

.form-control {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid var(--unidade-border);
  border-radius: 8px;
  font-size: 14px;
  color: #1a202c;
  transition: all 0.3s;
  background: white;
}

.form-control:focus {
  outline: none;
  border-color: var(--unidade-color-primary);
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-control.is-invalid {
  border-color: #ef4444;
  background: #fef2f2;
}

.form-control.is-invalid:focus {
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
}

.invalid-feedback {
  color: #ef4444;
  font-size: 12px;
  margin-top: 6px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.form-help {
  font-size: 12px;
  color: #718096;
  margin-top: 6px;
}

.input-group {
  display: flex;
}

.input-group-text {
  padding: 12px 16px;
  background: #f8fafc;
  border: 2px solid var(--unidade-border);
  border-right: none;
  border-radius: 8px 0 0 8px;
  color: #718096;
  font-size: 14px;
}

.input-group .form-control {
  border-radius: 0 8px 8px 0;
}

.date-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.date-group {
  display: flex;
  flex-direction: column;
}

.date-label {
  font-size: 12px;
  color: #718096;
  margin-bottom: 6px;
}

.status-options {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.status-option {
  cursor: pointer;
}

.status-radio {
  display: none;
}

.status-card {
  padding: 16px;
  border: 2px solid var(--unidade-border);
  border-radius: 8px;
  transition: all 0.3s;
  background: white;
}

.status-card.active {
  border-color: var(--unidade-color-primary);
  background: rgba(102, 126, 234, 0.05);
}

.status-card.inactive {
  border-color: #e5e7eb;
  background: #f9fafb;
}

.status-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--unidade-shadow-sm);
}

.status-icon {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  margin-bottom: 12px;
}

.status-icon.active {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.status-icon.inactive {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.status-info {
  text-align: center;
}

.status-title {
  font-weight: 600;
  color: #1a202c;
  margin-bottom: 4px;
}

.status-description {
  font-size: 12px;
  color: #718096;
  line-height: 1.4;
}

.btn-toggle-fields {
  background: none;
  border: none;
  color: var(--unidade-color-primary);
  font-size: 12px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  margin-left: 8px;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.3s;
}

.btn-toggle-fields:hover {
  background: rgba(102, 126, 234, 0.1);
}

.additional-fields {
  margin-top: 16px;
  padding: 16px;
  background: #f8fafc;
  border-radius: 8px;
  border: 1px solid var(--unidade-border);
  display: grid;
  gap: 16px;
}

.form-group-sm {
  margin-bottom: 0;
}

.form-label-sm {
  display: block;
  font-size: 12px;
  color: #4a5568;
  margin-bottom: 6px;
  font-weight: 600;
}

.form-control-sm {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--unidade-border);
  border-radius: 6px;
  font-size: 13px;
  color: #1a202c;
  transition: all 0.3s;
}

.form-control-sm:focus {
  outline: none;
  border-color: var(--unidade-color-primary);
}

.summary-card {
  background: #f8fafc;
  border: 1px solid var(--unidade-border);
  border-radius: 8px;
  padding: 20px;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #e2e8f0;
}

.summary-item:last-child {
  border-bottom: none;
}

.summary-label {
  font-weight: 600;
  color: #4a5568;
  font-size: 14px;
}

.summary-value {
  color: #1a202c;
  font-size: 14px;
  text-align: right;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
}

.badge-success,
.badge-danger {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.badge-success {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.badge-danger {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.form-footer {
  padding: 24px;
  border-top: 1px solid var(--unidade-border);
  background: #f8fafc;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.form-notes {
  flex: 1;
}

.form-notes p {
  margin: 0;
  font-size: 12px;
  color: #718096;
  display: flex;
  align-items: center;
  gap: 6px;
}

.form-notes p i {
  font-size: 10px;
}

.alert {
  margin: 20px 32px 0;
  padding: 16px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 10px;
  animation: slideIn 0.3s ease;
}

.alert-danger {
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
}

@media (max-width: 768px) {
  .form-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .form-actions {
    width: 100%;
  }

  .form-grid {
    padding: 20px;
    gap: 24px;
  }

  .date-grid {
    grid-template-columns: 1fr;
  }

  .status-options {
    grid-template-columns: 1fr;
  }

  .form-footer {
    flex-direction: column;
    gap: 16px;
    text-align: center;
  }
}
</style>