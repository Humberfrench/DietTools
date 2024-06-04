namespace Dietcode.Api.Core.Results
{
    public class BadRequestResult<T> : ErrorResult<T> where T : class
    {
        public BadRequestResult(ErrorValidation error, T content)
            : base(content, ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestResult(IEnumerable<ErrorValidation> errors, T content)
            : base(content, ResultStatusCode.BadRequest, errors)
        {
        }
    }
}
