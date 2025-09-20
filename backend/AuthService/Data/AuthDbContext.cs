using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; } // <-- ADICIONE ESTA LINHA

    // Este método é chamado quando o modelo do banco de dados está sendo criado.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Adiciona os perfis padrão "User" e "Admin" ao banco de dados na primeira vez.
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" }
        );
    }
}