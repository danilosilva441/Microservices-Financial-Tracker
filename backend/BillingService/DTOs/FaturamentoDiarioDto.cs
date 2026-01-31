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
    /// <summary>
    /// DTO usado para retornar os dados do Faturamento Diário para o frontend.
    /// </summary>
    public class FaturamentoDiarioResponseDto
    {
        public Guid Id { get; set; }
        public Guid UnidadeId { get; set; }
        public DateOnly Data { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusCaixa { get; set; } = string.Empty; // Novo campo

        // Campos do formulário
        public decimal FundoDeCaixa { get; set; }
        public string? Observacoes { get; set; }
        public decimal? ValorAtm { get; set; }
        public decimal? ValorBoletosMensalistas { get; set; }

        // Campos de fechamento (novos)
        public decimal? ValorTotalCalculado { get; set; }
        public decimal? ValorConferido { get; set; }
        public decimal? Diferenca { get; set; }
        public string? HashAssinatura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public DateTime? DataConferencia { get; set; }
        public string? ObservacoesConferencia { get; set; }

        // Campo calculado
        public decimal ValorTotalParciais { get; set; }

        // Campos de auditoria
        public Guid? FechadoPorUserId { get; set; }
        public Guid? ConferidoPorUserId { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class FecharCaixaDto
    {
        [Required(ErrorMessage = "O valor conferido é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor conferido não pode ser negativo.")]
        public decimal ValorConferido { get; set; }

        public decimal ValorTotal { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }
        
    }

    /// <summary>
    /// DTO para conferência de caixa pelo supervisor/gerente
    /// </summary>
    public class ConferenciaCaixaDto
    {
        [Required(ErrorMessage = "O status da conferência é obrigatório.")]
        public bool Aprovado { get; set; }

        public decimal ValorTotal { get; set; }

        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }

    /// <summary>
    /// DTO para reabertura de caixa pelo admin
    /// </summary>
    public class ReabrirCaixaDto
    {
        [Required(ErrorMessage = "O motivo da reabertura é obrigatório.")]
        [MaxLength(500)]
        public string Motivo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resultado do fechamento de caixa
    /// </summary>
    public class ResultadoFechamentoDto
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public string? Hash { get; set; }
        public decimal Diferenca { get; set; }
        public decimal ValorTotalCalculado { get; set; }
        
        public decimal ValorConferido { get; set; }
    }
}