using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public class InternalPersonalWarning : MethodResult<ProblemDetails>
    {
        public InternalPersonalWarning(ProblemDetails content)
                : base(content, ResultStatusCode.InternalPersonalWarning)
        {
        }
    }
}
