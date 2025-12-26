// formatters.js - ADICIONE esta função:

/**
 * Formata um número como porcentagem
 * @param {number} value - Valor a ser formatado (ex: 0.15 para 15%)
 * @param {number} decimals - Casas decimais (padrão: 1)
 * @returns {string} Valor formatado como porcentagem
 */
export const formatPercentage = (value, decimals = 1) => {
  if (value === null || value === undefined) return '0%';
  
  const formatted = (value * 100).toFixed(decimals);
  return `${formatted}%`;
};

/**
 * Formata um número como porcentagem com sinal
 * @param {number} value - Valor a ser formatado (ex: 0.15 para +15%)
 * @param {number} decimals - Casas decimais (padrão: 1)
 * @returns {string} Valor formatado como porcentagem com sinal
 */
export const formatPercentageWithSign = (value, decimals = 1) => {
  if (value === null || value === undefined) return '0%';
  
  const sign = value > 0 ? '+' : '';
  const formatted = (value * 100).toFixed(decimals);
  return `${sign}${formatted}%`;
};

// Função formatCurrency já existente
export const formatCurrency = (value) => {
  if (value === null || value === undefined) return 'R$ 0,00';
  
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value);
};