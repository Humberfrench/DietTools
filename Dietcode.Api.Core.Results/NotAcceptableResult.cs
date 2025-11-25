using Dietcode.Api.Core.Results.Interfaces;

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
    public class NotAcceptableResult<T> : ErrorResult, IContentResult<T>
    {
        public T Content { get; }

        public NotAcceptableResult(T content, ErrorValidation error)
            : base(ResultStatusCode.NotAcceptable, error)
        {
            Content = content;
        }

        public NotAcceptableResult(T content, IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.NotAcceptable, errors)
        {
            Content = content;
        }

        object IContentResult.Content => Content!;
    }

}
