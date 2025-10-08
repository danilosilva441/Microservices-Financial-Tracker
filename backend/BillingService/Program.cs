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

// --- 2. Configuração do Banco de Dados (Dinâmica) ---
string connectionString;
// As variáveis de ambiente do Railway (PGHOST, etc.) são lidas automaticamente pelo .NET
var pgHost = builder.Configuration["PGHOST"]; 

if (!string.IsNullOrEmpty(pgHost))
{
    // Ambiente de produção (Railway)
    Console.WriteLine("📡 BillingService: Conectando ao PostgreSQL do Railway...");
    var pgPort = builder.Configuration["PGPORT"];
    var pgUser = builder.Configuration["PGUSER"];
    var pgPassword = builder.Configuration["PGPASSWORD"];
    var pgDatabase = "billing_db"; // Banco de dados específico para este serviço
    connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};";
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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    db.Database.Migrate();
}

app.Run();