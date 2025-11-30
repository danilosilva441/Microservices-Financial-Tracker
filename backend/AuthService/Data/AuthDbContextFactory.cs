// Caminho: backend/AuthService/Data/AuthDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AuthService.Data;
using Microsoft.AspNetCore.Http; // 1. IMPORTANTE: Adicionar este using

namespace AuthService.Data
{
    // Esta classe existe APENAS para a ferramenta dotnet ef migrations
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            // 1. Configura para ler o appsettings.json do projeto
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // Lê o appsettings.json
                .AddJsonFile("appsettings.Development.json", optional: true) // Sobrepõe com o de Dev
                .Build();

            // 2. Pega a string de conexão
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada no appsettings.json.");
            }

            // 3. Cria as opções e o DbContext manualmente
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // 4. CORREÇÃO (CS7036): Passa 'null' para o httpContextAccessor.
            //    (Durante a migração, não há HttpContext)
            return new AuthDbContext(optionsBuilder.Options, null);
        }
    }
}