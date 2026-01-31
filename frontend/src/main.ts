// src/main.js
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

// Importa estilos globais
import './assets/main.css'

// Cria a app
const app = createApp(App)

// Usa Pinia para gerenciamento de estado
const pinia = createPinia()
app.use(pinia)

// Usa router
app.use(router)

// Monta a app
app.mount('#app')

// Handler para erros globais
app.config.errorHandler = (err, vm, info) => {
  console.error('Erro Vue:', err)
  console.error('Componente:', vm)
  console.error('Info:', info)
}

// Handler para warnings
app.config.warnHandler = (msg, vm, trace) => {
  console.warn('Warning Vue:', msg)
  console.warn('Trace:', trace)
}