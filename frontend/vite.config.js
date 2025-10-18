import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:8080',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  },
  // Configuração específica para build
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    // Define variáveis de ambiente para o build
    rollupOptions: {
      external: [], // Remove qualquer externalização que possa causar problemas
    }
  },
  // Define variáveis de ambiente
  define: {
    // Define um placeholder para o baseURL que será substituído em runtime
    '__API_BASE_URL__': JSON.stringify(process.env.VITE_API_URL || '')
  }
})