namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when a business rule is violated
    /// </summary>
    public class BusinessRuleException : AppException
    {
        public string RuleName { get; }

        public BusinessRuleException(string ruleName, string message)
            : base("BUSINESS_RULE_VIOLATION", message, 422)
        {
            RuleName = ruleName;
        }
    }
}
