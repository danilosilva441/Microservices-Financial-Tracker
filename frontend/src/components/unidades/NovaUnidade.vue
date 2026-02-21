<!--
 * src/components/unidades/NovaUnidade.vue
 * NovaUnidade.vue
 *
 * A Vue component that provides a form for creating a new unit (unidade) in the financial tracking application.
 * It includes a breadcrumb navigation, a header card with information and actions, and a main content area with the form and helpful tips.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 * It also integrates with the UnidadeForm component to handle the actual form submission and validation.
 -->
<template>
  <div class="min-h-screen transition-colors duration-200 bg-gray-50 dark:bg-gray-900" :class="{ 'dark': isDarkMode }">
    <div class="container px-4 py-6 mx-auto sm:px-6 lg:px-8 sm:py-8">
      <!-- Breadcrumb -->
      <nav class="flex items-center pb-2 mb-6 space-x-2 overflow-x-auto text-sm" aria-label="Breadcrumb">
        <ol class="flex flex-wrap items-center space-x-2">
          <li class="flex items-center">
            <router-link 
              to="/" 
              class="flex items-center text-gray-500 transition-colors dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 whitespace-nowrap"
            >
              <IconHome class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Home</span>
            </router-link>
            <IconChevronRight class="w-4 h-4 mx-1 text-gray-400 dark:text-gray-600" />
          </li>
          <li class="flex items-center">
            <router-link 
              to="/unidades" 
              class="flex items-center text-gray-500 transition-colors dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 whitespace-nowrap"
            >
              <IconStore class="w-4 h-4 mr-1" />
              <span class="hidden sm:inline">Unidades</span>
            </router-link>
            <IconChevronRight class="w-4 h-4 mx-1 text-gray-400 dark:text-gray-600" />
          </li>
          <li class="flex items-center font-semibold text-primary-600 dark:text-primary-400 whitespace-nowrap" aria-current="page">
            <IconPlusCircle class="w-4 h-4 mr-1" />
            <span>Nova Unidade</span>
          </li>
        </ol>
      </nav>

      <!-- Header Card -->
      <div class="p-4 mb-6 bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 sm:p-6">
        <div class="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div class="flex items-start space-x-4">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 text-white shadow-lg sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500">
                <IconPlusCircle class="w-6 h-6 sm:w-7 sm:h-7" />
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex flex-wrap items-center gap-2">
                <h1 class="text-2xl font-bold text-gray-900 sm:text-3xl dark:text-white">
                  Nova Unidade
                </h1>
                <span class="px-2.5 py-0.5 text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 rounded-full">
                  <IconPlus class="inline w-3 h-3 mr-1" />
                  Cadastro
                </span>
              </div>
              <p class="flex items-center mt-1 text-sm text-gray-600 sm:text-base dark:text-gray-400">
                <IconInfoCircle class="w-4 h-4 mr-1.5 text-gray-400 dark:text-gray-500" />
                Preencha os dados para cadastrar uma nova unidade
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2 ml-0 sm:gap-3 sm:ml-4">
            <router-link 
              to="/unidades" 
              class="justify-center flex-1 btn-outline sm:flex-initial"
            >
              <IconTimes class="w-4 h-4 mr-2" />
              <span class="hidden sm:inline">Cancelar</span>
              <span class="sm:hidden">Cancelar</span>
            </router-link>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <!-- Form Column -->
        <div class="lg:col-span-2">
          <div class="overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700">
            <div class="px-4 py-4 border-b border-gray-200 dark:border-gray-700 sm:px-6 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconForm class="w-5 h-5 text-primary-600 dark:text-primary-400" />
                <h2 class="text-base font-semibold text-gray-900 sm:text-lg dark:text-white">
                  Formulário de Cadastro
                </h2>
                <span class="ml-auto text-xs text-gray-500 dark:text-gray-400">
                  <IconInfoCircle class="w-3.5 h-3.5 inline mr-1" />
                  Campos com * são obrigatórios
                </span>
              </div>
            </div>
            <div class="p-4 sm:p-6">
              <UnidadeForm
                ref="unidadeForm"
                mode="create"
                @success="handleSuccess"
                @cancel="handleCancel"
                @error="handleError"
              />
            </div>
          </div>
        </div>

        <!-- Help Sidebar Column -->
        <div class="space-y-4 lg:col-span-1 sm:space-y-6">
          <!-- Dicas Card -->
          <div class="sticky overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 top-20">
            <div class="px-4 py-4 border-b border-gray-200 dark:border-gray-700 sm:px-6 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconLightbulb class="w-5 h-5 text-yellow-500" />
                <h3 class="text-base font-semibold text-gray-900 sm:text-lg dark:text-white">
                  Dicas para Cadastro
                </h3>
              </div>
            </div>
            
            <div class="p-4 space-y-6 sm:p-6">
              <!-- Boas Práticas -->
              <div>
                <h4 class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-3 flex items-center gap-1.5">
                  <IconCheckCircle class="w-4 h-4 text-green-500" />
                  Boas Práticas
                </h4>
                <ul class="space-y-2">
                  <li v-for="(tip, index) in goodPractices" :key="index" class="flex items-start gap-2 text-sm">
                    <div class="flex-shrink-0 w-5 h-5 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center mt-0.5">
                      <IconCheck class="w-3 h-3 text-green-600 dark:text-green-400" />
                    </div>
                    <span class="text-gray-600 dark:text-gray-300">{{ tip }}</span>
                  </li>
                </ul>
              </div>

              <!-- O que evitar -->
              <div>
                <h4 class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-3 flex items-center gap-1.5">
                  <IconAlertTriangle class="w-4 h-4 text-red-500" />
                  O que evitar
                </h4>
                <ul class="space-y-2">
                  <li v-for="(warning, index) in warnings" :key="index" class="flex items-start gap-2 text-sm">
                    <div class="flex-shrink-0 w-5 h-5 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center mt-0.5">
                      <IconTimes class="w-3 h-3 text-red-600 dark:text-red-400" />
                    </div>
                    <span class="text-gray-600 dark:text-gray-300">{{ warning }}</span>
                  </li>
                </ul>
              </div>

              <!-- Informações Adicionais -->
              <div class="p-4 rounded-lg bg-blue-50 dark:bg-blue-900/20">
                <h4 class="text-xs font-semibold text-blue-700 dark:text-blue-400 uppercase tracking-wider mb-2 flex items-center gap-1.5">
                  <IconInfoCircle class="w-4 h-4" />
                  Após o cadastro
                </h4>
                <ul class="space-y-1.5">
                  <li v-for="(info, index) in afterRegistration" :key="index" class="flex items-center gap-2 text-xs text-blue-600 dark:text-blue-300">
                    <IconCheck class="flex-shrink-0 w-3 h-3" />
                    <span>{{ info }}</span>
                  </li>
                </ul>
              </div>

              <!-- Ações Rápidas -->
              <div class="pt-4 border-t border-gray-200 dark:border-gray-700">
                <button 
                  @click="clearForm" 
                  class="justify-center w-full mb-2 btn-outline"
                >
                  <IconRefresh class="w-4 h-4 mr-2" />
                  Limpar formulário
                </button>
                <router-link 
                  to="/unidades" 
                  class="justify-center w-full btn-outline"
                >
                  <IconArrowLeft class="w-4 h-4 mr-2" />
                  Voltar para unidades
                </router-link>
              </div>
            </div>
          </div>

          <!-- Progress Card (Opcional) -->
          <div class="hidden overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 lg:block">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconActivity class="w-5 h-5 text-primary-600 dark:text-primary-400" />
                <h3 class="text-base font-semibold text-gray-900 dark:text-white">
                  Progresso do Cadastro
                </h3>
              </div>
            </div>
            <div class="p-6">
              <div class="flex flex-col items-center text-center">
                <div class="relative mb-4">
                  <svg class="w-24 h-24 transform -rotate-90">
                    <circle
                      class="text-gray-200 dark:text-gray-700"
                      stroke-width="4"
                      stroke="currentColor"
                      fill="transparent"
                      r="44"
                      cx="48"
                      cy="48"
                    />
                    <circle
                      class="transition-all duration-500 text-primary-500 dark:text-primary-400"
                      stroke-width="4"
                      stroke="currentColor"
                      fill="transparent"
                      r="44"
                      cx="48"
                      cy="48"
                      :stroke-dasharray="circumference"
                      :stroke-dashoffset="circumference - (circumference * progress) / 100"
                    />
                  </svg>
                  <div class="absolute inset-0 flex items-center justify-center">
                    <span class="text-xl font-bold text-gray-900 dark:text-white">{{ progress }}%</span>
                  </div>
                </div>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  Complete todos os campos obrigatórios
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useTheme } from '@/composables/useTheme';
import UnidadeForm from '@/components/unidades/UnidadeForm.vue';

// Ícones
import IconHome from '@/components/icons/home.vue';
import IconStore from '@/components/icons/store.vue';
import IconChevronRight from '@/components/icons/chevron-right.vue';
import IconPlusCircle from '@/components/icons/plus-circle.vue';
import IconPlus from '@/components/icons/plus.vue';
import IconTimes from '@/components/icons/times.vue';
import IconInfoCircle from '@/components/icons/info-circle.vue';
import IconForm from '@/components/icons/form.vue';
import IconLightbulb from '@/components/icons/lightbulb.vue';
import IconCheckCircle from '@/components/icons/check-circle.vue';
import IconCheck from '@/components/icons/check.vue';
import IconAlertTriangle from '@/components/icons/alert-triangle.vue';
import IconRefresh from '@/components/icons/refresh.vue';
import IconArrowLeft from '@/components/icons/arrow-left.vue';
import IconActivity from '@/components/icons/activity.vue';

const { isDarkMode } = useTheme();
const router = useRouter();
const unidadeForm = ref(null);

// Dados das dicas
const goodPractices = [
  'Use nomes curtos e descritivos',
  'Preencha todos os campos obrigatórios (*)',
  'Mantenha dados de contato atualizados',
  'Defina metas realistas baseadas em histórico'
];

const warnings = [
  'Nomes muito longos ou genéricos',
  'Metas irreais ou não mensuráveis',
  'Datas inconsistentes',
  'Endereços incompletos'
];

const afterRegistration = [
  'Adicionar funcionários à unidade',
  'Registrar faturamentos diários',
  'Acompanhar metas em tempo real',
  'Gerar relatórios personalizados'
];

// Progresso do formulário (mock - você pode conectar com o form real)
const progress = ref(0);

// Circunferência do círculo de progresso
const circumference = 2 * Math.PI * 44;

// Métodos
const handleSuccess = (novaUnidade) => {
  router.push({ 
    name: 'UnidadeDetalhes', 
    params: { id: novaUnidade.id },
    query: { created: 'true', timestamp: Date.now() }
  });
};

const handleCancel = () => {
  router.push({ name: 'Unidades' });
};

const handleError = (errorMessage) => {
  console.error('Erro ao criar unidade:', errorMessage);
  // Aqui você pode adicionar uma notificação toast
};

const clearForm = () => {
  if (unidadeForm.value) {
    unidadeForm.value.reset();
    progress.value = 0;
  }
};

// Simular progresso (remover quando integrar com o form real)
onMounted(() => {
  const interval = setInterval(() => {
    if (progress.value < 75) {
      progress.value += 5;
    }
  }, 1000);

  onUnmounted(() => {
    clearInterval(interval);
  });
});
</script>

<style scoped>
@import '@/assets/default.css';
@import './CSS/NovaUnidade.css';

/* Animações */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fadeIn {
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

/* Progress circle animations */
circle {
  transition: stroke-dashoffset 0.5s ease;
}

/* Sticky sidebar */
.sticky {
  position: sticky;
  top: 5rem;
}

/* Responsive adjustments */
@media (max-width: 1024px) {
  .sticky {
    position: static;
  }
}

/* Hover effects */
.hover-scale {
  transition: transform 0.2s ease;
}

.hover-scale:hover {
  transform: scale(1.02);
}
</style>