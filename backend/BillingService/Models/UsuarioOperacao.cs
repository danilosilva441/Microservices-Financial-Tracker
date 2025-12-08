// Caminho: backend/BillingService/Models/UsuarioOperacao.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. MUDANÇA: Usando o namespace local
using SharedKernel.Entities;

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity local, não do SharedKernel
    public class UsuarioOperacao : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; } 

        // --- 3. MUDANÇA (Roadmap v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        [Required]
        public Guid UnidadeId { get; set; }
        
        // --- 4. MUDANÇA (Roadmap v2.0) ---
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!; // Renomeado para Unidade

        [Required]
        public string RoleInOperation { get; set; } = "Operador"; 
    }
}