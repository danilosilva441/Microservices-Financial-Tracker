// composables/metas/useMetasUI.js
import { ref } from 'vue';

export function useMetasUI() {
  const isLoading = ref(false);
  const error = ref(null);
  const editando = ref(false);
  const modalAberto = ref(false);
  const modalTipo = ref(null); // 'criar', 'editar', 'visualizar', 'excluir'

  function setLoading(status) {
    isLoading.value = status;
  }

  function setError(mensagem) {
    error.value = mensagem;
  }

  function clearError() {
    error.value = null;
  }

  function abrirModal(tipo, dados = null) {
    modalTipo.value = tipo;
    modalAberto.value = true;
    if (dados) {
      // Armazenar dados se necessário
    }
  }

  function fecharModal() {
    modalAberto.value = false;
    modalTipo.value = null;
  }

  function resetarEstado() {
    error.value = null;
    editando.value = false;
    modalAberto.value = false;
    modalTipo.value = null;
  }

  return {
    isLoading,
    error,
    editando,
    modalAberto,
    modalTipo,
    setLoading,
    setError,
    clearError,
    abrirModal,
    fecharModal,
    resetarEstado,
  };
}