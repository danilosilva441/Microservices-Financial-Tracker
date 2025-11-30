using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == userId);
        }

        public async Task<List<User>> GetUsersByTenantAsync(Guid tenantId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<List<User>> GetSubordinatesAsync(Guid managerId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.ReportsToUserId == managerId)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return !await query.AnyAsync();
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.Name == roleName || r.NormalizedName == roleName.ToUpper()))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}