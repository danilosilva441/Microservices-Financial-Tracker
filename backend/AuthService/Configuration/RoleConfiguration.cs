using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Common; // <--- Importante: Adicionar este using

namespace AuthService.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(r => r.NormalizedName).IsUnique();
            
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                    Name = Constants.Roles.User,
                    NormalizedName = Constants.Roles.User.ToUpper() // Automático
                },
                new Role
                {
                    Id = Guid.Parse("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                    Name = Constants.Roles.Admin,
                    NormalizedName = Constants.Roles.Admin.ToUpper()
                },
                // --- NOVOS PERFIS HIERÁRQUICOS ---
                new Role
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                    Name = Constants.Roles.Dev,
                    NormalizedName = Constants.Roles.Dev.ToUpper()
                },
                new Role
                {
                    Id = Guid.Parse("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                    Name = Constants.Roles.Gerente,
                    NormalizedName = Constants.Roles.Gerente.ToUpper()
                },
                new Role
                {
                    Id = Guid.Parse("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                    Name = Constants.Roles.Supervisor,
                    NormalizedName = Constants.Roles.Supervisor.ToUpper()
                },
                new Role
                {
                    Id = Guid.Parse("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                    Name = Constants.Roles.Lider,
                    NormalizedName = Constants.Roles.Lider.ToUpper()
                },
                new Role
                {
                    Id = Guid.Parse("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                    Name = Constants.Roles.Operador,
                    NormalizedName = Constants.Roles.Operador.ToUpper()
                }
            );
        }
    }
}