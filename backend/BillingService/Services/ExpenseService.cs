// Caminho: backend/BillingService/Services/ExpenseService.cs
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using MiniExcelLibs; // 1. ADICIONADO: Para ler planilhas
using System.IO; // 1. ADICIONADO: Para o Stream
using BillingService.Data; // 1. ADICIONADO: Para o DbContext
using Microsoft.EntityFrameworkCore; // 1. ADICIONADO: Para o DbContext
using MiniExcelLibs.OpenXml;

namespace BillingService.Services
{

    // Ela deve ficar AQUI, no namespace, mas FORA da classe ExpenseService.
    internal class ExpenseImportDto
    {
        public Guid UnidadeId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
    }

    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepo;
        // 1. MUDANÇA: Renomeado para _unidadeRepo para clareza
        private readonly IUnidadeRepository _unidadeRepo;
        // 2. ADICIONADO: Injetando o DbContext para operações em LOTE (Batch)
        private readonly BillingDbContext _context;

        public ExpenseService(IExpenseRepository expenseRepo, IUnidadeRepository unidadeRepo, BillingDbContext context)
        {
            _expenseRepo = expenseRepo;
            _unidadeRepo = unidadeRepo;
            _context = context;
        }

        // --- Lógica de Categoria (Sem alteração) ---

        public async Task<(ExpenseCategory? category, string? errorMessage)> CreateCategoryAsync(ExpenseCategoryCreateDto dto, Guid tenantId)
        {
            var newCategory = new ExpenseCategory
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = dto.Name,
                Description = dto.Description
            };

            await _expenseRepo.AddCategoryAsync(newCategory);
            await _expenseRepo.SaveChangesAsync();

            return (newCategory, null);
        }

        public async Task<IEnumerable<ExpenseCategory>> GetCategoriesAsync(Guid tenantId)
        {
            return await _expenseRepo.GetAllCategoriesAsync(tenantId);
        }

        // --- Lógica de Despesa (Atualizada) ---

        public async Task<(Expense? expense, string? errorMessage)> CreateExpenseAsync(ExpenseCreateDto dto, Guid tenantId)
        {
            // 2. MUDANÇA: Valida a Unidade (usando o DTO v2.0)
            var unidade = await _unidadeRepo.GetByIdAsync(dto.UnidadeId, tenantId);
            if (unidade == null)
            {
                return (null, "Unidade não encontrada.");
            }

            // 2. Valida se a Categoria existe
            var category = await _expenseRepo.GetCategoryByIdAsync(dto.CategoryId, tenantId);
            if (category == null)
            {
                return (null, "Categoria de despesa não encontrada.");
            }

            // 3. MUDANÇA: Cria a entidade v2.0
            var newExpense = new Expense
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = dto.UnidadeId, // <-- Corrigido
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate.ToUniversalTime(),
                Description = dto.Description
            };

            await _expenseRepo.AddAsync(newExpense);
            await _expenseRepo.SaveChangesAsync();

            var result = await _expenseRepo.GetByIdAsync(newExpense.Id, tenantId);
            return (result, null);
        }

        // 4. MUDANÇA: Assinatura e lógica v2.0
        public async Task<IEnumerable<Expense>> GetExpensesByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            return await _expenseRepo.GetAllByUnidadeAsync(unidadeId, tenantId); // <-- Corrigido
        }

        public async Task<(bool success, string? errorMessage)> DeleteExpenseAsync(Guid expenseId, Guid tenantId)
        {
            var expense = await _expenseRepo.GetByIdAsync(expenseId, tenantId);
            if (expense == null)
            {
                return (false, "Despesa não encontrada.");
            }

            _expenseRepo.Remove(expense);
            await _expenseRepo.SaveChangesAsync();
            return (true, null);
        }
        // --- 3. NOSSO NOVO MÉTODO (Tarefa 2) ---
        public async Task<(bool success, string? errorMessage, int processedRows, List<int>? skippedRows)> ImportExpensesAsync(Stream stream, string fileType, Guid tenantId)
        {
            var processedRows = 0;
            var skippedRows = new List<int>();
            var expensesToAdd = new List<Expense>();

            // Para performance, buscamos todas as Ids válidas UMA VEZ
            // em vez de checar o banco de dados em cada linha do loop.
            var validUnidadeIds = (await _unidadeRepo.GetAllAsync(tenantId)).Select(u => u.Id).ToHashSet();
            var validCategoryIds = (await _expenseRepo.GetAllCategoriesAsync(tenantId)).Select(c => c.Id).ToHashSet();

            try
            {
                var config = new OpenXmlConfiguration { FillMergedCells = true };
                var rows = stream.Query<ExpenseImportDto>(
                    excelType: fileType == ".xlsx" ? ExcelType.XLSX : ExcelType.CSV,
                    configuration: config
                );

                int rowNumber = 1; // MiniExcel não conta o cabeçalho
                foreach (var row in rows)
                {
                    rowNumber++; // Começa na linha 2

                    // --- Validação ---
                    if (row.Amount <= 0 ||
                        !validUnidadeIds.Contains(row.UnidadeId) ||
                        !validCategoryIds.Contains(row.CategoryId))
                    {
                        skippedRows.Add(rowNumber);
                        continue; // Pula esta linha
                    }

                    // --- Criação da Entidade ---
                    var newExpense = new Expense
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        UnidadeId = row.UnidadeId,
                        CategoryId = row.CategoryId,
                        Amount = row.Amount,
                        ExpenseDate = row.ExpenseDate.ToUniversalTime(),
                        Description = row.Description ?? string.Empty
                    };

                    expensesToAdd.Add(newExpense);
                    processedRows++;
                }

                // --- Salvamento em Lote (Batch) ---
                // Adiciona todas as despesas válidas ao DbContext de uma só vez
                await _context.Expenses.AddRangeAsync(expensesToAdd);

                // Salva todas as mudanças no banco em uma ÚNICA transação
                await _context.SaveChangesAsync();

                return (true, null, processedRows, skippedRows);
            }
            catch (Exception ex)
            {
                // Se o MiniExcel falhar (ex: coluna com nome errado)
                return (false, $"Erro ao processar a planilha: {ex.Message}", 0, null);
            }
        }
    }
}