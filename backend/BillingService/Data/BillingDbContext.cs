using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        public DbSet<Operacao> Operacoes { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<UsuarioOperacao> UsuarioOperacoes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Mensalista> Mensalistas { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<Faturamento> Faturamentos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura a chave primária composta para a tabela de junção
            modelBuilder.Entity<UsuarioOperacao>()
                .HasKey(uo => new { uo.UserId, uo.OperacaoId });
        }
        public DbSet<SolicitacaoAjuste> SolicitacoesAjuste { get; set; }
    }
}