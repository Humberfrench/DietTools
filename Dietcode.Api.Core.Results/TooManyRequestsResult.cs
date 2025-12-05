namespace Dietcode.Api.Core.Results
{
    public class TooManyRequestsResult : ErrorResult
    {
        public TooManyRequestsResult()
            : base(ResultStatusCode.TooManyRequests,
                   new ErrorValidation("429", "Too Many Requests"))
        {
        }

        public TooManyRequestsResult(ErrorValidation error)
            : base(ResultStatusCode.TooManyRequests, error)
        {
        }

        public TooManyRequestsResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.TooManyRequests, errors)
        {
        }
    }
}
