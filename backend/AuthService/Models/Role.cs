using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Role : SharedKernel.BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!; // <--- MUDANÇA: 'required string' para null-safety

        // Adicionado para consistência de busca (ex: "USER")
        [Required]
        public string NormalizedName { get; set; } = null!;

        // Propriedade de navegação para a relação muitos-para-muitos
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}