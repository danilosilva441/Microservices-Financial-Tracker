// Normaliza listas vindas do .NET que retornam { $values: [...] }
export function unwrapDotNetList(data) {
  if (!data) return [];
  if (Array.isArray(data)) return data;
  if (Array.isArray(data.$values)) return data.$values;
  return [];
}
