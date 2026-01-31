// Caminho: backend/AuthService/Services/AuthService.cs
using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SharedKernel;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly TimeSpan _tokenExpiration;

        // Constantes para configuração
        private const int MIN_PASSWORD_LENGTH = 1; // Para login, só verifica se não está vazia
        private const int MAX_LOGIN_ATTEMPTS = 5;
        private const int LOCKOUT_DURATION_MINUTES = 15;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _tokenExpiration = TimeSpan.FromHours(8); // Configurável
        }

        public async Task<AuthResult> LoginAsync(LoginDto request)
        {
            using var logScope = _logger.BeginScope("Login attempt for {Email}", request.Email);
            
            try
            {
                // 1. Validação de entrada
                var validationResult = ValidateLoginInput(request);
                if (!validationResult.Success)
                {
                    _logger.LogWarning("Validação de entrada falhou: {Error}", validationResult.ErrorMessage);
                    return validationResult;
                }

                // 2. Buscar usuário
                var user = await _userRepository.GetUserByEmailAsync(request.Email.Trim().ToLower());
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de login com email não encontrado: {Email}", request.Email);
                    await LogFailedLoginAttempt(request.Email);
                    return AuthResult.Fail(ErrorMessages.InvalidCredentials);
                }

                // 3. Verificar se usuário está bloqueado
                if (await IsUserLockedOut(user))
                {
                    _logger.LogWarning("Tentativa de login para usuário bloqueado: {Email}", user.Email);
                    return AuthResult.Fail("Conta temporariamente bloqueada devido a múltiplas tentativas de login falhas. Tente novamente em alguns minutos.");
                }

                // 4. Verificar senha
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Senha incorreta para usuário: {Email}", user.Email);
                    await LogFailedLoginAttempt(user.Email);
                    return AuthResult.Fail(ErrorMessages.InvalidCredentials);
                }

                // 5. Resetar contador de tentativas falhas (login bem-sucedido)
                await ResetFailedLoginAttempts(user.Email);

                // 6. Gerar token
                string token = GenerateJwtToken(user);

                _logger.LogInformation("Login bem-sucedido para usuário: {Email}, UserId: {UserId}", user.Email, user.Id);

                var response = new
                {
                    token,
                    expiresIn = (int)_tokenExpiration.TotalSeconds,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        roles = user.Roles.Select(r => r.Name),
                        tenantId = user.TenantId
                    }
                };

                return AuthResult.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante login para: {Email}", request.Email);
                return AuthResult.Fail($"{ErrorMessages.DatabaseConnectionFailed} Detalhes: {ex.Message}");
            }
        }

        public async Task<AuthResult> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = GetJwtKey();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Zero tolerance for expiration
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return AuthResult.Fail("Token inválido: claim de usuário ausente.");
                }

                // Verificar se usuário ainda existe e está ativo
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return AuthResult.Fail("Usuário não encontrado.");
                }

                return AuthResult.Ok(new { valid = true, user = new { user.Id, user.Email } });
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthResult.Fail("Token expirado.");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return AuthResult.Fail("Assinatura do token inválida.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token");
                return AuthResult.Fail("Token inválido.");
            }
        }

        public async Task<AuthResult> RefreshTokenAsync(string expiredToken)
        {
            try
            {
                // Primeiro validar o token expirado (ignorando expiração)
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = GetJwtKey();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = false // Ignorar expiração para refresh
                };

                var principal = tokenHandler.ValidateToken(expiredToken, validationParameters, out _);
                
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return AuthResult.Fail("Token inválido para refresh.");
                }

                // Buscar usuário
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return AuthResult.Fail("Usuário não encontrado.");
                }

                // Gerar novo token
                string newToken = GenerateJwtToken(user);

                _logger.LogInformation("Token refresh bem-sucedido para usuário: {UserId}", user.Id);

                var response = new
                {
                    token = newToken,
                    expiresIn = (int)_tokenExpiration.TotalSeconds
                };

                return AuthResult.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante refresh do token");
                return AuthResult.Fail("Falha ao renovar token.");
            }
        }
        

        #region Métodos Privados

        private AuthResult ValidateLoginInput(LoginDto request)
        {
            if (request == null)
                return AuthResult.Fail("Dados de login não podem ser nulos.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return AuthResult.Fail("Email é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return AuthResult.Fail("Senha é obrigatória.");

            if (request.Password.Length < MIN_PASSWORD_LENGTH)
                return AuthResult.Fail($"Senha deve ter pelo menos {MIN_PASSWORD_LENGTH} caractere.");

            if (!IsValidEmail(request.Email))
                return AuthResult.Fail("Formato de email inválido.");

            return AuthResult.Ok();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = GetJwtKey();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            if (user.TenantId.HasValue)
            {
                claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));
            }

            // Adicionar roles como array e individualmente
            var roles = user.Roles.Select(r => r.Name).ToList();
            if (roles.Any())
            {
                claims.Add(new Claim("roles", string.Join(",", roles)));
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "AuthService",
                audience: _configuration["Jwt:Audience"] ?? "PortfolioApp",
                claims: claims,
                expires: DateTime.UtcNow.Add(_tokenExpiration),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GetJwtKey()
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("JWT Key não configurada");
                throw new InvalidOperationException(ErrorMessages.InvalidToken);
            }

            if (jwtKey.Length < 32)
            {
                _logger.LogWarning("JWT Key muito curta. Recomendado: mínimo 32 caracteres");
            }

            return jwtKey;
        }

        #region Simulação de Controle de Tentativas (Implementar com cache/distribuído em produção)

        private readonly Dictionary<string, (int attempts, DateTime lockoutEnd)> _loginAttempts = new();

        private async Task LogFailedLoginAttempt(string email)
        {
            // Em produção, usar Redis ou database distribuído
            var normalizedEmail = email.Trim().ToLower();
            
            if (_loginAttempts.ContainsKey(normalizedEmail))
            {
                var (attempts, lockoutEnd) = _loginAttempts[normalizedEmail];
                
                if (DateTime.UtcNow < lockoutEnd)
                {
                    return; // Já está bloqueado
                }

                attempts++;
                if (attempts >= MAX_LOGIN_ATTEMPTS)
                {
                    _loginAttempts[normalizedEmail] = (attempts, DateTime.UtcNow.AddMinutes(LOCKOUT_DURATION_MINUTES));
                    _logger.LogWarning("Usuário bloqueado devido a múltiplas tentativas falhas: {Email}", email);
                }
                else
                {
                    _loginAttempts[normalizedEmail] = (attempts, DateTime.MinValue);
                }
            }
            else
            {
                _loginAttempts[normalizedEmail] = (1, DateTime.MinValue);
            }

            await Task.CompletedTask;
        }

        private async Task<bool> IsUserLockedOut(User user)
        {
            var normalizedEmail = user.Email.Trim().ToLower();
            
            if (_loginAttempts.TryGetValue(normalizedEmail, out var attemptInfo))
            {
                return DateTime.UtcNow < attemptInfo.lockoutEnd;
            }

            return await Task.FromResult(false);
        }

        private async Task ResetFailedLoginAttempts(string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            _loginAttempts.Remove(normalizedEmail);
            await Task.CompletedTask;
        }

        #endregion

        #endregion
    }
}