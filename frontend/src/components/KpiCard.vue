<script setup>
defineProps({
  titulo: String,
  valor: String,
  subtitulo: String,
  cor: {
    type: String,
    validator: (value) => ['green', 'blue', 'orange', 'purple', 'red', 'yellow'].includes(value)
  },
  icone: {
    type: String,
    validator: (value) => ['check', 'currency', 'clock', 'document'].includes(value)
  }
});

const cores = {
  green: { bg: 'bg-green-100', text: 'text-green-600', border: 'border-green-500' },
  blue: { bg: 'bg-blue-100', text: 'text-blue-600', border: 'border-blue-500' },
  orange: { bg: 'bg-orange-100', text: 'text-orange-600', border: 'border-orange-500' },
  purple: { bg: 'bg-purple-100', text: 'text-purple-600', border: 'border-purple-500' },
  red: { bg: 'bg-red-100', text: 'text-red-600', border: 'border-red-500' },
  yellow: { bg: 'bg-yellow-100', text: 'text-yellow-600', border: 'border-yellow-500' }
};

const icones = {
  check: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
  currency: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1',
  clock: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
  document: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
};
</script>

<template>
  <div class="bg-white rounded-lg p-4 border-l-4 shadow-sm" :class="cores[cor].border">
    <div class="flex items-center justify-between">
      <div class="flex-1 min-w-0">
        <h3 class="text-sm text-gray-500 mb-1 font-medium">{{ titulo }}</h3>
        <p class="text-xl font-bold mb-1 truncate" :class="cores[cor].text">{{ valor }}</p>
        <p class="text-xs text-gray-500 truncate">{{ subtitulo }}</p>
      </div>
      <div class="w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0 ml-2" :class="cores[cor].bg">
        <svg class="w-5 h-5" :class="cores[cor].text" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="icones[icone]"/>
        </svg>
      </div>
    </div>
  </div>
</template>