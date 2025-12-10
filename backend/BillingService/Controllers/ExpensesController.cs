using BillingService.DTOs;
using BillingService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(
            IExpenseService expenseService,
            ILogger<ExpensesController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }

        // Função helper para pegar o TenantId do token JWT
        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            
            if (string.IsNullOrWhiteSpace(tenantIdClaim))
            {
                throw new UnauthorizedAccessException("Tenant ID not found in token.");
            }

            if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                throw new UnauthorizedAccessException("Invalid Tenant ID format in token.");
            }

            return tenantId;
        }

        // --- Endpoints de Categoria ---

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var tenantId = GetTenantId();
                var categories = await _expenseService.GetCategoriesAsync(tenantId);
                return Ok(categories);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories for tenant");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategoryCreateDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                // CORREÇÃO: Acessando a tupla diretamente (sem Result<T>)
                var (category, errorMessage) = await _expenseService.CreateCategoryAsync(dto, tenantId);

                if (errorMessage != null)
                    return BadRequest(errorMessage);

                return CreatedAtAction(
                    nameof(GetCategories), 
                    null, 
                    category);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category for tenant");
                return StatusCode(500, "An error occurred while creating the category.");
            }
        }

        // --- Endpoints de Despesa ---

        [HttpGet("unidade/{unidadeId:guid}")]
        public async Task<IActionResult> GetExpensesByUnidade(Guid unidadeId)
        {
            try
            {
                if (unidadeId == Guid.Empty)
                    return BadRequest("Invalid unidade ID.");

                var tenantId = GetTenantId();
                // CORREÇÃO: Apenas retorna a lista, não tem Result<T>
                var expenses = await _expenseService.GetExpensesByUnidadeAsync(unidadeId, tenantId);
                
                return Ok(expenses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expenses for unidade {UnidadeId}", unidadeId);
                return StatusCode(500, "An error occurred while fetching expenses.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseCreateDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                // CORREÇÃO: Acessando a tupla diretamente
                var (expense, errorMessage) = await _expenseService.CreateExpenseAsync(dto, tenantId);

                if (errorMessage != null)
                {
                    return errorMessage switch
                    {
                        string msg when msg.Contains("não encontrada") => NotFound(errorMessage),
                        _ => BadRequest(errorMessage)
                    };
                }

                return CreatedAtAction(
                    nameof(GetExpensesByUnidade),
                    new { unidadeId = expense!.UnidadeId },
                    expense);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense for tenant");
                return StatusCode(500, "An error occurred while creating the expense.");
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid expense ID.");

                var tenantId = GetTenantId();
                // CORREÇÃO: Acessando a tupla diretamente
                var (success, errorMessage) = await _expenseService.DeleteExpenseAsync(id, tenantId);

                if (!success)
                    return NotFound(errorMessage);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
                return StatusCode(500, "An error occurred while deleting the expense.");
            }
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin, Gerente")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadExpenses(IFormFile file)
        {
            try
            {
                // Validação do arquivo
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file was uploaded.");
                }

                // Limite de tamanho: 10MB
                const long maxFileSize = 10 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return StatusCode(413, "File size exceeds the maximum limit of 10MB.");
                }

                // Validação da extensão
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".xlsx", ".csv" };
                
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest($"Invalid file format. Only {string.Join(", ", allowedExtensions)} are allowed.");
                }

                var tenantId = GetTenantId();

                await using var stream = file.OpenReadStream();
                // CORREÇÃO: Acessando a tupla diretamente
                var (success, errorMessage, processedRows, skippedRows) = await _expenseService.ImportExpensesAsync(stream, extension, tenantId);

                if (!success)
                {
                    return BadRequest(new 
                    { 
                        ErrorMessage = errorMessage, 
                        SkippedRows = skippedRows 
                    });
                }

                // CORREÇÃO: Converter List<int> para List<SkippedRowDto>
                var skippedRowDtos = skippedRows?.Select(rowNum => new SkippedRowDto
                {
                    RowNumber = rowNum,
                    Reason = "Linha com dados inválidos ou duplicados"
                }).ToList() ?? new List<SkippedRowDto>();

                return Ok(new UploadResultDto
                {
                    Message = "Planilha importada com sucesso.",
                    ProcessedRows = processedRows,
                    SkippedRows = skippedRowDtos,
                    TotalRows = processedRows + skippedRowDtos.Count
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing expenses from file {FileName}", file?.FileName);
                return StatusCode(500, "An error occurred while importing the spreadsheet.");
            }
        }
    }
}