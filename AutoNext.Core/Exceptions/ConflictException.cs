using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoNext.Core.Exceptions
{
    /// <summary>
    /// Thrown when there's a conflict (e.g., duplicate entry)
    /// </summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base("CONFLICT", message, 409)
        {
        }

        public ConflictException(string entity, string property, string value)
            : base("CONFLICT", $"{entity} with {property} '{value}' already exists", 409)
        {
        }
    }
}
