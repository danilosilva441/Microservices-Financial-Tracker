import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import svgLoader from 'vite-svg-loader'

export default defineConfig({
  plugins: [
    vue({
      template: {
        compilerOptions: {
          isCustomElement: (tag) => tag.startsWith('icon-')
        }
      }
    }),
    vueDevTools(),
    svgLoader({
      defaultImport: 'component',
    }),
  ],

  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  
  // 👇 CONFIGURAÇÃO DO PROXY PARA EVITAR CORS 👇
  server: {
    proxy: {
      // Intercepta tudo que for para o /health
      '/health': {
        target: 'http://localhost:8080',
        changeOrigin: true
      },
      // Intercepta todas as requisições normais da API
      '/api': {
        target: 'http://localhost:8080',
        changeOrigin: true,
        // Se o seu backend não usa o prefixo /api na URL real dele, 
        // a linha abaixo remove o /api antes de entregar pro backend:
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
})