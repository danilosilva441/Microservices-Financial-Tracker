using AuthService.Configuration;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities; 
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        // 1. MUDANÇA: Propriedade pública para o EF Core ver
        public Guid? CurrentTenantId { get; private set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options) 
        {
            if (httpContextAccessor?.HttpContext != null)
            {
                var tenantIdClaim = httpContextAccessor.HttpContext
                    .User.FindFirst("tenantId")?.Value;

                if (!string.IsNullOrEmpty(tenantIdClaim))
                {
                    CurrentTenantId = Guid.Parse(tenantIdClaim);
                }
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tenant> Tenants { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Relações (Sem alteração) ---
            builder.Entity<User>()
                .HasMany(u => u.Roles).WithMany(r => r.Users).UsingEntity(j => j.ToTable("UserRoles")); 
            builder.Entity<Tenant>()
                .HasMany(t => t.Users).WithOne(u => u.Tenant).HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict); 
            builder.Entity<User>()
                .HasMany(u => u.Subordinates).WithOne(u => u.ReportsToUser).HasForeignKey(u => u.ReportsToUserId).OnDelete(DeleteBehavior.Restrict); 

            builder.ApplyConfiguration(new RoleConfiguration());

            // --- 7. CORREÇÃO DE SEGURANÇA (v2.2 - FINAL) ---
            var method = typeof(AuthDbContext).GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var entityType in builder.Model.GetEntityTypes()
                .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
            {
                var genericMethod = method?.MakeGenericMethod(entityType.ClrType);
                genericMethod?.Invoke(this, new object[] { builder });
            }
        }

        private void ConfigureTenantFilter<T>(ModelBuilder builder) where T : class, ITenantEntity
        {
            // 2. MUDANÇA: Usamos 'CurrentTenantId' (propriedade) em vez de '_tenantId' (campo)
            builder.Entity<T>().HasQueryFilter(e => CurrentTenantId == null || e.TenantId == CurrentTenantId);
        }
    }
}