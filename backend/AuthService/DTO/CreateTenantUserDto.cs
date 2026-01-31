using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class CreateTenantUserDto
    {
        // --- NOVOS CAMPOS ---
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 80 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string? PhoneNumber { get; set; } // Opcional
        // --------------------

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "A senha deve conter letras maiúsculas, minúsculas e números")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Perfil (Role) é obrigatório")]
        public string RoleName { get; set; } = string.Empty;
    }
}