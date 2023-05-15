using System.Collections.Generic;

namespace Dietcode.Api.Core.Results
{
    public class ConflictResult : ErrorResult
    {
        public ConflictResult(ErrorValidation error)
            : base(ResultStatusCode.Conflict, error)
        {
        }

        public ConflictResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.Conflict, errors)
        {
        }
    }
}
