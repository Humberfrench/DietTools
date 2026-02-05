namespace Dietcode.Api.Core.Results
{
    public class ClientClosedResult : ErrorResult
    {
        public ClientClosedResult(ErrorValidation error)
            : base(ResultStatusCode.ClientClosedRequest, error) { }

        public ClientClosedResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.ClientClosedRequest, errors) { }
    }

    public class ClientClosedResult<T> : ErrorResult<T>
    {
        public ClientClosedResult(T content, ErrorValidation error)
            : base(content, ResultStatusCode.ClientClosedRequest, error) { }

        public ClientClosedResult(T content, IEnumerable<ErrorValidation> errors)
            : base(content, ResultStatusCode.ClientClosedRequest, errors) { }
    }
}
