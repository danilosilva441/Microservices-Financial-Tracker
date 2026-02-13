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

    // (opcional) pode deixar, mas em build de produção não faz muita falta
    vueDevTools(),

    // ✅ agora existe de verdade
    svgLoader({
      defaultImport: 'component', // garante import como componente
    }),
  ],

  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
})
