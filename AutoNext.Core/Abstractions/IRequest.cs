namespace AutoNext.Core.Abstractions
{
    /// <summary>
    /// Base marker interface for requests (CQRS)
    /// </summary>
    public interface IRequest<out TResponse>
    {
    }

    /// <summary>
    /// Base marker interface for requests without response
    /// </summary>
    public interface IRequest : IRequest<Unit>
    {
    }

    /// <summary>
    /// Unit type for void returns
    /// </summary>
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Value = new();

        public bool Equals(Unit other) => true;
        public override bool Equals(object? obj) => obj is Unit;
        public override int GetHashCode() => 0;
        public override string ToString() => "()";
    }
}
