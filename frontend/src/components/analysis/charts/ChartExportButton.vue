<!-- /* src/components/analysis/charts/ChartExportButton.vue
 * ChartExportButton.vue
 *
 * A Vue component that provides a dropdown menu for exporting charts in various formats (PNG, JPEG, PDF, CSV).
 * It uses the v-click-outside directive to close the menu when clicking outside of it.
 */ -->
<template>
  <div class="relative">
    <button
      @click="isOpen = !isOpen"
      class="text-gray-700 bg-white action-button dark:bg-gray-800 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
      title="Exportar gráfico"
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
      </svg>
    </button>

    <div
      v-if="isOpen"
      v-click-outside="() => isOpen = false"
      class="absolute right-0 z-50 w-48 mt-2 bg-white border border-gray-200 rounded-lg shadow-xl dark:bg-gray-800 dark:border-gray-700"
    >
      <div class="py-1">
        <button
          @click="exportAs('png')"
          class="flex items-center w-full gap-2 px-4 py-2 text-sm text-left text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          PNG (Imagem)
        </button>
        <button
          @click="exportAs('jpg')"
          class="flex items-center w-full gap-2 px-4 py-2 text-sm text-left text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          JPEG (Imagem)
        </button>
        <button
          @click="exportAs('pdf')"
          class="flex items-center w-full gap-2 px-4 py-2 text-sm text-left text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
          </svg>
          PDF (Documento)
        </button>
        <div class="my-1 border-t border-gray-200 dark:border-gray-700"></div>
        <button
          @click="exportAs('csv')"
          class="flex items-center w-full gap-2 px-4 py-2 text-sm text-left text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M3 14h18m-9-4v8m-7 0h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
          </svg>
          CSV (Dados)
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { vClickOutside } from '@/directives/clickOutside'

const props = defineProps({
  chartId: {
    type: String,
    required: true
  },
  filename: {
    type: String,
    default: 'chart'
  }
})

const emit = defineEmits(['export'])

const isOpen = ref(false)

const exportAs = async (format) => {
  isOpen.value = false
  
  const canvas = document.querySelector(`#${props.chartId} canvas`)
  if (!canvas) return

  if (format === 'png' || format === 'jpg') {
    const link = document.createElement('a')
    link.download = `${props.filename}.${format}`
    link.href = canvas.toDataURL(`image/${format === 'jpg' ? 'jpeg' : 'png'}`)
    link.click()
  } else if (format === 'pdf') {
    emit('export', { format, canvas })
  } else if (format === 'csv') {
    emit('export', { format, data: canvas.chart?.data })
  }
}
</script>