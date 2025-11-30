// Caminho: SharedKernel/ITenantEntity.cs
namespace SharedKernel.Entities
{
    /// <summary>
    /// Interface obrigatória para todas as entidades
    /// que pertencem a um Tenant específico.
    /// Isto é usado pelo Filtro de Query Global.
    /// </summary>
    public interface ITenantEntity
    {
        // Define que a entidade DEVE ter um TenantId.
        // O '?' (anulável) é crucial, pois o BaseEntity
        // (e utilizadores Admin) têm TenantId nulo.
        public Guid? TenantId { get; set; }
    }
}