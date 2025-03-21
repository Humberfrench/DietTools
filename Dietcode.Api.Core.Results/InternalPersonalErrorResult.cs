using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public class InternalPersonalErrorResult : MethodResult<ProblemDetails>
    {
        public InternalPersonalErrorResult(ProblemDetails content)
                : base(content, ResultStatusCode.InternalPersonalError)
        {
        }
    }
}
