<!-- src/views/DashboardView.vue -->
<template>
  <div class="dashboard-view">
    <div class="dashboard-header">
      <h1>Dashboard</h1>
      <p>Bem-vindo, {{ User?.name || 'Usuário' }}!</p>
    </div>
    
    <div class="dashboard-grid">
      <div class="card">
        <div class="card-header">
          <h3>Visão Geral</h3>
        </div>
        <div class="card-body">
          <p>Aqui ficará o dashboard principal.</p>
        </div>
      </div>
      
      <div class="card">
        <div class="card-header">
          <h3>Estatísticas</h3>
        </div>
        <div class="card-body">
          <p>Em desenvolvimento...</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'DashboardView',
  
  setup() {
    const { userData } = useAuth()
    
    return {
      User: userData
    }
  }
}
</script>

<style scoped>
.dashboard-view {
  padding: 30px;
}

.dashboard-header {
  margin-bottom: 40px;
}

.dashboard-header h1 {
  font-size: 36px;
  color: #333;
  margin-bottom: 10px;
}

.dashboard-header p {
  color: #666;
  font-size: 18px;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 30px;
}

.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.card-header {
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.card-header h3 {
  margin: 0;
  font-size: 18px;
}

.card-body {
  padding: 20px;
}

.card-body p {
  color: #666;
  margin: 0;
}
</style>