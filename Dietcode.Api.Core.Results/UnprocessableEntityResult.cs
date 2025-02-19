namespace Dietcode.Api.Core.Results
{
    public class UnprocessableEntityResult : ErrorResult
    {
        public UnprocessableEntityResult(ErrorValidation error)
            : base(ResultStatusCode.UnprocessableEntity, error)
        {
        }

        public UnprocessableEntityResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.UnprocessableEntity, errors)
        {
        }
    }
}
