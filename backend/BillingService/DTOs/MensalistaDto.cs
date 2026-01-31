using System;
using System.ComponentModel.DataAnnotations;

namespace BillingService.DTOs
{
    public class MensalistaDto
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public string? CPF { get; set; }
        public decimal ValorMensalidade { get; set; }
        public bool IsAtivo { get; set; }
        public Guid? EmpresaId { get; set; }
        public Guid OperacaoId { get; set; }
        public Guid CriadoPor { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class UpdateMensalistaDto
    {
        [Required]
        public required string Nome { get; set; }
        public string? CPF { get; set; }
        [Required]
        public decimal ValorMensalidade { get; set; }
        public bool IsAtivo { get; set; }
        public Guid? EmpresaId { get; set; }
    }
}