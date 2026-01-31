using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class TenantProvisionDto
    {
        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da empresa deve ter no máximo 100 caracteres")]
        public string NomeDaEmpresa { get; set; } = null!;

        private string _emailDoGerente = null!;

        [Required(ErrorMessage = "O email do gerente é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string EmailDoGerente
        {
            get => _emailDoGerente;
            set => _emailDoGerente = NormalizeEmail(value);
        }

        [Required(ErrorMessage = "O nome completo do gerente é obrigatório")]
        [StringLength(150, ErrorMessage = "O nome completo deve ter no máximo 150 caracteres")]
        public string NomeCompletoGerente { get; set; } = null!;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "A senha deve conter letras maiúsculas, minúsculas e números")]
        public string SenhaDoGerente { get; set; } = null!;

        private static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;
                
            return email.Trim().ToLowerInvariant();
        }
    }
}