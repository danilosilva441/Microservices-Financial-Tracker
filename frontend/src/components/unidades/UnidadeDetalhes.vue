<!--
 * src/components/unidades/UnidadeDetalhes.vue
 * UnidadeDetalhes.vue
 *
 * A Vue component that displays detailed information about a specific business unit (unidade).
 * It includes a header with the unit's name, status, and contact information, as well as tabs for different sections like dashboard, info, documents, financials, and employees.
 * The component handles loading states, error states, and provides action buttons for editing and deleting the unit.
 * It is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="min-h-screen transition-colors duration-200 bg-gray-50 dark:bg-gray-900" :class="{ 'dark': isDarkMode }">
    <div class="container px-4 py-6 mx-auto sm:px-6 lg:px-8 sm:py-8">
      <!-- Loading State -->
      <div v-if="isLoading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <IconLoader class="w-12 h-12 mx-auto mb-4 animate-spin text-primary-500 dark:text-primary-400" />
          <p class="text-gray-600 dark:text-gray-400">Carregando informações da unidade...</p>
        </div>
      </div>

      <template v-else-if="unidade">
        <!-- Header Card -->
        <div class="p-4 mb-6 bg-white border border-gray-200 shadow-sm dark:bg-gray-800 rounded-xl dark:border-gray-700 sm:p-6">
          <div class="flex flex-col justify-between gap-6 lg:flex-row lg:items-start">
            <!-- Left Section -->
            <div class="flex-1 min-w-0">
              <div class="flex items-start gap-4">
                <!-- Avatar -->
                <div class="flex-shrink-0">
                  <div class="flex items-center justify-center text-white shadow-lg w-14 h-14 sm:w-16 sm:h-16 rounded-xl bg-gradient-to-br from-primary-500 to-secondary-500">
                    <IconStore class="w-7 h-7 sm:w-8 sm:h-8" />
                  </div>
                </div>
                
                <!-- Title and Status -->
                <div class="flex-1 min-w-0">
                  <div class="flex flex-wrap items-center gap-3 mb-3">
                    <h1 class="text-2xl font-bold text-gray-900 truncate sm:text-3xl dark:text-white">
                      {{ unidade.nome }}
                    </h1>
                    <span :class="statusClasses">
                      <span :class="statusDotClasses" class="w-1.5 h-1.5 rounded-full mr-1.5"></span>
                      {{ unidade.status || 'Status indisponível' }}
                    </span>
                  </div>

                  <!-- Info Items -->
                  <div class="space-y-2 text-sm text-gray-600 dark:text-gray-400">
                    <div class="flex items-start gap-2">
                      <IconMapPin class="w-4 h-4 flex-shrink-0 text-gray-400 dark:text-gray-500 mt-0.5" />
                      <span class="flex-1">{{ unidade.endereco }}, {{ unidade.cidade }} - {{ unidade.estado }}</span>
                    </div>
                    
                    <div class="flex flex-wrap items-center gap-4">
                      <div v-if="unidade.telefone" class="flex items-center gap-2">
                        <IconPhone class="w-4 h-4 text-gray-400 dark:text-gray-500" />
                        <span>{{ unidade.telefone }}</span>
                      </div>
                      <div v-if="unidade.email" class="flex items-center gap-2">
                        <IconMail class="w-4 h-4 text-gray-400 dark:text-gray-500" />
                        <span class="truncate max-w-[200px]">{{ unidade.email }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex flex-col items-stretch gap-3 sm:flex-row lg:flex-col sm:items-center lg:items-stretch lg:ml-6">
              <button @click="goToEdit(unidade.id)" class="btn-primary whitespace-nowrap">
                <IconEdit class="w-4 h-4 mr-2" />
                Editar
              </button>
              <button @click="openDeleteModal(unidade)" class="btn-outline-danger whitespace-nowrap">
                <IconTrash class="w-4 h-4 mr-2" />
                Excluir
              </button>
            </div>
          </div>
        </div>

        <!-- Tabs -->
        <div class="mb-6 overflow-x-auto border-b border-gray-200 dark:border-gray-700 scrollbar-hide">
          <div class="flex space-x-1 min-w-max">
            <button
              v-for="tab in tabs"
              :key="tab.id"
              @click="changeTab(tab.id)"
              :class="[
                'tab-item px-4 sm:px-6 py-3 text-sm font-medium transition-colors relative',
                activeTab === tab.id
                  ? 'text-primary-600 dark:text-primary-400'
                  : 'text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300'
              ]"
            >
              <div class="flex items-center gap-2">
                <component :is="tab.iconComponent" class="w-4 h-4" />
                <span>{{ tab.label }}</span>
              </div>
              <div v-if="activeTab === tab.id" class="absolute bottom-0 left-0 right-0 h-0.5 bg-primary-500 dark:bg-primary-400"></div>
            </button>
          </div>
        </div>

        <!-- Tab Content -->
        <div class="transition-opacity duration-300">
          <!-- Dashboard Tab -->
          <div v-if="activeTab === 'dashboard'" class="space-y-6">
            <!-- Loading State -->
            <div v-if="dashboardLoading" class="p-8 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
              <div class="flex flex-col items-center justify-center text-center">
                <IconLoader class="w-12 h-12 mb-4 animate-spin text-primary-500 dark:text-primary-400" />
                <p class="text-gray-600 dark:text-gray-400">Carregando dados do dashboard...</p>
              </div>
            </div>

            <!-- Error State -->
            <div v-else-if="dashboardError" class="p-8 bg-white border border-red-200 dark:bg-gray-800 rounded-xl dark:border-red-900/30">
              <div class="flex flex-col items-center justify-center text-center">
                <IconAlertCircle class="w-12 h-12 mb-4 text-red-500 dark:text-red-400" />
                <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Erro ao carregar dados</h3>
                <p class="mb-4 text-sm text-gray-600 dark:text-gray-400">{{ dashboardError }}</p>
                <button @click="refreshDashboardData" class="btn-outline">
                  <IconRefresh class="w-4 h-4 mr-2" />
                  Tentar novamente
                </button>
              </div>
            </div>

            <!-- Dashboard Content -->
            <div v-else-if="dashboardData" class="space-y-6">
              <!-- Progress Card -->
              <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
                <div class="flex items-center justify-between mb-6">
                  <h3 class="flex items-center gap-2 text-lg font-semibold text-gray-900 dark:text-white">
                    <IconChartLine class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                    Progresso da Meta
                  </h3>
                  <button @click="refreshDashboardData" class="action-button" title="Atualizar dados">
                    <IconRefresh class="w-4 h-4" />
                  </button>
                </div>

                <div class="space-y-6">
                  <!-- Progress Bar -->
                  <div>
                    <div class="flex flex-col justify-between gap-2 mb-2 sm:flex-row sm:items-center">
                      <div class="space-y-1">
                        <p class="text-sm text-gray-600 dark:text-gray-400">
                          Meta: <span class="font-semibold text-gray-900 dark:text-white">{{ formatCurrency(dashboardData.metaMensal || 0) }}</span>
                        </p>
                        <p class="text-sm text-gray-600 dark:text-gray-400">
                          Atual: <span class="font-semibold text-gray-900 dark:text-white">{{ formatCurrency(dashboardData.faturamentoAtual || 0) }}</span>
                        </p>
                      </div>
                      <div class="text-2xl font-bold" :style="{ color: getProgressColor(dashboardData.progressoMeta || 0) }">
                        {{ dashboardData.progressoMeta || 0 }}%
                      </div>
                    </div>

                    <div class="h-3 overflow-hidden bg-gray-200 rounded-full dark:bg-gray-700">
                      <div 
                        class="h-full transition-all duration-500 rounded-full"
                        :style="{
                          width: `${dashboardData.progressoMeta || 0}%`,
                          backgroundColor: getProgressColor(dashboardData.progressoMeta || 0)
                        }"
                      ></div>
                    </div>

                    <div class="flex justify-between mt-2 text-xs text-gray-500 dark:text-gray-400">
                      <span>0%</span>
                      <span>50%</span>
                      <span>100%</span>
                    </div>
                  </div>

                  <!-- Stats Grid -->
                  <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
                    <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900/50">
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Meta Restante</p>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ formatCurrency(dashboardData.metaRestante || 0) }}</p>
                    </div>
                    <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900/50">
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Média Diária</p>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ formatCurrency(dashboardData.mediaDiariaNecessaria || 0) }}</p>
                    </div>
                    <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900/50">
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Projeção</p>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ formatCurrency(dashboardData.projecaoFaturamento || 0) }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Financial Summary and General Info -->
              <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
                <!-- Financial Summary -->
                <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
                  <h3 class="flex items-center gap-2 mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                    <IconChartPie class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                    Resumo Financeiro
                  </h3>
                  
                  <div class="space-y-3">
                    <div class="flex items-center justify-between py-2 border-b border-gray-100 dark:border-gray-700">
                      <span class="text-sm text-gray-600 dark:text-gray-400">Faturamento Total</span>
                      <span class="text-sm font-semibold text-green-600 dark:text-green-400">
                        {{ formatCurrency(dashboardData.faturamentoTotal || 0) }}
                      </span>
                    </div>
                    <div class="flex items-center justify-between py-2 border-b border-gray-100 dark:border-gray-700">
                      <span class="text-sm text-gray-600 dark:text-gray-400">Despesas Totais</span>
                      <span class="text-sm font-semibold text-red-600 dark:text-red-400">
                        {{ formatCurrency(dashboardData.despesasTotais || 0) }}
                      </span>
                    </div>
                    <div class="flex items-center justify-between py-2 border-b border-gray-100 dark:border-gray-700">
                      <span class="text-sm text-gray-600 dark:text-gray-400">Lucro Líquido</span>
                      <span :class="[
                        'text-sm font-semibold',
                        (dashboardData.lucroLiquido || 0) >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'
                      ]">
                        {{ formatCurrency(dashboardData.lucroLiquido || 0) }}
                      </span>
                    </div>
                    <div class="flex items-center justify-between py-2">
                      <span class="text-sm text-gray-600 dark:text-gray-400">Margem</span>
                      <span class="text-sm font-semibold text-gray-900 dark:text-white">
                        {{ dashboardData.margem || 0 }}%
                      </span>
                    </div>
                  </div>
                </div>

                <!-- General Info -->
                <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
                  <h3 class="flex items-center gap-2 mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                    <IconInfoCircle class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                    Informações Gerais
                  </h3>
                  
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">CNPJ</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ unidade.cnpj || 'Não informado' }}</p>
                    </div>
                    <div>
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Abertura</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(unidade.dataAbertura) || 'Não informada' }}</p>
                    </div>
                    <div>
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Responsável</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ unidade.responsavel || 'Não informado' }}</p>
                    </div>
                    <div>
                      <p class="mb-1 text-xs text-gray-500 dark:text-gray-400">Funcionários</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ dashboardData.funcionariosAtivos || 0 }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Info Tab -->
          <div v-if="activeTab === 'info'" class="grid grid-cols-1 gap-6 lg:grid-cols-3">
            <!-- Company Info -->
            <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
              <h3 class="flex items-center gap-2 mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                <IconBuilding class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                Dados da Empresa
              </h3>
              <div class="space-y-3">
                <!-- ✅ InfoRow implementado diretamente no template -->
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Razão Social</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.razaoSocial || unidade.nome">
                    {{ unidade.razaoSocial || unidade.nome }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">CNPJ</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.cnpj || 'Não informado'">
                    {{ unidade.cnpj || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Inscrição Estadual</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.inscricaoEstadual || 'Não informada'">
                    {{ unidade.inscricaoEstadual || 'Não informada' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Segmento</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.segmento || 'Não informado'">
                    {{ unidade.segmento || 'Não informado' }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Address Info -->
            <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
              <h3 class="flex items-center gap-2 mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                <IconMapPin class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                Endereço
              </h3>
              <div class="space-y-3">
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Endereço</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.endereco || 'Não informado'">
                    {{ unidade.endereco || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Complemento</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.complemento || 'Não informado'">
                    {{ unidade.complemento || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Bairro</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.bairro || 'Não informado'">
                    {{ unidade.bairro || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Cidade/UF</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="`${unidade.cidade || ''} - ${unidade.estado || ''}`">
                    {{ unidade.cidade || '' }} - {{ unidade.estado || '' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">CEP</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.cep || 'Não informado'">
                    {{ unidade.cep || 'Não informado' }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Contact Info -->
            <div class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
              <h3 class="flex items-center gap-2 mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                <IconUser class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                Contato
              </h3>
              <div class="space-y-3">
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Responsável</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.responsavel || 'Não informado'">
                    {{ unidade.responsavel || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Telefone</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.telefone || 'Não informado'">
                    {{ unidade.telefone || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Email</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.email || 'Não informado'">
                    {{ unidade.email || 'Não informado' }}
                  </span>
                </div>
                <div class="flex flex-col justify-between gap-1 py-2 border-b border-gray-100 sm:flex-row sm:items-center dark:border-gray-700 last:border-0">
                  <span class="text-xs text-gray-600 sm:text-sm dark:text-gray-400">Celular</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white break-words max-w-[250px]" :title="unidade.celular || 'Não informado'">
                    {{ unidade.celular || 'Não informado' }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Documents Tab -->
          <div v-if="activeTab === 'docs'" class="p-6 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
            <div class="flex flex-col items-start justify-between gap-4 mb-6 sm:flex-row sm:items-center">
              <h3 class="flex items-center gap-2 text-lg font-semibold text-gray-900 dark:text-white">
                <IconFile class="w-5 h-5 text-primary-500 dark:text-primary-400" />
                Documentos da Unidade
              </h3>
              <button class="w-full btn-primary sm:w-auto">
                <IconPlus class="w-4 h-4 mr-2" />
                Adicionar Documento
              </button>
            </div>

            <div class="space-y-3">
              <div v-for="i in 3" :key="i" class="flex flex-col justify-between gap-4 p-4 rounded-lg sm:flex-row sm:items-center bg-gray-50 dark:bg-gray-900/50">
                <div class="flex items-center gap-3">
                  <IconFileText class="flex-shrink-0 w-8 h-8 text-gray-400 dark:text-gray-500" />
                  <div class="min-w-0">
                    <h4 class="font-medium text-gray-900 truncate dark:text-white">Contrato Social</h4>
                    <p class="text-xs text-gray-500 dark:text-gray-400">Última atualização: 15/01/2024</p>
                  </div>
                </div>
                <div class="flex items-center gap-2 ml-11 sm:ml-0">
                  <button class="action-button" title="Download">
                    <IconDownload class="w-4 h-4" />
                  </button>
                  <button class="action-button" title="Visualizar">
                    <IconEye class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Financeiro Tab (Placeholder) -->
          <div v-if="activeTab === 'financeiro'" class="p-8 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
            <div class="text-center">
              <IconChartLine class="w-16 h-16 mx-auto mb-4 text-gray-300 dark:text-gray-600" />
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Módulo Financeiro</h3>
              <p class="text-gray-600 dark:text-gray-400">Em breve você poderá visualizar dados financeiros detalhados aqui.</p>
            </div>
          </div>

          <!-- Funcionários Tab (Placeholder) -->
          <div v-if="activeTab === 'funcionarios'" class="p-8 bg-white border border-gray-200 dark:bg-gray-800 rounded-xl dark:border-gray-700">
            <div class="text-center">
              <IconUsers class="w-16 h-16 mx-auto mb-4 text-gray-300 dark:text-gray-600" />
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Gestão de Funcionários</h3>
              <p class="text-gray-600 dark:text-gray-400">Em breve você poderá gerenciar os funcionários da unidade aqui.</p>
            </div>
          </div>
        </div>
      </template>
    </div>

    <!-- Delete Confirmation Modal -->
    <ConfirmModal 
      v-if="showDeleteModal" 
      :title="`Excluir ${selectedUnidade?.nome}?`"
      message="Esta ação não pode ser desfeita. A unidade será marcada como inativa." 
      confirm-text="Excluir"
      cancel-text="Cancelar" 
      variant="danger" 
      @confirm="confirmDelete" 
      @cancel="closeDeleteModal" 
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useTheme } from '@/composables/useTheme';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import { useUnidades } from '@/composables/unidades';
import { useUnidadesUI } from '@/composables/unidades/useUnidadesUI';

// Ícones
import IconStore from '@/components/icons/store.vue';
import IconChevronRight from '@/components/icons/chevron-right.vue';
import IconLoader from '@/components/icons/loader.vue';
import IconMapPin from '@/components/icons/map-pin.vue';
import IconPhone from '@/components/icons/phone.vue';
import IconMail from '@/components/icons/mail.vue';
import IconEdit from '@/components/icons/edit.vue';
import IconTrash from '@/components/icons/trash.vue';
import IconChartLine from '@/components/icons/chart-line.vue';
import IconChartPie from '@/components/icons/chart-pie.vue';
import IconInfoCircle from '@/components/icons/info-circle.vue';
import IconBuilding from '@/components/icons/building.vue';
import IconUser from '@/components/icons/user.vue';
import IconFile from '@/components/icons/file.vue';
import IconFileText from '@/components/icons/file-text.vue';
import IconRefresh from '@/components/icons/refresh.vue';
import IconAlertCircle from '@/components/icons/alert-circle.vue';
import IconPlus from '@/components/icons/plus.vue';
import IconDownload from '@/components/icons/download.vue';
import IconEye from '@/components/icons/eye.vue';
import IconUsers from '@/components/icons/users.vue';

const { isDarkMode } = useTheme();
const route = useRoute();
const router = useRouter();
const unidadeId = route.params.id;

const {
  store,
  actions,
  loadUnidades,
  getUnidadeById
} = useUnidades();

const { formatCurrency, formatDate } = useUnidadesUI();

// Estado do componente
const activeTab = ref('dashboard');
const showDeleteModal = ref(false);
const selectedUnidade = ref(null);
const isLoading = ref(false);

// Dados do dashboard
const dashboardLoading = ref(false);
const dashboardError = ref(null);
const dashboardData = ref(null);

// Tabs disponíveis
const tabs = [
  { id: 'dashboard', label: 'Dashboard', iconComponent: IconChartLine },
  { id: 'info', label: 'Informações', iconComponent: IconInfoCircle },
  { id: 'docs', label: 'Documentos', iconComponent: IconFile },
  { id: 'financeiro', label: 'Financeiro', iconComponent: IconChartPie },
  { id: 'funcionarios', label: 'Funcionários', iconComponent: IconUsers }
];

// Computed
const unidade = computed(() => {
  if (store.unidadeAtual && store.unidadeAtual.id === unidadeId) {
    return store.unidadeAtual;
  }
  return getUnidadeById(unidadeId);
});

const statusClasses = computed(() => {
  const baseClasses = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium';
  const status = unidade.value?.status?.toLowerCase();
  
  if (status === 'ativa' || status === 'active') {
    return `${baseClasses} bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400`;
  }
  if (status === 'inativa' || status === 'inactive') {
    return `${baseClasses} bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-400`;
  }
  if (status === 'pendente' || status === 'pending') {
    return `${baseClasses} bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400`;
  }
  return `${baseClasses} bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-400`;
});

const statusDotClasses = computed(() => {
  const status = unidade.value?.status?.toLowerCase();
  
  if (status === 'ativa' || status === 'active') return 'bg-green-500';
  if (status === 'inativa' || status === 'inactive') return 'bg-gray-500';
  if (status === 'pendente' || status === 'pending') return 'bg-yellow-500';
  return 'bg-gray-500';
});

// Métodos
const changeTab = (tabId) => {
  activeTab.value = tabId;
  if (tabId === 'dashboard' && !dashboardData.value) {
    loadDashboardData();
  }
};

const goToEdit = (id) => {
  router.push({ name: 'EditarUnidade', params: { id } });
};

const openDeleteModal = (unidade) => {
  selectedUnidade.value = unidade;
  showDeleteModal.value = true;
};

const closeDeleteModal = () => {
  showDeleteModal.value = false;
  selectedUnidade.value = null;
};

const confirmDelete = async () => {
  if (selectedUnidade.value) {
    try {
      await actions.deleteUnidade(selectedUnidade.value.id);
      closeDeleteModal();
      router.push('/unidades');
    } catch (error) {
      console.error('Erro ao excluir unidade:', error);
    }
  }
};

const loadDashboardData = async () => {
  if (!unidadeId) return;

  dashboardLoading.value = true;
  dashboardError.value = null;

  try {
    // Mock de dados do dashboard - substitua por chamada real à API
    dashboardData.value = await mockDashboardData();
  } catch (error) {
    console.error('Erro ao carregar dados do dashboard:', error);
    dashboardError.value = error.message || 'Erro ao carregar dados do dashboard';
  } finally {
    dashboardLoading.value = false;
  }
};

const refreshDashboardData = () => {
  loadDashboardData();
};

const getProgressColor = (progress) => {
  if (progress >= 75) return '#10b981';
  if (progress >= 50) return '#f59e0b';
  if (progress >= 25) return '#f97316';
  return '#ef4444';
};

// Mock de dados do dashboard (REMOVA QUANDO IMPLEMENTAR API REAL)
const mockDashboardData = async () => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve({
        metaMensal: 50000,
        faturamentoAtual: 32500,
        progressoMeta: 65,
        metaRestante: 17500,
        mediaDiariaNecessaria: 1166.67,
        projecaoFaturamento: 48750,
        faturamentoTotal: 32500,
        despesasTotais: 15000,
        lucroLiquido: 17500,
        margem: 35,
        funcionariosAtivos: 12,
        ticketMedio: 125.50,
        clientesAtendidos: 259,
        melhorDia: '2024-01-15',
        piorDia: '2024-01-08'
      });
    }, 500);
  });
};

const fetchUnidade = async () => {
  if (!unidadeId) return;
  
  isLoading.value = true;
  try {
    await actions.getUnidadeById(unidadeId);
  } catch (error) {
    console.error('Erro ao carregar unidade:', error);
  } finally {
    isLoading.value = false;
  }
};

// Watchers
watch(
  () => route.params.id,
  (newId) => {
    if (newId) {
      fetchUnidade();
      if (activeTab.value === 'dashboard') {
        loadDashboardData();
      }
    }
  }
);

// Lifecycle
onMounted(async () => {
  await fetchUnidade();
  
  if (store.unidades.length === 0) {
    await loadUnidades();
  }
  
  if (activeTab.value === 'dashboard') {
    await loadDashboardData();
  }
});
</script>

<style scoped>
@import '@/assets/default.css';
@import './CSS/UnidadeDetalhes.css'; 

/* Hide scrollbar but keep functionality */
.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

/* Tab item styles */
.tab-item {
  position: relative;
  white-space: nowrap;
  transition: all 0.2s ease;
}

.tab-item:hover {
  background-color: rgba(102, 126, 234, 0.05);
}

/* Action button */
.action-button {
  @apply p-2 rounded-lg transition-all duration-200 hover:scale-110 hover:bg-gray-100 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 dark:focus:ring-offset-gray-900;
}

/* Botão danger outline */
.btn-outline-danger {
  @apply inline-flex items-center px-4 py-2 border-2 border-red-300 dark:border-red-800 text-red-600 dark:text-red-400 font-semibold rounded-lg hover:bg-red-50 dark:hover:bg-red-900/20 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-red-500;
}

/* Animações */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.tab-content {
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