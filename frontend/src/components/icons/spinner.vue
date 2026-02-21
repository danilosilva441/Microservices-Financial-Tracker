<template>
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="100%" height="100%"
    preserveAspectRatio="xMidYMid meet" fill="currentColor">
    <g class="spinner-ring">
      <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2.5" fill="none" stroke-dasharray="32" stroke-dashoffset="16" stroke-linecap="round" />
    </g>
  </svg>
</template>

<script>
export default {
  name: 'IconSpinner'
}
</script>

<style scoped>
.spinner-ring {
  transform-origin: center;
  animation: spin 1.2s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>