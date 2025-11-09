using BillingService.Repositories;
using BillingService.Services;
using BillingService.Services.Interfaces;
using BillingService.Repositories.Interfaces;

namespace BillingService.Configuration
{
    public static class DependencyInjectionConfig
    {
        // Método de extensão que adiciona todas as injeções de dependência do BillingService
        public static IServiceCollection AddBillingServices(this IServiceCollection services)
        {
            // --- Repositories ---
            // Registra a implementação (Classe) contra o seu contrato (Interface)
            
            services.AddScoped<IUnidadeRepository, UnidadeRepository>();
            services.AddScoped<IFaturamentoParcialRepository, FaturamentoParcialRepository>(); 
            services.AddScoped<ISolicitacaoRepository, SolicitacaoRepository>();
            services.AddScoped<IMetaRepository, MetaRepository>();
            services.AddScoped<IMensalistaRepository, MensalistaRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            


            // --- Services ---
            // Registra a implementação (Classe) contra o seu contrato (Interface)

            services.AddScoped<IUnidadeService, UnidadeService>();
            services.AddScoped<IFaturamentoParcialService, FaturamentoParcialService>(); 
            services.AddScoped<ISolicitacaoService, SolicitacaoService>();
            services.AddScoped<IMetaService, MetaService>();
            services.AddScoped<IMensalistaService, MensalistaService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            

            return services;
        }
    }
}