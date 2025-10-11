using AuthService.Configuration;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração do CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        // IMPORTANTE: Substitua 'SEU_DOMINIO_DA_RAILWAY_AQUI.up.railway.app' pela sua URL real da Railway
        policy.WithOrigins("http://localhost:5173", "https://apigateway-production-de54.up.railway.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Configuração do Banco de Dados (Dinâmica) ---
string? connectionString;

// Railway fornece a string de conexão completa na variável DATABASE_URL
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

if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("String de conexão com o banco de dados não foi encontrada.");
}

// Oculta a senha nos logs para segurança
Console.WriteLine($"🔗 String de conexão usada: {connectionString.Split("Password=")[0]}Password=*****");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));


// --- 3. Configuração Modular de Autenticação ---
builder.Services.AddAuthConfiguration(builder.Configuration);

// --- 4. Outros Serviços ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 5. Pipeline de Middlewares ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Aplica as migrations na inicialização de forma segura
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        Console.WriteLine("🔄 AuthService: Aplicando migrations...");
        db.Database.Migrate();
        Console.WriteLine("✅ AuthService: Migrations aplicadas com sucesso!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao aplicar migrations no AuthService: {ex.Message}");
    throw;
}

app.Run();

// --- Função Auxiliar para Converter a DATABASE_URL do Railway ---
static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        
        var host = uri.Host;
        var port = uri.Port;
        var username = userInfo[0];
        var password = userInfo[1];
        // CORREÇÃO: Força o nome do banco de dados para ser 'auth_db'
        var database = "auth_db"; 
        
        // Adiciona SSL Mode e Trust Server Certificate, essenciais para a maioria das conexões em nuvem
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Falha ao converter DATABASE_URL: {ex.Message}", ex);
    }
}