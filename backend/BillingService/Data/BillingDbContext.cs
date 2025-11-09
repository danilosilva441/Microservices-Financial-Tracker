using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        // --- 1. MUDANÇA CRÍTICA (Roadmap v2.0) ---
        // Renomeado de Operacoes para Unidades
        public DbSet<Unidade> Unidades { get; set; }
        
        // --- (DbSets restantes) ---
        public DbSet<Meta> Metas { get; set; }
        public DbSet<UsuarioOperacao> UsuarioOperacoes { get; set; }
        public DbSet<Mensalista> Mensalistas { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<SolicitacaoAjuste> SolicitacoesAjuste { get; set; }
        
        public DbSet<FaturamentoParcial> FaturamentosParciais { get; set; }
        public DbSet<FaturamentoDiario> FaturamentosDiarios { get; set; }
        public DbSet<MetodoPagamento> MetodosPagamento { get; set; }

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 2. MUDANÇA: Chave Primária Composta Atualizada ---
            // A propriedade foi renomeada de OperacaoId para UnidadeId
            // (Assumindo que vamos atualizar o modelo UsuarioOperacao.cs)
            modelBuilder.Entity<UsuarioOperacao>()
                .HasKey(uo => new { uo.TenantId, uo.UserId, uo.UnidadeId }); 

            // --- 3. MUDANÇA: Índices de Performance atualizados ---
            // Renomeado de Operacao para Unidade
            modelBuilder.Entity<Unidade>().HasIndex(u => u.TenantId);
            
            // (Índices restantes)
            modelBuilder.Entity<Meta>().HasIndex(m => m.TenantId);
            modelBuilder.Entity<Fatura>().HasIndex(f => f.TenantId);
            modelBuilder.Entity<Mensalista>().HasIndex(m => m.TenantId);
            modelBuilder.Entity<SolicitacaoAjuste>().HasIndex(sa => sa.TenantId);
            modelBuilder.Entity<FaturamentoParcial>().HasIndex(fp => fp.TenantId);
            modelBuilder.Entity<FaturamentoDiario>().HasIndex(fd => fd.TenantId);
            modelBuilder.Entity<MetodoPagamento>().HasIndex(mp => mp.TenantId);
            
            modelBuilder.Entity<ExpenseCategory>().HasIndex(ec => ec.TenantId);
            modelBuilder.Entity<Expense>().HasIndex(e => e.TenantId);
            
            // 4. MUDANÇA: Índice de Despesa atualizado
            // Renomeado de OperacaoId para UnidadeId
            modelBuilder.Entity<Expense>().HasIndex(e => e.UnidadeId);
        }
    }
}