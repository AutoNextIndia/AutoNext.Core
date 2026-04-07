namespace AutoNext.Core.Models
{
    public class ErrorResponse
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    /// <summary>
    /// Standard error response for APIs
    /// </summary>
    public class ErrorResult
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ErrorResult Create(string code, string message, string? traceId = null)
        {
            return new ErrorResult
            {
                Code = code,
                Message = message,
                TraceId = traceId,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ErrorResult FromError(Error error, string? traceId = null)
        {
            return new ErrorResult
            {
                Code = error.Code,
                Message = error.Message,
                TraceId = traceId,
                Timestamp = DateTime.UtcNow
            };
        }
    }

}
