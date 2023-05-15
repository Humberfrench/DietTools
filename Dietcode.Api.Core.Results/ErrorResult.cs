using System.Collections.Generic;

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
}
