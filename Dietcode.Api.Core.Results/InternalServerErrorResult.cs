using System;

namespace Dietcode.Api.Core.Results
{
    public class InternalServerErrorResult : MethodResult
    {
        public Exception Exception { get; set; }

        public InternalServerErrorResult(Exception exception)
            : base(ResultStatusCode.InternalServerError)
        {
            Exception = exception;
        }
    }
}
