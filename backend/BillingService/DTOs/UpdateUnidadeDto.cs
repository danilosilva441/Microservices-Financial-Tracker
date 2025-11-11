// Caminho: backend/BillingService/DTOs/UpdateUnidadeDto.cs
// (Antigo UpdateOperacaoDto.cs)
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO v2.0 para atualizar Unidade
    public class UpdateUnidadeDto
    {
        [Required]
        public string Nome { get; set; } = null!;
        
        public string? Descricao { get; set; }
        public string? Endereco { get; set; }
        
        [Required]
        public decimal MetaMensal { get; set; }
        
        [Required]
        public DateTime DataInicio { get; set; }
        
        public DateTime? DataFim { get; set; }
    }
}