using System.Text.Json.Serialization;

namespace SharedKernel.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MetodoPagamentoEnum
{
    Dinheiro = 1,
    Pix = 2,
    CartaoCredito = 3,
    CartaoDebito = 4,
    ValeRefeicao = 5,
    Outros = 99
}