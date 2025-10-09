using System.Text;
using BillingService.Configuration;
using BillingService.Data;
using BillingService.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração do CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://SEU_DOMINIO_DA_RAILWAY_AQUI.up.railway.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Configuração do Banco de Dados (Corrigida) ---
string connectionString;

// Verifica primeiro a DATABASE_URL do Railway (formato padrão)
var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Ambiente de produção (Railway) - Converte DATABASE_URL para formato Npgsql
    Console.WriteLine("📡 BillingService: Conectando ao PostgreSQL do Railway...");
    connectionString = ConvertDatabaseUrlToConnectionString(databaseUrl, "billing_db");
}
else
{
    // Ambiente de desenvolvimento local
    Console.WriteLine("💻 BillingService: Conectando ao PostgreSQL local...");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("String de conexão com o banco de dados não foi encontrada.");
}

Console.WriteLine($"🔗 String de conexão: {connectionString.Replace("Password=", "Password=*****")}");

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. Injeção de Dependência ---
builder.Services.AddBillingServices();

// --- 4. Autenticação (Unificada) ---
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey)) throw new InvalidOperationException("Chave JWT não configurada.");

builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection(ApiKeySettings.SectionName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, null);

// --- 5. Autorização ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtOrApiKey", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, ApiKeyAuthenticationHandler.SchemeName);
        policy.RequireAuthenticatedUser();
    });
});

// --- 6. Outros Serviços ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 7. Pipeline de Middlewares ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Aplica as migrations na inicialização
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
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
static string ConvertDatabaseUrlToConnectionString(string databaseUrl, string databaseName = null)
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        
        var host = uri.Host;
        var port = uri.Port;
        var database = !string.IsNullOrEmpty(databaseName) ? databaseName : uri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Falha ao converter DATABASE_URL: {ex.Message}");
    }
}