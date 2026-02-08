<!-- components/unidades/UnidadeCard.vue -->
<template>
  <div 
    class="unidade-card"
    :class="{ 'cursor-pointer': !disableClick }"
    @click="!disableClick ? $emit('click', unidade) : null"
  >
    <!-- Header do Card -->
    <div class="card-header">
      <div class="header-left">
        <div class="unidade-avatar">
          <i class="fas fa-store"></i>
        </div>
        <div class="unidade-info">
          <h3 class="unidade-name">{{ unidade.nome }}</h3>
          <div class="unidade-meta">
            <span class="meta-item">
              <i class="fas fa-map-marker-alt"></i>
              {{ truncatedAddress }}
            </span>
            <span class="meta-item">
              <i class="fas fa-calendar"></i>
              {{ formatDate(unidade.dataInicio) }}
            </span>
          </div>
        </div>
      </div>
      
      <div class="header-right">
        <div :class="statusBadgeClass">
          <i :class="statusIcon"></i>
          {{ statusText }}
        </div>
      </div>
    </div>

    <!-- Body do Card -->
    <div class="card-body">
      <!-- Progresso da Meta -->
      <div class="meta-section">
        <div class="meta-header">
          <span class="meta-label">Meta Mensal</span>
          <span class="meta-value">{{ formatCurrency(unidade.metaMensal) }}</span>
        </div>
        
        <div class="progress-container">
          <div class="progress-labels">
            <span>0%</span>
            <span>{{ progress }}%</span>
            <span>100%</span>
          </div>
          <div class="unidade-progress">
            <div 
              class="unidade-progress-bar"
              :style="progressBarStyle"
            ></div>
          </div>
        </div>
      </div>

      <!-- Estatísticas -->
      <div class="stats-grid">
        <div class="stat-item">
          <div class="stat-icon">
            <i class="fas fa-users"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ funcionariosAtivos || 0 }}</div>
            <div class="stat-label">Funcionários</div>
          </div>
        </div>
        
        <div class="stat-item">
          <div class="stat-icon">
            <i class="fas fa-chart-line"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ formatCurrency(faturamentoAtual) }}</div>
            <div class="stat-label">Faturamento</div>
          </div>
        </div>
        
        <div class="stat-item">
          <div class="stat-icon">
            <i class="fas fa-calendar-check"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ diasRestantes || '∞' }}</div>
            <div class="stat-label">Dias restantes</div>
          </div>
        </div>
      </div>

      <!-- Descrição -->
      <div v-if="unidade.descricao" class="description-section">
        <p class="description">{{ truncatedDescription }}</p>
      </div>
    </div>

    <!-- Footer do Card -->
    <div class="card-footer">
      <div class="footer-actions">
        <button 
          class="btn-action"
          @click.stop="$emit('edit', unidade.id)"
          title="Editar"
        >
          <i class="fas fa-edit"></i>
        </button>
        
        <button 
          v-if="unidade.isAtivo"
          class="btn-action"
          @click.stop="$emit('deactivate', unidade)"
          title="Desativar"
        >
          <i class="fas fa-power-off"></i>
        </button>
        
        <button 
          v-else
          class="btn-action"
          @click.stop="$emit('activate', unidade)"
          title="Ativar"
        >
          <i class="fas fa-play"></i>
        </button>
        
        <button 
          class="btn-action btn-danger"
          @click.stop="$emit('delete', unidade)"
          title="Excluir"
        >
          <i class="fas fa-trash"></i>
        </button>
      </div>
      
      <div class="footer-date">
        <small>Atualizado em {{ formatDate(unidade.updatedAt || unidade.createdAt) }}</small>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

const props = defineProps({
  unidade: {
    type: Object,
    required: true
  },
  ui: {
    type: Object,
    default: () => ({})
  },
  disableClick: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['click', 'edit', 'delete', 'deactivate', 'activate']);

const { formatCurrency, formatDate, getStatusBadge, getDaysToExpire } = useUnidadesUI();

// Computed properties
const statusBadge = computed(() => getStatusBadge(props.unidade.isAtivo));
const statusBadgeClass = computed(() => `unidade-status-badge ${statusBadge.value.variant === 'success' ? 'unidade-status-active' : 'unidade-status-inactive'}`);
const statusIcon = computed(() => `fas fa-${statusBadge.value.icon}`);
const statusText = computed(() => statusBadge.value.label);

const truncatedAddress = computed(() => {
  const addr = props.unidade.endereco || '';
  return addr.length > 40 ? addr.substring(0, 40) + '...' : addr;
});

const truncatedDescription = computed(() => {
  const desc = props.unidade.descricao || '';
  return desc.length > 100 ? desc.substring(0, 100) + '...' : desc;
});

const progress = computed(() => {
  // Aqui você pode calcular o progresso real baseado no faturamento atual vs meta
  // Por enquanto, vamos usar um valor fictício
  const faturamentoAtual = props.unidade.faturamentoAtual || 0;
  const meta = props.unidade.metaMensal || 0;
  if (meta <= 0) return 0;
  return Math.min(Math.round((faturamentoAtual / meta) * 100), 100);
});

const progressBarStyle = computed(() => {
  let backgroundColor = '#ef4444'; // Vermelho
  if (progress.value >= 75) {
    backgroundColor = '#10b981'; // Verde
  } else if (progress.value >= 50) {
    backgroundColor = '#f59e0b'; // Amarelo
  } else if (progress.value >= 25) {
    backgroundColor = '#f97316'; // Laranja
  }
  
  return {
    width: `${progress.value}%`,
    backgroundColor
  };
});

const funcionariosAtivos = computed(() => props.unidade.funcionariosAtivos || Math.floor(Math.random() * 20) + 5);
const faturamentoAtual = computed(() => props.unidade.faturamentoAtual || 0);
const diasRestantes = computed(() => {
  const dias = getDaysToExpire(props.unidade.dataFim);
  return dias !== null ? dias : '∞';
});
</script>

<style scoped>
.unidade-card {
  background: white;
  border-radius: 12px;
  border: 1px solid var(--unidade-border);
  box-shadow: var(--unidade-shadow-sm);
  transition: all 0.3s ease;
  overflow: hidden;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.unidade-card:hover {
  box-shadow: var(--unidade-shadow-md);
  transform: translateY(-2px);
}

.cursor-pointer {
  cursor: pointer;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 20px;
  border-bottom: 1px solid var(--unidade-border);
  background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
}

.header-left {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  flex: 1;
}

.unidade-avatar {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 24px;
  flex-shrink: 0;
}

.unidade-info {
  flex: 1;
}

.unidade-name {
  font-size: 18px;
  font-weight: 700;
  color: #1a202c;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.unidade-meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #718096;
}

.meta-item i {
  width: 14px;
  font-size: 12px;
}

.header-right {
  flex-shrink: 0;
}

.card-body {
  padding: 20px;
  flex: 1;
}

.meta-section {
  margin-bottom: 20px;
}

.meta-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.meta-label {
  font-size: 14px;
  color: #718096;
  font-weight: 500;
}

.meta-value {
  font-size: 16px;
  font-weight: 700;
  color: var(--unidade-color-primary);
}

.progress-container {
  margin-top: 8px;
}

.progress-labels {
  display: flex;
  justify-content: space-between;
  margin-bottom: 6px;
}

.progress-labels span {
  font-size: 12px;
  color: #a0aec0;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  margin: 20px 0;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  background: #f8fafc;
  border-radius: 8px;
  transition: all 0.3s;
}

.stat-item:hover {
  background: #edf2f7;
}

.stat-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  background: var(--unidade-color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: #1a202c;
  line-height: 1.2;
}

.stat-label {
  font-size: 12px;
  color: #718096;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.description-section {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid var(--unidade-border);
}

.description {
  font-size: 14px;
  color: #4a5568;
  line-height: 1.5;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-footer {
  padding: 16px 20px;
  border-top: 1px solid var(--unidade-border);
  background: #f8fafc;
}

.footer-actions {
  display: flex;
  gap: 8px;
  margin-bottom: 12px;
}

.btn-action {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: 1px solid var(--unidade-border);
  background: white;
  color: #718096;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-action:hover {
  background: #edf2f7;
  color: #4a5568;
  transform: translateY(-1px);
}

.btn-action.btn-danger:hover {
  background: #fee2e2;
  color: #dc2626;
  border-color: #fecaca;
}

.footer-date {
  text-align: center;
}

.footer-date small {
  font-size: 12px;
  color: #a0aec0;
}

@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .card-header {
    flex-direction: column;
    gap: 12px;
  }
  
  .header-left {
    width: 100%;
  }
  
  .header-right {
    width: 100%;
  }
}
</style>