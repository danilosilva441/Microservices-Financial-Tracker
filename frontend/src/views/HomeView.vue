<!-- src/views/HomeView.vue -->
<template>
  <div class="home-view">
    <div class="hero">
      <h1>Bem-vindo ao Sistema</h1>
      <p>Gerencie seu portfolio de projetos de forma eficiente</p>
      <div class="hero-actions">
        <router-link v-if="!isAuthenticated" to="/login" class="btn-primary">
          Começar
        </router-link>
        <router-link v-else to="/dashboard" class="btn-primary">
          Ir para Dashboard
        </router-link>
        <router-link to="/about" class="btn-secondary">
          Saiba mais
        </router-link>
      </div>
    </div>
    
    <div class="features">
      <div class="feature-card">
        <i class="fas fa-chart-line"></i>
        <h3>Dashboard</h3>
        <p>Visualize métricas e desempenho dos projetos</p>
      </div>
      <div class="feature-card">
        <i class="fas fa-briefcase"></i>
        <h3>Projetos</h3>
        <p>Gerencie todos os seus projetos em um só lugar</p>
      </div>
      <div class="feature-card">
        <i class="fas fa-users"></i>
        <h3>Equipe</h3>
        <p>Colabore com sua equipe de forma eficiente</p>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuth } from '@/composables/auth/useAuth'

export default {
  name: 'HomeView',
  
  setup() {
    const { isAuthenticated } = useAuth()
    
    return {
      isAuthenticated
    }
  }
}
</script>

<style scoped>
.home-view {
  padding: 60px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.hero {
  text-align: center;
  padding: 80px 0;
}

.hero h1 {
  font-size: 48px;
  margin-bottom: 20px;
  color: #333;
}

.hero p {
  font-size: 20px;
  color: #666;
  margin-bottom: 40px;
}

.hero-actions {
  display: flex;
  gap: 20px;
  justify-content: center;
}

.btn-primary, .btn-secondary {
  padding: 15px 30px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.3s;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
}

.btn-secondary {
  background: white;
  color: #667eea;
  border: 2px solid #667eea;
}

.btn-secondary:hover {
  background: rgba(102, 126, 234, 0.1);
}

.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 40px;
  margin-top: 80px;
}

.feature-card {
  text-align: center;
  padding: 40px 30px;
  border-radius: 16px;
  background: white;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s;
}

.feature-card:hover {
  transform: translateY(-10px);
}

.feature-card i {
  font-size: 48px;
  color: #667eea;
  margin-bottom: 20px;
}

.feature-card h3 {
  margin-bottom: 15px;
  color: #333;
}

.feature-card p {
  color: #666;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .hero h1 {
    font-size: 36px;
  }
  
  .hero-actions {
    flex-direction: column;
    align-items: center;
  }
  
  .btn-primary, .btn-secondary {
    width: 100%;
    max-width: 300px;
    text-align: center;
  }
}
</style>