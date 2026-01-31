using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using SharedKernel;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger;
        private readonly AuthDbContext _context;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ILogger<UserService> logger,
            AuthDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _context = context;
        }

        // Constantes para validação
        private const int MIN_PASSWORD_LENGTH = 8;
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Hierarquia de roles
        private static readonly Dictionary<string, int> RoleHierarchy = new()
        {
            { "DEV", 100 },
            { "ADMIN", 90 },
            { "GERENTE", 80 },
            { "SUPERVISOR", 70 },
            { "LIDER", 60 },
            { "OPERADOR", 50 },
            { "USER", 40 }
        };

        // --- NOVO MÉTODO: Para o endpoint /me ---
        public async Task<AuthResult> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return AuthResult.Fail("Usuário não encontrado.");

                // Mapeia para o UserResponseDto (Segurança: não devolve senha)
                var response = new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Role = user.Roles.FirstOrDefault()?.Name ?? "User"
                };

                return AuthResult.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", userId);
                return AuthResult.Fail("Erro interno ao buscar dados do usuário.");
            }
        }

        // --- ATUALIZADO: Agora aceita CreateTenantUserDto para salvar o NOME ---
        public async Task<AuthResult> RegisterAsync(CreateTenantUserDto request)
        {
            try
            {
                // Validações de entrada
                var validationResult = ValidateBasicInput(request);
                if (!validationResult.Success) return validationResult;

                // Verificar duplicidade
                if (await _userRepository.UserExistsAsync(request.Email))
                {
                    return AuthResult.Fail(ErrorMessages.EmailInUse);
                }

                // Buscar role padrão (Dev ou a que vier no request se for permitido)
                var roleName = string.IsNullOrEmpty(request.RoleName) ? "Dev" : request.RoleName;
                var userRole = await _roleRepository.GetRoleByNameAsync(roleName);

                if (userRole == null)
                    return AuthResult.Fail($"Perfil '{roleName}' não encontrado.");

                // Criar usuário
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName, // <--- NOVO CAMPO
                    Email = request.Email.Trim().ToLower(),
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Roles = new List<Role> { userRole },
                    TenantId = null, // Usuário de sistema/dev não tem tenant fixo inicialmente
                    IsActive = true
                };

                await _userRepository.AddUserAsync(user);

                // Retorna DTO seguro
                var response = new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = userRole.Name,
                    IsActive = true
                };

                return AuthResult.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário: {Email}", request.Email);
                return AuthResult.Fail("Erro interno ao registrar usuário.");
            }
        }

        public async Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId)
        {
            try
            {
                var basicValidation = ValidateBasicInput(request);
                if (!basicValidation.Success) return basicValidation;

                if (!IsValidEmail(request.Email))
                    return AuthResult.Fail("Formato de email inválido.");

                if (await _userRepository.UserExistsAsync(request.Email.Trim().ToLower()))
                    return AuthResult.Fail(ErrorMessages.EmailInUse);

                var hierarchyValidation = await ValidateRoleHierarchy(request.RoleName, managerUserId);
                if (!hierarchyValidation.Success) return hierarchyValidation;

                var userRole = await _roleRepository.GetRoleByNameAsync(request.RoleName);
                if (userRole == null)
                    return AuthResult.Fail($"O Perfil '{request.RoleName}' não foi encontrado.");

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName, // <--- NOVO CAMPO
                    Email = request.Email.Trim().ToLower(),
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    TenantId = tenantId,
                    ReportsToUserId = managerUserId,
                    Roles = new List<Role> { userRole },
                    IsActive = true, // <--- NOVO CAMPO
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddUserAsync(newUser);

                var data = new UserResponseDto
                {
                    Id = newUser.Id,
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    Role = userRole.Name,
                    IsActive = true
                };

                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário de tenant: {Email}", request.Email);
                return AuthResult.Fail($"Erro interno: {ex.Message}");
            }
        }

        // --- MÉTODOS EXISTENTES MANTIDOS ---

        public async Task<AuthResult> PromoteToAdminAsync(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail)) return AuthResult.Fail("Email obrigatório.");

                var user = await _userRepository.GetUserByEmailAsync(userEmail.Trim().ToLower());
                if (user == null) return AuthResult.Fail(ErrorMessages.UserNotFound);

                var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
                if (adminRole == null) return AuthResult.Fail(ErrorMessages.RoleNotFound);

                if (!user.Roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    user.Roles.Add(adminRole);
                    await _userRepository.UpdateUserAsync(user);
                }

                return AuthResult.Ok($"Usuário {userEmail} promovido a Admin.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro promote admin");
                return AuthResult.Fail("Erro interno.");
            }
        }

        public async Task<AuthResult> DemoteFromAdminAsync(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail)) return AuthResult.Fail("Email vazio.");

                var user = await _userRepository.GetUserByEmailAsync(userEmail.Trim().ToLower());
                if (user == null) return AuthResult.Fail("Usuário não encontrado.");

                var adminRole = user.Roles.FirstOrDefault(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                if (adminRole != null)
                {
                    user.Roles.Remove(adminRole);
                    await _userRepository.UpdateUserAsync(user);
                }
                return AuthResult.Ok("Permissões de Admin removidas.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro demote admin");
                return AuthResult.Fail("Erro interno.");
            }
        }

        public async Task<AuthResult> GetAdminUsersAsync()
        {
            try
            {
                var adminUsers = await _userRepository.GetUsersByRoleAsync("Admin");
                var result = adminUsers.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = "Admin",
                    IsActive = u.IsActive
                }).ToList();

                return AuthResult.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro GetAdmins");
                return AuthResult.Fail("Erro interno.");
            }
        }

        public async Task<AuthResult> GetSubordinatesAsync(Guid managerId)
        {
            try
            {
                var subordinates = await _userRepository.GetUsersByManagerAsync(managerId);
                var result = subordinates.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Roles.FirstOrDefault()?.Name ?? "Sem Cargo",
                    IsActive = u.IsActive
                }).ToList();

                return AuthResult.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro GetSubordinates");
                return AuthResult.Fail("Erro interno.");
            }
        }


        public async Task<User> CreateSystemUserAsync(CreateSystemUserDto dto)
        {
            // 1. Verificação de Segurança
            var validKey = Environment.GetEnvironmentVariable("BOOT_STRAP_SECRET") ?? "minha-chave-secreta-de-infra-123";
            
            if (dto.SystemCreationKey != validKey)
            {
                throw new UnauthorizedAccessException("Chave de criação de sistema inválida.");
            }

            // 2. Verificar se já existe (Async corrigido)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Usuário de sistema já existe.");
            }

            // 3. Criar o Usuário Limpo
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FullName = "System Administrator",
                
                // CORREÇÃO CS0117: Verifique se no seu Model 'User' o nome é 'Role' ou 'RoleName'. 
                // Geralmente em setups manuais é 'RoleName'. Se for 'Role', mantenha 'Role'.
                Roles = dto.Role != null 
                    ? new List<Role> { await _roleRepository.GetRoleByNameAsync(dto.Role) ?? throw new InvalidOperationException("Role inválida.") } 
                    : new List<Role>(), 
                
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                TenantId = null, 
                PhoneNumber = null 
            };

            // 4. Hash da Senha
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        #region Helpers de Validação

        private AuthResult ValidateBasicInput(CreateTenantUserDto request)
        {
            if (request == null) return AuthResult.Fail("Dados nulos.");
            if (string.IsNullOrWhiteSpace(request.FullName)) return AuthResult.Fail("Nome completo obrigatório.");
            if (string.IsNullOrWhiteSpace(request.Email)) return AuthResult.Fail("Email obrigatório.");
            if (string.IsNullOrWhiteSpace(request.Password)) return AuthResult.Fail("Senha obrigatória.");

            // Se for Register (dev), RoleName pode ser nulo (assume Dev)
            // Se for CreateTenantUser, RoleName é obrigatório. 
            // Validamos role específica dentro do método se necessário.

            return AuthResult.Ok();
        }

        private async Task<AuthResult> ValidateRoleHierarchy(string requestedRoleName, Guid managerUserId)
        {
            var manager = await _userRepository.GetUserByIdAsync(managerUserId);
            if (manager == null) return AuthResult.Fail("Manager não encontrado.");

            var managerHighestRole = manager.Roles
                .Select(r => r.Name.ToUpper())
                .Where(RoleHierarchy.ContainsKey)
                .OrderByDescending(r => RoleHierarchy[r])
                .FirstOrDefault();

            if (string.IsNullOrEmpty(managerHighestRole)) return AuthResult.Fail("Manager sem permissões.");

            var requestedRoleUpper = requestedRoleName.ToUpper();
            if (!RoleHierarchy.ContainsKey(requestedRoleUpper)) return AuthResult.Fail("Perfil inválido.");

            if (RoleHierarchy[requestedRoleUpper] >= RoleHierarchy[managerHighestRole])
                return AuthResult.Fail("Permissão negada (Nível hierárquico).");

            return AuthResult.Ok();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { return EmailRegex.IsMatch(email.Trim()); } catch { return false; }
        }

        public async Task<AuthResult> GetUserHierarchyAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) return AuthResult.Fail("Usuário não encontrado.");

                var highestRole = user.Roles
                    .Select(r => r.Name.ToUpper())
                    .Where(RoleHierarchy.ContainsKey)
                    .OrderByDescending(r => RoleHierarchy[r])
                    .FirstOrDefault();

                var managerRoleValue = !string.IsNullOrEmpty(highestRole) && RoleHierarchy.ContainsKey(highestRole)
                    ? RoleHierarchy[highestRole]
                    : 0;

                var allowedRoles = RoleHierarchy
                    .Where(r => r.Value < managerRoleValue)
                    .Select(r => r.Key)
                    .ToList();

                return AuthResult.Ok(new { AllowedRoles = allowedRoles });
            }
            catch { return AuthResult.Fail("Erro hierarquia."); }
        }

        #endregion
    }
}