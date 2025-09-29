/**
 * Formata um número como moeda.
 * @param {number | null | undefined} value O valor numérico.
 * @param {string | null | undefined} currency O código da moeda (ex: 'BRL', 'USD').
 * @returns {string} O valor formatado ou uma string vazia se os dados forem inválidos.
 */
export function formatCurrency(value) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(value);
}