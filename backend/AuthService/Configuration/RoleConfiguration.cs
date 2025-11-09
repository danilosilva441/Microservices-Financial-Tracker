using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Configuration
{
    // Esta classe configura a SUA entidade 'Role', não a 'IdentityRole'
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Define 'NormalizedName' como uma chave única para buscas
            builder.HasIndex(r => r.NormalizedName).IsUnique();
            
            builder.HasData(
                // O perfil "User" que o seu Controller procura
                new Role
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                    Name = "User",
                    NormalizedName = "USER"
                },
                new Role
                {
                    Id = Guid.Parse("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                
                // --- NOVOS PERFIS HIERÁRQUICOS (Fase 1.1) ---
                new Role
                {
                    Id = Guid.Parse("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                    Name = "Dev",
                    NormalizedName = "DEV"
                },
                new Role
                {
                    Id = Guid.Parse("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                    Name = "Gerente",
                    NormalizedName = "GERENTE"
                },
                new Role
                {
                    Id = Guid.Parse("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                    Name = "Supervisor",
                    NormalizedName = "SUPERVISOR"
                },
                new Role
                {
                    Id = Guid.Parse("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                    Name = "Lider",
                    NormalizedName = "LIDER"
                },
                new Role
                {
                    Id = Guid.Parse("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                    Name = "Operador",
                    NormalizedName = "OPERADOR"
                }
            );
        }
    }
}