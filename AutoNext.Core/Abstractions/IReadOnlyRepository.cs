namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Read-only repository for query operations
    /// </summary>
    public interface IReadOnlyRepository<T, TId> where T : class
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default);
    }
}
