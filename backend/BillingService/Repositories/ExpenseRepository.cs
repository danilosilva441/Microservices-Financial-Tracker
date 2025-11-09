// Caminho: backend/BillingService/Repositories/ExpenseRepository.cs
using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly BillingDbContext _context;

        public ExpenseRepository(BillingDbContext context)
        {
            _context = context;
        }

        // --- Métodos de Despesa (Expense) ---

        public async Task<Expense?> GetByIdAsync(Guid id, Guid tenantId)
        {
            return await _context.Expenses
                .Include(e => e.Category) // Inclui o nome da categoria
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
        }

        // --- MUDANÇA (v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        public async Task<IEnumerable<Expense>> GetAllByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            return await _context.Expenses
                .Where(e => e.UnidadeId == unidadeId && e.TenantId == tenantId) // <-- Corrigido
                .Include(e => e.Category) // Inclui o nome da categoria
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
        }

        public void Update(Expense expense)
        {
            _context.Expenses.Update(expense);
        }

        public void Remove(Expense expense)
        {
            _context.Expenses.Remove(expense);
        }

        // --- Métodos de Categoria (ExpenseCategory) ---

        public async Task AddCategoryAsync(ExpenseCategory category)
        {
            await _context.ExpenseCategories.AddAsync(category);
        }

        public async Task<ExpenseCategory?> GetCategoryByIdAsync(Guid id, Guid tenantId)
        {
            return await _context.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        }

        public async Task<IEnumerable<ExpenseCategory>> GetAllCategoriesAsync(Guid tenantId)
        {
            return await _context.ExpenseCategories
                .Where(c => c.TenantId == tenantId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // --- Método de Unidade de Trabalho (Salvar) ---
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}