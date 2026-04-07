using AutoNext.Core.Abstractions;
using AutoNext.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoNext.Core.Services.Database
{
    public class EfReadOnlyRepository<T, TId> : IReadOnlyRepository<T, TId> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfReadOnlyRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FindFirstAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T?> FindSingleAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification?.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.CountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification?.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.AnyAsync(cancellationToken);
        }

        public virtual async Task<PagedResult<T>> GetPagedAsync(
            ISpecification<T> specification,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual async Task<IReadOnlyList<T>> GetAllWithIncludesAsync(
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TResult>> ProjectAsync<TResult>(
            ISpecification<T> specification,
            Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.Select(selector).ToListAsync(cancellationToken);
        }

        protected IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
        }
    }
}