// Caminho: backend/BillingService/Models/Unidade.cs
// (Arquivo renomeado de Operacao.cs)

using System.ComponentModel.DataAnnotations;
using BillingService.Models; // <-- 1. MUDANÇA: Usando o namespace local

namespace BillingService.Models
{
    // 2. MUDANÇA: Renomeado para "Unidade" (Roadmap 4.1)
    // 3. MUDANÇA: Herda do BaseEntity local (para ganhar Id e TenantId)
    public class Unidade : BaseEntity 
    {
        [Required]
        public string Nome { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; } // O ID do "dono/gerente" da operação (do AuthService)

        public decimal? ProjecaoFaturamento { get; set; }
        public string? Descricao { get; set; }
        public string? Endereco { get; set; }
        
        [Required]
        public decimal MetaMensal { get; set; }
        
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool IsAtiva { get; set; } = true;

        // --- 4. NOVOS CAMPOS (Baseado nos formulários de papel) ---
        // (Isso permite que o Frontend saiba quais campos mostrar)
        
        public bool TemMensalistas { get; set; } = false; // Você já tinha este
        public bool AceitaCartaoCredito { get; set; } = true;
        public bool AceitaCartaoDebito { get; set; } = true;
        public bool AceitaPix { get; set; } = true;
        public bool AceitaSemParar { get; set; } = false;
        public bool LancaBoletosMensalista { get; set; } = false; // Para o Supervisor
        public bool LancaAtm { get; set; } = false; // Para o Supervisor


        // --- 5. REMOVIDO (Crítico) ---
        // A ICollection<FaturamentoParcial> foi removida.
        // Na v2.0, o FaturamentoParcial é que tem o "UnidadeId".
        // Manter isso aqui quebra a lógica do Repository que corrigimos.
    }
}