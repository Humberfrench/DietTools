using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Api.Core.Results
{
    public class ServiceUnavailableResult : MethodResult
    {
        public Exception Exception { get; set; }

        public ServiceUnavailableResult(Exception exception)
            : base(ResultStatusCode.ServiceUnavailable)
        {
            Exception = exception;
        }
    }
}
