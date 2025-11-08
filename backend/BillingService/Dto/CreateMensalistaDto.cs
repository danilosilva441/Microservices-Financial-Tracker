// Caminho: backend/BillingService/DTOs/CreateMensalistaDto.cs
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    public class CreateMensalistaDto
    {
        [Required]
        public string Nome { get; set; } = null!;
        public string? CPF { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorMensalidade { get; set; }
        // EmpresaId (v1.0) foi removido
    }
}