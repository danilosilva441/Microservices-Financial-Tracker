using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using BillingService.Data;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using SharedKernel;
using System.Globalization;

namespace BillingService.Services
{
    // DTO interno para importação
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
        private readonly IUnidadeRepository _unidadeRepo;
        private readonly BillingDbContext _context;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(
            IExpenseRepository expenseRepo,
            IUnidadeRepository unidadeRepo,
            BillingDbContext context,
            ILogger<ExpenseService> logger)
        {
            _expenseRepo = expenseRepo;
            _unidadeRepo = unidadeRepo;
            _context = context;
            _logger = logger;
        }

        #region Category Operations
        /// <summary>
        /// Cria uma nova categoria de despesa
        /// </summary>
        /// <param name="dto">Dados da categoria</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Categoria criada ou mensagem de erro</returns>
        public async Task<(ExpenseCategory? category, string? errorMessage)> CreateCategoryAsync(
            ExpenseCategoryCreateDto dto, Guid tenantId)
        {
            try
            {
                var validationError = ValidateCategoryCreateDto(dto);
                if (validationError != null)
                {
                    _logger.LogWarning("Validação falhou para criação de categoria: {Error}", validationError);
                    return (null, validationError);
                }

                // Verifica se já existe categoria com mesmo nome (usando método disponível)
                var existingCategories = await _expenseRepo.GetAllCategoriesAsync(tenantId);
                var existingCategory = existingCategories.FirstOrDefault(c =>
                    c.Name.Trim().Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase));

                if (existingCategory != null)
                {
                    _logger.LogWarning("Categoria já existe: {CategoryName}", dto.Name);
                    return (null, "Já existe uma categoria com este nome");
                }

                var newCategory = new ExpenseCategory
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim()
                };

                await _expenseRepo.AddCategoryAsync(newCategory);
                await _expenseRepo.SaveChangesAsync();

                _logger.LogInformation("Categoria criada com sucesso: {CategoryId}", newCategory.Id);
                return (newCategory, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria para tenant {TenantId}", tenantId);
                throw new ExpenseServiceException("Erro ao criar categoria", ex);
            }
        }

        public async Task<IEnumerable<ExpenseCategory>> GetCategoriesAsync(Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando categorias para tenant {TenantId}", tenantId);
                return await _expenseRepo.GetAllCategoriesAsync(tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias para tenant {TenantId}", tenantId);
                throw new ExpenseServiceException("Erro ao buscar categorias", ex);
            }
        }

        #endregion

        #region Expense Operations

        public async Task<(Expense? expense, string? errorMessage)> CreateExpenseAsync(
            ExpenseCreateDto dto, Guid tenantId)
        {
            try
            {
                var validationError = ValidateExpenseCreateDto(dto);
                if (validationError != null)
                {
                    _logger.LogWarning("Validação falhou para criação de despesa: {Error}", validationError);
                    return (null, validationError);
                }

                // Valida Unidade
                var unidade = await _unidadeRepo.GetByIdAsync(dto.UnidadeId, tenantId);
                if (unidade == null)
                {
                    _logger.LogWarning("Unidade não encontrada: {UnidadeId}", dto.UnidadeId);
                    return (null, ErrorMessages.UnidadeNotFound);
                }

                // Valida Categoria
                var category = await _expenseRepo.GetCategoryByIdAsync(dto.CategoryId, tenantId);
                if (category == null)
                {
                    _logger.LogWarning("Categoria não encontrada: {CategoryId}", dto.CategoryId);
                    return (null, ErrorMessages.CategoriaNotFound);
                }

                var newExpense = CreateExpenseEntity(dto, tenantId);

                await _expenseRepo.AddAsync(newExpense);
                await _expenseRepo.SaveChangesAsync();

                // Recupera a expense usando o método disponível no repository
                var result = await _expenseRepo.GetByIdAsync(newExpense.Id, tenantId);

                _logger.LogInformation("Despesa criada com sucesso: {ExpenseId}", newExpense.Id);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar despesa para unidade {UnidadeId}", dto.UnidadeId);
                throw new ExpenseServiceException("Erro ao criar despesa", ex);
            }
        }

        public async Task<IEnumerable<Expense>> GetExpensesByUnidadeAsync(Guid unidadeId, Guid tenantId)
        {            
            try
            {
                _logger.LogDebug("Buscando despesas da unidade {UnidadeId}", unidadeId);
                return await _expenseRepo.GetAllByUnidadeAsync(unidadeId, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar despesas da unidade {UnidadeId}", unidadeId);
                throw new ExpenseServiceException("Erro ao buscar despesas", ex);
            }
            
        }

        public async Task<Expense?> GetExpenseByIdAsync(Guid expenseId, Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando despesa {ExpenseId}", expenseId);
                return await _expenseRepo.GetByIdAsync(expenseId, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar despesa {ExpenseId}", expenseId);
                throw new ExpenseServiceException("Erro ao buscar despesa", ex);
            }
        }

        public async Task<(bool success, string? errorMessage)> DeleteExpenseAsync(Guid expenseId, Guid tenantId)
        {
            try
            {
                var expense = await _expenseRepo.GetByIdAsync(expenseId, tenantId);
                if (expense == null)
                {
                    _logger.LogWarning("Despesa não encontrada para exclusão: {ExpenseId}", expenseId);
                    return (false, ErrorMessages.DespesaNotFound);
                }

                _expenseRepo.Remove(expense);
                await _expenseRepo.SaveChangesAsync();

                _logger.LogInformation("Despesa excluída com sucesso: {ExpenseId}", expenseId);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir despesa {ExpenseId}", expenseId);
                throw new ExpenseServiceException("Erro ao excluir despesa", ex);
            }
        }

        #endregion

        #region Import Operations

        public async Task<(bool success, string? errorMessage, int processedRows, List<int>? skippedRows)>
            ImportExpensesAsync(Stream stream, string fileType, Guid tenantId)
        {
            var processedRows = 0;
            var skippedRows = new List<int>();
            var expensesToAdd = new List<Expense>();

            try
            {
                // Validação do tipo de arquivo
                if (!IsValidFileType(fileType))
                {
                    _logger.LogWarning("Tipo de arquivo inválido para importação: {FileType}", fileType);
                    return (false, "Tipo de arquivo não suportado. Use .xlsx ou .csv", 0, null);
                }

                // Cache de IDs válidos para performance
                var (validUnidadeIds, validCategoryIds) = await GetValidIdsCacheAsync(tenantId);

                var config = new OpenXmlConfiguration { FillMergedCells = true };
                var rows = stream.Query<ExpenseImportDto>(
                    excelType: GetExcelType(fileType),
                    configuration: config
                );

                int rowNumber = 1; // Cabeçalho
                foreach (var row in rows)
                {
                    rowNumber++;

                    var validationResult = ValidateImportRow(row, validUnidadeIds, validCategoryIds, rowNumber);
                    if (!validationResult.isValid)
                    {
                        skippedRows.Add(rowNumber);
                        _logger.LogDebug("Linha {RowNumber} ignorada: {Reason}", rowNumber, validationResult.reason);
                        continue;
                    }

                    var newExpense = CreateExpenseFromImportRow(row, tenantId);
                    expensesToAdd.Add(newExpense);
                    processedRows++;
                }

                if (expensesToAdd.Any())
                {
                    await _context.Expenses.AddRangeAsync(expensesToAdd);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation(
                    "Importação concluída: {Processed} processadas, {Skipped} ignoradas",
                    processedRows, skippedRows.Count);

                return (true, null, processedRows, skippedRows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante importação de despesas");
                return (false, $"Erro ao processar a planilha: {ex.Message}", 0, null);
            }
        }

        #endregion

        #region Private Methods

        private static Expense CreateExpenseEntity(ExpenseCreateDto dto, Guid tenantId)
        {
            return new Expense
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = dto.UnidadeId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate.ToUniversalTime().Date,
                Description = dto.Description?.Trim() ?? string.Empty
            };
        }

        private static Expense CreateExpenseFromImportRow(ExpenseImportDto row, Guid tenantId)
        {
            return new Expense
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = row.UnidadeId,
                CategoryId = row.CategoryId,
                Amount = row.Amount,
                ExpenseDate = row.ExpenseDate.ToUniversalTime().Date,
                Description = row.Description?.Trim() ?? string.Empty
            };
        }

        private async Task<(HashSet<Guid> unidadeIds, HashSet<Guid> categoryIds)> GetValidIdsCacheAsync(Guid tenantId)
        {
            var unidadeIds = (await _unidadeRepo.GetAllAsync(tenantId))
                .Select(u => u.Id)
                .ToHashSet();

            var categoryIds = (await _expenseRepo.GetAllCategoriesAsync(tenantId))
                .Select(c => c.Id)
                .ToHashSet();

            return (unidadeIds, categoryIds);
        }

        private static (bool isValid, string reason) ValidateImportRow(
            ExpenseImportDto row,
            HashSet<Guid> validUnidadeIds,
            HashSet<Guid> validCategoryIds,
            int rowNumber)
        {
            if (row.Amount <= 0)
                return (false, "Amount deve ser maior que zero");

            if (!validUnidadeIds.Contains(row.UnidadeId))
                return (false, "UnidadeId inválido");

            if (!validCategoryIds.Contains(row.CategoryId))
                return (false, "CategoryId inválido");

            if (row.ExpenseDate > DateTime.UtcNow.Date)
                return (false, "ExpenseDate não pode ser futura");

            return (true, string.Empty);
        }

        private static string? ValidateExpenseCreateDto(ExpenseCreateDto dto)
        {
            if (dto == null)
                return "DTO não pode ser nulo";

            if (dto.Amount <= 0)
                return "Amount deve ser maior que zero";

            if (dto.ExpenseDate > DateTime.UtcNow.Date)
                return "ExpenseDate não pode ser futura";

            if (string.IsNullOrWhiteSpace(dto.Description))
                return "Description é obrigatória";

            if (dto.Description.Length > 500)
                return "Description não pode exceder 500 caracteres";

            return null;
        }

        private static string? ValidateCategoryCreateDto(ExpenseCategoryCreateDto dto)
        {
            if (dto == null)
                return "DTO não pode ser nulo";

            if (string.IsNullOrWhiteSpace(dto.Name))
                return "Name é obrigatório";

            if (dto.Name.Length > 100)
                return "Name não pode exceder 100 caracteres";

            if (dto.Description?.Length > 500)
                return "Description não pode exceder 500 caracteres";

            return null;
        }

        private static bool IsValidFileType(string fileType)
        {
            return fileType == ".xlsx" || fileType == ".csv";
        }

        private static ExcelType GetExcelType(string fileType)
        {
            return fileType == ".xlsx" ? ExcelType.XLSX : ExcelType.CSV;
        }

        #endregion
    }

    // Exceção customizada
    public class ExpenseServiceException : Exception
    {
        public ExpenseServiceException(string message) : base(message) { }
        public ExpenseServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}