// Caminho: backend/BillingService/Models/Meta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local
using SharedKernel.Entities;

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity (Já estava correto!)
    public class Meta : BaseEntity
    {
        // Id e TenantId vêm da BaseEntity
        
        [Required]
        public int Mes { get; set; }
        [Required]
        public int Ano { get; set; }

        [Required]
        public decimal ValorAlvo { get; set; }
        
        // --- 3. MUDANÇA (v2.0) ---
        // A Meta agora está ligada à Unidade, não ao Usuário
        [Required]
        public Guid UnidadeId { get; set; }

        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!;

        // --- 4. REMOVIDO (v1.0) ---
        // public Guid UserId { get; set; } 
    }
}