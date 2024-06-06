namespace Dietcode.Api.Core.Results
{
    public class NotFoundResult<T> : ErrorResult<T> where T : class , new()
    {
        public NotFoundResult(ErrorValidation error, T content)
            : base(content, ResultStatusCode.BadRequest, error)
        {
        }
        public NotFoundResult(ErrorValidation error)
            : base(new T(),ResultStatusCode.NotFound, error)
        {
        }
        public NotFoundResult(IEnumerable<ErrorValidation> errors)
            : base(new T(), ResultStatusCode.NotFound, errors)
        {
        }
        public NotFoundResult(IEnumerable<ErrorValidation> errors, T content)
            : base(content, ResultStatusCode.NotFound, errors)
        {
        }
    }
}
