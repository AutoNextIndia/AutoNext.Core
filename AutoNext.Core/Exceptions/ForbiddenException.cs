namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when user is authenticated but doesn't have permission
    /// </summary>
    public class ForbiddenException : AppException
    {
        public string? RequiredPermission { get; }
        public string? RequiredRole { get; }

        public ForbiddenException(string message = "You don't have permission to access this resource")
            : base("FORBIDDEN", message, 403)
        {
        }

        public ForbiddenException(string requiredPermission, bool isPermission = true)
            : base("FORBIDDEN", $"Permission '{requiredPermission}' is required", 403)
        {
            RequiredPermission = requiredPermission;
        }
    }
}
