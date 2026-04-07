namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when validation fails
    /// </summary>
    public class ValidationException : AppException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message)
            : base("VALIDATION_ERROR", message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("VALIDATION_ERROR", "One or more validation errors occurred", 400)
        {
            Errors = errors;
        }

        public ValidationException(string property, string error)
            : base("VALIDATION_ERROR", $"Validation failed for {property}", 400)
        {
            Errors = new Dictionary<string, string[]>
        {
            { property, new[] { error } }
        };
        }
    }
}
