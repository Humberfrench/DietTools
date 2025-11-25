using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.Core.Results.Interfaces
{
    public interface IContentResult
    {
        ResultStatusCode Status { get; }
        object Content { get; }
    }

    public interface IContentResult<T> : IContentResult
    {
        new T Content { get; }
    }

}
