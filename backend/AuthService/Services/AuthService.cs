// Caminho: backend/AuthService/Services/AuthService.cs
using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces; // 1. IMPORTANTE: Usando a nova interface
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SharedKernel;

namespace AuthService.Services
{
    // 2. A classe agora implementa a interface limpa
    public class AuthService : IAuthService
    {
        // 3. Dependências reduzidas. Só precisa disto para o Login.
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // 4. O LoginAsync permanece
        public async Task<AuthResult> LoginAsync(UserDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return AuthResult.Fail(ErrorMessages.InvalidCredentials);
            }

            string token = GenerateJwtToken(user);
            return AuthResult.Ok(new { token });
        }

        // 5. Os métodos RegisterAsync, PromoteToAdminAsync, e ProvisionTenantAsync
        //    foram REMOVIDOS daqui. Eles serão movidos para os outros serviços.

        // 6. O GenerateJwtToken permanece, pois é usado pelo LoginAsync
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException(ErrorMessages.InvalidToken);
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.TenantId.HasValue)
            {
                claims.Add(new Claim("tenantId", user.TenantId.Value.ToString()));
            }

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}