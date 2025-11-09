using AuthService.Configuration;
using AuthService.Data;
using AuthService.Repositories; 
using AuthService.Services; 
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração do CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://apigateway-production-de54.up.railway.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// --- 2. Configuração do Banco de Dados ---
string? connectionString;
var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("📡 AuthService: Conectando ao PostgreSQL do Railway...");
    connectionString = ConvertDatabaseUrlToConnectionString(databaseUrl);
}
else
{
    Console.WriteLine("💻 AuthService: Conectando ao PostgreSQL local...");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("String de conexão com o banco de dados não foi encontrada.");

Console.WriteLine($"🔗 String de conexão: {connectionString.Replace("Password=", "Password=*****")}");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. Configuração Modular de Autenticação ---
builder.Services.AddAuthConfiguration(builder.Configuration);

// --- 4. REGISTRO DOS NOVOS SERVIÇOS (Injeção de Dependência) ---
// Adiciona os repositórios e serviços ao contêiner de DI
// Usamos AddScoped para que sejam criados uma vez por requisição HTTP
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();


// --- 5. Outros Serviços ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 6. Pipeline de Middlewares ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- 7. Aplica as migrations na inicialização ---
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        Console.WriteLine("🔄 Aplicando migrations do banco de dados...");
        db.Database.Migrate();
        Console.WriteLine("✅ Migrations aplicadas com sucesso!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao aplicar migrations: {ex.Message}");
    throw;
}

app.Run();

// --- Função para converter DATABASE_URL do Railway ---
static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        var host = uri.Host;
        var port = uri.Port;
        var database = uri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Falha ao converter DATABASE_URL: {ex.Message}");
    }
}