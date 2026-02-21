<!-- /*
 * src/components/analysis/charts/LineChart.vue
 * LineChart.vue
 *
 * A reusable Vue component for rendering a line chart using Chart.js.
 * It includes features like responsive design, theme-aware styling, and export functionality.
 */ -->
<template>
  <BaseChart
    type="line"
    :data="chartData"
    :options="chartOptions"
    :height="height"
    :loading="loading"
    :filename="filename"
    @export="$emit('export', $event)"
  />
</template>

<script setup>
import { computed } from 'vue'
import BaseChart from './BaseChart.vue'

const props = defineProps({
  data: {
    type: Object,
    required: true
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
    default: 'line-chart'
  },
  showMarkers: {
    type: Boolean,
    default: true
  },
  smooth: {
    type: Boolean,
    default: true
  }
})

defineEmits(['export'])

const chartData = computed(() => ({
  labels: props.data.labels || [],
  datasets: props.data.datasets?.map(dataset => ({
    ...dataset,
    tension: props.smooth ? 0.4 : 0,
    pointRadius: props.showMarkers ? 3 : 0,
    pointHoverRadius: 5,
    fill: dataset.fill || false
  })) || []
}))

const chartOptions = {
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        color: (context) => {
          return document.documentElement.classList.contains('dark')
            ? 'rgba(255, 255, 255, 0.1)'
            : 'rgba(0, 0, 0, 0.1)'
        }
      },
      ticks: {
        callback: (value) => {
          return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            minimumFractionDigits: 0
          }).format(value)
        }
      }
    }
  },
  elements: {
    line: {
      borderWidth: 2
    }
  }
}
</script>