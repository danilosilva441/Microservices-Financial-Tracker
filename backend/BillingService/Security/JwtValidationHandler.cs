using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;

namespace BillingService.Security
{
    public class JwtValidationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public JwtValidationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Log para debug
            Logger.LogInformation("🔐 Iniciando validação JWT no BillingService...");

            // Verifica se o header Authorization existe
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Logger.LogInformation("❌ Header Authorization não encontrado");
                return AuthenticateResult.NoResult();
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            
            // Verifica se é um token Bearer
            if (authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                Logger.LogInformation("❌ Scheme não é Bearer ou token está vazio");
                return AuthenticateResult.NoResult();
            }

            var token = authHeader.Parameter;
            Logger.LogInformation($"🔑 Token recebido: {token.Substring(0, Math.Min(20, token.Length))}...");

            try
            {
                // Validar token com Auth Service
                var httpClient = _httpClientFactory.CreateClient();
                
                // URL do Auth Service - usa variável de ambiente ou valor padrão
                var authServiceUrl = _configuration["AUTH_SERVICE_URL"] ?? 
                                   Environment.GetEnvironmentVariable("AUTH_SERVICE_URL") ?? 
                                   "http://microservices-financial-tracker.railway.internal:8080";
                
                Logger.LogInformation($"🌐 Validando token com Auth Service: {authServiceUrl}");

                var request = new HttpRequestMessage(HttpMethod.Get, $"{authServiceUrl}/api/validate");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Timeout de 10 segundos
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                
                var response = await httpClient.SendAsync(request);
                Logger.LogInformation($"📡 Resposta do Auth Service: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Token é válido - extrair informações do usuário
                    var userData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userData?["id"]?.ToString() ?? "unknown"),
                        new Claim(ClaimTypes.Name, userData?["username"]?.ToString() ?? "unknown"),
                        new Claim("token_validated", "true")
                    };

                    // Adicionar roles se existirem
                    if (userData?.ContainsKey("roles") == true)
                    {
                        var roles = userData["roles"]?.ToString()?.Split(',');
                        if (roles != null)
                        {
                            foreach (var role in roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                            }
                        }
                    }

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    
                    Logger.LogInformation($"✅ Token validado com sucesso para usuário: {userData?["username"]}");
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    Logger.LogWarning($"❌ Token inválido. Status: {response.StatusCode}");
                    return AuthenticateResult.Fail("Token inválido ou expirado");
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, $"🌐 Erro de conexão com Auth Service: {ex.Message}");
                return AuthenticateResult.Fail($"Erro de conexão com serviço de autenticação: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"❌ Erro inesperado na validação do token: {ex.Message}");
                return AuthenticateResult.Fail($"Erro na validação do token: {ex.Message}");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Bearer";
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    }
}