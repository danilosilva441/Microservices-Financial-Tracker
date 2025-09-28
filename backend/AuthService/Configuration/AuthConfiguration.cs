using System.Text;
using AuthService.Controllers;
using AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims; // Adicione este using

namespace AuthService.Configuration;

public static class AuthConfiguration
{
    // Este é um método de extensão. Ele adiciona o "superpoder" AddAuthConfiguration
    // à classe IServiceCollection (builder.Services).
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Chave JWT não está configurada.");
        }

        // 1. Configuração da Autenticação com JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        // 2. Configuração da Autorização
        services.AddAuthorization();

        // 3. Injeção de Dependência dos Controllers
        // (O .NET 8 faz isso automaticamente para controllers, mas podemos ser explícitos se quisermos)
        // services.AddScoped<UsersController>();
        // services.AddScoped<TokenController>();
        // services.AddScoped<AdminController>();

        return services;
    }
}