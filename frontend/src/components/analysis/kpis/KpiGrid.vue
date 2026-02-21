<!--
 * src/components/analysis/kpis/KpiGrid.vue
 * KpiGrid.vue
 *
 * A Vue component that displays a grid of KPI (Key Performance Indicator) cards in the financial analysis dashboard.
 * It receives an array of KPI data as props and renders a responsive grid layout using Tailwind CSS.
 * Each KPI is displayed using the KpiCard component, which shows the KPI's value, trend, and comparison information.
 -->
<template>
  <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
    <KpiCard
      v-for="kpi in kpis"
      :key="kpi.id"
      v-bind="kpi"
    />
  </div>
</template>

<script setup>
import KpiCard from './KpiCard.vue'

defineProps({
  kpis: {
    type: Array,
    required: true
  }
})
</script>