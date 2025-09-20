using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class Role
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    // Propriedade de navegação para a relação muitos-para-muitos
    public ICollection<User> Users { get; set; } = new List<User>();
}