// BillingService/Exceptions/ExpenseCategoryAlreadyExistsException.cs
using SharedKernel.Exceptions;

namespace BillingService.Exceptions
{
    public class ExpenseCategoryAlreadyExistsException : BusinessException
    {
        public ExpenseCategoryAlreadyExistsException(string categoryName)
            : base(
                businessRuleCode: "EXPENSE_CATEGORY_ALREADY_EXISTS",
                errorCode: "EXPENSE_CATEGORY_ALREADY_EXISTS",
                message: $"Expense category '{categoryName}' already exists.")
        {
        }
        
        public ExpenseCategoryAlreadyExistsException(string categoryName, Guid tenantId)
            : base(
                businessRuleCode: "EXPENSE_CATEGORY_ALREADY_EXISTS",
                errorCode: "EXPENSE_CATEGORY_ALREADY_EXISTS",
                message: $"Expense category '{categoryName}' already exists for tenant {tenantId}.")
        {
        }
    }
}