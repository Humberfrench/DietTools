using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public class InternalPersonalErrorResult : MethodResult
    {
        public InternalPersonalErrorResult()
            : base(ResultStatusCode.InternalPersonalError)
        {
        }
    }
}
