<script setup>
import { computed } from 'vue';

const props = defineProps({
  user: {
    type: Object,
    default: () => ({})
  },
  size: {
    type: String,
    default: 'medium',
    validator: (value) => ['small', 'medium', 'large', 'xlarge'].includes(value)
  },
  showName: {
    type: Boolean,
    default: false
  },
  showRole: {
    type: Boolean,
    default: false
  }
});

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'small': return 'h-8 w-8 text-xs';
    case 'large': return 'h-12 w-12 text-base';
    case 'xlarge': return 'h-16 w-16 text-xl';
    default: return 'h-10 w-10 text-sm';
  }
});

const avatarColor = computed(() => {
  const colors = [
    'bg-blue-100 text-blue-600',
    'bg-green-100 text-green-600',
    'bg-purple-100 text-purple-600',
    'bg-yellow-100 text-yellow-600',
    'bg-pink-100 text-pink-600',
    'bg-indigo-100 text-indigo-600'
  ];
  
  const name = props.user?.name || 'User';
  const charCode = name.charCodeAt(0);
  return colors[charCode % colors.length];
});

const initials = computed(() => {
  const name = props.user?.name || '';
  if (!name) return 'U';
  
  const parts = name.split(' ');
  if (parts.length >= 2) {
    return (parts[0].charAt(0) + parts[1].charAt(0)).toUpperCase();
  }
  return name.charAt(0).toUpperCase();
});

const hasImage = computed(() => {
  return !!props.user?.avatar_url || !!props.user?.photoURL;
});
</script>

<template>
  <div class="flex items-center">
    <!-- Avatar -->
    <div class="flex-shrink-0">
      <div v-if="hasImage" 
           :class="['rounded-full overflow-hidden', sizeClasses]">
        <img 
          :src="user.avatar_url || user.photoURL" 
          :alt="user.name || 'Usuário'"
          class="h-full w-full object-cover"
        />
      </div>
      
      <div v-else 
           :class="[
             'rounded-full flex items-center justify-center font-semibold',
             sizeClasses,
             avatarColor
           ]">
        {{ initials }}
      </div>
    </div>

    <!-- User Info -->
    <div v-if="showName" class="ml-3">
      <p class="text-sm font-medium text-gray-900">
        {{ user?.name || 'Usuário' }}
      </p>
      
      <p v-if="showRole" class="text-xs text-gray-500 capitalize">
        {{ user?.role || 'user' }}
      </p>
    </div>
  </div>
</template>