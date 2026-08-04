namespace Dietcode.Api.Core.Results
{
    public class PreconditionRequiredResult : ErrorResult
    {
        public PreconditionRequiredResult(ErrorValidation error)
            : base(ResultStatusCode.PreconditionRequired, error)
        {
        }

        public PreconditionRequiredResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.PreconditionRequired, errors)
        {
        }
    }
}
