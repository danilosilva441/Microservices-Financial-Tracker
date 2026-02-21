<!-- /
* src/components/analysis/charts/DoughnutChart.vue
 * DoughnutChart.vue
 *
 * A reusable Vue component for rendering a doughnut chart using Chart.js.
 * It includes features like responsive design, theme-aware styling, and export functionality.
 */ -->
<template>
  <BaseChart
    type="doughnut"
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
    default: 'doughnut-chart'
  },
  cutout: {
    type: String,
    default: '60%'
  }
})

defineEmits(['export'])

const chartData = computed(() => ({
  labels: props.data.labels || [],
  datasets: props.data.datasets?.map(dataset => ({
    ...dataset,
    borderWidth: 0
  })) || []
}))

const chartOptions = {
  cutout: props.cutout,
  plugins: {
    tooltip: {
      callbacks: {
        label: (context) => {
          const label = context.label || ''
          const value = context.raw || 0
          const total = context.dataset.data.reduce((a, b) => a + b, 0)
          const percentage = ((value / total) * 100).toFixed(1)
          return `${label}: ${new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          }).format(value)} (${percentage}%)`
        }
      }
    }
  }
}
</script>