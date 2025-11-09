// Caminho: backend/BillingService/Services/Interfaces/IExpenseService.cs
using BillingService.DTOs;
using BillingService.Models;
using System.IO; // 1. ADICIONE ESTE USING (para o Stream)

namespace BillingService.Services.Interfaces
{
    public interface IExpenseService
    {
        // Métodos de Categoria de Despesa
        Task<(ExpenseCategory? category, string? errorMessage)> CreateCategoryAsync(ExpenseCategoryCreateDto dto, Guid tenantId);
        Task<IEnumerable<ExpenseCategory>> GetCategoriesAsync(Guid tenantId);

        // Métodos de Despesa
        Task<(Expense? expense, string? errorMessage)> CreateExpenseAsync(ExpenseCreateDto dto, Guid tenantId);
        Task<IEnumerable<Expense>> GetExpensesByUnidadeAsync(Guid unidadeId, Guid tenantId);
        
        // --- 2. CORREÇÃO (Errors 1, 2, 3) ---
        // Este método estava faltando na interface
        Task<(bool success, string? errorMessage)> DeleteExpenseAsync(Guid expenseId, Guid tenantId);

        // Método de Upload
        Task<(bool success, string? errorMessage, int processedRows, List<int>? skippedRows)> ImportExpensesAsync(Stream stream, string fileType, Guid tenantId);
    }
}