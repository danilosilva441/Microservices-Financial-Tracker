using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Data; // <-- IMPORTANTE: Para o DbContext (Transação)
using Microsoft.EntityFrameworkCore; // <-- IMPORTANTE: Para a Transação
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantRepository _tenantRepository; // <-- 1. NOVA INJEÇÃO
        private readonly AuthDbContext _context; // <-- 2. NOVA INJEÇÃO (Para Transação)
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ITenantRepository tenantRepository, // <-- 3. NOVA INJEÇÃO
            AuthDbContext context, // <-- 4. NOVA INJEÇÃO
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository; // <-- 5. NOVA INJEÇÃO
            _context = context; // <-- 6. NOVA INJEÇÃO
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(UserDto request)
        {
            if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
            {
                return AuthResult.Fail("Usuário com este email já existe.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var userRole = await _roleRepository.GetRoleByNameAsync("User");
            if (userRole == null)
            {
                return AuthResult.Fail("Perfil 'User' padrão não encontrado.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                Roles = new List<Role> { userRole }
            };

            await _userRepository.AddUserAsync(user);

            var data = new { UserId = user.Id, Email = user.Email };
            return AuthResult.Ok(data);
        }

        public async Task<AuthResult> LoginAsync(UserDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return AuthResult.Fail("Credenciais inválidas.");
            }

            string token = GenerateJwtToken(user);
            return AuthResult.Ok(new { token });
        }

        public async Task<AuthResult> PromoteToAdminAsync(string userEmail)
        {
            var user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return AuthResult.Fail("Usuário não encontrado.");
            }

            var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
            if (adminRole == null)
            {
                return AuthResult.Fail("Perfil 'Admin' não encontrado no banco de dados.");
            }

            if (!user.Roles.Any(r => r.Name == "Admin"))
            {
                user.Roles.Add(adminRole);
                await _userRepository.UpdateUserAsync(user);
            }

            return AuthResult.Ok($"Usuário {userEmail} foi promovido a Admin.");
        }

        // --- NOVA IMPLEMENTAÇÃO (Fase 1 / Tarefa 2.4) ---
        public async Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request)
        {
            // 1. Validar se o usuário Gerente já existe
            if (await _userRepository.GetUserByEmailAsync(request.EmailDoGerente) != null)
            {
                return AuthResult.Fail("Um usuário com este email já existe.");
            }

            // 2. Buscar o perfil "Gerente" (que foi semeado pela migração)
            var gerenteRole = await _roleRepository.GetRoleByNameAsync("Gerente");
            if (gerenteRole == null)
            {
                // Erro grave de configuração do banco
                return AuthResult.Fail("Perfil 'Gerente' padrão não encontrado. O seed falhou.");
            }

            // 3. Iniciar uma Transação
            // Isso garante que ou TUDO (Tenant e User) é salvo, ou NADA é salvo.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 4. Criar a nova Empresa (Tenant)
                var newTenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    NomeDaEmpresa = request.NomeDaEmpresa,
                    StatusDaSubscricao = "Ativa", // Valor padrão
                    DataDeCriacao = DateTime.UtcNow
                };
                await _tenantRepository.AddTenantAsync(newTenant);

                // 5. Criar o primeiro Usuário (Gerente)
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaDoGerente);
                var newGerente = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.EmailDoGerente,
                    PasswordHash = passwordHash,
                    TenantId = newTenant.Id, // <-- PONTO CRÍTICO: Linkando ao Tenant
                    ReportsToUserId = null, // Gerente não se reporta a ninguém
                    Roles = new List<Role> { gerenteRole }
                };
                // Adiciona o usuário (AddUserAsync não salva, apenas rastreia)
                await _userRepository.AddUserAsync(newGerente);

                // 6. Salvar TUDO (Tenant e User) no banco de uma vez
                await _context.SaveChangesAsync();

                // 7. Efetivar a Transação
                await transaction.CommitAsync();

                // 8. Retornar sucesso
                var data = new
                {
                    TenantId = newTenant.Id,
                    UserId = newGerente.Id,
                    Email = newGerente.Email,
                    NomeDaEmpresa = newTenant.NomeDaEmpresa
                };
                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                // 8b. Desfazer tudo se algo der errado
                await transaction.RollbackAsync();
                return AuthResult.Fail($"Erro ao provisionar: {ex.Message}");
            }
        }


        // Movida do TokenController para este serviço
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("Chave JWT não está configurada.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 1. Claims iniciais (Jti já está aqui)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 2. ERRO CORRIGIDO: A linha duplicada de Jti foi REMOVIDA daqui.

            // 3. Adiciona a claim CRÍTICA do TenantId
            //    (LÓGICA ATUALIZADA: checa se o TenantId não é o Guid "vazio")
            if (user.TenantId != Guid.Empty)
            {
                // (LÓGICA ATUALIZADA: acessa o TenantId diretamente, sem ".Value")
                claims.Add(new Claim("tenantId", user.TenantId.ToString()));
            }

            // 4. Adiciona as claims de Role
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Você pode alterar para 24h se quiser
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}