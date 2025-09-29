namespace BillingService.DTOs;

public class CriarSolicitacaoDto
{
    public Guid FaturamentoId { get; set; }
    public required string Tipo { get; set; }
    public required string Motivo { get; set; }
    public string? DadosAntigos { get; set; }
    public string? DadosNovos { get; set; }
}

public class DadosNovosDto
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}