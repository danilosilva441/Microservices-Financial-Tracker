// Caminho: SharedKernel/BaseEntity.cs
using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Entities
{
    // 1. MUDANÇA: O BaseEntity agora implementa oficialmente
    //    o contrato ITenantEntity.
    public abstract class BaseEntity : ITenantEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // 2. O '?' (anulável) é CRÍTICO para permitir Admins/Sistema
        //    (e corresponde à interface).
        public Guid? TenantId { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}