using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Entities; 

namespace AuthService.Models
{
    public class User : BaseEntity
    {
        // --- NOVOS CAMPOS ---
        public string FullName { get; set; } = null!; // Nome para relatórios e telas

        public bool IsActive { get; set; } = true; // Bloquear acesso sem deletar o user

        [Phone]
        public string? PhoneNumber { get; set; } // Opcional: contato do funcionário
        // --------------------

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        // Chave Estrangeira para a Hierarquia
        public Guid? ReportsToUserId { get; set; }

        // --- PROPRIEDADES DE NAVEGAÇÃO ---
        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey("ReportsToUserId")]
        public virtual User? ReportsToUser { get; set; }

        public virtual ICollection<User> Subordinates { get; set; } = new List<User>();
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}