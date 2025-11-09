namespace AuthService.DTO
{
    // Uma classe padrão para retornar resultados dos nossos serviços
    public class AuthResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Data { get; set; }

        // Métodos de fábrica para criar respostas facilmente
        public static AuthResult Ok(object? data = null) => new() { Success = true, Data = data };
        public static AuthResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}