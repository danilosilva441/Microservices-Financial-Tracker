// BillingService/Exceptions/ExpenseCategoryNotFoundException.cs
using SharedKernel.Exceptions;

namespace BillingService.Exceptions
{
    public class ExpenseCategoryNotFoundException : NotFoundException
    {
        public ExpenseCategoryNotFoundException(Guid categoryId)
            : base(
                errorCode: "EXPENSE_CATEGORY_NOT_FOUND",
                message: $"Expense category with ID '{categoryId}' not found.")
        {
        }
        
        public ExpenseCategoryNotFoundException(string categoryName)
            : base(
                errorCode: "EXPENSE_CATEGORY_NOT_FOUND",
                message: $"Expense category '{categoryName}' not found.")
        {
        }
    }
}