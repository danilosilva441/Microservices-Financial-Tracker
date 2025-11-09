using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    // DTO para o novo endpoint de provisionamento (Tarefa 2.4)
    public class TenantProvisionDto
    {
        [Required]
        [StringLength(100)]
        public string NomeDaEmpresa { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string EmailDoGerente { get; set; } = null!;

        [Required]
        [MinLength(8)] // Boa prática exigir senha forte
        public string SenhaDoGerente { get; set; } = null!;
    }
}