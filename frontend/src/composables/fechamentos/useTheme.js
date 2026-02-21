import { ref, computed, onMounted } from 'vue';

export function useTheme() {
  const theme = ref(localStorage.getItem('theme') || 'light');
  const isDark = computed(() => theme.value === 'dark');

  const setTheme = (newTheme) => {
    theme.value = newTheme;
    localStorage.setItem('theme', newTheme);
    
    if (newTheme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  };

  const toggleTheme = () => {
    setTheme(isDark.value ? 'light' : 'dark');
  };

  onMounted(() => {
    if (theme.value === 'dark') {
      document.documentElement.classList.add('dark');
    }
  });

  return {
    theme,
    isDark,
    setTheme,
    toggleTheme,
  };
}