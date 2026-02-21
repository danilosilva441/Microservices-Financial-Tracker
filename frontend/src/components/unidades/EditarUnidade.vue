<!--
 * src/components/unidades/EditarUnidade.vue
 * EditarUnidade.vue
 *
 * A Vue component that provides an interface for editing a specific "unidade" (unit) in the financial tracking application.
 * It includes a breadcrumb navigation, a header card with unit information, and a form for editing the unit's details.
 * The component also handles loading states, error states, and displays helpful tips in a sidebar.
 * It is designed to be responsive and theme-aware, using Tailwind CSS for styling.
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
          <li v-if="unidade" class="flex items-center">
            <router-link 
              :to="`/unidades/${unidade.id}`" 
              class="flex items-center text-gray-500 transition-colors dark:text-gray-400 hover:text-primary-600 dark:hover:text-primary-400 whitespace-nowrap"
            >
              <span class="max-w-[150px] truncate">{{ unidade.nome }}</span>
            </router-link>
            <IconChevronRight class="w-4 h-4 mx-1 text-gray-400 dark:text-gray-600" />
          </li>
          <li class="flex items-center font-semibold text-primary-600 dark:text-primary-400 whitespace-nowrap" aria-current="page">
            <IconEdit class="w-4 h-4 mr-1" />
            <span>Editar</span>
          </li>
        </ol>
      </nav>

      <!-- Header Card -->
      <div class="p-4 mb-6 bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 sm:p-6">
        <div class="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div class="flex items-start space-x-4">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 text-white shadow-lg sm:w-14 sm:h-14 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500">
                <IconStore class="w-6 h-6 sm:w-7 sm:h-7" />
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex flex-wrap items-center gap-2">
                <h1 class="text-2xl font-bold text-gray-900 truncate sm:text-3xl dark:text-white">
                  Editar Unidade
                </h1>
                <span class="px-2.5 py-0.5 text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400 rounded-full">
                  <IconClock class="inline w-3 h-3 mr-1" />
                  Em edição
                </span>
              </div>
              <p v-if="unidade" class="flex items-center mt-1 text-sm text-gray-600 sm:text-base dark:text-gray-400">
                <IconInfoCircle class="w-4 h-4 mr-1.5 text-gray-400 dark:text-gray-500" />
                Editando: <span class="ml-1 font-semibold text-gray-900 truncate dark:text-white">{{ unidade.nome }}</span>
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2 ml-0 sm:gap-3 sm:ml-4">
            <router-link 
              :to="`/unidades/${unidadeId}`" 
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
          <!-- Loading State -->
          <div v-if="loading" class="p-8 bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 sm:p-12">
            <div class="flex flex-col items-center justify-center text-center">
              <div class="relative">
                <div class="w-16 h-16 border-4 rounded-full sm:w-20 sm:h-20 border-primary-200 dark:border-primary-900 border-t-primary-600 dark:border-t-primary-400 animate-spin"></div>
                <div class="absolute inset-0 flex items-center justify-center">
                  <IconStore class="w-6 h-6 sm:w-8 sm:h-8 text-primary-600 dark:text-primary-400 animate-pulse" />
                </div>
              </div>
              <p class="mt-6 text-base font-medium text-gray-900 sm:text-lg dark:text-white">
                Carregando unidade
              </p>
              <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
                Aguarde enquanto buscamos os dados...
              </p>
            </div>
          </div>

          <!-- Error State -->
          <div v-else-if="error" class="p-8 bg-white border border-red-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-red-900/30 sm:p-12">
            <div class="flex flex-col items-center justify-center text-center">
              <div class="flex items-center justify-center w-20 h-20 mb-6 bg-red-100 rounded-full dark:bg-red-900/30">
                <IconExclamationTriangle class="w-10 h-10 text-red-600 dark:text-red-400" />
              </div>
              <h3 class="mb-3 text-xl font-bold text-gray-900 sm:text-2xl dark:text-white">
                Erro ao carregar
              </h3>
              <p class="max-w-md mb-6 text-sm text-gray-600 sm:text-base dark:text-gray-400">
                {{ error }}
              </p>
              <div class="flex flex-col gap-3 sm:flex-row">
                <button @click="fetchUnidade" class="btn-primary">
                  <IconRefresh class="w-4 h-4 mr-2" />
                  Tentar novamente
                </button>
                <router-link to="/unidades" class="btn-outline">
                  <IconArrowLeft class="w-4 h-4 mr-2" />
                  Voltar para unidades
                </router-link>
              </div>
            </div>
          </div>

          <!-- Not Found State -->
          <div v-else-if="!unidade" class="p-8 bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 sm:p-12">
            <div class="flex flex-col items-center justify-center text-center">
              <div class="flex items-center justify-center w-20 h-20 mb-6 bg-gray-100 rounded-full dark:bg-gray-700">
                <IconStoreSlash class="w-10 h-10 text-gray-500 dark:text-gray-400" />
              </div>
              <h3 class="mb-3 text-xl font-bold text-gray-900 sm:text-2xl dark:text-white">
                Unidade não encontrada
              </h3>
              <p class="max-w-md mb-6 text-sm text-gray-600 sm:text-base dark:text-gray-400">
                A unidade que você está tentando editar não existe ou foi removida.
              </p>
              <router-link to="/unidades" class="btn-primary">
                <IconArrowLeft class="w-4 h-4 mr-2" />
                Voltar para unidades
              </router-link>
            </div>
          </div>

          <!-- Form -->
          <div v-else class="overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700">
            <div class="px-4 py-4 border-b border-gray-200 dark:border-gray-700 sm:px-6 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconPencil class="w-5 h-5 text-primary-600 dark:text-primary-400" />
                <h2 class="text-base font-semibold text-gray-900 sm:text-lg dark:text-white">
                  Formulário de Edição
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
                :initial-data="unidade"
                mode="edit"
                @success="handleSuccess"
                @cancel="handleCancel"
                @error="handleError"
              />
            </div>
          </div>
        </div>

        <!-- Help Sidebar Column -->
        <div class="space-y-4 lg:col-span-1 sm:space-y-6">
          <!-- Help Card -->
          <div class="sticky overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 top-20">
            <div class="px-4 py-4 border-b border-gray-200 dark:border-gray-700 sm:px-6 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconQuestionCircle class="w-5 h-5 text-primary-600 dark:text-primary-400" />
                <h3 class="text-base font-semibold text-gray-900 sm:text-lg dark:text-white">
                  Ajuda Rápida
                </h3>
              </div>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-4 sm:space-y-6">
                <!-- Dicas -->
                <div>
                  <h4 class="mb-3 text-xs font-semibold tracking-wider text-gray-500 uppercase dark:text-gray-400">
                    Dicas para edição
                  </h4>
                  <ul class="space-y-3">
                    <li class="flex items-start gap-3 text-sm">
                      <div class="flex-shrink-0 w-5 h-5 rounded-full bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center mt-0.5">
                        <span class="text-xs font-bold text-primary-600 dark:text-primary-400">1</span>
                      </div>
                      <div>
                        <span class="font-medium text-gray-900 dark:text-white">Nome da unidade</span>
                        <p class="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                          Use um nome claro e descritivo que identifique facilmente a unidade.
                        </p>
                      </div>
                    </li>
                    <li class="flex items-start gap-3 text-sm">
                      <div class="flex-shrink-0 w-5 h-5 rounded-full bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center mt-0.5">
                        <span class="text-xs font-bold text-primary-600 dark:text-primary-400">2</span>
                      </div>
                      <div>
                        <span class="font-medium text-gray-900 dark:text-white">Endereço completo</span>
                        <p class="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                          Inclua rua, número, bairro, cidade, estado e CEP para localização precisa.
                        </p>
                      </div>
                    </li>
                    <li class="flex items-start gap-3 text-sm">
                      <div class="flex-shrink-0 w-5 h-5 rounded-full bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center mt-0.5">
                        <span class="text-xs font-bold text-primary-600 dark:text-primary-400">3</span>
                      </div>
                      <div>
                        <span class="font-medium text-gray-900 dark:text-white">Meta mensal</span>
                        <p class="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                          Baseie-se em dados históricos e seja realista para motivar a equipe.
                        </p>
                      </div>
                    </li>
                    <li class="flex items-start gap-3 text-sm">
                      <div class="flex-shrink-0 w-5 h-5 rounded-full bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center mt-0.5">
                        <span class="text-xs font-bold text-primary-600 dark:text-primary-400">4</span>
                      </div>
                      <div>
                        <span class="font-medium text-gray-900 dark:text-white">Datas importantes</span>
                        <p class="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                          Mantenha as datas de início e vencimento sempre atualizadas.
                        </p>
                      </div>
                    </li>
                  </ul>
                </div>

                <!-- Divider -->
                <div class="border-t border-gray-200 dark:border-gray-700"></div>

                <!-- Status Info -->
                <div>
                  <h4 class="mb-3 text-xs font-semibold tracking-wider text-gray-500 uppercase dark:text-gray-400">
                    Informações da Unidade
                  </h4>
                  <div v-if="unidade" class="space-y-2">
                    <div class="flex items-center justify-between text-sm">
                      <span class="text-gray-600 dark:text-gray-400">Status:</span>
                      <span :class="unidade.isAtivo ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'">
                        <span class="flex items-center">
                          <span class="w-2 h-2 mr-2 rounded-full" :class="unidade.isAtivo ? 'bg-green-500' : 'bg-gray-400'"></span>
                          {{ unidade.isAtivo ? 'Ativo' : 'Inativo' }}
                        </span>
                      </span>
                    </div>
                    <div class="flex items-center justify-between text-sm">
                      <span class="text-gray-600 dark:text-gray-400">ID:</span>
                      <span class="font-mono text-xs text-gray-900 dark:text-white">{{ unidade.id }}</span>
                    </div>
                    <div class="flex items-center justify-between text-sm">
                      <span class="text-gray-600 dark:text-gray-400">Criado em:</span>
                      <span class="text-gray-900 dark:text-white">{{ formatDate(unidade.createdAt) }}</span>
                    </div>
                    <div class="flex items-center justify-between text-sm">
                      <span class="text-gray-600 dark:text-gray-400">Última atualização:</span>
                      <span class="text-gray-900 dark:text-white">{{ formatDate(unidade.updatedAt) }}</span>
                    </div>
                  </div>
                </div>

                <!-- Action Buttons -->
                <div class="pt-4 border-t border-gray-200 dark:border-gray-700">
                  <button 
                    @click="$refs.unidadeForm?.submitForm()" 
                    class="justify-center w-full mb-2 btn-primary"
                  >
                    <IconSave class="w-4 h-4 mr-2" />
                    Salvar alterações
                  </button>
                  <router-link 
                    :to="`/unidades/${unidadeId}`" 
                    class="justify-center w-full btn-outline"
                  >
                    <IconEye class="w-4 h-4 mr-2" />
                    Visualizar unidade
                  </router-link>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Activity Card (Opcional) -->
          <div class="hidden overflow-hidden bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 lg:block">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50">
              <div class="flex items-center gap-2">
                <IconActivity class="w-5 h-5 text-primary-600 dark:text-primary-400" />
                <h3 class="text-base font-semibold text-gray-900 dark:text-white">
                  Atividade Recente
                </h3>
              </div>
            </div>
            <div class="p-6">
              <div class="flex flex-col items-center text-center">
                <IconHistory class="w-8 h-8 mb-2 text-gray-400 dark:text-gray-600" />
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  Histórico de alterações disponível em breve
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
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useTheme } from '@/composables/useTheme';
import UnidadeForm from '@/components/unidades/UnidadeForm.vue';
import { useUnidades } from '@/composables/unidades';

// Ícones personalizados (assumindo que você tem esses SVGs)
import IconHome from "@/components/icons/home.vue"
import IconChartLine from "@/components/icons/chart-line.vue"
import IconStore from "@/components/icons/store.vue"
import IconBriefcase from "@/components/icons/briefcase.vue"
import IconChartBar from "@/components/icons/chart-bar.vue"
import IconUsers from "@/components/icons/users.vue"
import IconCalendarAlt from "@/components/icons/calendar-alt.vue"

import IconRocket from "@/components/icons/rocket.vue"
import IconSignInAlt from "@/components/icons/sign-in-alt.vue"
import IconUserPlus from "@/components/icons/user-plus.vue"

import IconUserCircle from "@/components/icons/user-circle.vue"
import IconCog from "@/components/icons/cog.vue"
import IconBell from "@/components/icons/bell.vue"
import IconCreditCard from "@/components/icons/credit-card.vue"
import IconSignOutAlt from "@/components/icons/sign-out-alt.vue"

import IconChevronDown from "@/components/icons/chevron-down.vue"
import IconChevronRight from "@/components/icons/chevron-right.vue"
import IconTimes from "@/components/icons/times.vue"
import IconBars from "@/components/icons/bars.vue"

import IconSun from "@/components/icons/sun.vue"
import IconMoon from "@/components/icons/moon.vue"

import IconShieldAlt from "@/components/icons/shield-alt.vue"
import IconQuestionCircle from "@/components/icons/question-circle.vue"
import IconBook from "@/components/icons/book.vue"
import IconEnvelope from "@/components/icons/envelope.vue"
import IconServer from "@/components/icons/server.vue"

import IconTwitter from "@/components/icons/twitter.vue"
import IconLinkedin from "@/components/icons/linkedin.vue"
import IconGithub from "@/components/icons/github.vue"
import IconDiscord from "@/components/icons/discord.vue"

const route = useRoute();
const router = useRouter();
const { isDarkMode } = useTheme();
const unidadeId = route.params.id;

const {
  store,
  actions,
  loadUnidades,
  getUnidadeById
} = useUnidades();

// Estado local
const loading = ref(true);
const error = ref(null);
const unidadeForm = ref(null);

// Computed
const unidade = computed(() => store.unidadeAtual || getUnidadeById(unidadeId));

// Formatação de data
const formatDate = (date) => {
  if (!date) return 'N/A';
  return new Date(date).toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

// Métodos
const fetchUnidade = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    if (!unidade.value) {
      await actions.getUnidade(unidadeId);
    }
  } catch (err) {
    error.value = err.response?.data?.message || err.message || 'Erro ao carregar unidade';
    console.error('Erro ao carregar unidade:', err);
  } finally {
    loading.value = false;
  }
};

const handleSuccess = (updatedUnidade) => {
  router.push({ 
    name: 'UnidadeDetalhes', 
    params: { id: unidadeId },
    query: { updated: 'true', timestamp: Date.now() }
  });
};

const handleCancel = () => {
  router.push({ name: 'UnidadeDetalhes', params: { id: unidadeId } });
};

const handleError = (errorMessage) => {
  console.error('Erro no formulário:', errorMessage);
};

// Watch para mudanças no ID da rota
watch(() => route.params.id, (newId) => {
  if (newId) {
    fetchUnidade();
  }
});

// Lifecycle
onMounted(async () => {
  if (store.unidades.length === 0) {
    await loadUnidades();
  }
  await fetchUnidade();
});
</script>

<style scoped>
@import '@/assets/default.css';
@import './CSS/EditarUnidade.css';
</style>