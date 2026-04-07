namespace AutoNext.Core.Entities
{
    /// <summary>
    /// Aggregate root base class for DDD
    /// </summary>
    public abstract class AggregateRoot : SoftDeleteEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
    
    
    /// <summary>
    /// Base domain event
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public abstract string EventType { get; }
    }
}
