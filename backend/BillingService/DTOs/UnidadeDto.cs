// Caminho: backend/BillingService/DTOs/UnidadeDto.cs
// (Antigo OperacaoDto.cs)
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO v2.0 para criar Unidade
    public class UnidadeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; } = null!;
        
        public string? Descricao { get; set; }
        public string? Endereco { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MetaMensal { get; set; }
        
        [Required]
        public DateTime DataInicio { get; set; }
        
        public DateTime? DataFim { get; set; }
    }
}