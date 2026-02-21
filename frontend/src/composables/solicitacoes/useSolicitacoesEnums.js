import { 
  TipoSolicitacaoEnum, 
  StatusSolicitacaoEnum,
  AcaoRevisaoEnum 
} from '@/stores/solicitacoes.store';

export function useSolicitacoesEnums() {
  const getNomeTipo = (tipo) => {
    return TipoSolicitacaoEnum.getNome(tipo);
  };

  const getNomeStatus = (status) => {
    return StatusSolicitacaoEnum.getNome(status);
  };

  const getCorStatus = (status) => {
    return StatusSolicitacaoEnum.getCor(status);
  };

  const tiposSolicitacao = TipoSolicitacaoEnum.getAll();
  const statusSolicitacao = StatusSolicitacaoEnum.getAll();
  const acoesRevisao = Object.values(AcaoRevisaoEnum);

  return {
    // Enums
    TipoSolicitacaoEnum,
    StatusSolicitacaoEnum,
    AcaoRevisaoEnum,
    
    // Métodos de acesso
    getNomeTipo,
    getNomeStatus,
    getCorStatus,
    
    // Listas completas
    tiposSolicitacao,
    statusSolicitacao,
    acoesRevisao,
  };
}