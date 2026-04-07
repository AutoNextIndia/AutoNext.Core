using AutoNext.Core.Abstractions;
using System.Linq.Expressions;

namespace AutoNext.Core.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification()
        {
            Includes = new List<Expression<Func<T, object>>>();
            IncludeStrings = new List<string>();
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria) : this()
        {
            Criteria = criteria;
        }

        // Filter
        public Expression<Func<T, bool>>? Criteria { get; protected set; }

        // Includes
        public List<Expression<Func<T, object>>> Includes { get; }
        public List<string> IncludeStrings { get; }

        // Ordering
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
        public Expression<Func<T, object>>? GroupBy { get; protected set; }

        // Paging
        public int Take { get; protected set; }
        public int Skip { get; protected set; }
        public bool IsPagingEnabled { get; protected set; }

        // Query options
        public bool AsNoTracking { get; protected set; } = true;
        public bool AsSplitQuery { get; protected set; }

        // Protected methods
        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddIncludes(params Expression<Func<T, object>>[] includeExpressions)
        {
            foreach (var include in includeExpressions)
            {
                Includes.Add(include);
            }
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected void DisableTracking()
        {
            AsNoTracking = true;
        }

        protected void EnableTracking()
        {
            AsNoTracking = false;
        }

        protected void EnableSplitQuery()
        {
            AsSplitQuery = true;
        }

        protected void ApplyTake(int take)
        {
            if (take > 0)
            {
                Take = take;
                IsPagingEnabled = true;
            }
        }

        protected void ApplySkip(int skip)
        {
            if (skip >= 0)
            {
                Skip = skip;
                IsPagingEnabled = true;
            }
        }
    }
}