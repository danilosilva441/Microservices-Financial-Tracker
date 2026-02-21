// composables/mensalistas/useMensalistasForm.js
import { computed, ref } from 'vue';

export function useMensalistasForm(store) {
  const mensalistaAtual = computed(() => store.mensalistaAtual);
  const editando = computed(() => store.editando);
  
  const formData = ref({
    nome: '',
    cpf: '',
    email: '',
    telefone: '',
    valorMensalidade: store.config.valorMensalidadePadrao,
    empresaId: null,
    endereco: '',
    observacoes: '',
    isAtivo: true,
  });

  const iniciarCriacao = () => {
    store.editando = false;
    resetForm();
  };

  const iniciarEdicao = (mensalista) => {
    store.editando = true;
    store.mensalistaAtual = mensalista;
    preencherForm(mensalista);
  };

  const preencherForm = (mensalista) => {
    formData.value = {
      nome: mensalista.nome || '',
      cpf: mensalista.cpf || '',
      email: mensalista.email || '',
      telefone: mensalista.telefone || '',
      valorMensalidade: mensalista.valorMensalidade || store.config.valorMensalidadePadrao,
      empresaId: mensalista.empresaId || null,
      endereco: mensalista.endereco || '',
      observacoes: mensalista.observacoes || '',
      isAtivo: mensalista.isAtivo !== undefined ? mensalista.isAtivo : true,
    };
  };

  const resetForm = () => {
    formData.value = {
      nome: '',
      cpf: '',
      email: '',
      telefone: '',
      valorMensalidade: store.config.valorMensalidadePadrao,
      empresaId: null,
      endereco: '',
      observacoes: '',
      isAtivo: true,
    };
    store.mensalistaAtual = null;
  };

  const cancelarEdicao = () => {
    store.editando = false;
    resetForm();
  };

  return {
    mensalistaAtual,
    editando,
    formData,
    iniciarCriacao,
    iniciarEdicao,
    preencherForm,
    resetForm,
    cancelarEdicao,
  };
}