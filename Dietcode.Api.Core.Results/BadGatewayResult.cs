namespace Dietcode.Api.Core.Results
{
    public class BadGatewayResult : ErrorResult
    {
        public BadGatewayResult(ErrorValidation error)
            : base(ResultStatusCode.BadGateway, error)
        {
        }

        public BadGatewayResult(IEnumerable<ErrorValidation> errors)
            : base(ResultStatusCode.BadGateway, errors)
        {
        }
    }
}
