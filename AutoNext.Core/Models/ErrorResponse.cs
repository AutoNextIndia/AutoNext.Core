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
}
