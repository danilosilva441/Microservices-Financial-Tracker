// Caminho: backend/AuthService/DTO/CreateTenantUserDto.cs
using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class CreateTenantUserDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O perfil (role) é obrigatório.")]
        public string RoleName { get; set; } = string.Empty; // Ex: "Supervisor", "Lider", "Operador"
    }
}