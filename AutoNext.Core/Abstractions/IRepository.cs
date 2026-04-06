namespace AutoNext.Core.Abstractions
{
    // ✅ Add generic parameters <T, TId>
    public interface IRepository<T, TId> where T : class where TId : notnull
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    }
}