namespace Dietcode.Api.Core.Results
{
    public class MethodNotAllowedResult : ErrorResult
    {
        public MethodNotAllowedResult(ErrorValidation error)
            : base(ResultStatusCode.MethodNotAllowed, error)
        {
        }

        public MethodNotAllowedResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.MethodNotAllowed, errors)
        {
        }
    }
}
