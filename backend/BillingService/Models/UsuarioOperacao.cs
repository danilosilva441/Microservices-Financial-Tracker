using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingService.Models;

public class UsuarioOperacao
{
    // Chave estrangeira para o Usuário
    // Usamos Guid pois é o tipo do Id no AuthService
    public Guid UserId { get; set; }

    // Chave estrangeira para a Operação
    public Guid OperacaoId { get; set; }
    public Operacao Operacao { get; set; } = null!;
}