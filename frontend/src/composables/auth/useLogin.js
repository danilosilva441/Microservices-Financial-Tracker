import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

/**
 * Composable responsável por gerenciar o estado e a lógica da página de login.
 * Segue o padrão "Separation of Concerns" (Separação de Preocupações).
 * * @returns {Object} Variáveis reativas e métodos de ação.
 */
export function useLogin() {
  // 1. Dependências
  const authStore = useAuthStore();
  const router = useRouter();

  // 2. Estado Local (Reativo)
  const email = ref('');
  const password = ref('');
  const errorMessage = ref('');
  const isLoading = ref(false);

  /**
   * Executa a tentativa de login.
   * Valida, chama a store e redireciona.
   */
  const handleLogin = async () => {
    // Limpa estados anteriores
    errorMessage.value = '';
    
    // Validação básica de Frontend
    if (!email.value || !password.value) {
      errorMessage.value = 'Por favor, preencha todos os campos.';
      return;
    }

    isLoading.value = true;

    try {
      // Chama a ação da Store (que já lida com API e Token)
      const result = await authStore.login({
        email: email.value,
        password: password.value
      });

      if (result.success) {
        // Redirecionamento bem-sucedido
        router.push('/dashboard');
      } else {
        // Captura erro retornado pela store
        errorMessage.value = result.error || 'Falha na autenticação.';
      }
    } catch (err) {
      console.error('Erro crítico no login:', err);
      errorMessage.value = 'Ocorreu um erro inesperado. Tente novamente.';
    } finally {
      isLoading.value = false;
    }
  };

  return {
    email,
    password,
    errorMessage,
    isLoading,
    handleLogin
  };
}