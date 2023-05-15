using System.Collections.Generic;

namespace Dietcode.Api.Core.Results
{
    public class TimeOutResult : ErrorResult
    {
        public TimeOutResult(ErrorValidation error)
            : base(ResultStatusCode.Conflict, error)
        {
        }

        public TimeOutResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.Conflict, errors)
        {
        }
    }
}
