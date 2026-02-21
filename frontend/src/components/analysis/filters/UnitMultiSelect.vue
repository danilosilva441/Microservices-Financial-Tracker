<!--
 * src/components/analysis/filters/UnitMultiSelect.vue
 * UnitMultiSelect.vue
 *
 * A Vue component that provides a multi-select dropdown for filtering by units in the financial analysis dashboard.
 * It includes search functionality, select all/clear options, and displays the selected units in a user-friendly way.
 * The component emits events to update the selected units in the parent component.
 -->
<template>
  <div class="relative">
    <button
      @click="isOpen = !isOpen"
      class="flex items-center justify-between w-full px-4 py-2 text-left bg-white border border-gray-300 rounded-lg sm:w-64 dark:border-gray-600 dark:bg-gray-800"
      :class="{ 'ring-2 ring-primary-500': isOpen }"
    >
      <span class="truncate">
        {{ selectedLabel }}
      </span>
      <IconChevronDown class="w-5 h-5 text-gray-400" :class="{ 'transform rotate-180': isOpen }" />
    </button>

    <div
      v-if="isOpen"
      v-click-outside="() => isOpen = false"
      class="absolute z-50 w-full mt-2 overflow-hidden bg-white border border-gray-200 rounded-lg shadow-xl sm:w-64 dark:bg-gray-800 dark:border-gray-700 max-h-96"
    >
      <div class="p-2 border-b border-gray-200 dark:border-gray-700">
        <input
          type="text"
          v-model="search"
          placeholder="Buscar unidades..."
          class="w-full px-3 py-2 text-sm text-gray-900 bg-white border border-gray-300 rounded-lg dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
      </div>

      <div class="overflow-y-auto max-h-60">
        <div
          v-for="unit in filteredUnits"
          :key="unit.id"
          @click="toggleUnit(unit.id)"
          class="flex items-center gap-3 px-4 py-2 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <input
            type="checkbox"
            :checked="isSelected(unit.id)"
            class="border-gray-300 rounded text-primary-600 focus:ring-primary-500"
            @change="toggleUnit(unit.id)"
          />
          <span class="text-sm text-gray-700 dark:text-gray-300">{{ unit.name }}</span>
        </div>

        <div v-if="filteredUnits.length === 0" class="px-4 py-3 text-center text-gray-500 dark:text-gray-400">
          Nenhuma unidade encontrada
        </div>
      </div>

      <div class="flex gap-2 p-2 border-t border-gray-200 dark:border-gray-700">
        <button
          @click="selectAll"
          class="flex-1 px-3 py-1.5 text-xs font-medium bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600"
        >
          Selecionar Todos
        </button>
        <button
          @click="clearAll"
          class="flex-1 px-3 py-1.5 text-xs font-medium bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600"
        >
          Limpar
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import IconChevronDown from '@/components/icons/ChevronDownIcon.vue'
import { vClickOutside } from '@/directives/clickOutside'

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  },
  units: {
    type: Array,
    required: true
  }
})

const emit = defineEmits(['update:modelValue'])

const isOpen = ref(false)
const search = ref('')

const filteredUnits = computed(() => {
  if (!search.value) return props.units
  return props.units.filter(unit => 
    unit.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

const selectedLabel = computed(() => {
  if (props.modelValue.length === 0) return 'Todas as unidades'
  if (props.modelValue.length === 1) {
    const unit = props.units.find(u => u.id === props.modelValue[0])
    return unit ? unit.name : '1 unidade selecionada'
  }
  return `${props.modelValue.length} unidades selecionadas`
})

const isSelected = (id) => props.modelValue.includes(id)

const toggleUnit = (id) => {
  if (isSelected(id)) {
    emit('update:modelValue', props.modelValue.filter(v => v !== id))
  } else {
    emit('update:modelValue', [...props.modelValue, id])
  }
}

const selectAll = () => {
  emit('update:modelValue', props.units.map(u => u.id))
  isOpen.value = false
}

const clearAll = () => {
  emit('update:modelValue', [])
  isOpen.value = false
}
</script>