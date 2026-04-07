namespace AutoNext.Core.Queries
{
    /// <summary>
    /// Base query class with pagination
    /// </summary>
    public abstract class QueryBase<TResponse> : IQuery<TResponse>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
