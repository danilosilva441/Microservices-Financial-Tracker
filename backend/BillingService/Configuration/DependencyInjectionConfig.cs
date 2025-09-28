using BillingService.Repositories;
using BillingService.Services;

namespace BillingService.Configuration;

public static class DependencyInjectionConfig
{
    // Método de extensão que adiciona todas as injeções de dependência do BillingService
    public static IServiceCollection AddBillingServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<OperacaoRepository>();
        services.AddScoped<FaturamentoRepository>();
        services.AddScoped<MetaRepository>();
        services.AddScoped<EmpresaRepository>();
        services.AddScoped<MensalistaRepository>();

        // Services
        services.AddScoped<OperacaoService>();
        services.AddScoped<FaturamentoService>();
        services.AddScoped<MetaService>();
        services.AddScoped<EmpresaService>();
        services.AddScoped<MensalistaService>();

        return services;
    }
}