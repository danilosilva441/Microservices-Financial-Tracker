<!-- src/views/NotFoundView.vue -->
<template>
  <div class="not-found-view">
    <div class="not-found-content">
      <h1>404</h1>
      <h2>Página não encontrada</h2>
      <p>A página que você está procurando não existe ou foi movida.</p>
      <router-link to="/" class="btn-home">
        <i class="fas fa-home"></i>
        Voltar para Home
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.not-found-view {
  min-height: 60vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.not-found-content {
  text-align: center;
  max-width: 500px;
}

.not-found-content h1 {
  font-size: 120px;
  color: #667eea;
  margin: 0;
  line-height: 1;
}

.not-found-content h2 {
  font-size: 32px;
  color: #333;
  margin: 20px 0;
}

.not-found-content p {
  color: #666;
  margin-bottom: 40px;
  font-size: 18px;
}

.btn-home {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 15px 30px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-decoration: none;
  border-radius: 8px;
  font-weight: 500;
  transition: all 0.3s;
}

.btn-home:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
}
</style>