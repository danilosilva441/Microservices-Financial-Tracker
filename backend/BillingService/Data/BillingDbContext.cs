using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        // --- 1. MUDANÇA (v2.0): Renomeado de Operacoes para Unidades ---
        public DbSet<Unidade> Unidades { get; set; }
        
        // --- (Modelos v1.0 mantidos e refatorados) ---
        public DbSet<Meta> Metas { get; set; }
        public DbSet<UsuarioOperacao> UsuarioOperacoes { get; set; }
        public DbSet<Mensalista> Mensalistas { get; set; }
        public DbSet<Fatura> Faturas { get; set; }
        public DbSet<SolicitacaoAjuste> SolicitacoesAjuste { get; set; }
        
        // --- 2. NOVOS Modelos (v2.0 Fluxo de Faturamento) ---
        public DbSet<FaturamentoParcial> FaturamentosParciais { get; set; }
        public DbSet<FaturamentoDiario> FaturamentosDiarios { get; set; }
        public DbSet<MetodoPagamento> MetodosPagamento { get; set; }

        // --- 3. NOVOS Modelos (v2.0 Módulo de Despesas) ---
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        // --- 4. REMOVIDOS (v1.0) ---
        // public DbSet<Empresa> Empresas { get; set; } // Removido
        // public DbSet<Faturamento> Faturamentos { get; set; } // Removido


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 5. MUDANÇA (v2.0): Chave Primária Composta Atualizada ---
            // Usa TenantId e UnidadeId
            modelBuilder.Entity<UsuarioOperacao>()
                .HasKey(uo => new { uo.TenantId, uo.UserId, uo.UnidadeId });

            // --- 6. NOVO (v2.0): Índices de Performance para Multi-Tenancy ---
            // Adiciona índices na coluna TenantId em todas as tabelas
            // de negócio para otimizar consultas.
            
            modelBuilder.Entity<Unidade>().HasIndex(o => o.TenantId);
            modelBuilder.Entity<Meta>().HasIndex(m => m.TenantId);
            modelBuilder.Entity<Fatura>().HasIndex(f => f.TenantId);
            modelBuilder.Entity<Mensalista>().HasIndex(m => m.TenantId);
            modelBuilder.Entity<SolicitacaoAjuste>().HasIndex(sa => sa.TenantId);
            modelBuilder.Entity<FaturamentoParcial>().HasIndex(fp => fp.TenantId);
            modelBuilder.Entity<FaturamentoDiario>().HasIndex(fd => fd.TenantId);
            modelBuilder.Entity<MetodoPagamento>().HasIndex(mp => mp.TenantId);
            
            // Índices para o Módulo de Despesas
            modelBuilder.Entity<ExpenseCategory>().HasIndex(ec => ec.TenantId);
            modelBuilder.Entity<Expense>().HasIndex(e => e.TenantId);
            modelBuilder.Entity<Expense>().HasIndex(e => e.UnidadeId); // Índice para performance
        }
    }
}