namespace Dietcode.Api.Core.Results
{
    public class NotAcceptableResult<TContent> : ErrorResult<TContent>
    {
        public NotAcceptableResult(TContent content, ErrorValidation error)
            : base(content, ResultStatusCode.NotAcceptable, error)
        {
        }

        public NotAcceptableResult(TContent content, IEnumerable<ErrorValidation> errors)
            : base(content, ResultStatusCode.NotAcceptable, errors)
        {
        }
    }
}
