using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class RegisterSystemUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        // Opcional: Se não passar, assumimos "Admin" ou "System"
        public string Role { get; set; } = "Admin";
    }
}