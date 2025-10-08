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
    Console.WriteLine("📡 AuthService: Conectando ao PostgreSQL do Railway...");
    var pgPort = builder.Configuration["PGPORT"];
    var pgUser = builder.Configuration["PGUSER"];
    var pgPassword = builder.Configuration["PGPASSWORD"];
    var pgDatabase = "auth_db"; // Banco de dados específico para este serviço
    connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};";
}
else
{
    // Ambiente de desenvolvimento local
    Console.WriteLine("💻 AuthService: Conectando ao PostgreSQL local...");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("String de conexão com o banco de dados não foi encontrada.");
}

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

// Aplica as migrations na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

app.Run();