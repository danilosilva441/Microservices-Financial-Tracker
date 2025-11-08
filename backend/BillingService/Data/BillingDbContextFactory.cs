// Caminho: backend/BillingService/Data/BillingDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BillingService.Data;

namespace BillingService.Data
{
    // Esta classe existe APENAS para a ferramenta dotnet ef migrations
    public class BillingDbContextFactory : IDesignTimeDbContextFactory<BillingDbContext>
    {
        public BillingDbContext CreateDbContext(string[] args)
        {
            // 1. Configura para ler o appsettings.json do projeto
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Lê o appsettings.json principal
                .AddJsonFile("appsettings.json") 
                // Sobrepõe com o de Desenvolvimento (se existir)
                .AddJsonFile("appsettings.Development.json", optional: true) 
                .Build();

            // 2. Pega a string de conexão
            // !!! IMPORTANTE: Verifique se o nome "DefaultConnection"
            //     é o mesmo que está no seu appsettings.json!
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada no appsettings.json.");
            }

            // 3. Cria as opções e o DbContext manualmente
            var optionsBuilder = new DbContextOptionsBuilder<BillingDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new BillingDbContext(optionsBuilder.Options);
        }
    }
}