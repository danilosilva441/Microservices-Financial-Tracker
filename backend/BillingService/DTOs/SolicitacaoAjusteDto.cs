namespace BillingService.DTOs;

public class SolicitacaoAjusteDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
    public DateTime DataSolicitacao { get; set; }

    public FaturamentoSimplesDto Faturamento { get; set; } = null!;
}

public class FaturamentoSimplesDto
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public OperacaoSimplesDto Operacao { get; set; } = null!;
}

public class OperacaoSimplesDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

