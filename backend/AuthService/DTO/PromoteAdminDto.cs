using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class PromoteAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}