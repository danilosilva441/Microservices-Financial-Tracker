// composables/useTheme.js
import { ref, onMounted, onUnmounted } from 'vue'

export function useTheme() {
  const isDarkMode = ref(false)

  const loadTheme = () => {
    const savedTheme = localStorage.getItem('theme')
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
    isDarkMode.value = savedTheme ? savedTheme === 'dark' : prefersDark
    
    // Aplica a classe dark ao HTML
    if (isDarkMode.value) {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }

  const toggleTheme = () => {
    isDarkMode.value = !isDarkMode.value
    localStorage.setItem('theme', isDarkMode.value ? 'dark' : 'light')
    
    if (isDarkMode.value) {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }

  onMounted(() => {
    loadTheme()
    
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
    const handleThemeChange = (e) => {
      if (!localStorage.getItem('theme')) {
        isDarkMode.value = e.matches
        if (isDarkMode.value) {
          document.documentElement.classList.add('dark')
        } else {
          document.documentElement.classList.remove('dark')
        }
      }
    }
    
    mediaQuery.addEventListener('change', handleThemeChange)
    
    onUnmounted(() => {
      mediaQuery.removeEventListener('change', handleThemeChange)
    })
  })

  return {
    isDarkMode,
    toggleTheme,
    loadTheme
  }
}