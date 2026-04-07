using AutoNext.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoNext.Core.Services.Database
{
    internal static class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// Applies specification to IQueryable for execution
        /// </summary>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // 1. Filter
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // 2. Includes (Expression-based)
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // 3. Includes (String-based for nested properties)
            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            // 4. Ordering (only one ordering allowed)
            query = ApplyOrdering(query, specification);

            // 5. Grouping (WARNING: Use carefully - changes return type)
            if (specification.GroupBy != null)
            {
                query = ApplyGrouping(query, specification.GroupBy);
            }

            // 6. Paging
            if (specification.IsPagingEnabled && specification.Take > 0)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            // 7. Query options
            query = ApplyQueryOptions(query, specification);

            return query;
        }

        private static IQueryable<T> ApplyOrdering(IQueryable<T> query, ISpecification<T> specification)
        {
            // Prefer OrderByDescending if both are set (common pattern)
            if (specification.OrderByDescending != null)
            {
                return query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.OrderBy != null)
            {
                return query.OrderBy(specification.OrderBy);
            }

            return query;
        }

        private static IQueryable<T> ApplyGrouping(IQueryable<T> query, Expression<Func<T, object>>? groupBy)
        {
            if (groupBy == null) return query;

            // WARNING: GroupBy changes the query shape - use SelectMany to flatten
            // This is a simplified version - adjust based on your needs
            return query.GroupBy(groupBy).SelectMany(g => g);
        }

        private static IQueryable<T> ApplyQueryOptions(IQueryable<T> query, ISpecification<T> specification)
        {
            var optionsQuery = query;

            // NoTracking (performance for read-only)
            if (specification.AsNoTracking)
            {
                optionsQuery = optionsQuery.AsNoTracking();
            }

            // SplitQuery (for complex includes - avoids Cartesian explosion)
            if (specification.AsSplitQuery)
            {
                optionsQuery = optionsQuery.AsSplitQuery();
            }

            return optionsQuery;
        }
    }
}