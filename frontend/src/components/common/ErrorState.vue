<script setup>
defineProps({
  title: {
    type: String,
    default: 'Ops! Algo deu errado'
  },
  message: {
    type: String,
    default: 'Não foi possível carregar os dados. Verifique sua conexão.'
  }
});

defineEmits(['retry']);
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 px-4 text-center min-h-[200px]">
    <div class="bg-red-50 p-4 rounded-full mb-4">
      <svg 
        class="w-8 h-8 text-red-500" 
        fill="none" 
        stroke="currentColor" 
        viewBox="0 0 24 24"
      >
        <path 
          stroke-linecap="round" 
          stroke-linejoin="round" 
          stroke-width="2" 
          d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z"
        />
      </svg>
    </div>

    <h3 class="text-lg font-bold text-gray-900 mb-2">
      {{ title }}
    </h3>
    
    <p class="text-gray-500 max-w-md mb-6">
      {{ message }}
    </p>

    <button 
      @click="$emit('retry')"
      class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
    >
      <svg 
        class="w-4 h-4 mr-2" 
        fill="none" 
        stroke="currentColor" 
        viewBox="0 0 24 24"
      >
        <path 
          stroke-linecap="round" 
          stroke-linejoin="round" 
          stroke-width="2" 
          d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
        />
      </svg>
      Tentar Novamente
    </button>
  </div>
</template>