using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class NotFoundResult<T> : ErrorResult<T>, IContentResult<T>
        where T : class, new()
    {
        public new T Content { get; }

        public NotFoundResult(ErrorValidation error, T content)
            : base(content, ResultStatusCode.NotFound, error)
        {
            Content = content;
        }

        public NotFoundResult(ErrorValidation error)
            : base(new T(), ResultStatusCode.NotFound, error)
        {
            Content = new T();
        }

        public NotFoundResult(IEnumerable<ErrorValidation> errors)
            : base(new T(), ResultStatusCode.NotFound, errors)
        {
            Content = new T();
        }

        public NotFoundResult(IEnumerable<ErrorValidation> errors, T content)
            : base(content, ResultStatusCode.NotFound, errors)
        {
            Content = content;
        }

        object IContentResult.Content => Content!;
    }
}
