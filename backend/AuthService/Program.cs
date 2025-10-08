using AuthService.Configuration;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Definição da Política de CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Configuração do Banco de Dados (com suporte ao Railway) ---

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Se não houver ConnectionString no appsettings.json, tenta usar DATABASE_URL (Railway)
if (string.IsNullOrEmpty(connectionString))
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            var host = uri.Host;
            var port = uri.Port;
            var database = uri.AbsolutePath.Trim('/');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";

            connectionString =
                $"Host={host};Port={port};Database={database};Username={username};Password={password};Ssl Mode=Require;Trust Server Certificate=true";

            Console.WriteLine("📡 Conectando ao PostgreSQL via DATABASE_URL do Railway...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao processar DATABASE_URL: {ex.Message}");
            throw;
        }
    }
    else
    {
        Console.WriteLine("⚠️ Nenhuma ConnectionString ou DATABASE_URL encontrada!");
    }
}
else
{
    Console.WriteLine("💻 Conectando ao PostgreSQL via Connection String local...");
}

// Log para debug (esconde a senha)
Console.WriteLine($"Connection String: {connectionString?.Replace("Password=", "Password=***")}");

// Validação final
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ Connection string não configurada.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. Configuração Modular de Autenticação ---
builder.Services.AddAuthConfiguration(builder.Configuration);

// --- 4. Configurações Padrão ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 5. Aplica migrations automaticamente ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao aplicar migrations: {ex.Message}");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- 6. Middlewares (A Ordem é Importante) ---
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
