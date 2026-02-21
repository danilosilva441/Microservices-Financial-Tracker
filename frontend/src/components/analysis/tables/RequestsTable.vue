<!--
 * src/components/analysis/tables/RequestsTable.vue
 * RequestsTable.vue
 *
 * A Vue component that displays a table of financial requests in the analysis dashboard.
 * It includes features like status filtering, type badges, and action buttons.
 * The component is designed to be responsive and theme-aware, using Tailwind CSS for styling.
 -->
<template>
  <div class="overflow-hidden bg-white shadow-sm dark:bg-gray-800 rounded-xl">
    <div class="p-4 border-b border-gray-200 dark:border-gray-700">
      <div class="flex flex-wrap gap-2">
        <button
          v-for="status in statusOptions"
          :key="status.value"
          @click="$emit('update:statusFilter', status.value)"
          class="px-3 py-1.5 text-sm font-medium rounded-lg transition-colors"
          :class="[
            statusFilter === status.value
              ? status.activeClass
              : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
          ]"
        >
          <Component v-if="status.icon" :is="status.icon" class="inline w-4 h-4 mr-1" />
          {{ status.label }}
        </button>
      </div>
    </div>

    <div class="overflow-x-auto">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700/50">
          <tr>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              ID
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Tipo
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Unidade
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
              Solicitante
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-right text-gray-500 uppercase dark:text-gray-400">
              Valor
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              Data
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              Status
            </th>
            <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-400">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr
            v-for="item in filteredItems"
            :key="item.id"
            class="transition-colors hover:bg-gray-50 dark:hover:bg-gray-700/50"
          >
            <td class="px-6 py-4 font-mono text-sm text-gray-900 whitespace-nowrap dark:text-white">
              #{{ item.id.slice(0, 8) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 py-1 text-xs font-medium rounded-full" :class="getTipoClass(item.tipo)">
                {{ item.tipo }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
              {{ item.unidade }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="flex items-center justify-center w-8 h-8 bg-gray-200 rounded-full dark:bg-gray-700">
                  <IconUser class="w-4 h-4 text-gray-500" />
                </div>
                <div class="ml-3">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ item.solicitante }}
                  </div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">
                    {{ item.cargo }}
                  </div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-right text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatCurrency(item.valor) }}
            </td>
            <td class="px-6 py-4 text-sm text-center text-gray-900 whitespace-nowrap dark:text-white">
              {{ formatDate(item.data) }}
            </td>
            <td class="px-6 py-4 text-center whitespace-nowrap">
              <span
                class="px-2 py-1 text-xs font-medium rounded-full"
                :class="getStatusClass(item.status)"
              >
                <Component :is="getStatusIcon(item.status)" class="inline w-3 h-3 mr-1" />
                {{ item.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-center whitespace-nowrap">
              <button
                @click="$emit('view', item)"
                class="action-button text-primary-600 hover:bg-primary-50 dark:hover:bg-primary-900/30"
                title="Visualizar"
              >
                <IconEye class="w-5 h-5" />
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="filteredItems.length === 0" class="p-8">
      <EmptyState
        title="Nenhuma solicitação encontrada"
        :message="`Não há solicitações com o status '${getCurrentStatusLabel}' para exibir.`"
        type="table"
      />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import EmptyState from '../shared/EmptyState.vue'
import IconUser from '@/components/icons/user.vue'
import IconEye from '@/components/icons/eye.vue'
import IconCheckCircle from '@/components/icons/check-circle.vue'
import IconXCircle from '@/components/icons/x-circle.vue'
import IconClock from '@/components/icons/clock.vue'
import IconTrash from '@/components/icons/trash.vue'
import IconPencil from '@/components/icons/pencil.vue'
import IconPlus from '@/components/icons/plus.vue'

const props = defineProps({
  items: {
    type: Array,
    required: true
  },
  statusFilter: {
    type: String,
    default: 'all'
  }
})

const emit = defineEmits(['update:statusFilter', 'view'])

const statusOptions = [
  { value: 'all', label: 'Todos', icon: null, activeClass: 'bg-primary-500 text-white' },
  { value: 'aprovado', label: 'Aprovados', icon: IconCheckCircle, activeClass: 'bg-green-500 text-white' },
  { value: 'rejeitado', label: 'Rejeitados', icon: IconXCircle, activeClass: 'bg-red-500 text-white' },
  { value: 'pendente', label: 'Pendentes', icon: IconClock, activeClass: 'bg-yellow-500 text-white' },
  { value: 'cancelado', label: 'Cancelados', icon: IconTrash, activeClass: 'bg-gray-500 text-white' },
  { value: 'remocao', label: 'Remoção', icon: IconTrash, activeClass: 'bg-orange-500 text-white' },
  { value: 'alteracao', label: 'Alteração', icon: IconPencil, activeClass: 'bg-blue-500 text-white' },
  { value: 'adicao', label: 'Adição', icon: IconPlus, activeClass: 'bg-purple-500 text-white' }
]

const filteredItems = computed(() => {
  if (props.statusFilter === 'all') return props.items
  return props.items.filter(item => 
    item.status.toLowerCase() === props.statusFilter.toLowerCase()
  )
})

const getCurrentStatusLabel = computed(() => {
  const option = statusOptions.find(opt => opt.value === props.statusFilter)
  return option ? option.label.toLowerCase() : 'selecionado'
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pt-BR')
}

const getTipoClass = (tipo) => {
  const classes = {
    'Remoção': 'bg-orange-100 dark:bg-orange-900/30 text-orange-700 dark:text-orange-300',
    'Alteração': 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-300',
    'Adição': 'bg-purple-100 dark:bg-purple-900/30 text-purple-700 dark:text-purple-300'
  }
  return classes[tipo] || 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300'
}

const getStatusClass = (status) => {
  const classes = {
    'Aprovado': 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300',
    'Rejeitado': 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300',
    'Pendente': 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300',
    'Cancelado': 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300'
  }
  return classes[status] || 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300'
}

const getStatusIcon = (status) => {
  const icons = {
    'Aprovado': IconCheckCircle,
    'Rejeitado': IconXCircle,
    'Pendente': IconClock,
    'Cancelado': IconTrash
  }
  return icons[status] || IconClock
}
</script>