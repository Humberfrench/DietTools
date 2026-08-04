namespace Dietcode.Api.Core.Results
{
    public class UnsupportedMediaTypeResult : ErrorResult
    {
        public UnsupportedMediaTypeResult(ErrorValidation error)
            : base(ResultStatusCode.UnsupportedMediaType, error)
        {
        }

        public UnsupportedMediaTypeResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.UnsupportedMediaType, errors)
        {
        }
    }
}
