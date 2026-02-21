// composables/mensalistas/useMensalistasValidacao.js
import { computed } from 'vue';

export function useMensalistasValidacao(store) {
  const validarCPF = (cpf) => {
    return store.validarCPF(cpf);
  };

  const verificarDuplicidadeCPF = (cpf, mensalistaId = null) => {
    return store.verificarDuplicidadeCPF(cpf, mensalistaId);
  };

  const validarFormulario = (formData) => {
    const erros = [];

    if (!formData.nome?.trim()) {
      erros.push('Nome é obrigatório');
    }

    if (formData.cpf && !validarCPF(formData.cpf)) {
      erros.push('CPF inválido');
    }

    if (formData.cpf && verificarDuplicidadeCPF(formData.cpf, store.mensalistaAtual?.id)) {
      erros.push('CPF já cadastrado');
    }

    if (!formData.valorMensalidade || formData.valorMensalidade <= 0) {
      erros.push('Valor da mensalidade deve ser maior que zero');
    }

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      erros.push('E-mail inválido');
    }

    return {
      valido: erros.length === 0,
      erros,
    };
  };

  const validarValorMensalidade = (valor) => {
    return valor && valor > 0;
  };

  const mascaras = {
    cpf: (value) => {
      return value
        .replace(/\D/g, '')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
        .substring(0, 14);
    },
    telefone: (value) => {
      return value
        .replace(/\D/g, '')
        .replace(/(\d{2})(\d)/, '($1) $2')
        .replace(/(\d{4})(\d)/, '$1-$2')
        .replace(/(\d{4})-(\d)(\d{4})/, '$1$2-$3')
        .substring(0, 15);
    },
    valor: (value) => {
      const valor = value.replace(/\D/g, '');
      return (parseInt(valor) / 100).toFixed(2).replace('.', ',');
    },
  };

  return {
    validarCPF,
    verificarDuplicidadeCPF,
    validarFormulario,
    validarValorMensalidade,
    mascaras,
  };
}