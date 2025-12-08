namespace SharedKernel.Common
{
    public class Result
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        public bool IsFailure => !Success;

        protected Result(bool success, string error)
        {
            Success = success;
            ErrorMessage = error;
        }

        // Métodos Factory para facilitar o uso
        public static Result Fail(string message) => new Result(false, message);
        public static Result<T> Fail<T>(string message) => new Result<T>(default, false, message);
        
        public static Result Ok() => new Result(true, string.Empty);
        public static Result<T> Ok<T>(T value) => new Result<T>(value, true, string.Empty);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected internal Result(T? value, bool success, string error) : base(success, error)
        {
            Value = value;
        }
    }
}