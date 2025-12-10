// BillingService/Exceptions/ExpenseNotFoundException.cs
using SharedKernel.Exceptions;

namespace BillingService.Exceptions
{
    public class ExpenseNotFoundException : NotFoundException
    {
        public ExpenseNotFoundException(Guid expenseId)
            : base(
                errorCode: "EXPENSE_NOT_FOUND",
                message: $"Expense with ID '{expenseId}' not found.")
        {
        }
        
        public ExpenseNotFoundException(Guid expenseId, Guid tenantId)
            : base(
                errorCode: "EXPENSE_NOT_FOUND",
                message: $"Expense with ID '{expenseId}' not found for tenant {tenantId}.")
        {
        }
    }
}