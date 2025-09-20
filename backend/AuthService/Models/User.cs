using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }
}