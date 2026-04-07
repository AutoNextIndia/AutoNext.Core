namespace AutoNext.Core.Entities
{
    /// <summary>
    /// Domain event interface
    /// </summary>
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
        string EventType { get; }
    }
}
