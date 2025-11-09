// Caminho: backend/BillingService/Models/BaseEntity.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.Models
{
    // Classe "abstrata" significa que ela não pode ser usada sozinha,
    // ela DEVE ser herdada (ex: Operacao, Expense).
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        // --- INCLUSÃO CRÍTICA (Roadmap v2.0) ---
        // O ID do "Tenant" (Empresa/Unidade) do AuthService
        // que é dono deste registro.
        // O '?' (anulável) é para registros globais (se houver).
        public Guid? TenantId { get; set; }

        // --- Campos de Auditoria (Boa Prática) ---
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}