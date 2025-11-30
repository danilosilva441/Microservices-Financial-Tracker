using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class CreateTenantUserDto
    {
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