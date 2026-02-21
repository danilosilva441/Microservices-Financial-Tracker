<!-- /*
 * src/components/analysis/charts/BarChart.vue
 *
 * A reusable Vue component for rendering various types of charts (line, bar, doughnut, pie) using Chart.js.
 * It includes features like responsive design, theme-aware styling, and export functionality.
 *
 * Props:
 * - type: The type of chart to render (line, bar, doughnut, pie).
 * - data: The data object for the chart (labels and datasets).
 * - options: Custom Chart.js options to override defaults.
 * - height: The height of the chart container (default: '300px').
 * - loading: A boolean to indicate if the chart is in a loading state (default: false).
 * - filename: The default filename for exported charts (default: 'chart').
 *
 * Emits:
 * - export: Emitted when the user clicks the export button, passing the exported image data.
 *
 * Usage:
 * <BarChart
 *   type="bar"
 *   :data="chartData"
 *   :options="chartOptions"
 *   :loading="isLoading"
 *   filename="my-chart"
 *   @export="handleExport"
 * />
 */ -->
<template>
  <div class="relative">
    <div class="absolute z-10 flex gap-2 top-2 right-2">
      <ChartExportButton
        :chart-id="chartId"
        :filename="filename"
        @export="$emit('export', $event)"
      />
    </div>

    <div
      :id="chartId"
      class="w-full"
      :style="{ height: height }"
    ></div>

    <div v-if="loading" class="absolute inset-0 flex items-center justify-center bg-white/80 dark:bg-gray-900/80">
      <LoadingSpinner />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue'
import Chart from 'chart.js/auto'
import LoadingSpinner from '../shared/LoadingSpinner.vue'
import ChartExportButton from './ChartExportButton.vue'

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: (value) => ['line', 'bar', 'doughnut', 'pie'].includes(value)
  },
  data: {
    type: Object,
    required: true
  },
  options: {
    type: Object,
    default: () => ({})
  },
  height: {
    type: String,
    default: '300px'
  },
  loading: {
    type: Boolean,
    default: false
  },
  filename: {
    type: String,
    default: 'chart'
  }
})

defineEmits(['export'])

const chartId = `chart-${Math.random().toString(36).substr(2, 9)}`
let chartInstance = null

const defaultOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom',
      labels: {
        usePointStyle: true,
        boxWidth: 8,
        padding: 20,
        color: document.documentElement.classList.contains('dark') ? '#fff' : '#374151'
      }
    },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: document.documentElement.classList.contains('dark') ? '#1f2937' : '#fff',
      titleColor: document.documentElement.classList.contains('dark') ? '#fff' : '#111827',
      bodyColor: document.documentElement.classList.contains('dark') ? '#d1d5db' : '#4b5563',
      borderColor: document.documentElement.classList.contains('dark') ? '#374151' : '#e5e7eb',
      borderWidth: 1
    }
  }
}

const initChart = () => {
  const canvas = document.getElementById(chartId)
  if (!canvas) return

  if (chartInstance) {
    chartInstance.destroy()
  }

  const ctx = canvas.getContext('2d')
  chartInstance = new Chart(ctx, {
    type: props.type,
    data: props.data,
    options: { ...defaultOptions, ...props.options }
  })
}

const updateChartTheme = () => {
  if (chartInstance) {
    const isDark = document.documentElement.classList.contains('dark')
    chartInstance.options.plugins.legend.labels.color = isDark ? '#fff' : '#374151'
    chartInstance.options.plugins.tooltip.backgroundColor = isDark ? '#1f2937' : '#fff'
    chartInstance.options.plugins.tooltip.titleColor = isDark ? '#fff' : '#111827'
    chartInstance.options.plugins.tooltip.bodyColor = isDark ? '#d1d5db' : '#4b5563'
    chartInstance.options.plugins.tooltip.borderColor = isDark ? '#374151' : '#e5e7eb'
    chartInstance.update()
  }
}

onMounted(() => {
  initChart()
  
  // Observar mudanças no tema
  const observer = new MutationObserver(updateChartTheme)
  observer.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class']
  })
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})

watch(() => props.data, () => {
  initChart()
}, { deep: true })

watch(() => props.options, () => {
  initChart()
}, { deep: true })
</script>