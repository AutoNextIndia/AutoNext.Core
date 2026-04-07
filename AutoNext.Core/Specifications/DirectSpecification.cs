using System.Linq.Expressions;


namespace AutoNext.Core.Specifications
{
    public class DirectSpecification<T> : BaseSpecification<T>
    {
        public DirectSpecification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }
    }
}
