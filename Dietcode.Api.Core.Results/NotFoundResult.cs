namespace Dietcode.Api.Core.Results
{
    public class NotFoundResult : ErrorResult
    {
        public NotFoundResult(ErrorValidation error)
            : base(ResultStatusCode.NotFound, error)
        {
        }

        public NotFoundResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.NotFound, errors)
        {
        }
    }

}
