using AutoNext.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoNext.Core.Queries
{
    /// <summary>
    /// Query marker interface (CQRS - read operations)
    /// </summary>
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }

  
}
