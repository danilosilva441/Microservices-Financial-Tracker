using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class LoginDto
    {
        private string _email = string.Empty;
        
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email
        {
            get => _email;
            set => _email = value?.ToLowerInvariant() ?? string.Empty;
        }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }
}