using System.ComponentModel.DataAnnotations;
using SharedKernel.Entities;

namespace AuthService.Models
{
    public class Tenant : BaseEntity
    {

        [Required]
        [StringLength(100)]
        public string NomeDaEmpresa { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string StatusDaSubscricao { get; set; } = "Ativa"; // Valor Padrão

        public DateTime DataDeCriacao { get; set; } = DateTime.UtcNow;

        // Propriedade de Navegação: Um Tenant (Empresa) pode ter muitos usuários
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}