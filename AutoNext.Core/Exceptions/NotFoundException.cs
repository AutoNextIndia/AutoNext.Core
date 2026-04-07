namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : AppException
    {
        public string EntityType { get; }
        public string EntityId { get; }

        public NotFoundException(string entityType, string entityId)
            : base("NOT_FOUND", $"{entityType} with id '{entityId}' was not found", 404)
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        public NotFoundException(string message) : base("NOT_FOUND", message, 404)
        {
            EntityType = "Resource";
            EntityId = "Unknown";
        }
    }
}
