export function getErrorMessage(err, fallback = "Erro inesperado.") {
  if (err?.code === "ERR_NETWORK") return "Sem conexão com o servidor.";
  if (err?.response?.data?.message) return err.response.data.message;
  if (typeof err?.response?.data === "string") return err.response.data;
  return fallback;
}
