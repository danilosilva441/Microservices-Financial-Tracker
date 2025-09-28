using AuthService.Configuration;        
using AuthService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços do DbContext e CORS (se tiver)
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ... (código do CORS, se você o adicionou aqui)

// 2. CHAME NOSSO NOVO MÉTODO DE CONFIGURAÇÃO EM UMA ÚNICA LINHA!
builder.Services.AddAuthConfiguration(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // ... (configuração do Json para ciclos, se necessário)
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica as migrations automaticamente
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

// A ordem aqui é importante
app.UseCors("_myAllowSpecificOrigins"); // Se você tiver uma política de CORS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();