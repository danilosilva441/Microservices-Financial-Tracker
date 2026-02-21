// composables/metas/useMetasValidacao.js
export function useMetasValidacao() {

  function validarMeta(metaData) {
    const erros = [];
    
    if (!metaData.valorAlvo || metaData.valorAlvo <= 0) {
      erros.push('valorAlvo: O valor da meta deve ser maior que zero');
    }
    
    if (!metaData.mes || metaData.mes < 1 || metaData.mes > 12) {
      erros.push('mes: Mês inválido');
    }
    
    if (!metaData.ano || metaData.ano < 2000 || metaData.ano > 2100) {
      erros.push('ano: Ano inválido');
    }
    
    if (!metaData.unidadeId) {
      erros.push('unidadeId: Unidade é obrigatória');
    }
    
    return erros;
  }

  function verificarMetaExistente(metas, metaData, unidadeId) {
    return metas.find(m => 
      m.mes === metaData.mes && 
      m.ano === metaData.ano &&
      m.unidadeId === unidadeId
    );
  }

  function validarPeriodo(mes, ano) {
    const hoje = new Date();
    const mesAtual = hoje.getMonth() + 1;
    const anoAtual = hoje.getFullYear();
    
    if (ano < anoAtual) return true;
    if (ano > anoAtual) return false;
    return mes <= mesAtual;
  }

  return {
    validarMeta,
    verificarMetaExistente,
    validarPeriodo,
  };
}