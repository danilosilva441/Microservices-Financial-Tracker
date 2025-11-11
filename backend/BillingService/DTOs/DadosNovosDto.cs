// Caminho: backend/BillingService/DTOs/DadosNovosDto.cs
namespace BillingService.DTOs
{
    /// <summary>
    /// DTO Helper usado para desserializar o JSON
    /// da propriedade 'DadosNovos' em uma SolicitacaoAjuste.
    /// </summary>
    public class DadosNovosDto
    {
        // Garante que o JSON "valor" seja mapeado para "Valor"
        [System.Text.Json.Serialization.JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        // Garante que o JSON "data" seja mapeado para "Data"
        [System.Text.Json.Serialization.JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}