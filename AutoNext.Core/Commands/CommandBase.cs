namespace AutoNext.Core.Commands
{
    /// <summary>
    /// Base command class with tracking
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public Guid CommandId { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }
}
