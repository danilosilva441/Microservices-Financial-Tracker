<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 transition-colors duration-200">
    <!-- Header da View -->
    <div class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 sticky top-0 z-10">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-gradient-to-br from-green-500 to-emerald-500 flex items-center justify-center text-white shadow-md">
              <IconPlusCircle class="w-5 h-5" />
            </div>
            <div>
              <h1 class="text-xl sm:text-2xl font-bold text-gray-900 dark:text-white">Nova Unidade</h1>
              <p class="text-xs sm:text-sm text-gray-600 dark:text-gray-400">Cadastre uma nova unidade no sistema</p>
            </div>
          </div>
          
          <!-- Breadcrumb -->
          <nav class="flex items-center space-x-1 text-sm" aria-label="Breadcrumb">
            <ol class="flex items-center space-x-1">
              <li>
                <router-link to="/" class="text-gray-500 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
                  <IconHome class="w-4 h-4" />
                  <span class="sr-only">Home</span>
                </router-link>
              </li>
              <li>
                <IconChevronRight class="w-3 h-3 text-gray-400 dark:text-gray-600" />
              </li>
              <li>
                <router-link to="/unidades" class="text-gray-500 dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 transition-colors">
                  Unidades
                </router-link>
              </li>
              <li>
                <IconChevronRight class="w-3 h-3 text-gray-400 dark:text-gray-600" />
              </li>
              <li>
                <span class="text-gray-900 dark:text-white font-medium">Nova</span>
              </li>
            </ol>
          </nav>
        </div>
      </div>
    </div>

    <!-- Conteúdo Principal -->
    <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <NovaUnidade />
    </div>
  </div>
</template>

<script setup>
import NovaUnidade from '@/components/unidades/NovaUnidade.vue';
import IconPlusCircle from '@/components/icons/plus-circle.vue';
import IconHome from '@/components/icons/home.vue';
import IconChevronRight from '@/components/icons/chevron-right.vue';

defineOptions({
  name: 'NovaUnidadeView'
});

// ✅ REMOVIDO: Não precisa mais do useTheme aqui
</script>

<style scoped>
@import '@/assets/default.css';

/* Animações de entrada */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.container > * {
  animation: fadeIn 0.3s ease-out;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100 dark:bg-gray-800;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-300 dark:bg-gray-600 rounded-full;
}

::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400 dark:bg-gray-500;
}
</style>