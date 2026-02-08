<!-- components/ui/AppLoading.vue -->
<template>
  <div class="app-loading">
    <div class="loading-content">
      <div class="logo-animation">
        <i class="fas fa-store"></i>
      </div>
      <div class="loading-text">
        <h3>DS SysTech</h3>
        <p>{{ message }}</p>
        <div class="progress-bar">
          <div class="progress" :style="{ width: progress + '%' }"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  message: {
    type: String,
    default: 'Carregando...'
  }
})

const progress = ref(0)

onMounted(() => {
  // Simula progresso de carregamento
  const interval = setInterval(() => {
    progress.value += 5
    if (progress.value >= 100) {
      clearInterval(interval)
    }
  }, 100)
})
</script>

<style scoped>
.app-loading {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  z-index: 9999;
}

.loading-content {
  text-align: center;
  padding: 40px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  animation: popIn 0.5s ease;
}

@keyframes popIn {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.logo-animation {
  font-size: 64px;
  color: #667eea;
  margin-bottom: 24px;
  animation: bounce 2s infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.loading-text h3 {
  font-size: 24px;
  color: #2d3748;
  margin-bottom: 8px;
}

.loading-text p {
  color: #718096;
  margin-bottom: 24px;
}

.progress-bar {
  height: 4px;
  background: #e2e8f0;
  border-radius: 2px;
  overflow: hidden;
}

.progress {
  height: 100%;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  border-radius: 2px;
  transition: width 0.3s ease;
}
</style>