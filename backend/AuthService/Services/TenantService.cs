// Caminho: backend/AuthService/Services/TenantService.cs
using AuthService.DTO;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services.Interfaces;
using AuthService.Data; // Para o DbContext (Transação)
using Microsoft.EntityFrameworkCore;
using SharedKernel; // Para o DbContext (Transação)

namespace AuthService.Services
{
    public class TenantService : ITenantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly AuthDbContext _context;

        public TenantService(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            ITenantRepository tenantRepository, 
            AuthDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository;
            _context = context;
        }

        // --- LÓGICA MOVIDA DO AUTHSERVICE ---
        public async Task<AuthResult> ProvisionTenantAsync(TenantProvisionDto request)
        {
            if (await _userRepository.GetUserByEmailAsync(request.EmailDoGerente) != null)
            {
                return AuthResult.Fail(ErrorMessages.EmailInUse);
            }
            
            var gerenteRole = await _roleRepository.GetRoleByNameAsync("Gerente");
            if (gerenteRole == null)
            {
                return AuthResult.Fail(ErrorMessages.RoleNotFound);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newTenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    NomeDaEmpresa = request.NomeDaEmpresa,
                    StatusDaSubscricao = "Ativa",
                    DataDeCriacao = DateTime.UtcNow
                };
                await _tenantRepository.AddTenantAsync(newTenant);

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaDoGerente);
                var newGerente = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.EmailDoGerente,
                    PasswordHash = passwordHash,
                    TenantId = newTenant.Id, // Linkando ao Tenant
                    ReportsToUserId = null, 
                    Roles = new List<Role> { gerenteRole }
                };
                await _userRepository.AddUserAsync(newGerente);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
                return AuthResult.Fail($"Erro ao provisionar: {ex.Message}");
            }
        }
    }
}