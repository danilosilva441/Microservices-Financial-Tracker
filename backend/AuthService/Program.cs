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

// --- 2. Configuração do Banco de Dados ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback para variáveis de ambiente individuais (Railway)
    var pgHost = Environment.GetEnvironmentVariable("PGHOST");
    if (!string.IsNullOrEmpty(pgHost))
    {
        var pgPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
        var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE") ?? "auth_db";
        var pgUser = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
        var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");
        
        connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};SSL Mode=Require;Trust Server Certificate=true";
        Console.WriteLine("📡 Conectando ao PostgreSQL do Railway...");
    }
    else
    {
        Console.WriteLine("💻 Conectando ao PostgreSQL local...");
    }
}
else
{
    Console.WriteLine("💻 Conectando ao PostgreSQL via Connection String...");
}

// Log para debug (remova em produção)
Console.WriteLine($"Connection String: {connectionString?.Replace("Password=", "Password=***")}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string não configurada");
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