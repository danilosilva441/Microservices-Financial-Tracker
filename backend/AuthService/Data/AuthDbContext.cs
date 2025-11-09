using AuthService.Configuration; // <-- Importar a nova configuração
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tenant> Tenants { get; set; } // <-- NOVO DBSET (Fase 1.1)

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Relação User-Role (Muitos-para-Muitos) ---
            // (Já existia, mas garantindo que está aqui)
            builder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRoles")); // Tabela de junção

            // --- INÍCIO DAS NOVAS RELAÇÕES (Fase 1.1) ---

            // Relação 1: Tenant -> Users (Um-para-Muitos)
            builder.Entity<Tenant>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // Não deixa apagar um Tenant se ele tiver usuários

            // Relação 2: User -> User (Hierarquia)
            builder.Entity<User>()
                .HasMany(u => u.Subordinates) // Um 'User' (chefe) tem muitos 'Subordinates'
                .WithOne(u => u.ReportsToUser) // Cada 'Subordinate' reporta a um 'ReportsToUser'
                .HasForeignKey(u => u.ReportsToUserId)
                .OnDelete(DeleteBehavior.Restrict); // Não deixa apagar um chefe se ele tiver subordinados

            // Aplica a configuração dos perfis (Roles) do arquivo de configuração
            builder.ApplyConfiguration(new RoleConfiguration());
            
            // --- FIM DAS NOVAS RELAÇÕES ---
        }
    }
}