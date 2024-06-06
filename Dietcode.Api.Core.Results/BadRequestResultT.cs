namespace Dietcode.Api.Core.Results
{
    public class BadRequestResult<T> : ErrorResult<T> where T : class, new()
    {
        public BadRequestResult(ErrorValidation error)
            : base(new T(), ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestResult(ErrorValidation error, T content)
            : base(content, ResultStatusCode.BadRequest, error)
        {
        }

        public BadRequestResult(IEnumerable<ErrorValidation> errors)
            : base(new T(), ResultStatusCode.BadRequest, errors)
        {
        }
        public BadRequestResult(IEnumerable<ErrorValidation> errors, T content)
            : base(content, ResultStatusCode.BadRequest, errors)
        {
        }
    }
}
