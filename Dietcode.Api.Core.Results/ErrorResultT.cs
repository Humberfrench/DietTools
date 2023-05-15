using System.Collections.Generic;

namespace Dietcode.Api.Core.Results
{
    public class ErrorResult<TContent> : MethodResult<TContent>
    {
        public IEnumerable<ErrorValidation> Errors { get; set; }

        public ErrorResult(TContent content, ResultStatusCode statusCode, ErrorValidation error) : base(content,statusCode)
        {
            Errors = new[] { error };
        }

        public ErrorResult(TContent content, ResultStatusCode statusCode, IEnumerable<ErrorValidation> errors) : base(content, statusCode)
        {
            Errors = errors;
        }
    }
}
