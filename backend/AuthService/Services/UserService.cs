// Caminho: backend/AuthService/Services/UserService.cs
using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using AuthService.Data;
using SharedKernel;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // --- LÓGICA MOVIDA DO AUTHSERVICE ---
        public async Task<AuthResult> RegisterAsync(UserDto request)
        {
            if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
            {
                return AuthResult.Fail(ErrorMessages.EmailInUse);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // IMPORTANTE: O registo padrão agora procura o Role "Dev"
            var userRole = await _roleRepository.GetRoleByNameAsync("Dev");
            if (userRole == null)
            {
                return AuthResult.Fail(ErrorMessages.RoleNotFound);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                Roles = new List<Role> { userRole },
                TenantId = null // Utilizadores Dev/Admin não têm Tenant
            };

            await _userRepository.AddUserAsync(user);

            var data = new { UserId = user.Id, Email = user.Email };
            return AuthResult.Ok(data);
        }

        // --- LÓGICA MOVIDA DO AUTHSERVICE ---
        public async Task<AuthResult> PromoteToAdminAsync(string userEmail)
        {
            var user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return AuthResult.Fail(ErrorMessages.UserNotFound);
            }

            var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
            if (adminRole == null)
            {
                return AuthResult.Fail(ErrorMessages.RoleNotFound);
            }

            if (!user.Roles.Any(r => r.Name == "Admin"))
            {
                user.Roles.Add(adminRole);
                await _userRepository.UpdateUserAsync(user);
            }

            return AuthResult.Ok($"Utilizador {userEmail} foi promovido a Admin.");
        }

        // --- NOSSO NOVO MÉTODO (Tarefa 7) ---
        public async Task<AuthResult> CreateTenantUserAsync(CreateTenantUserDto request, Guid managerUserId, Guid tenantId)
        {
            // 1. Validar se o email já existe
            if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
            {
                return AuthResult.Fail(ErrorMessages.EmailInUse);
            }

            // 2. Validar o Perfil (Role)
            // (Um Gerente não pode criar um "Admin" ou outro "Gerente")
            var normalizedRole = request.RoleName.ToUpper();
            if (normalizedRole == "ADMIN" || normalizedRole == "GERENTE" || normalizedRole == "DEV")
            {
                return AuthResult.Fail(ErrorMessages.InvalidRoleCreation);
            }

            var userRole = await _roleRepository.GetRoleByNameAsync(request.RoleName);
            if (userRole == null)
            {
                return AuthResult.Fail($"O Perfil (Role) '{request.RoleName}' não foi encontrado no sistema.");
            }

            // 3. Criar o novo Utilizador
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,

                // 4. Ligar à Hierarquia v2.0
                TenantId = tenantId,           // Liga à "Empresa" (Tenant) do Gerente
                ReportsToUserId = managerUserId, // Liga ao "Chefe" (o Gerente)

                Roles = new List<Role> { userRole }
            };

            // 5. Salvar
            await _userRepository.AddUserAsync(newUser);

            var data = new { UserId = newUser.Id, Email = newUser.Email, Role = userRole.Name };
            return AuthResult.Ok(data);
        }
    }
}