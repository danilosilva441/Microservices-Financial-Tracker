// Caminho: backend/AuthService/Services/TenantService.cs
using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Microsoft.Extensions.Logging;

namespace AuthService.Services
{
    public class TenantService : ITenantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly AuthDbContext _context;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            ITenantRepository tenantRepository, 
            AuthDbContext context,
            ILogger<TenantService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request)
        {
            // 1. Validação de entrada
            var validationResult = ValidateProvisionRequest(request);
            if (!validationResult.Success)
            {
                _logger.LogWarning("Validação falhou para provisionamento de tenant: {Errors}", validationResult.ErrorMessage);
                return validationResult;
            }

            // 2. Verificar duplicidade de email e nome
            if (await _userRepository.UserExistsAsync(request.EmailDoGerente))
            {
                _logger.LogWarning("Tentativa de provisionamento com email já existente: {Email}", request.EmailDoGerente);
                return AuthResult.Fail(ErrorMessages.EmailInUse);
            }

            if (!await _tenantRepository.IsNameUniqueAsync(request.NomeDaEmpresa))
            {
                _logger.LogWarning("Tentativa de provisionamento com nome de empresa duplicado: {NomeEmpresa}", request.NomeDaEmpresa);
                return AuthResult.Fail("Já existe um tenant com este nome de empresa.");
            }

            // 3. Buscar role do gerente
            var gerenteRole = await _roleRepository.GetRoleByNameAsync("Gerente");
            if (gerenteRole == null)
            {
                _logger.LogError("Role 'Gerente' não encontrada no sistema durante provisionamento");
                return AuthResult.Fail(ErrorMessages.RoleNotFound);
            }

            // 4. Transação atômica
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 5. Criar tenant
                var newTenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    NomeDaEmpresa = request.NomeDaEmpresa.Trim(),
                    StatusDaSubscricao = "Ativa",
                    DataDeCriacao = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _tenantRepository.AddTenantAsync(newTenant);

                // 6. Criar usuário gerente
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaDoGerente);
                var newGerente = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.EmailDoGerente.Trim().ToLower(),
                    PasswordHash = passwordHash,
                    TenantId = newTenant.Id,
                    ReportsToUserId = null,
                    Roles = new List<Role> { gerenteRole },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _userRepository.AddUserAsync(newGerente);

                // 7. Confirmar transação
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Tenant provisionado com sucesso: {TenantName}, Gerente: {ManagerEmail}, TenantId: {TenantId}",
                    newTenant.NomeDaEmpresa, newGerente.Email, newTenant.Id);

                var data = new
                {
                    TenantId = newTenant.Id,
                    UserId = newGerente.Id,
                    Email = newGerente.Email,
                    NomeDaEmpresa = newTenant.NomeDaEmpresa,
                    Role = gerenteRole.Name
                };
                
                return AuthResult.Ok(data);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, 
                    "Erro de banco de dados ao provisionar tenant: {TenantName}", 
                    request.NomeDaEmpresa);
                return AuthResult.Fail("Erro de persistência no banco de dados.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, 
                    "Erro inesperado ao provisionar tenant: {TenantName}, Gerente: {ManagerEmail}", 
                    request.NomeDaEmpresa, request.EmailDoGerente);
                return AuthResult.Fail($"Erro interno ao provisionar tenant: {ex.Message}");
            }
        }

        public async Task<AuthResult> GetTenantByIdAsync(Guid tenantId)
        {
            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(tenantId);
                if (tenant == null)
                {
                    _logger.LogWarning("Tentativa de buscar tenant não encontrado: {TenantId}", tenantId);
                    return AuthResult.Fail("Tenant não encontrado.");
                }

                var userCount = await _tenantRepository.GetUserCountAsync(tenantId);

                var data = new
                {
                    tenant.Id,
                    tenant.NomeDaEmpresa,
                    tenant.StatusDaSubscricao,
                    tenant.DataDeCriacao,
                    QuantidadeUsuarios = userCount
                };

                return AuthResult.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tenant: {TenantId}", tenantId);
                return AuthResult.Fail("Erro interno ao buscar tenant.");
            }
        }

        public async Task<AuthResult> UpdateTenantStatusAsync(Guid tenantId, string novoStatus)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(tenantId);
                if (tenant == null)
                {
                    return AuthResult.Fail("Tenant não encontrado.");
                }

                var statusValidos = new[] { "Ativa", "Suspensa", "Cancelada", "Expirada" };
                if (!statusValidos.Contains(novoStatus))
                {
                    return AuthResult.Fail($"Status inválido. Status válidos: {string.Join(", ", statusValidos)}");
                }

                var statusAnterior = tenant.StatusDaSubscricao;
                tenant.StatusDaSubscricao = novoStatus;
                tenant.UpdatedAt = DateTime.UtcNow;

                await _tenantRepository.UpdateAsync(tenant);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Status do tenant atualizado: {TenantId}, De: {StatusAnterior}, Para: {NovoStatus}",
                    tenantId, statusAnterior, novoStatus);

                return AuthResult.Ok($"Status do tenant atualizado para '{novoStatus}'.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao atualizar status do tenant: {TenantId}", tenantId);
                return AuthResult.Fail("Erro interno ao atualizar status do tenant.");
            }
        }

        public async Task<AuthResult> GetTenantUsersAsync(Guid tenantId)
        {
            try
            {
                if (!await _tenantRepository.ExistsAsync(tenantId))
                {
                    return AuthResult.Fail("Tenant não encontrado.");
                }

                var users = await _context.Users
                    .Where(u => u.TenantId == tenantId)
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        Roles = u.Roles.Select(r => r.Name),
                        u.CreatedAt,
                        u.UpdatedAt
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return AuthResult.Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários do tenant: {TenantId}", tenantId);
                return AuthResult.Fail("Erro interno ao buscar usuários do tenant.");
            }
        }

        public async Task<AuthResult> GetAllTenantsAsync()
        {
            try
            {
                var tenants = await _tenantRepository.GetAllAsync();
                var result = tenants.Select(t => new
                {
                    t.Id,
                    t.NomeDaEmpresa,
                    t.StatusDaSubscricao,
                    t.DataDeCriacao
                }).ToList();

                return AuthResult.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os tenants");
                return AuthResult.Fail("Erro interno ao buscar tenants.");
            }
        }

        #region Métodos Privados de Validação

        private AuthResult ValidateProvisionRequest(TenantProvisionDto request)
        {
            if (request == null)
                return AuthResult.Fail("Dados de provisionamento não podem ser nulos.");

            if (string.IsNullOrWhiteSpace(request.NomeDaEmpresa))
                return AuthResult.Fail("Nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.EmailDoGerente))
                return AuthResult.Fail("Email do gerente é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.SenhaDoGerente))
                return AuthResult.Fail("Senha do gerente é obrigatória.");

            if (request.NomeDaEmpresa.Trim().Length < 2)
                return AuthResult.Fail("Nome da empresa deve ter pelo menos 2 caracteres.");

            if (request.NomeDaEmpresa.Trim().Length > 100)
                return AuthResult.Fail("Nome da empresa deve ter no máximo 100 caracteres.");

            if (!IsValidEmail(request.EmailDoGerente))
                return AuthResult.Fail("Formato de email do gerente é inválido.");

            var passwordValidation = ValidatePasswordStrength(request.SenhaDoGerente);
            if (!passwordValidation.Success)
                return passwordValidation;

            return AuthResult.Ok();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private AuthResult ValidatePasswordStrength(string password)
        {
            if (password.Length < 8)
                return AuthResult.Fail("A senha deve ter pelo menos 8 caracteres.");

            if (!password.Any(char.IsDigit))
                return AuthResult.Fail("A senha deve conter pelo menos um número.");

            if (!password.Any(char.IsUpper))
                return AuthResult.Fail("A senha deve conter pelo menos uma letra maiúscula.");

            if (!password.Any(char.IsLower))
                return AuthResult.Fail("A senha deve conter pelo menos uma letra minúscula.");

            return AuthResult.Ok();
        }

        #endregion
    }
}