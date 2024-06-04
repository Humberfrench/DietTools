namespace Dietcode.Api.Core.Results
{
    public class NotAcceptableResult : ErrorResult
    {
        public NotAcceptableResult(ErrorValidation error)
            : base(ResultStatusCode.NotAcceptable, error)
        {
        }

        public NotAcceptableResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.NotAcceptable, errors)
        {
        }
    }
}
