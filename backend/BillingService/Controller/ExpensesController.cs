using BillingService.DTOs;
using BillingService.Services.Interfaces;
using BillingService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todo o controller
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // Função helper para pegar o TenantId do token JWT
        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value;
            if (tenantIdClaim == null)
                throw new InvalidOperationException("Tenant ID (tenantId) not found in token.");
            return Guid.Parse(tenantIdClaim);
        }

        // --- Endpoints de Categoria ---

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var tenantId = GetTenantId();
            var categories = await _expenseService.GetCategoriesAsync(tenantId);
            return Ok(categories);
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategoryCreateDto dto)
        {
            var tenantId = GetTenantId();
            var (category, errorMessage) = await _expenseService.CreateCategoryAsync(dto, tenantId);

            if (errorMessage != null)
                return BadRequest(errorMessage);

            return CreatedAtAction(nameof(GetCategories), new { id = category!.Id }, category);
        }

        // --- Endpoints de Despesa ---

        // 1. MUDANÇA (v2.0): Rota e parâmetro atualizados
        [HttpGet("unidade/{unidadeId}")]
        public async Task<IActionResult> GetExpensesByUnidade(Guid unidadeId)
        {
            var tenantId = GetTenantId();
            // 2. MUDANÇA (v2.0): Chamando o método v2.0 (corrige o erro CS1061)
            var expenses = await _expenseService.GetExpensesByUnidadeAsync(unidadeId, tenantId);
            return Ok(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseCreateDto dto)
        {
            var tenantId = GetTenantId();
            var (expense, errorMessage) = await _expenseService.CreateExpenseAsync(dto, tenantId);

            if (errorMessage != null)
            {
                if (errorMessage.Contains("não encontrada"))
                    return NotFound(errorMessage);

                return BadRequest(errorMessage);
            }

            // 3. MUDANÇA (v2.0): Corrigindo o CreatedAtAction (corrige o erro CS1061)
            return CreatedAtAction(
                nameof(GetExpensesByUnidade), // 3a. Nome do método v2.0
                new { unidadeId = expense!.UnidadeId }, // 3b. Propriedade v2.0
                expense);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var tenantId = GetTenantId();
            var (success, errorMessage) = await _expenseService.DeleteExpenseAsync(id, tenantId);

            if (!success)
                return NotFound(errorMessage);

            return NoContent(); // Sucesso
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> UploadExpenses(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            // Validação simples do tipo de arquivo (opcional, mas recomendado)
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".csv")
            {
                return BadRequest("Formato de arquivo inválido. Apenas .xlsx ou .csv são aceitos.");
            }

            var tenantId = GetTenantId();

            // Usamos 'await using' para garantir que o stream do arquivo seja fechado
            await using var stream = file.OpenReadStream();

            // 4. Delegamos a lógica de leitura e salvamento para o serviço
            var (success, errorMessage, processedRows, skippedRows) = await _expenseService.ImportExpensesAsync(stream, extension, tenantId);

            if (!success)
            {
                return BadRequest(new { ErrorMessage = errorMessage, SkippedRows = skippedRows });
            }

            return Ok(new
            {
                Message = "Planilha importada com sucesso.",
                ProcessedRows = processedRows,
                SkippedRows = skippedRows
            });
        }
    }
}