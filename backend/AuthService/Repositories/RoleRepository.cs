using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using AuthService.Repositories;

namespace AuthService.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName || r.NormalizedName == roleName.ToUpper());
        }

        public async Task<Role?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Role>> GetRolesByNamesAsync(IEnumerable<string> roleNames)
        {
            var normalizedNames = roleNames.Select(r => r.ToUpper()).ToList();
            
            return await _context.Roles
                .Where(r => normalizedNames.Contains(r.NormalizedName))
                .ToListAsync();
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _context.Roles
                .AnyAsync(r => r.Name == roleName || r.NormalizedName == roleName.ToUpper());
        }

        public Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}