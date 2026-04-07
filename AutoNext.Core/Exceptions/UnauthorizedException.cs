namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when user is not authenticated
    /// </summary>
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Authentication is required to access this resource")
            : base("UNAUTHORIZED", message, 401)
        {
        }
    }
}
