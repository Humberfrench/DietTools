namespace Dietcode.Api.Core.Results
{
    public class BadRequestResult : ErrorResult
    {
        public BadRequestResult(ErrorValidation error)
            : base(ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.BadRequest, errors)
        {
        }
    }
}
