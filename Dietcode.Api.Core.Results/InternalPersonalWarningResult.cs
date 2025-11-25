using Microsoft.AspNetCore.Mvc;

namespace Dietcode.Api.Core.Results
{
    public class InternalPersonalWarningResult : MethodResult
    {
        public InternalPersonalWarningResult()
            : base(ResultStatusCode.InternalPersonalWarning)
        {
        }
    }
}
