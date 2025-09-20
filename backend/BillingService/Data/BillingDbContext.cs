using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        public DbSet<Operacao> Operacoes { get; set; }
        public DbSet<Meta> Metas { get; set; }
    }
}