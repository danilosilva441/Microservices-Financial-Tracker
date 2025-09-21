/**
 * Formata um número como moeda.
 * @param {number | null | undefined} value O valor numérico.
 * @param {string | null | undefined} currency O código da moeda (ex: 'BRL', 'USD').
 * @returns {string} O valor formatado ou uma string vazia se os dados forem inválidos.
 */
export function formatCurrency(value, currency) {
  // 1. Verifica se o valor é um número válido
  if (typeof value !== 'number') {
    return ''; // Retorna vazio se o valor não for um número
  }

  // 2. Garante que temos um código de moeda válido, usando 'BRL' como padrão.
  const validCurrency = currency || 'BRL';

  // 3. Tenta formatar e captura qualquer erro
  try {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: validCurrency,
    }).format(value);
  } catch (error) {
    console.error('Erro ao formatar moeda:', error);
    // Retorna o valor original com a moeda se a formatação falhar
    return `${validCurrency} ${value.toFixed(2)}`;
  }
}