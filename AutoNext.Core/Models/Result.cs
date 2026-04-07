namespace AutoNext.Core.Models
{
    /// <summary>
    /// Result pattern for explicit error handling
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Successful result cannot have an error");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failed result must have an error");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);
        public static Result<T> Success<T>(T value) => new(value, true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    /// <summary>
    /// Generic result with value
    /// </summary>
    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot access value of failed result");
                return _value!;
            }
        }

        internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure<T>(error);
    }

    /// <summary>
    /// Error model
    /// </summary>
    public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
        public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
        public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
        public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);
    }

    public enum ErrorType
    {
        None,
        NotFound,
        Validation,
        Conflict,
        Unauthorized,
        Forbidden,
        Failure
    }
}
