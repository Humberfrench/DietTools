using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    public class ErrorResult : MethodResult
    {
        public IEnumerable<ErrorValidation> Errors { get; set; }

        public ErrorResult(ResultStatusCode statusCode, ErrorValidation error) : base(statusCode)
        {
            Errors = new[] { error };
        }

        public ErrorResult(ResultStatusCode statusCode, IEnumerable<ErrorValidation> errors) : base(statusCode)
        {
            Errors = errors;
        }
    }

    public class ErrorResult<TContent> : MethodResult<TContent>, IContentResult<TContent>
    {
        public IEnumerable<ErrorValidation> Errors { get; }

        public ErrorResult(TContent content, ResultStatusCode statusCode, ErrorValidation error)
            : base(content, statusCode)
        {
            Errors = new[] { error };
        }

        public ErrorResult(TContent content, ResultStatusCode statusCode, IEnumerable<ErrorValidation> errors)
            : base(content, statusCode)
        {
            Errors = errors;
        }

        object IContentResult.Content => Content!;
    }

}
