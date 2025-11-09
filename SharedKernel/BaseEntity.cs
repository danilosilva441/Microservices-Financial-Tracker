using System.ComponentModel.DataAnnotations;

namespace SharedKernel
    {
        public abstract class BaseEntity
        {
            [Key]
            public Guid Id { get; set; } = Guid.NewGuid();

            // Esta é a propriedade mágica para o multi-tenant
            public Guid TenantId { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
            public bool IsDeleted { get; set; } = false;
        }
    }
    