// Caminho: backend/BillingService/DTOs/UpdateUnidadeDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    // DTO v2.0 para atualizar Unidade (antigo UpdateOperacaoDto)
    public class UpdateUnidadeDto
    {
        [Required]
        public required string Nome { get; set; }
        
        public string? Descricao { get; set; }
        public string? Endereco { get; set; }
        
        // Campo 'Moeda' (v1.0) foi removido
        
        [Required]
        public decimal MetaMensal { get; set; }
        
        [Required]
        public DateTime DataInicio { get; set; }
        
        public DateTime? DataFim { get; set; }
    }
}