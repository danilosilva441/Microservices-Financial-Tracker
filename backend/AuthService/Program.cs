using System.Text;
using AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ---- ADICIONE ESTA LINHA ----
// Registra os serviços necessários para a autorização funcionar.
builder.Services.AddAuthorization();
// ----------------------------

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- SEÇÃO DE AUTENTICAÇÃO ATUALIZADA PARA CORRIGIR O WARNING ---
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    // Lança um erro claro se a chave não estiver no appsettings.json
    throw new InvalidOperationException("Chave JWT não está configurada no appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) // Usa a variável que já foi verificada
    };
});
// --- FIM DA SEÇÃO ATUALIZADA ---


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// A ordem aqui é importante: primeiro autentica, depois autoriza.
app.UseAuthentication();
app.UseAuthorization(); // Esta linha agora tem o serviço que ela precisa.

app.MapControllers();

app.Run();