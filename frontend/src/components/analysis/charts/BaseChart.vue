<!-- /*
 * src/components/analysis/charts/BarChart.vue
 * BarChart.vue
 *
 * A reusable Vue component for rendering various types of charts (line, bar, doughnut, pie) using Chart.js.
 * It includes features like responsive design, theme-aware styling, and export functionality.
 */ -->
<template>
  <BaseChart
    type="bar"
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
    default: 'bar-chart'
  },
  horizontal: {
    type: Boolean,
    default: false
  },
  stacked: {
    type: Boolean,
    default: false
  }
})

defineEmits(['export'])

const chartData = computed(() => ({
  labels: props.data.labels || [],
  datasets: props.data.datasets?.map(dataset => ({
    ...dataset,
    borderRadius: 4,
    barPercentage: 0.8,
    categoryPercentage: 0.9
  })) || []
}))

const chartOptions = {
  indexAxis: props.horizontal ? 'y' : 'x',
  scales: {
    x: {
      stacked: props.stacked,
      grid: {
        display: !props.horizontal
      }
    },
    y: {
      stacked: props.stacked,
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
          if (props.horizontal) {
            return value
          }
          return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            minimumFractionDigits: 0
          }).format(value)
        }
      }
    }
  }
}
</script>