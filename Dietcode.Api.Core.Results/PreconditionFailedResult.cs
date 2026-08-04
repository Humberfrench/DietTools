namespace Dietcode.Api.Core.Results
{
    public class PreconditionFailedResult : ErrorResult
    {
        public PreconditionFailedResult(ErrorValidation error)
            : base(ResultStatusCode.PreconditionFailed, error)
        {
        }

        public PreconditionFailedResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.PreconditionFailed, errors)
        {
        }
    }
}
