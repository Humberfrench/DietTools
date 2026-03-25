namespace Dietcode.Api.Core.Results
{

    public class NotFoundResult<T> : ErrorResult<T>
    {
        public NotFoundResult(T content, ErrorValidation error)
            : base(content, ResultStatusCode.NotFound, error) { }

        public NotFoundResult(T content, IEnumerable<ErrorValidation> errors)
            : base(content, ResultStatusCode.NotFound, errors) { }
    }

}
