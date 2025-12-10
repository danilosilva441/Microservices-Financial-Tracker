// BillingService/Exceptions/InvalidExpenseFileException.cs
using SharedKernel.Exceptions;

namespace BillingService.Exceptions
{
    public class InvalidExpenseFileException : ValidationException
    {
        public InvalidExpenseFileException(string message)
            : base(
                errors: new Dictionary<string, string[]> 
                { 
                    ["File"] = new[] { message } 
                },
                errorCode: "INVALID_EXPENSE_FILE")
        {
        }
        
        public InvalidExpenseFileException(Dictionary<string, string[]> errors)
            : base(
                errors: errors,
                errorCode: "INVALID_EXPENSE_FILE")
        {
        }
        
        public InvalidExpenseFileException(string message, Dictionary<string, string[]> errors)
            : base(
                errors: AddGlobalError(errors, message),
                errorCode: "INVALID_EXPENSE_FILE")
        {
        }
        
        private static Dictionary<string, string[]> AddGlobalError(
            Dictionary<string, string[]> errors, 
            string globalMessage)
        {
            if (!errors.ContainsKey("_global"))
            {
                errors["_global"] = new[] { globalMessage };
            }
            else
            {
                var globalErrors = errors["_global"].ToList();
                globalErrors.Add(globalMessage);
                errors["_global"] = globalErrors.ToArray();
            }
            
            return errors;
        }
    }
}