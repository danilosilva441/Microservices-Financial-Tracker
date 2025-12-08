using Microsoft.EntityFrameworkCore;
using BillingService.Models;
using SharedKernel.Entities;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace BillingService.Data
{
    public class BillingDbContext : DbContext
    {
        // 1. MUDANÇA: Tornamos pública (mas set privado) para o EF Core ver como propriedade
        public Guid? CurrentTenantId { get; private set; }

        public BillingDbContext(DbContextOptions<BillingDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options)
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

        // --- DbSets ---
        public DbSet<Unidade> Unidades { get; set; }
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
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<ShiftBreak> ShiftBreaks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioOperacao>().HasKey(uo => new { uo.TenantId, uo.UserId, uo.UnidadeId });
            modelBuilder.Entity<Unidade>().HasIndex(u => u.TenantId);
            modelBuilder.Entity<Meta>().HasIndex(m => m.TenantId);
            modelBuilder.Entity<Expense>().HasIndex(e => e.UnidadeId);

            // --- CONFIGURAÇÃO DO FILTRO ---
            var method = typeof(BillingDbContext).GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
            {
                var genericMethod = method?.MakeGenericMethod(entityType.ClrType);
                genericMethod?.Invoke(this, new object[] { modelBuilder });
            }

            modelBuilder.Entity<WorkSchedule>()
        .HasIndex(w => new { w.TenantId, w.UnidadeId });

            modelBuilder.Entity<EmployeeShift>()
                .HasIndex(s => new { s.TenantId, s.UnidadeId, s.UserId, s.Date }); // Índice para busca rápida de escala

            modelBuilder.Entity<EmployeeShift>()
                .HasMany(s => s.Breaks)
                .WithOne(b => b.EmployeeShift)
                .HasForeignKey(b => b.EmployeeShiftId)
                .OnDelete(DeleteBehavior.Cascade); // Apagou o turno -> Apaga as pausas
        }

        private void ConfigureTenantFilter<T>(ModelBuilder builder) where T : class, ITenantEntity
        {
            // 2. MUDANÇA: Usamos a propriedade 'CurrentTenantId'.
            // O EF Core reconhece propriedades da classe DbContext como parâmetros de query
            // e evita "cachear" o valor null.
            builder.Entity<T>().HasQueryFilter(e => CurrentTenantId == null || e.TenantId == CurrentTenantId);
        }
    }
}