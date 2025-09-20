using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    // REMOVEMOS a propriedade 'string Role'
    // E ADICIONAMOS a coleção de perfis
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}