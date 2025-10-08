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
        // Adiciona a URL do seu frontend em produção (Railway)
        // Substitua 'SUA-URL-DA-RAILWAY.up.railway.app' pela sua URL real
        policy.WithOrigins("http://localhost:5173", "https://SEU_DOMINIO_DA_RAILWAY_AQUI.up.railway.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Configuração do Banco de Dados (Railway ou Local) ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var pgHost = builder.Configuration["PGHOST"];
var pgPort = builder.Configuration["PGPORT"];
var pgUser = builder.Configuration["PGUSER"];
var pgPassword = builder.Configuration["PGPASSWORD"];
var pgDatabase = "billing_db"; // Nome do banco de dados para este serviço

// Se estiver rodando no Railway, monta a connection string dinamicamente
if (!string.IsNullOrEmpty(pgHost))
{
    connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};";
    Console.WriteLine("📡 Conectando ao PostgreSQL do Railway...");
}
else
{
    Console.WriteLine("💻 Conectando ao PostgreSQL local...");
}

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. Configuração Modular de Injeção de Dependência ---
builder.Services.AddBillingServices();

// --- 4. Configuração de Autenticação (UNIFICADA) ---
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Chave JWT não está configurada.");
}
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

// --- 5. Configuração de Autorização ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtOrApiKey", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, ApiKeyAuthenticationHandler.SchemeName);
        policy.RequireAuthenticatedUser();
    });
});

// --- 6. Configurações Padrão ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migrations na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Middlewares ---
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();