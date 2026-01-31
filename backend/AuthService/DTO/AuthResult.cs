namespace AuthService.DTO
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static AuthResult Ok(object? data = null) => new() { Success = true, Data = data };
        public static AuthResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
}