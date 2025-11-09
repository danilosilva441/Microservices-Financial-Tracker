// Caminho: backend/BillingService/Repositories/Interfaces/IExpenseRepository.cs
using BillingService.Models;

namespace BillingService.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetByIdAsync(Guid id, Guid tenantId);
        
        // --- MUDANÇA (v2.0) ---
        // Renomeado de OperacaoId para UnidadeId
        Task<IEnumerable<Expense>> GetAllByUnidadeAsync(Guid unidadeId, Guid tenantId);

        Task AddAsync(Expense expense);
        void Update(Expense expense);
        void Remove(Expense expense);
        Task SaveChangesAsync();

        // Métodos para Categoria de Despesa
        Task AddCategoryAsync(ExpenseCategory category);
        Task<ExpenseCategory?> GetCategoryByIdAsync(Guid id, Guid tenantId);
        Task<IEnumerable<ExpenseCategory>> GetAllCategoriesAsync(Guid tenantId);
    }
}