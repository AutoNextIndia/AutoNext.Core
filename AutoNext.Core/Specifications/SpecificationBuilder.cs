using AutoNext.Core.Abstractions;
using System;
using System.Linq.Expressions;

namespace AutoNext.Core.Specifications
{
    public class SpecificationBuilder<T> : BaseSpecification<T> where T : class
    {
        public SpecificationBuilder() { }

        public SpecificationBuilder(Expression<Func<T, bool>> criteria) : base(criteria) { }

        public SpecificationBuilder<T> Filter(Expression<Func<T, bool>> criteria)
        {
            AddCriteria(criteria);
            return this;
        }

        public SpecificationBuilder<T> Include(Expression<Func<T, object>> includeExpression)
        {
            AddInclude(includeExpression);
            return this;
        }

        public SpecificationBuilder<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            AddIncludes(includeExpressions);
            return this;
        }

        public SpecificationBuilder<T> Include(string includeString)
        {
            AddInclude(includeString);
            return this;
        }

        public SpecificationBuilder<T> OrderBy(Expression<Func<T, object>> orderByExpression)
        {
            ApplyOrderBy(orderByExpression);
            return this;
        }

        public SpecificationBuilder<T> OrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            ApplyOrderByDescending(orderByDescendingExpression);
            return this;
        }

        public SpecificationBuilder<T> GroupBy(Expression<Func<T, object>> groupByExpression)
        {
            ApplyGroupBy(groupByExpression);
            return this;
        }

        public SpecificationBuilder<T> Page(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;
            ApplyPaging(skip, pageSize);
            return this;
        }

        public SpecificationBuilder<T> Take(int take)
        {
            ApplyTake(take);
            return this;
        }

        public SpecificationBuilder<T> Skip(int skip)
        {
            ApplySkip(skip);
            return this;
        }

        public SpecificationBuilder<T> NoTracking()
        {
            DisableTracking();
            return this;
        }

        public SpecificationBuilder<T> Track()
        {
            EnableTracking();
            return this;
        }

        public SpecificationBuilder<T> SplitQuery()
        {
            EnableSplitQuery();
            return this;
        }

        public ISpecification<T> Build() => this;
    }
}