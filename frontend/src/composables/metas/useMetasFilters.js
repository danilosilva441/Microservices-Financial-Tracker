// composables/metas/useMetasFilters.js
export function useMetasFilters() {
  
  function aplicarFiltros(metas, filtros) {
    let resultado = [...metas];
    
    if (filtros.ano) {
      resultado = resultado.filter(m => m.ano === filtros.ano);
    }
    
    if (filtros.mes) {
      resultado = resultado.filter(m => m.mes === filtros.mes);
    }
    
    if (filtros.busca) {
      const busca = filtros.busca.toLowerCase();
      resultado = resultado.filter(m => 
        m.descricao?.toLowerCase().includes(busca) ||
        m.valorAlvo?.toString().includes(busca)
      );
    }
    
    return resultado;
  }

  function ordenarMetas(metas, ordenacao) {
    const ordenado = [...metas];
    
    switch (ordenacao) {
      case 'periodo_asc':
        ordenado.sort((a, b) => (a.ano * 100 + a.mes) - (b.ano * 100 + b.mes));
        break;
      case 'valor_desc':
        ordenado.sort((a, b) => b.valorAlvo - a.valorAlvo);
        break;
      case 'valor_asc':
        ordenado.sort((a, b) => a.valorAlvo - b.valorAlvo);
        break;
      case 'periodo_desc':
      default:
        ordenado.sort((a, b) => (b.ano * 100 + b.mes) - (a.ano * 100 + a.mes));
        break;
    }
    
    return ordenado;
  }

  function limparFiltros(unidadeId) {
    return {
      unidadeId,
      ano: new Date().getFullYear(),
      mes: null,
      busca: '',
      ordenacao: 'periodo_desc',
    };
  }

  function aplicarFiltrosAvancados(filtrosAtuais, novosFiltros) {
    return { ...filtrosAtuais, ...novosFiltros };
  }

  return {
    aplicarFiltros,
    ordenarMetas,
    limparFiltros,
    aplicarFiltrosAvancados,
  };
}