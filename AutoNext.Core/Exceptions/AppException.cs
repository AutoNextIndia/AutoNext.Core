namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Base application exception
    /// </summary>
    public abstract class AppException : Exception
    {
        public string Code { get; }
        public int StatusCode { get; }
        public object? AdditionalData { get; }

        protected AppException(string code, string message, int statusCode = 400, object? additionalData = null)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
            AdditionalData = additionalData;
        }
    }
}
