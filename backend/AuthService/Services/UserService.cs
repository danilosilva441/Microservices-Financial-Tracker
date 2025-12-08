using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using SharedKernel;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        // Constantes para validação
        private const int MIN_PASSWORD_LENGTH = 8;
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Hierarquia de roles (do mais alto para o mais baixo)
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

        public async Task<AuthResult> RegisterAsync(UserDto request)
        {
            try
            {
                // Validações de entrada
                var validationResult = ValidateUserInput(request.Email, request.Password);
                if (!validationResult.Success)
                    return validationResult;

                // Verificar duplicidade
                if (await _userRepository.UserExistsAsync(request.Email))
                {
                    _logger.LogWarning("Tentativa de registro com email já existente: {Email}", request.Email);
                    return AuthResult.Fail(ErrorMessages.EmailInUse);
                }

                // Buscar role padrão
                var userRole = await _roleRepository.GetRoleByNameAsync("Dev");
                if (userRole == null)
                {
                    _logger.LogError("Role padrão 'Dev' não encontrada no sistema");
                    return AuthResult.Fail("Perfil 'Dev' padrão não encontrado.");
                }

                // Criar usuário
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Roles = new List<Role> { userRole },
                    TenantId = null
                };

                await _userRepository.AddUserAsync(user);
                _logger.LogInformation("Usuário registrado com sucesso: {Email}", user.Email);

                var data = new { UserId = user.Id, Email = user.Email };
                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário: {Email}", request.Email);
                return AuthResult.Fail("Erro interno ao registrar usuário.");
            }
        }

        public async Task<AuthResult> PromoteToAdminAsync(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                    return AuthResult.Fail("Email não pode estar vazio.");

                var user = await _userRepository.GetUserByEmailAsync(userEmail.Trim().ToLower());
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de promover usuário não encontrado: {Email}", userEmail);
                    return AuthResult.Fail(ErrorMessages.UserNotFound);
                }

                var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
                if (adminRole == null)
                {
                    _logger.LogError("Role 'Admin' não encontrada no sistema");
                    return AuthResult.Fail(ErrorMessages.RoleNotFound);
                }

                if (!user.Roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    user.Roles.Add(adminRole);
                    await _userRepository.UpdateUserAsync(user);
                    _logger.LogInformation("Usuário promovido a Admin: {Email}", userEmail);
                }

                return AuthResult.Ok($"Usuário {userEmail} foi promovido a Admin.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao promover usuário a Admin: {Email}", userEmail);
                return AuthResult.Fail("Erro interno ao promover usuário.");
            }
        }

        public async Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId)
        {
            try
            {
                // 1. Validações básicas
                var basicValidation = ValidateBasicInput(request);
                if (!basicValidation.Success)
                    return basicValidation;

                // 2. Validar formato de email
                if (!IsValidEmail(request.Email))
                {
                    _logger.LogWarning("Tentativa de criação com email inválido: {Email}", request.Email);
                    return AuthResult.Fail("Formato de email inválido.");
                }

                // 3. Validar força da senha
                var passwordValidation = ValidatePasswordStrength(request.Password);
                if (!passwordValidation.Success)
                    return passwordValidation;

                // 4. Verificar duplicidade de email
                if (await _userRepository.UserExistsAsync(request.Email.Trim().ToLower()))
                {
                    _logger.LogWarning("Tentativa de criação com email já existente: {Email}", request.Email);
                    return AuthResult.Fail(ErrorMessages.EmailInUse);
                }

                // 5. Validar hierarquia de roles
                var hierarchyValidation = await ValidateRoleHierarchy(request.RoleName, managerUserId);
                if (!hierarchyValidation.Success)
                    return hierarchyValidation;

                // 6. Buscar role no banco
                var userRole = await _roleRepository.GetRoleByNameAsync(request.RoleName);
                if (userRole == null)
                {
                    _logger.LogWarning("Tentativa de criação com role não encontrada: {Role}", request.RoleName);
                    return AuthResult.Fail($"O Perfil (Role) '{request.RoleName}' não foi encontrado no sistema.");
                }

                // 7. Criar usuário
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    TenantId = tenantId,
                    ReportsToUserId = managerUserId,
                    Roles = new List<Role> { userRole },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddUserAsync(newUser);

                _logger.LogInformation(
                    "Usuário de tenant criado com sucesso: {Email}, Tenant: {TenantId}, Role: {Role}",
                    newUser.Email, tenantId, userRole.Name);

                var data = new
                {
                    UserId = newUser.Id,
                    Email = newUser.Email,
                    Role = userRole.Name,
                    TenantId = tenantId
                };

                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao criar usuário de tenant: {Email}, Manager: {ManagerId}, Tenant: {TenantId}",
                    request.Email, managerUserId, tenantId);

                return AuthResult.Fail($"Erro interno ao criar usuário: {ex.Message}");
            }
        }

        #region Métodos de Validação Privados

        private AuthResult ValidateUserInput(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                return AuthResult.Fail("Email é obrigatório.");

            if (string.IsNullOrWhiteSpace(password))
                return AuthResult.Fail("Senha é obrigatória.");

            if (!IsValidEmail(email))
                return AuthResult.Fail("Formato de email inválido.");

            if (password.Length < MIN_PASSWORD_LENGTH)
                return AuthResult.Fail($"A senha deve ter pelo menos {MIN_PASSWORD_LENGTH} caracteres.");

            return AuthResult.Ok();
        }

        private AuthResult ValidateBasicInput(CreateTenantUserDto request)
        {
            if (request == null)
                return AuthResult.Fail("Dados de entrada não podem ser nulos.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return AuthResult.Fail("Email é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return AuthResult.Fail("Senha é obrigatória.");

            if (string.IsNullOrWhiteSpace(request.RoleName))
                return AuthResult.Fail("Perfil (Role) é obrigatório.");

            return AuthResult.Ok();
        }

        private AuthResult ValidatePasswordStrength(string password)
        {
            if (password.Length < MIN_PASSWORD_LENGTH)
                return AuthResult.Fail($"A senha deve ter pelo menos {MIN_PASSWORD_LENGTH} caracteres.");

            // Verificar se tem pelo menos um número
            if (!password.Any(char.IsDigit))
                return AuthResult.Fail("A senha deve conter pelo menos um número.");

            // Verificar se tem pelo menos uma letra maiúscula
            if (!password.Any(char.IsUpper))
                return AuthResult.Fail("A senha deve conter pelo menos uma letra maiúscula.");

            // Verificar se tem pelo menos uma letra minúscula
            if (!password.Any(char.IsLower))
                return AuthResult.Fail("A senha deve conter pelo menos uma letra minúscula.");

            return AuthResult.Ok();
        }

        private async Task<AuthResult> ValidateRoleHierarchy(string requestedRoleName, Guid managerUserId)
        {
            try
            {
                // Buscar o manager para verificar suas roles
                var manager = await _userRepository.GetUserByIdAsync(managerUserId);
                if (manager == null)
                {
                    _logger.LogWarning("Manager não encontrado: {ManagerId}", managerUserId);
                    return AuthResult.Fail("Manager não encontrado.");
                }

                // Encontrar a role mais alta do manager
                var managerHighestRole = manager.Roles
                    .Select(r => r.Name.ToUpper())
                    .Where(roleName => RoleHierarchy.ContainsKey(roleName))
                    .OrderByDescending(roleName => RoleHierarchy[roleName])
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(managerHighestRole))
                {
                    _logger.LogWarning("Manager sem roles válidas: {ManagerId}", managerUserId);
                    return AuthResult.Fail("Manager não possui permissões válidas.");
                }

                var requestedRoleUpper = requestedRoleName.ToUpper();

                // Verificar se a role solicitada existe na hierarquia
                if (!RoleHierarchy.ContainsKey(requestedRoleUpper))
                {
                    _logger.LogWarning("Role não reconhecida na hierarquia: {Role}", requestedRoleName);
                    return AuthResult.Fail($"Perfil '{requestedRoleName}' não é válido.");
                }

                // Verificar hierarquia: manager só pode criar roles de nível inferior
                if (RoleHierarchy[requestedRoleUpper] >= RoleHierarchy[managerHighestRole])
                {
                    _logger.LogWarning(
                        "Tentativa de criar role de nível igual ou superior: Manager={ManagerRole}, Requested={RequestedRole}",
                        managerHighestRole, requestedRoleUpper);

                    return AuthResult.Fail("Permissão negada. Você só pode criar usuários com perfis de nível inferior ao seu.");
                }

                return AuthResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar hierarquia de roles para manager: {ManagerId}", managerUserId);
                return AuthResult.Fail("Erro ao validar permissões.");
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return EmailRegex.IsMatch(email.Trim());
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Métodos Adicionais Úteis

        public async Task<AuthResult> GetUserHierarchyAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return AuthResult.Fail("Usuário não encontrado.");

                var highestRole = user.Roles
                    .Select(r => r.Name.ToUpper())
                    .Where(roleName => RoleHierarchy.ContainsKey(roleName))
                    .OrderByDescending(roleName => RoleHierarchy[roleName])
                    .FirstOrDefault();

                var managerRoleValue = !string.IsNullOrEmpty(highestRole) && RoleHierarchy.ContainsKey(highestRole)
                    ? RoleHierarchy[highestRole]
                    : 0;

                var allowedRoles = RoleHierarchy
                    .Where(r => r.Value < managerRoleValue)
                    .Select(r => r.Key)
                    .ToList();

                var data = new
                {
                    UserId = user.Id,
                    HighestRole = highestRole,
                    AllowedRoles = allowedRoles,
                    CanCreateUsers = allowedRoles.Any()
                };

                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter hierarquia do usuário: {UserId}", userId);
                return AuthResult.Fail("Erro ao obter hierarquia de permissões.");
            }
        }

        #endregion
        public async Task<AuthResult> DemoteFromAdminAsync(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                    return AuthResult.Fail("Email não pode estar vazio.");

                var user = await _userRepository.GetUserByEmailAsync(userEmail.Trim().ToLower());
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de remover admin de usuário não encontrado: {Email}", userEmail);
                    return AuthResult.Fail("Usuário não encontrado.");
                }

                // Verificar se o usuário tem role de Admin
                var adminRole = user.Roles.FirstOrDefault(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                if (adminRole == null)
                {
                    _logger.LogInformation("Usuário {Email} não possui permissões de Admin para remover", userEmail);
                    return AuthResult.Ok($"Usuário {userEmail} não possui permissões de Admin.");
                }

                // Remover role de Admin
                user.Roles.Remove(adminRole);
                await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation("Permissões de Admin removidas do usuário: {Email}", userEmail);
                return AuthResult.Ok($"Usuário {userEmail} teve as permissões de Admin removidas com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover permissões de Admin do usuário: {Email}", userEmail);
                return AuthResult.Fail("Erro interno ao remover permissões de Admin.");
            }
        }

        public async Task<AuthResult> GetAdminUsersAsync()
        {
            try
            {
                // Buscar role Admin primeiro para garantir que existe
                var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
                if (adminRole == null)
                {
                    _logger.LogError("Role 'Admin' não encontrada no sistema");
                    return AuthResult.Fail("Perfil 'Admin' não encontrado no sistema.");
                }

                // Buscar todos os usuários que têm a role Admin
                // Nota: Você precisará adicionar este método ao IUserRepository
                var adminUsers = await _userRepository.GetUsersByRoleAsync("Admin");

                var result = adminUsers.Select(u => new
                {
                    u.Id,
                    u.Email,
                    Roles = u.Roles.Select(r => r.Name),
                    u.TenantId,
                    u.CreatedAt,
                    u.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retornados {Count} usuários administradores", result.Count);
                return AuthResult.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários administradores");
                return AuthResult.Fail("Erro interno ao buscar usuários administradores.");
            }
        }


        public async Task<AuthResult> GetSubordinatesAsync(Guid managerId)
        {
            try
            {
                // Busca usuários onde ReportsToUserId == managerId
                // Nota: Precisará adicionar este método no IUserRepository
                var subordinates = await _userRepository.GetUsersByManagerAsync(managerId);

                var result = subordinates.Select(u => new
                {
                    u.Id,
                    u.Email,
                    Role = u.Roles.FirstOrDefault()?.Name ?? "Sem Cargo",
                    u.CreatedAt
                }).ToList();

                return AuthResult.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar subordinados do manager: {ManagerId}", managerId);
                return AuthResult.Fail("Erro ao buscar lista de subordinados.");
            }
        }



    }

}