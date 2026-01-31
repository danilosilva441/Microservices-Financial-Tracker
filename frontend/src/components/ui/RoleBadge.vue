<script setup>
import { computed } from 'vue';

const props = defineProps({
  role: {
    type: String,
    default: 'user'
  },
  size: {
    type: String,
    default: 'medium',
    validator: (value) => ['small', 'medium', 'large'].includes(value)
  }
});

const roleConfig = computed(() => {
  const configs = {
    admin: {
      label: 'Administrador',
      color: 'bg-red-100 text-red-800',
      icon: '👑'
    },
    supervisor: {
      label: 'Supervisor',
      color: 'bg-purple-100 text-purple-800',
      icon: '👔'
    },
    manager: {
      label: 'Gerente',
      color: 'bg-blue-100 text-blue-800',
      icon: '💼'
    },
    user: {
      label: 'Usuário',
      color: 'bg-green-100 text-green-800',
      icon: '👤'
    },
    guest: {
      label: 'Convidado',
      color: 'bg-gray-100 text-gray-800',
      icon: '👋'
    }
  };

  return configs[props.role] || configs.user;
});

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'small': return 'px-2 py-0.5 text-xs';
    case 'large': return 'px-4 py-2 text-base';
    default: return 'px-3 py-1 text-sm';
  }
});
</script>

<template>
  <span 
    :class="[
      'inline-flex items-center rounded-full font-medium',
      sizeClasses,
      roleConfig.color
    ]"
  >
    <span class="mr-1.5">{{ roleConfig.icon }}</span>
    {{ roleConfig.label }}
  </span>
</template>