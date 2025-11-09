// Caminho: backend/BillingService/DTOs/FaturamentoDiarioDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using BillingService.Models; // Importa o Enum 'RegistroStatus'

namespace BillingService.DTOs
{
    /// <summary>
    /// DTO usado pelo Líder/Operador para submeter o fechamento do dia.
    /// (Tarefa 3 - Roadmap 4.1)
    /// </summary>
    public class FaturamentoDiarioCreateDto
    {
        [Required(ErrorMessage = "A data do fechamento é obrigatória.")]
        public DateOnly Data { get; set; }

        [Required(ErrorMessage = "O fundo de caixa (troco inicial) é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O fundo de caixa não pode ser negativo.")]
        public decimal FundoDeCaixa { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }

    /// <summary>
    /// DTO usado pelo Supervisor para aprovar ou editar o fechamento.
    /// (Tarefa 3 - Roadmap 4.1)
    /// </summary>
    public class FaturamentoDiarioSupervisorUpdateDto
    {
        [Required(ErrorMessage = "O Status é obrigatório (Pendente, Aprovado ou Rejeitado).")]
        public RegistroStatus Status { get; set; }

        // Supervisor pode corrigir os campos do operador
        [Required(ErrorMessage = "O fundo de caixa (troco inicial) é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O fundo de caixa não pode ser negativo.")]
        public decimal FundoDeCaixa { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // Campos exclusivos do Supervisor
        [Range(0, double.MaxValue, ErrorMessage = "O valor do ATM não pode ser negativo.")]
        public decimal? ValorAtm { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O valor dos boletos não pode ser negativo.")]
        public decimal? ValorBoletosMensalistas { get; set; }
    }

    /// <summary>
    /// DTO usado para retornar os dados do Faturamento Diário para o frontend.
    /// (Tarefa 3 - Roadmap 4.1)
    /// </summary>
    public class FaturamentoDiarioResponseDto
    {
        public Guid Id { get; set; }
        public Guid UnidadeId { get; set; }
        public DateOnly Data { get; set; }
        public string Status { get; set; } = string.Empty; // Retorna o nome do Enum

        // Campos do formulário
        public decimal FundoDeCaixa { get; set; }
        public string? Observacoes { get; set; }
        public decimal? ValorAtm { get; set; }
        public decimal? ValorBoletosMensalistas { get; set; }

        // Campo calculado (será preenchido pelo Serviço)
        public decimal ValorTotalParciais { get; set; } // Soma de todos FaturamentoParcial.Valor
    }
}