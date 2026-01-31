using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class CreateSystemUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "Admin"; // Padrão Admin, mas pode ser "System"

        // Uma chave de segurança simples para evitar que qualquer um chame esse endpoint publicamente
        [Required]
        public string SystemCreationKey { get; set; } = null!;
    }
}