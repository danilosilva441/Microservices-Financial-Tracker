// Caminho: backend/BillingService/Models/Fatura.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity (ganha Id e TenantId)
    public class Fatura : BaseEntity
    {
        // Id e TenantId vêm da BaseEntity
        
        public int MesReferencia { get; set; }
        public int AnoReferencia { get; set; }
        
        [Required]
        public decimal ValorTotal { get; set; }
        
        public DateTime DataVencimento { get; set; }
        
        [Required]
        public string Status { get; set; } = "PENDENTE"; // PENDENTE, PAGO, ATRASADO
        
        public DateTime? DataPagamento { get; set; }

        // --- 3. MUDANÇA (v2.0) ---
        // Substitui EmpresaId (v1.0) por MensalistaId (v2.0)
        [Required]
        public Guid MensalistaId { get; set; }

        [ForeignKey("MensalistaId")]
        public virtual Mensalista Mensalista { get; set; } = null!;

        // --- 4. REMOVIDO (v1.0) ---
        // public Guid EmpresaId { get; set; }
        // public Empresa Empresa { get; set; } = null!;
    }
}