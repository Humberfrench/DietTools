using System.Collections.Generic;

namespace Dietcode.Api.Core.Results
{
    public class UnauthorizedResult : ErrorResult
    {
        public UnauthorizedResult(ErrorValidation error)
            : base(ResultStatusCode.Unauthorized, error)
        {
        }

        public UnauthorizedResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.Unauthorized, errors)
        {
        }
    }
}
