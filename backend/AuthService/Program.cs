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

// --- 2. Configuração do Banco de Dados (Railway ou Local) ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Tenta pegar as variáveis de ambiente do Railway
var pgHost = builder.Configuration["PGHOST"];
var pgPort = builder.Configuration["PGPORT"];
var pgUser = builder.Configuration["PGUSER"];
var pgPassword = builder.Configuration["PGPASSWORD"];
var pgDatabase = "auth_db"; // Nome do banco de dados para este serviço

// Se estiver rodando no Railway, monta a connection string dinamicamente
if (!string.IsNullOrEmpty(pgHost))
{
    connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};";
    Console.WriteLine("📡 AuthService: Conectando ao PostgreSQL do Railway...");
}
else
{
    Console.WriteLine("💻 AuthService: Conectando ao PostgreSQL local...");
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

// Aplica migrations na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Middlewares (A Ordem é Importante) ---
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();