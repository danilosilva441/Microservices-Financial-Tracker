/**
 * Composable para gerenciamento de Tema (Dark/Light Mode).
 * Persiste a escolha do usuário e detecta preferência do sistema.
 */
import { ref, onMounted } from 'vue';

export function useTheme() {
  const isDark = ref(false);

  /**
   * Aplica ou remove a classe 'dark' no elemento HTML root.
   * @param {boolean} value - True para Dark Mode.
   */
  const applyTheme = (value) => {
    isDark.value = value;
    const html = document.documentElement;
    
    if (value) {
      html.classList.add('dark');
    } else {
      html.classList.remove('dark');
    }
    
    localStorage.setItem('theme', value ? 'dark' : 'light');
  };

  /**
   * Alterna entre os temas.
   */
  const toggleTheme = () => {
    applyTheme(!isDark.value);
  };

  /**
   * Inicializa verificando localStorage ou preferência do sistema.
   */
  onMounted(() => {
    const savedTheme = localStorage.getItem('theme');
    
    if (savedTheme) {
      applyTheme(savedTheme === 'dark');
    } else {
      const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      applyTheme(systemPrefersDark);
    }
  });

  return {
    isDark,
    toggleTheme
  };
}