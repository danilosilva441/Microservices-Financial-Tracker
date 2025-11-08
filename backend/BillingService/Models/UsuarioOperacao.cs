// Caminho: backend/BillingService/Models/UsuarioOperacao.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BillingService.Models; // 1. IMPORTANTE: Adiciona o using local

namespace BillingService.Models
{
    // 2. MUDANÇA: Herda do BaseEntity (ganha Id, TenantId, CreatedAt)
    public class UsuarioOperacao : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        // 3. MUDANÇA: Renomeado de OperacaoId para UnidadeId
        [Required]
        public Guid UnidadeId { get; set; }

        // 4. MUDANÇA: Renomeado de Operacao para Unidade
        [ForeignKey("UnidadeId")]
        public virtual Unidade Unidade { get; set; } = null!;

        // (Roadmap 4.1) Define o papel do usuário *dentro* desta unidade
        [Required]
        public string RoleInOperation { get; set; } = "Operador"; 
    }
}