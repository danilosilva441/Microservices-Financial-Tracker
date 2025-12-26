<script setup>
defineProps({
  count: { type: Number, required: true },
  isAdmin: { type: Boolean, required: true }
});

defineEmits(['new']);
</script>

<template>
  <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 sm:mb-8 gap-4">
    <div>
      <h1 class="text-2xl sm:text-3xl font-bold text-slate-800 dark:text-white">Minhas Unidades</h1>
      <p class="text-slate-600 dark:text-slate-400 text-sm sm:text-base mt-1">
        {{ count }} unidade{{ count !== 1 ? 's' : '' }} cadastrada{{ count !== 1 ? 's' : '' }}
      </p>
    </div>
    
    <button 
      v-if="isAdmin" 
      @click="$emit('new')"
      class="flex items-center justify-center bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-4 sm:py-2 sm:px-4 rounded-lg transition-colors duration-200 w-full sm:w-auto shadow-sm"
    >
      <span class="mr-2 text-xl">+</span>
      Nova Unidade
    </button>
  </div>
</template>