using System.Text;
using BillingService.Configuration; // <-- A linha-chave que chama o arquivo de correção
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
        policy.WithOrigins("http://localhost:5173", "https://apigatewayh.up.railway.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Configuração do Banco de Dados (Dinâmica) ---
string? connectionString;
var databaseUrl = builder.Configuration["DATABASE_URL"];

Console.WriteLine($"🌍 DATABASE_URL detectada: {databaseUrl}");

if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("📡 BillingService: Conectando ao PostgreSQL do Railway...");
    connectionString = ConvertDatabaseUrlToConnectionString(databaseUrl);
}
else
{
    Console.WriteLine("💻 BillingService: Conectando ao PostgreSQL local...");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("String de conexão com o banco de dados não foi encontrada.");
}

Console.WriteLine($"🔗 BillingService - String de conexão: {connectionString.Split("Password=")[0]}Password=*****");

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. Injeção de Dependência ---
// Esta linha chama o arquivo ServiceCollectionExtensions.cs (o arquivo que vamos corrigir)
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

// (A CORREÇÃO ESTÁ AQUI)
// Adiciona o serviço que lê o HttpContext (necessário para o DbContext v2.1)
builder.Services.AddHttpContextAccessor();

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
        Console.WriteLine("🔄 BillingService: Aplicando migrations...");
        db.Database.Migrate();
        Console.WriteLine("✅ BillingService: Migrations aplicadas com sucesso!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao aplicar migrations no BillingService: {ex.Message}");
    throw;
}

app.Run();

// --- Função Auxiliar para Converter a DATABASE_URL do Railway ---
static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    var host = uri.Host;
    var port = uri.Port;
    var username = userInfo[0];
    var password = userInfo[1];
    var database = "billing_db";

    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}