import { ref, computed, watch } from 'vue';
import { useSolicitacoesStore } from '@/stores/solicitacoes.store';
import { TipoSolicitacaoEnum } from '@/stores/solicitacoes.store';

export function useSolicitacaoForm() {
  const store = useSolicitacoesStore();

  const formData = ref({
    tipo: '',
    faturamentoParcialId: '',
    motivo: '',
    dadosAntigos: null,
    dadosNovos: null,
    justificativa: '',
  });

  const errors = ref({});
  const etapaAtual = ref(1);
  const totalEtapas = ref(3);

  const validacoes = {
    tipo: (value) => {
      if (!value) return 'Tipo de solicitação é obrigatório';
      return '';
    },
    motivo: (value) => {
      if (!value) return 'Motivo é obrigatório';
      if (value.length < 10) return 'Motivo deve ter pelo menos 10 caracteres';
      return '';
    },
    faturamentoParcialId: (value) => {
      if (!value) return 'Faturamento parcial é obrigatório';
      return '';
    },
  };

  const validarCampo = (campo) => {
    if (validacoes[campo]) {
      errors.value[campo] = validacoes[campo](formData.value[campo]);
      return !errors.value[campo];
    }
    return true;
  };

  const validarFormulario = () => {
    const campos = ['tipo', 'motivo', 'faturamentoParcialId'];
    let valido = true;
    
    campos.forEach(campo => {
      if (!validarCampo(campo)) {
        valido = false;
      }
    });

    // Validações específicas por tipo
    if (formData.value.tipo !== 'OUTROS') {
      if (!formData.value.dadosAntigos || !formData.value.dadosNovos) {
        errors.value.dados = 'Dados antigos e novos são obrigatórios';
        valido = false;
      }
    }

    return valido;
  };

  const tiposDisponiveis = computed(() => {
    return TipoSolicitacaoEnum.getAll();
  });

  const camposPorTipo = computed(() => {
    switch (formData.value.tipo) {
      case 'AJUSTE_VALOR':
        return ['valorAntigo', 'valorNovo'];
      case 'AJUSTE_METODO_PAGAMENTO':
        return ['metodoAntigo', 'metodoNovo'];
      case 'AJUSTE_ORIGEM':
        return ['origemAntiga', 'origemNova'];
      case 'AJUSTE_HORARIO':
        return ['horarioAntigo', 'horarioNovo'];
      case 'EXCLUSAO_LANCAMENTO':
        return ['confirmacaoExclusao'];
      default:
        return [];
    }
  });

  const podeAvancar = computed(() => {
    switch (etapaAtual.value) {
      case 1:
        return formData.value.tipo && formData.value.faturamentoParcialId;
      case 2:
        return formData.value.motivo && formData.value.motivo.length >= 10;
      case 3:
        return true;
      default:
        return false;
    }
  });

  const avancarEtapa = () => {
    if (etapaAtual.value < totalEtapas.value && podeAvancar.value) {
      etapaAtual.value++;
    }
  };

  const voltarEtapa = () => {
    if (etapaAtual.value > 1) {
      etapaAtual.value--;
    }
  };

  const resetarForm = () => {
    formData.value = {
      tipo: '',
      faturamentoParcialId: '',
      motivo: '',
      dadosAntigos: null,
      dadosNovos: null,
      justificativa: '',
    };
    errors.value = {};
    etapaAtual.value = 1;
  };

  const enviarFormulario = async () => {
    if (!validarFormulario()) {
      return { success: false, errors: errors.value };
    }

    return await store.criarSolicitacao(formData.value);
  };

  // Watch para limpar erros quando o campo muda
  watch(formData, () => {
    errors.value = {};
  }, { deep: true });

  return {
    formData,
    errors,
    etapaAtual,
    totalEtapas,
    tiposDisponiveis,
    camposPorTipo,
    podeAvancar,
    validarCampo,
    validarFormulario,
    avancarEtapa,
    voltarEtapa,
    resetarForm,
    enviarFormulario,
  };
}