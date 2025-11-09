using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Necessário para ForeignKey

namespace AuthService.Models
{
    public class User : SharedKernel.BaseEntity
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

       
        // Chave Estrangeira para a Hierarquia (O chefe deste usuário)
        // O '?' permite que seja nulo (ex: o Gerente/Dono não se reporta a ninguém)
        public Guid? ReportsToUserId { get; set; }

        // --- PROPRIEDADES DE NAVEGAÇÃO ---

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; } // <-- Também deve ser anulável

        [ForeignKey("ReportsToUserId")]
        public virtual User? ReportsToUser { get; set; } // Pode ser nulo

        // Relação um-para-muitos de hierarquia
        public virtual ICollection<User> Subordinates { get; set; } = new List<User>();

        // Relação muitos-para-muitos com Role
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}