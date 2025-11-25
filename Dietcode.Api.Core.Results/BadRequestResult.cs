using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class BadRequestResult : ErrorResult
    {
        public BadRequestResult(ErrorValidation error)
            : base(ResultStatusCode.BadRequest, error) { }

        public BadRequestResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.BadRequest, errors) { }
    }

    public class BadRequestResult<T> : ErrorResult<T>
    {
        public BadRequestResult(T content, ErrorValidation error)
            : base(content, ResultStatusCode.BadRequest, error) { }

        public BadRequestResult(T content, IEnumerable<ErrorValidation> errors)
            : base(content, ResultStatusCode.BadRequest, errors) { }
    }

}
