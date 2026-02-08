<!-- components/unidades/UnidadeStats.vue -->
<template>
  <div class="unidade-stats">
    <div class="stats-grid">
      <!-- Total de Unidades -->
      <div class="stat-card">
        <div class="stat-header">
          <div class="stat-icon total">
            <i class="fas fa-store"></i>
          </div>
          <div class="stat-trend" :class="trendClass(stats.total)">
            <i :class="trendIcon(stats.total)"></i>
          </div>
        </div>
        <div class="stat-body">
          <div class="stat-value">{{ stats.total }}</div>
          <div class="stat-label">Total de Unidades</div>
        </div>
        <div class="stat-footer">
          <div class="stat-detail">
            <span class="detail-label">Ativas:</span>
            <span class="detail-value">{{ stats.ativas }}</span>
          </div>
        </div>
      </div>

      <!-- Faturamento Projetado -->
      <div class="stat-card">
        <div class="stat-header">
          <div class="stat-icon revenue">
            <i class="fas fa-chart-line"></i>
          </div>
          <div class="stat-trend" :class="trendClass(stats.faturamentoProjetado)">
            <i :class="trendIcon(stats.faturamentoProjetado)"></i>
          </div>
        </div>
        <div class="stat-body">
          <div class="stat-value">{{ formatCurrency(stats.faturamentoProjetado) }}</div>
          <div class="stat-label">Faturamento Projetado</div>
        </div>
        <div class="stat-footer">
          <div class="stat-detail">
            <span class="detail-label">Média:</span>
            <span class="detail-value">{{ formatCurrency(stats.mediaFaturamento) }}</span>
          </div>
        </div>
      </div>

      <!-- Vencimento Próximo -->
      <div class="stat-card">
        <div class="stat-header">
          <div class="stat-icon warning">
            <i class="fas fa-clock"></i>
          </div>
          <div class="stat-trend" :class="trendClass(stats.vencimentoProximo)">
            <i :class="trendIcon(stats.vencimentoProximo)"></i>
          </div>
        </div>
        <div class="stat-body">
          <div class="stat-value">{{ stats.vencimentoProximo }}</div>
          <div class="stat-label">Vencem em 30 dias</div>
        </div>
        <div class="stat-footer">
          <div class="stat-detail">
            <span class="detail-label">Atenção</span>
            <span class="detail-value" v-if="stats.vencimentoProximo > 0">⚠️</span>
            <span class="detail-value" v-else>✅</span>
          </div>
        </div>
      </div>

      <!-- Meta de Crescimento -->
      <div class="stat-card">
        <div class="stat-header">
          <div class="stat-icon growth">
            <i class="fas fa-trophy"></i>
          </div>
          <div class="stat-trend" :class="trendClass(growthRate)">
            <i :class="trendIcon(growthRate)"></i>
          </div>
        </div>
        <div class="stat-body">
          <div class="stat-value">{{ growthRate }}%</div>
          <div class="stat-label">Taxa de Crescimento</div>
        </div>
        <div class="stat-footer">
          <div class="stat-detail">
            <span class="detail-label">Meta:</span>
            <span class="detail-value">20%</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

const props = defineProps({
  stats: {
    type: Object,
    required: true,
    default: () => ({
      total: 0,
      ativas: 0,
      faturamentoProjetado: 0,
      mediaFaturamento: 0,
      vencimentoProximo: 0
    })
  },
  loading: {
    type: Boolean,
    default: false
  }
});

const { formatCurrency } = useUnidadesUI();

// Computed
const growthRate = computed(() => {
  // Cálculo fictício da taxa de crescimento
  return Math.min(Math.floor((props.stats.ativas / Math.max(props.stats.total, 1)) * 100), 100);
});

// Métodos
const trendClass = (value) => {
  if (value > 0) return 'trend-up';
  if (value < 0) return 'trend-down';
  return 'trend-neutral';
};

const trendIcon = (value) => {
  if (value > 0) return 'fas fa-arrow-up';
  if (value < 0) return 'fas fa-arrow-down';
  return 'fas fa-minus';
};
</script>

<style scoped>
.unidade-stats {
  margin-bottom: 30px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 20px;
}

@media (min-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

.stat-card {
  background: white;
  border-radius: 12px;
  border: 1px solid var(--unidade-border);
  box-shadow: var(--unidade-shadow-sm);
  padding: 24px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.stat-card:hover {
  box-shadow: var(--unidade-shadow-md);
  transform: translateY(-2px);
}

.stat-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
}

.stat-card:nth-child(1)::before {
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
}

.stat-card:nth-child(2)::before {
  background: linear-gradient(90deg, #10b981 0%, #34d399 100%);
}

.stat-card:nth-child(3)::before {
  background: linear-gradient(90deg, #f59e0b 0%, #fbbf24 100%);
}

.stat-card:nth-child(4)::before {
  background: linear-gradient(90deg, #8b5cf6 0%, #a78bfa 100%);
}

.stat-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  color: white;
}

.stat-icon.total {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.revenue {
  background: linear-gradient(135deg, #10b981 0%, #34d399 100%);
}

.stat-icon.warning {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
}

.stat-icon.growth {
  background: linear-gradient(135deg, #8b5cf6 0%, #a78bfa 100%);
}

.stat-trend {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  font-size: 12px;
}

.trend-up {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.trend-down {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.trend-neutral {
  background: rgba(148, 163, 184, 0.1);
  color: #94a3b8;
}

.stat-body {
  margin-bottom: 20px;
}

.stat-value {
  font-size: 32px;
  font-weight: 800;
  color: #1a202c;
  line-height: 1;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #718096;
  line-height: 1.4;
}

.stat-footer {
  padding-top: 16px;
  border-top: 1px solid var(--unidade-border);
}

.stat-detail {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.detail-label {
  font-size: 13px;
  color: #718096;
}

.detail-value {
  font-size: 14px;
  font-weight: 600;
  color: #1a202c;
}

@media (max-width: 768px) {
  .stat-card {
    padding: 20px;
  }
  
  .stat-value {
    font-size: 28px;
  }
  
  .stat-icon {
    width: 48px;
    height: 48px;
    font-size: 20px;
  }
}
</style>