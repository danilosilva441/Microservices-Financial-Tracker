using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthService.Models; // 1. IMPORTANTE: Adicione este using
using SharedKernel.Entities; 
namespace AuthService.Models
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;


        // Chave Estrangeira para a Hierarquia
        public Guid? ReportsToUserId { get; set; }

        // --- PROPRIEDADES DE NAVEGAÇÃO ---

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; } // (Já estava correto)

        [ForeignKey("ReportsToUserId")]
        public virtual User? ReportsToUser { get; set; }

        public virtual ICollection<User> Subordinates { get; set; } = new List<User>();
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}